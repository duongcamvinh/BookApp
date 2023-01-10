using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Entities
{
    public class AppBooks
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string TitleBooks { get; set; }
        public string Introduction { get; set; }
        public ICollection<Photo> Photos { get; set; }
    }
}
