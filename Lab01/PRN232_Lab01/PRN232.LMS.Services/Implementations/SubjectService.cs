using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Subjects;
namespace PRN232.LMS.Services.Implementations
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SubjectService.
    /// </summary>
    public class SubjectService(IUnitOfWork unitOfWork) : ISubjectService
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSubjectsAsync.
        /// </summary>
        public async Task<PaginatedResponse<SubjectResponse>> GetSubjectsAsync(QueryParameters query)
        {
            var subjects = unitOfWork.Subjects.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                subjects = subjects.Where(s =>
                    s.SubjectCode.ToLower().Contains(keyword) ||
                    s.SubjectName.ToLower().Contains(keyword));
            }

            subjects = SortHelper.ApplySort(subjects, query.Sort, "SubjectId",
                ("subjectcode", "SubjectCode"),
                ("subjectname", "SubjectName"),
                ("credit", "Credit"));
            var totalItems = await subjects.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            var items = await subjects
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => ToResponse(ToModel(s)))
                .ToListAsync();

            return new PaginatedResponse<SubjectResponse>
            {
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            };
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSubjectByIdAsync.
        /// </summary>
        public async Task<SubjectResponse?> GetSubjectByIdAsync(int id)
        {
            var subject = await unitOfWork.Subjects.GetByIdAsync(id);
            return subject == null ? null : ToResponse(ToModel(subject));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateSubjectAsync.
        /// </summary>
        public async Task<SubjectResponse> CreateSubjectAsync(SubjectRequest request)
        {
            var subject = new Subject
            {
                SubjectCode = request.SubjectCode.Trim(),
                SubjectName = request.SubjectName.Trim(),
                Credit = request.Credit
            };

            await unitOfWork.Subjects.AddAsync(subject);
            await unitOfWork.SaveChangesAsync();
            return ToResponse(ToModel(subject));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateSubjectAsync.
        /// </summary>
        public async Task<SubjectResponse?> UpdateSubjectAsync(int id, SubjectRequest request)
        {
            var subject = await unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null) return null;

            subject.SubjectCode = request.SubjectCode.Trim();
            subject.SubjectName = request.SubjectName.Trim();
            subject.Credit = request.Credit;

            unitOfWork.Subjects.Update(subject);
            await unitOfWork.SaveChangesAsync();
            return ToResponse(ToModel(subject));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteSubjectAsync.
        /// </summary>
        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await unitOfWork.Subjects.GetByIdAsync(id);
            if (subject == null) return false;

            unitOfWork.Subjects.Delete(subject);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        private static SubjectModel ToModel(Subject subject)
        {
            return new SubjectModel
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }

        private static SubjectResponse ToResponse(SubjectModel subject)
        {
            return new SubjectResponse
            {
                SubjectId = subject.SubjectId,
                SubjectCode = subject.SubjectCode,
                SubjectName = subject.SubjectName,
                Credit = subject.Credit
            };
        }
    }
}
