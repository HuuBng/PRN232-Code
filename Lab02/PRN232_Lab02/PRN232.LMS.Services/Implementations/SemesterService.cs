using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Business;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Semesters;
namespace PRN232.LMS.Services.Implementations
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SemesterService.
    /// </summary>
    public class SemesterService(IUnitOfWork unitOfWork) : ISemesterService
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSemestersAsync.
        /// </summary>
        public async Task<PaginatedResponse<SemesterResponse>> GetSemestersAsync(QueryParameters query)
        {
            var semesters = unitOfWork.Semesters.GetAll();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                semesters = semesters.Where(s => s.SemesterName.ToLower().Contains(keyword));
            }

            semesters = ApplySort(semesters, query.Sort);
            var totalItems = await semesters.CountAsync();
            var page = query.ValidPage;
            var pageSize = query.ValidSize;

            var items = await semesters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => ToResponse(ToModel(s)))
                .ToListAsync();

            return new PaginatedResponse<SemesterResponse>
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
        ///     Xử lý request/nghiệp vụ GetSemesterByIdAsync.
        /// </summary>
        public async Task<SemesterResponse?> GetSemesterByIdAsync(int id)
        {
            var semester = await unitOfWork.Semesters.GetByIdAsync(id);
            return semester == null ? null : ToResponse(ToModel(semester));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateSemesterAsync.
        /// </summary>
        public async Task<SemesterResponse> CreateSemesterAsync(SemesterRequest request)
        {
            var semester = new Semester
            {
                SemesterName = request.SemesterName.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await unitOfWork.Semesters.AddAsync(semester);
            await unitOfWork.SaveChangesAsync();
            return ToResponse(ToModel(semester));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateSemesterAsync.
        /// </summary>
        public async Task<SemesterResponse?> UpdateSemesterAsync(int id, SemesterRequest request)
        {
            var semester = await unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null) return null;

            semester.SemesterName = request.SemesterName.Trim();
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;

            unitOfWork.Semesters.Update(semester);
            await unitOfWork.SaveChangesAsync();
            return ToResponse(ToModel(semester));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteSemesterAsync.
        /// </summary>
        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var semester = await unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null) return false;

            unitOfWork.Semesters.Delete(semester);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        private static IQueryable<Semester> ApplySort(IQueryable<Semester> semesters, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
            {
                return semesters.OrderBy(s => s.SemesterId);
            }

            IOrderedQueryable<Semester>? orderedSemesters = null;
            foreach (var field in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var descending = field.StartsWith('-');
                var name = descending ? field[1..] : field;
                var isFirstSort = orderedSemesters == null;

                orderedSemesters = name.ToLower() switch
                {
                    "semestername" => isFirstSort
                        ? descending ? semesters.OrderByDescending(s => s.SemesterName) : semesters.OrderBy(s => s.SemesterName)
                        : descending
                            ? orderedSemesters!.ThenByDescending(s => s.SemesterName)
                            : orderedSemesters!.ThenBy(s => s.SemesterName),
                    "startdate" => isFirstSort
                        ? descending ? semesters.OrderByDescending(s => s.StartDate) : semesters.OrderBy(s => s.StartDate)
                        : descending
                            ? orderedSemesters!.ThenByDescending(s => s.StartDate)
                            : orderedSemesters!.ThenBy(s => s.StartDate),
                    "enddate" => isFirstSort
                        ? descending ? semesters.OrderByDescending(s => s.EndDate) : semesters.OrderBy(s => s.EndDate)
                        : descending
                            ? orderedSemesters!.ThenByDescending(s => s.EndDate)
                            : orderedSemesters!.ThenBy(s => s.EndDate),
                    _ => isFirstSort
                        ? semesters.OrderBy(s => s.SemesterId)
                        : orderedSemesters!.ThenBy(s => s.SemesterId)
                };
            }

            return orderedSemesters ?? semesters.OrderBy(s => s.SemesterId);
        }

        private static SemesterModel ToModel(Semester semester)
        {
            return new SemesterModel
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }

        private static SemesterResponse ToResponse(SemesterModel semester)
        {
            return new SemesterResponse
            {
                SemesterId = semester.SemesterId,
                SemesterName = semester.SemesterName,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate
            };
        }
    }
}
