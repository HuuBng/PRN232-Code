using Microsoft.Extensions.Caching.Distributed;
using PRN232.LMS.Protos;
using System.Text.Json;

namespace PRN232.LMS.CourseService.Grpc
{
    public class CachedStudentGrpcClient : IStudentGrpcClient
    {
        private readonly IStudentGrpcClient _inner;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedStudentGrpcClient> _logger;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        public CachedStudentGrpcClient(IStudentGrpcClient inner, IDistributedCache cache, ILogger<CachedStudentGrpcClient> logger)
        {
            _inner = inner;
            _cache = cache;
            _logger = logger;
        }

        public async Task<StudentGrpcResponse?> GetStudentByIdAsync(int studentId)
        {
            var cacheKey = $"student:{studentId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                _logger.LogDebug("Cache HIT for student {StudentId}", studentId);
                return JsonSerializer.Deserialize<StudentGrpcResponse>(cached);
            }

            _logger.LogDebug("Cache MISS for student {StudentId}", studentId);
            var result = await _inner.GetStudentByIdAsync(studentId);
            if (result != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheDuration
                });
            }
            return result;
        }

        public async Task<bool> CheckStudentExistsAsync(int studentId)
        {
            var cacheKey = $"student:exists:{studentId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                _logger.LogDebug("Cache HIT for student exists {StudentId}", studentId);
                return bool.Parse(cached);
            }

            _logger.LogDebug("Cache MISS for student exists {StudentId}", studentId);
            var result = await _inner.CheckStudentExistsAsync(studentId);
            await _cache.SetStringAsync(cacheKey, result.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });
            return result;
        }
    }
}
