using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PersonalizedRecommendationEngine.Logic.Data.Models;
using PersonalizedRecommendationEngine.Logic.Services;

namespace PersonalizedRecommendationEngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public RecommendationsController(
            IRecommendationService recommendationService,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _recommendationService = recommendationService;
            _cache = cache;
            _configuration = configuration;
        }

        [HttpGet("{playerId}")]
        public IActionResult GetRecommendations(int playerId)
        {
            var apiKey = Request.Headers["X-Api-Key"];
            var expectedApiKey = _configuration["ApiKey"];
            if (string.IsNullOrEmpty(apiKey) || apiKey != expectedApiKey)
            {
                return Unauthorized("Invalid or missing API key.");
            }

            try
            {
                string cacheKey = $"Recommendations_{playerId}";
                if (!_cache.TryGetValue(cacheKey, out List<RecommendationResponse> recommendations))
                {
                    recommendations = _recommendationService.GetRecommendations(playerId, topK: 5);
                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    };
                    _cache.Set(cacheKey, recommendations, cacheOptions);
                }

                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating recommendations: {ex.Message}");
            }
        }
    }
}