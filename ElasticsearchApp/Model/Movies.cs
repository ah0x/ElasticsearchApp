using System.Globalization;

namespace ElasticsearchApp.Model
{
    public class Movies
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Genre { get; set; } = new List<string> { string.Empty };
        public string Year { get; set; } = string.Empty;
    }
}
