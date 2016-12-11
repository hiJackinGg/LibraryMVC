using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryMVC.Models
{
    public class Book
    {
        public Book()
        {
            Authors = new List<string>();
        }

        public int ID { get; set; }
        public string Title { get; set; }
        public IList<string> Authors { get; set; }
        public int Quantity { get; set; }
    }
    
}
