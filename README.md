# LibraryMVC

<b><i>The application functionality:</i></b><br>
<ul>
<li>managing books available in the library: adding, removing, changing quantity. Each book
can have a few authors, an author can write a few books;<br></li>
<li>a book can be taken by different people and at different time;<br></li>
<li>registration is required to take books (email);<br></li>
<li>filter that shows all books / books available / books taken by the user;<br></li>
<li>library tracks its readers (users). Displays a history of a book (when and by whom was taken);<br></li>
<li>implemented sending notifications by mail to people who took a book (“You took the
following books in our library”)<br></li>
<li>on client side, it has a front-end page with grid. Grid allows paging,
sorting by book titles and author names.<br></li>
<li>requests to the server implemented via AJAX (like single-page app)<br></li>
</ul>
<b><i>Features:</i></b>
<ul>
<li>intensive usage of JDBC and SQL (crud, transactions, batch update, stored procedures, complex queries with window functions)<br></li>
<li>effective memory usage (pagination on database side, retrieved only what it's needed to client)<br></li>
<li>4 tables with relationships: one-to-many, many-to-many<br></li>
<li>exception handling, HTTP codes status<br></li>
<li>server- and client-side validation with appropriated error message to the user<br></li>
<li>working with http sessions (autorisation)<br></li>
<li>manual realisation of ajax table with pagination, filters, sorting<br></li>
<li>intensive usage of AJAX<br></li>
<li>intensive working with DOM model via javascript<br></li>
</ul><br>
<b>Installation guide:</b><br>
1. Download project.<br>
2. To start project for sure, open it in Visual Studio (LibraryMVC.sln) and just run.<br>

Note:in developing it's used .NET Framework 4.5 and LocalDB as database. Be sure the database is running.<br>

<b>User guide:</b><br>
In the left side of screen there is grid which allow books filtering (all/in stock(quantity > 0)/taken by user(must be registered)).<br>
User can manage books by appropriate buttons opposite the grid rows (remove, book details, change quantity).<br>
You can sort by book title and author name (links in header). Sorting by title, groupes authors by each book.<br> Sorting by author name, each book will corresponds each its author in new row.<br>

To add new book, you have to click "Add book" button above grid. Next it will be displayed form to fill book fields. 
You can append several authors to book by clicking appropriate button ("+") and fill author fields.
Each new author field can be removed on form (button "-").<br>

Underneath the grid, there is the field to input email (to register user). After it, user can display taken books and take new books (it will be displayed new button "Take book").
User can't take the book which quantity is 0. After taking a book, it will be sent message to user email.<br>

In the right side of the screen there is book details (when and by whom was taken).
