using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.CourseService.Models.Business;
using PRN232.LMS.CourseService.Models.Subjects;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class SubjectService(CourseDbContext context) : ISubjectService
    {
        public async Task<PaginatedResponse<SubjectResponse>> GetSubjectsAsync(QueryParameters query)
        {
            var subjects = context.Subjects.AsQueryable();

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

        public async Task<SubjectResponse?> GetSubjectByIdAsync(int id)
        {
            var subject = await context.Subjects.FindAsync(id);
            return subject == null ? null : ToResponse(ToModel(subject));
        }

        public async Task<SubjectResponse> CreateSubjectAsync(SubjectRequest request)
        {
            var subject = new Subject
            {
                SubjectCode = request.SubjectCode.Trim(),
                SubjectName = request.SubjectName.Trim(),
                Credit = request.Credit
            };

            context.Subjects.Add(subject);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(subject));
        }

        public async Task<SubjectResponse?> UpdateSubjectAsync(int id, SubjectRequest request)
        {
            var subject = await context.Subjects.FindAsync(id);
            if (subject == null) return null;

            subject.SubjectCode = request.SubjectCode.Trim();
            subject.SubjectName = request.SubjectName.Trim();
            subject.Credit = request.Credit;

            context.Subjects.Update(subject);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(subject));
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var subject = await context.Subjects.FindAsync(id);
            if (subject == null) return false;

            context.Subjects.Remove(subject);
            await context.SaveChangesAsync();
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
