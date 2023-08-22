using ElasticsearchApp.Model;

namespace ElasticsearchApp.MovieService
{
    public interface IMovieService
    {
        List<Movies> Pasrser(string filePath);
        Task<string> PushToServer(string filePath);
    }
}
