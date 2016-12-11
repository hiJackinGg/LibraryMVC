# LibraryMVC

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
