using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Cosmos; // Corrected namespace



namespace Library_Management_System.Models
{
    public class BookModel
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string UId { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]

        public string Title { get; set; }
        [JsonProperty(PropertyName = "author", NullValueHandling = NullValueHandling.Ignore)]

        public string Author { get; set; }
        [JsonProperty(PropertyName = "publishedDate", NullValueHandling = NullValueHandling.Ignore)]

        public DateTime PublishedDate { get; set; }
        [JsonProperty(PropertyName = "ISBN", NullValueHandling = NullValueHandling.Ignore)]

        public string ISBN { get; set; }
        [Required]
        [JsonProperty(PropertyName = "isIssued", NullValueHandling = NullValueHandling.Ignore)]

        public bool IsIssued { get; set; }
    }
}
