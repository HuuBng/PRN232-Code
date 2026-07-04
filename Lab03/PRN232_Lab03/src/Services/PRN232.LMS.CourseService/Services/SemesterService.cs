using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Data;
using PRN232.LMS.CourseService.Entities;
using PRN232.LMS.CourseService.Models.Business;
using PRN232.LMS.CourseService.Models.Semesters;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class SemesterService(CourseDbContext context) : ISemesterService
    {
        public async Task<PaginatedResponse<SemesterResponse>> GetSemestersAsync(QueryParameters query)
        {
            var semesters = context.Semesters.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var keyword = query.Search.Trim().ToLower();
                semesters = semesters.Where(s => s.SemesterName.ToLower().Contains(keyword));
            }

            semesters = SortHelper.ApplySort(semesters, query.Sort, "SemesterId",
                ("semestername", "SemesterName"),
                ("startdate", "StartDate"),
                ("enddate", "EndDate"));

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

        public async Task<SemesterResponse?> GetSemesterByIdAsync(int id)
        {
            var semester = await context.Semesters.FindAsync(id);
            return semester == null ? null : ToResponse(ToModel(semester));
        }

        public async Task<SemesterResponse> CreateSemesterAsync(SemesterRequest request)
        {
            var semester = new Semester
            {
                SemesterName = request.SemesterName.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            context.Semesters.Add(semester);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(semester));
        }

        public async Task<SemesterResponse?> UpdateSemesterAsync(int id, SemesterRequest request)
        {
            var semester = await context.Semesters.FindAsync(id);
            if (semester == null) return null;

            semester.SemesterName = request.SemesterName.Trim();
            semester.StartDate = request.StartDate;
            semester.EndDate = request.EndDate;

            context.Semesters.Update(semester);
            await context.SaveChangesAsync();
            return ToResponse(ToModel(semester));
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var semester = await context.Semesters.FindAsync(id);
            if (semester == null) return false;

            context.Semesters.Remove(semester);
            await context.SaveChangesAsync();
            return true;
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
