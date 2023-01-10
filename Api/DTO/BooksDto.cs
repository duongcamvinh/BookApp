using Api.Entities;
using System.Collections.Generic;

namespace Api.DTO
{
    public class BooksDto
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string TitleBooks { get; set; }
        public string Introduction { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}
