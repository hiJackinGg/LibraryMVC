using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMVC.Models
{
    public class BooksWithGridParams
    {
        public BooksWithGridParams()
        {
            Books = new List<Book>();
        }
        public IList<Book> Books { get; set; }
        //count of rows to be displayed without paging
        public int TotalRows { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get { return (int) Math.Ceiling((double)TotalRows / PageSize); }}

    }
}