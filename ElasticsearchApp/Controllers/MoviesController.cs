using ElasticsearchApp.Model;
using ElasticsearchApp.MovieService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nest;
using System.Text.Json;
using System.Text.RegularExpressions;
using X.PagedList;

namespace ElasticsearchApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ElasticClient _elasticClient;

        public MoviesController(IMovieService movieService, ElasticClient elasticClient)
        {
            _movieService = movieService;
            _elasticClient = elasticClient;
        }

        [HttpGet("ReadJsonFile")]
        public IActionResult ReadJsonFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest("File path is required.");
                }

                var jsonData = _movieService.Pasrser(filePath);
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ReadFromElasticsearch")]
        public async Task<ActionResult> ReadFromElasticsearch(string? searchTerm, int? page)
        {
            try
            {
                const int pageSize = 12;

                var searchResponse = await _elasticClient.SearchAsync<Movies>(s => s
                    .Index("movies")
                    .Query(q =>
                    {
                        if (string.IsNullOrWhiteSpace(searchTerm))
                        {
                            return q.MatchAll();
                        }
                        else
                        {
                            return q
                                .FunctionScore(fs => fs
                                    .Query(qd => qd
                                        .Match(m => m
                                            .Field(f => f.Name)
                                            .Query(searchTerm)
                                        )
                                    )
                                    .Functions(funcs => funcs
                                        .ScriptScore(ss => ss
                                            .Script(sc => sc
                                                .Source("_score * (1.0 + doc['clickCount'].value * 0.1 + doc['otherFeature'].value * 0.2)")
                                            )
                                        )
                                    )// this function will be used on LTR if i had more information and making user tracking with actionfilter from dot net
                                );
                        }
                    })
                    .Sort(sort => sort
                        .Ascending(m=>m.Id)
                    )
                    .Size(pageSize)
                    .From((page ?? 1 - 1) * pageSize)
                );

                if (!searchResponse.IsValid)
                {
                    var errorMessage = "Error querying Elasticsearch: " + searchResponse.OriginalException?.Message;
                    return BadRequest(errorMessage);
                }

                var searchResults = searchResponse.Documents;

                foreach (var movie in searchResults)
                {
                    movie.Name = RemoveYearFromName(movie.Name);
                    movie.Year = ExtractYear(DateTime.Parse(movie.Year));
                }

                var totalHits = (int)searchResponse.Total;

                var totalPages = (int)Math.Ceiling((double)totalHits / pageSize);

                var responseData = new
                {
                    Results = searchResults,
                    TotalPages = totalPages - 1
                };

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string RemoveYearFromName(string name)
        {
            return Regex.Replace(name, @"\s\(\d{4}\)$", "");
        }

        private string ExtractYear(DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value.Year.ToString();
            }
            return "";
        }


        [HttpPost("pushToServer")]
        public async Task<IActionResult> PushDataToElasticsearch(string filePath)
        {
            string result = await _movieService.PushToServer(filePath);

            if (result.StartsWith("Error"))
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
