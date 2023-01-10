 using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    [Table("Photos")]
    public class Photo
    {
       
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public AppBooks AppBooks { get; set; }
        public int AppBooksId { get; set; }

    }
}