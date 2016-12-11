using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using LibraryMVC.Models;


namespace LibraryMVC.DataAccess
{
    public class DAO
    {
       // private static string connectionString = ConfigurationManager.ConnectionStrings["Library"].ConnectionString;
        private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public static BooksWithGridParams getAllBooks(int pageNumber, int pageSize, string fieldSortBy = "title", bool asc = true)
        {

            string query = null;
            string sortDirection = asc == true ? "asc" : "desc";
            Func<SqlDataReader, KeyValuePair<int, List<Book>>> buildFunc = null;

            if (fieldSortBy == "title")
            {
                query = string.Format(DAOQueries.GET_ALL_BOOKS_SORTED_BY_TITLE, sortDirection);
                buildFunc = buildBookList;
            }
            else if (fieldSortBy == "author")
            {
                query = string.Format(DAOQueries.GET_ALL_BOOKS_SORTED_BY_AUTHOR, sortDirection);
                buildFunc = buildBookPerAuthorList;
            }

            BooksWithGridParams booksWithGridParams = new BooksWithGridParams();

            KeyValuePair<int, List<Book>> res = new KeyValuePair<int, List<Book>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@pageNumber", pageNumber * pageSize));
                command.Parameters.Add(new SqlParameter("@pageSize", pageSize));
                SqlDataReader reader = command.ExecuteReader();

                res = buildFunc(reader);

                reader.Close();
            }

            booksWithGridParams.TotalRows = res.Key;
            booksWithGridParams.Books = res.Value;
            booksWithGridParams.PageSize = pageSize;
            booksWithGridParams.PageNumber = pageNumber;

            return booksWithGridParams;

        }

        public static BooksWithGridParams getBooksAvailable(int pageNumber, int pageSize, string fieldSortBy = "title", bool asc = true)
        {

            string query = null;
            string sortDirection = asc == true ? "asc" : "desc";
            Func<SqlDataReader, KeyValuePair<int, List<Book>>> buildFunc = null;

            if (fieldSortBy == "title")
            {
                query = string.Format(DAOQueries.GET_AVAILABLE_BOOKS_SORTED_BY_TITLE, sortDirection);
                buildFunc = buildBookList;
            }
            else if (fieldSortBy == "author")
            {
                query = string.Format(DAOQueries.GET_AVAILABLE_BOOKS_SORTED_BY_AUTHOR, sortDirection);
                buildFunc = buildBookPerAuthorList;
            }

            BooksWithGridParams booksWithGridParams = new BooksWithGridParams();

            KeyValuePair<int, List<Book>> res = new KeyValuePair<int, List<Book>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@pageNumber", pageNumber * pageSize));
                command.Parameters.Add(new SqlParameter("@pageSize", pageSize));
                SqlDataReader reader = command.ExecuteReader();

                res = buildFunc(reader);

                reader.Close();
            }

            booksWithGridParams.TotalRows = res.Key;
            booksWithGridParams.Books = res.Value;
            booksWithGridParams.PageSize = pageSize;
            booksWithGridParams.PageNumber = pageNumber;

            return booksWithGridParams;
        }

        public static BooksWithGridParams getBooksTakenByUser(int pageNumber, int pageSize, string userEmail, string fieldSortBy = "title", bool asc = true)
        {

            string query = null;
            string sortDirection = asc == true ? "asc" : "desc";
            Func<SqlDataReader, KeyValuePair<int, List<Book>>> buildFunc = null;

            if (fieldSortBy == "title")
            {
                query = string.Format(DAOQueries.GET_TAKEN_BY_USER_BOOKS_SORTED_BY_TITLE, sortDirection);
                buildFunc = buildBookList;
            }
            else if (fieldSortBy == "author")
            {
                query = string.Format(DAOQueries.GET_TAKEN_BY_USER_BOOKS_SORTED_BY_AUTHOR, sortDirection);
                buildFunc = buildBookPerAuthorList;
            }

            BooksWithGridParams booksWithGridParams = new BooksWithGridParams();

            KeyValuePair<int, List<Book>> res = new KeyValuePair<int, List<Book>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@pageNumber", pageNumber * pageSize));
                command.Parameters.Add(new SqlParameter("@pageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@email", userEmail));
                SqlDataReader reader = command.ExecuteReader();

                res = buildFunc(reader);

                reader.Close();
            }

            booksWithGridParams.TotalRows = res.Key;
            booksWithGridParams.Books = res.Value;
            booksWithGridParams.PageSize = pageSize;
            booksWithGridParams.PageNumber = pageNumber;

            return booksWithGridParams;
        }

        public static IList<KeyValuePair<string, string>> getBookDetails(int bookId)
        {

            StringBuilder query = new StringBuilder();
            query.Append("select l.date, l.email from Books b ");
            query.Append("inner join Lib_log l ");
            query.Append("on l.bookID = b.id ");
            query.Append("where b.id = @bookId ");

            IList<KeyValuePair<string, string>> details = new List<KeyValuePair<string, string>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query.ToString(), connection);
                command.Parameters.Add(new SqlParameter("@bookId", bookId));
                SqlDataReader reader = command.ExecuteReader();

                KeyValuePair<string, string> pair;
                string date, user;
                while (reader.Read())
                {
                    date = Convert.ToString(reader.GetDateTime(0));
                    user = reader.GetString(1);
                    pair = new KeyValuePair<string, string>(date, user);
                    details.Add(pair);
                }

                reader.Close();
            }

            return details;
        }
        public static int addBook(Book book)
        {

            String query = "insert into Books (title, quantity) output Inserted.id values (@title, @quantity)";

            int bookId = 0;
            IEnumerable<string> authors = book.Authors;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Transaction = transaction;
                        command.Parameters.Add(new SqlParameter("@title", book.Title));
                        command.Parameters.Add(new SqlParameter("@quantity", book.Quantity));

                        bookId = (int)command.ExecuteScalar();

                        attachAuthors(bookId, authors, connection, transaction);

                        transaction.Commit();

                    }
                }
            }

            return bookId;
        }

        private static void attachAuthors(int bookId, IEnumerable<string> authors, SqlConnection connection, SqlTransaction transaction)
        {
            String sp_name = "sp_attachAuthorToBook";

            using (SqlCommand command = new SqlCommand(sp_name, connection))
            {
                command.Transaction = transaction;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter param1 = new SqlParameter
                {
                    ParameterName = "@bookId",
                    Value = bookId
                };
                command.Parameters.Add(param1);

                foreach (string name in authors)
                {
                    if (name.Length == 0)
                        continue;

                    SqlParameter param2 = new SqlParameter
                    {
                        ParameterName = "@authorName",
                        Value = name
                    };
                    command.Parameters.Add(param2);

                    command.ExecuteNonQuery();

                    command.Parameters.Remove(param2);

                }
            }
        }


        public static void removeBook(int bookId)
        {

            String query1 = "delete from Books_Authors where bookId = @bookId";
            String query2 = "delete from Lib_log where bookId = @bookId";
            String query3 = "delete from Books where id = @bookId";

            SqlCommand[] commands;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                commands = new SqlCommand[]{
                    new SqlCommand(query1, connection),
                    new SqlCommand(query2, connection),
                    new SqlCommand(query3, connection)
                };

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    foreach (SqlCommand command in commands)
                    {
                        command.Transaction = transaction;
                        command.Parameters.Add(new SqlParameter("@bookId", bookId));
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
        }

        public static void takeBook(int bookId, string email)
        {

            String query1 = "insert into Lib_log (email, bookId) values (@email, @bookId)";
            String query2 = "update Books set quantity = quantity-1 where id = @bookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    SqlCommand command1 = new SqlCommand(query1, connection);
                    command1.Transaction = transaction;
                    command1.Parameters.Add(new SqlParameter("@email", email));
                    command1.Parameters.Add(new SqlParameter("@bookId", bookId));
                    command1.ExecuteNonQuery();

                    SqlCommand command2 = new SqlCommand(query2, connection);
                    command2.Transaction = transaction;
                    command2.Parameters.Add(new SqlParameter("@bookId", bookId));
                    command2.ExecuteNonQuery();

                    transaction.Commit();
                }
            }
        }

        public static void changeBookQuantity(int bookId, int quantity)
        {

            String query1 = "update Books set quantity = @quantity where id = @bookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command1 = new SqlCommand(query1, connection);
                command1.Parameters.Add(new SqlParameter("@quantity", quantity));
                command1.Parameters.Add(new SqlParameter("@bookId", bookId));
                command1.ExecuteNonQuery();
            }
        }

        public static int getBookQuantity(int bookId)
        {

            String query1 = "select quantity from Books where id = @bookId";
            int qty = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query1, connection);
                command.Parameters.Add(new SqlParameter("@bookId", bookId));
                qty = (int)command.ExecuteScalar();

            }

            return qty;
        }


        private static KeyValuePair<int, List<Book>> buildBookList(SqlDataReader reader)
        {

            Dictionary<int, Book> books = new Dictionary<int, Book>();
            int totalRows = 0;

            Book book = null;

            while (reader.Read())
            {

                int id = reader.GetInt32(0);

                if (books.ContainsKey(id))
                    book = books[id];
                else
                {
                    book = new Book();
                    book.ID = id;
                    book.Title = reader.GetString(1);
                    book.Quantity = reader.GetInt32(2);
                    books[id] = book;
                }

                String authorName = reader.IsDBNull(3) ? "" : reader.GetString(3);
                book.Authors.Add(authorName);

                totalRows = reader.GetInt32(4);
            }


            return new KeyValuePair<int, List<Book>>(totalRows, new List<Book>(books.Values));
        }

        private static KeyValuePair<int, List<Book>> buildBookPerAuthorList(SqlDataReader reader)
        {
            List<Book> books = new List<Book>();
            int totalRows = 0;

            Book book;

            while (reader.Read())
            {
                book = new Book();
                book.ID = reader.GetInt32(0);
                book.Title = reader.GetString(1);
                book.Quantity = reader.GetInt32(2);
                book.Authors.Add(reader.IsDBNull(3) ? "" : reader.GetString(3));
                books.Add(book);

                totalRows = reader.GetInt32(4);
            }

            return new KeyValuePair<int, List<Book>>(totalRows, books);
        }
    }
}
