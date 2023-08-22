using ElasticsearchApp.Model;
using Nest;
using System.Globalization;

namespace ElasticsearchApp.MovieService
{
    public class MovieService : IMovieService
    {
        private readonly ElasticClient _elasticClient;

        public MovieService(ElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        List<Movies> IMovieService.Pasrser(string filePath)
        {
            var moviesList = new List<Movies>();
            int idCounter = 1;

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split("::");

                    if (parts.Length >= 3)
                    {
                        var movie = new Movies
                        {
                            Id = idCounter++,
                            Name = parts[1],
                            Genre = parts[2].Split('|').ToList(),
                            Year = parts[1]
                        };

                        moviesList.Add(movie);
                    }
                }
            }

            return moviesList;
        }

        public async Task<string> PushToServer(string filePath)
        {
            var connectionString = "https://localhost:9200/";
            var indexName = "movies";

            var settings = new ConnectionSettings(new Uri(connectionString))
                .DefaultIndex(indexName)
                .CertificateFingerprint("81436ca0b53045454c21d6b89f4511bbad595e6b8d362a64874f2a581a6d20134")
                .BasicAuthentication("elastic", "aYyr8ya9=Selo-YiRvP_c");
            var client = new ElasticClient(settings);

            //Delete the existing index (if it exists)
            var indexExistsResponse = client.Indices.Exists(indexName);
            if (indexExistsResponse.Exists)
            {
                var deleteIndexResponse = await client.Indices.DeleteAsync(indexName);
                if (!deleteIndexResponse.IsValid)
                {
                    return $"Error deleting existing index: {deleteIndexResponse.DebugInformation}";
                }
            }
            

            //Index new data
            var movies = File.ReadAllLines(filePath)
                .Select(line =>
                {
                    var parts = line.Split("::");
                    return new Movies
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Genre = parts[2].Split('|').ToList(),
                        Year = parts[1]
                    };
                })
                .ToList();

            foreach (var movie in movies)
            {
                var indexResponse = await client.IndexDocumentAsync(movie);
                if (!indexResponse.IsValid)
                {
                    return $"Error indexing document: {indexResponse.DebugInformation}";
                }
            }

            return "Data indexed successfully.";
        }
    }
}
