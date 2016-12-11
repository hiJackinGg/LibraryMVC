create database Library
go

use Library
go

IF OBJECT_ID('Books') IS NULL
BEGIN
create table Books (
   id          int			        not null identity(1,1),
   title       varchar(100)         null,
   quantity    int				    null
   constraint PK_BOOKS primary key (id)
)
END
go

IF OBJECT_ID('Authors') IS NULL
BEGIN
create table Authors (
   id            int			      not null identity(1,1),
   name          varchar(100)         null
   constraint PK_AUTHORS primary key (id)
)
END
go

IF OBJECT_ID('Books_Authors') IS NULL
BEGIN
create table Books_Authors (
	id			  int			       not null	identity(1,1),
	bookID		  int                  not null,
	authorID	  int                  not null,
	constraint PK_BA primary key (id),
	constraint FK_BOOK_AUTHOR foreign key (BookID) references Books (id),
	constraint FK_AUTHOR_BOOK foreign key (AuthorID) references  Authors (id)

)
END
go

IF OBJECT_ID('Lib_log') IS NULL
BEGIN
create table Lib_log (
   id            int							not null identity(1,1),
   email         varchar(100)					not null,
   bookID        int							not null,
   [date]		 date			default convert(date, getdate())			      not null
   constraint PK_LL primary key (id),
   constraint FK_LB foreign key (bookID) references Books (id)
)
END

create PROCEDURE sp_attachAuthorToBook
    @bookId int,
    @authorName varchar(20)
AS
declare @authorId int
select @authorId = id from Authors where name = @authorName

   IF (@authorId is null)
	   BEGIN
		   INSERT INTO Authors (name)
		   VALUES (@authorName) 
		   SELECT @authorId = SCOPE_IDENTITY()  
	   END

   insert into Books_Authors (bookID, authorID)
   values (@bookId, @authorId)

   --select @authorId
GO

  INSERT INTO Books(title, quantity)
  VALUES('The Golden Calf', 3);
  INSERT INTO Books(title, quantity)
  VALUES('The Old Man and the Sea', 2);
  INSERT INTO Books(title, quantity)
  VALUES('The Metamorphosis', 1);
  INSERT INTO Books(title, quantity)
  VALUES('For Whom the Bell Tolls', 2);
  INSERT INTO Books(title, quantity)
  VALUES('The Master and Margarita', 0);
  INSERT INTO Books(title, quantity)
  VALUES('Java programming', 5);
  INSERT INTO Books(title, quantity)
  VALUES('Heart of a Dog', 0);

  INSERT INTO Authors(name)
  VALUES('I.Ilf');
  INSERT INTO Authors(name)
  VALUES('Y.Petrov');
    INSERT INTO Authors(name)
  VALUES('E.Hemingway');
    INSERT INTO Authors(name)
  VALUES('F.Kafka');
    INSERT INTO Authors(name)
  VALUES('M.Bulgakov');
      INSERT INTO Authors(name)
  VALUES('C.Horstmann');
    INSERT INTO Authors(name)
  VALUES('G.Cornell');

    INSERT INTO Books_Authors(bookID, authorID)
  VALUES(1, 1);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(1, 2);
    INSERT INTO Books_Authors(bookID, authorID)
  VALUES(2, 3);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(3, 4);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(4, 3);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(5, 5);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(6, 6);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(6, 7);
      INSERT INTO Books_Authors(bookID, authorID)
  VALUES(7, 5);

    INSERT INTO Lib_log(email, bookID)
  VALUES('asd@gmail.com', 5);
    INSERT INTO Lib_log(email, bookID)
  VALUES('mmulo@yandex.com', 7);
    INSERT INTO Lib_log(email, bookID)
  VALUES('ff223f@gmail.com', 1);
    INSERT INTO Lib_log(email, bookID)
  VALUES('mmulo@yandex.com', 3);
    INSERT INTO Lib_log(email, bookID)
  VALUES('asd@gmail.com', 7);
    INSERT INTO Lib_log(email, bookID)
  VALUES('ff223f@gmail.com', 5);
      INSERT INTO Lib_log(email, bookID)
  VALUES('ff223f@gmail.com', 1);

