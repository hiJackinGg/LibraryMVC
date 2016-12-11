using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryMVC.Models
{
    public abstract class DAOQueries
    {

        public static readonly String GET_ALL_BOOKS_SORTED_BY_TITLE = "select b.id, b.title, b.quantity, a.name, b.totalrows from "+
                          "(select id, title, quantity, count(*) OVER() AS totalrows from Books "+
                              "order by title {0} offset @pageNumber rows fetch next @pageSize  rows only) b " +
                          "left join Books_Authors ba "+
                          "on ba.bookId = b.id left join Authors a " +
                          "on ba.authorId = a.id ";

        public static readonly String GET_ALL_BOOKS_SORTED_BY_AUTHOR = "select b.id, b.title, b.quantity, a.name, count(*) OVER() AS totalrows from Books b " +
                          "left join Books_Authors ba " +
                          "on ba.bookId = b.id left join Authors a "+
                          "on ba.authorId = a.id "+
                          "order by a.name {0} offset @pageNumber rows fetch next @pageSize  rows only ";

        public static readonly String GET_AVAILABLE_BOOKS_SORTED_BY_TITLE = " select b.id, b.title, b.quantity, a.name, b.totalrows from " +
                          "(select id, title, quantity, count(*) OVER() AS totalrows from Books "+
                             "where quantity > 0 order by title {0} offset @pageNumber rows fetch next @pageSize rows only) b " +
                          "left join Books_Authors ba " +
                          "on ba.bookId = b.id left join Authors a " +
                          "on ba.authorId = a.id ";

        public static readonly String GET_AVAILABLE_BOOKS_SORTED_BY_AUTHOR = "select b.id, b.title, b.quantity, a.name, count(*) OVER() AS totalrows from Books b " +
                          "left join Books_Authors ba " +
                          "on ba.bookId = b.id left join Authors a " +
                          "on ba.authorId = a.id "+	
						  "where quantity > 0 "+
                          "order by a.name {0} offset @pageNumber rows fetch next @pageSize rows only ";

        public static readonly String GET_TAKEN_BY_USER_BOOKS_SORTED_BY_TITLE = "select b.*, c.totalrows from ( "+
                         "select b.*, a.name from ( "+
                              "select distinct b.id, b.title, b.quantity from Books b inner join Lib_log l "+
						       "on b.id = l.bookID "+
						       "where l.email = @email order by title {0} offset @pageNumber rows fetch next @pageSize rows only) b "+
                         "inner join Books_Authors ba "+
                         "on ba.bookId = b.id inner join Authors a "+
                         "on ba.authorId = a.id) b "+
                    "cross join ( "+
                         "select count(distinct b.id) as totalrows from Books b "+
                       	 "inner join Lib_log l "+
						 "on l.bookID = b.id "+
						 "where l.email = @email) c";

        public static readonly String GET_TAKEN_BY_USER_BOOKS_SORTED_BY_AUTHOR = "select b.*, c.totalrows from ( "+
                          "select distinct b.id, b.title, b.quantity, a.name from Books b inner join Books_Authors ba "+
                          "on ba.bookId = b.id inner join Authors a "+
                          "on ba.authorId = a.id inner join Lib_log l "+
						  "on l.bookID = b.id "+
						  "where l.email = @email "+
						  "order by a.name {0} offset @pageNumber rows fetch next @pageSize rows only) b "+
                    "cross join ( "+
                          "select count(distinct b.id) as totalrows from Books b inner join Lib_log l "+
						  "on l.bookID = b.id "+
						  "where l.email = @email) c ";
    }
}