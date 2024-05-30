using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class IssueModel
    {
        [Key]
        [JsonProperty(PropertyName = "uId", NullValueHandling = NullValueHandling.Ignore)]

        public string UId { get; set; }
        [JsonProperty(PropertyName = "bookId", NullValueHandling = NullValueHandling.Ignore)]
        public string BookId { get; set; }
        [JsonProperty(PropertyName = "librarianName", NullValueHandling = NullValueHandling.Ignore)]

        public string LibrarianName { get; set; }
        [JsonProperty(PropertyName = "memberId", NullValueHandling = NullValueHandling.Ignore)]

        public string MemberId { get; set; }
        [JsonProperty(PropertyName = "issueDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime IssueDate { get; set; }
        [JsonProperty(PropertyName = "returnDate", NullValueHandling = NullValueHandling.Ignore)]

        public DateTime? ReturnDate { get; set; }
        [JsonProperty(PropertyName = "isReturned", NullValueHandling = NullValueHandling.Ignore)]

        public bool IsReturned { get; set; }
    }
}
