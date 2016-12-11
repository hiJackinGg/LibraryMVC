using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryMVC.Models;
using LibraryMVC.DataAccess;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;



namespace LibraryMVC.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BooksPage()
        {
            int pageSize = 3;
            int pageNumber = (Request["pageNumber"] == null) ? 0 : Convert.ToInt32(Request["pageNumber"]);
            int filter = (Request["filter"] == null) ? 1 : Convert.ToInt32(Request["filter"]);
            string fieldSortBy = (Request["fieldSortBy"] == null) ? "title" : Convert.ToString(Request["fieldSortBy"]);
            bool asc = (Request["sortASC"] == null) ? true : Convert.ToBoolean(Request["sortASC"]);
            string userEmail = (string)this.Session["user_email"];

            BooksWithGridParams booksWithGridParams = new BooksWithGridParams();

            if (filter == 1)
            {
                booksWithGridParams = DAO.getAllBooks(pageNumber, pageSize, fieldSortBy, asc);
            }
            else if (filter == 2)
            {
                booksWithGridParams = DAO.getBooksAvailable(pageNumber, pageSize, fieldSortBy, asc);
            }
            else if (filter == 3)
            {
                if (userEmail != null)
                {
                    booksWithGridParams = DAO.getBooksTakenByUser(pageNumber, pageSize, userEmail, fieldSortBy, asc);
                }
                else
                {
                    return new HttpStatusCodeResult(403);
                }
            }
            else
            {
                throw new ArgumentException();
            }


            return PartialView(booksWithGridParams);
        }

        public ActionResult RememberUser()
        {
            string email = Request["email"];

            if (email == null)
                return new HttpStatusCodeResult(400, "Incorrect email address.");

            this.Session["user_email"] = email;

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult AddBook(Book book)
        {
            if (book == null) throw new Exception();

            DAO.addBook(book);

            return RedirectToAction("BooksPage");
        }

        public ActionResult ChangeQuantity(int quantity, int bookId)
        {
            if (quantity < 0)
                return new HttpStatusCodeResult(400, "Incorrect quantity value.");

            DAO.changeBookQuantity(bookId, quantity);

            return null;
        }

        [HttpPost]
        public ActionResult TakeBook(int bookId)
        {
            string userEmail = (string) this.Session["user_email"];

            if (userEmail == null)
                return new HttpStatusCodeResult(403);

            if (DAO.getBookQuantity(bookId) <= 0)
                return new HttpStatusCodeResult(400, "There is no books left.");
            
            DAO.takeBook(bookId, userEmail);

            this.SendNotification(bookId, userEmail);

            return new HttpStatusCodeResult(200);
        }

        [HttpPost]
        public ActionResult BookDetails(int bookId)
        {
            IEnumerable<KeyValuePair<string, string>> details = DAO.getBookDetails(bookId);
            return PartialView(details);
        }

        [HttpPost]
        public ActionResult DeleteBook(int bookId)
        {

            DAO.removeBook(bookId);

            return RedirectToAction("BooksPage");
        }




        private void SendNotification(int bookId, string userEmail)
        {
            string emailSendFrom = "kdv19071994@gmail.com";
            string password = "19071994";

            string body = "You took a book in our library. (Id of the book is - " + bookId + ")";
            string subject = "Library";

            MailMessage o = new MailMessage(emailSendFrom, userEmail, subject, body);
            NetworkCredential netCred = new NetworkCredential(emailSendFrom, password);
            SmtpClient smtpobj = new SmtpClient("smtp.gmail.com", 587);
            smtpobj.EnableSsl = true;
            smtpobj.Credentials = netCred;
            smtpobj.Send(o);
        }
       
    }
}
