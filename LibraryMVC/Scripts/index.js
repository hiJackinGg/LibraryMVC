
var grid = {
    pageNumber: 0,  
    pageSize: 3,   
    fieldSortBy: 'title',
    asc: true
}

var inputs = [];        //author input fields


$(function () {

    //primary loading of books from server (loads first page)
    $("#container").load('Home/BooksPage');

    $("#appendAuthor").click(function () {
        
        input = document.createElement('input');
        $(input).attr('id', 'authors')
                .attr('name', 'authors')
                .attr('list', 'authorList')
                .attr('type', 'text')
                .attr('placeholder', 'Author name');
        inputs.push(input);

        var i = inputs.indexOf(input);
        
        removeInput = document.createElement('button');
        $(removeInput).attr('type', 'button')
                      .attr('id', i)
                      .append('-')
                      .append('<br/>');
        inputs.push(removeInput);

        $(removeInput).click(function () {
            var index = $(this).attr('id');     //button id is equal to array index with element to be removed 
            $(inputs[index]).remove();
            $(this).remove();
            delete inputs[index];
            //inputs.splice(index, 1);
        });

        $("#authorsDiv").prepend(input);     
        $(input).after(removeInput);

    });

    
    //displays form for book adding
    $("#add").click(function (e) {
        e.preventDefault();
        $("#bookDiv").show();
    });
    //hides book form
    $("#book #cancel").click(function () {
        $("#bookDiv").hide();
    });

    $("#book").submit(function (e) {
        $.post("/Home/AddBook",
            $('#book').serialize(),
            function (data) {
                $("#container").html(data);
                grid.pageNumber = 0;
                grid.pageSiz = 3;
                grid.fieldSortBy = 'title';
                grid.asc = true;
            });

        e.preventDefault();
        $("#bookDiv").hide();

        //clear suplementary fields
        while (inputs.length > 0) {
            var inputField = inputs.pop();
            $(inputField).remove();
        }

        this.reset();

    });



    window.next = function () {

            grid.pageNumber++;

        updateGrid();
    }

    window.prev = function () {
        if (grid.pageNumber <= 0) {
            return;
        }
        else {
            grid.pageNumber--;
        }

        updateGrid();
    }



    window.sort = function (field) {
        grid.pageNumber = 0;
        grid.fieldSortBy = field;
        grid.asc = !grid.asc;
        updateGrid();

    }

    $('#filter').change(function (e) {
        grid.pageNumber = 0;
        updateGrid();

    });

    window.updateGrid = function () {      
        $.ajax({
            type: "POST",
            url: "/Home/BooksPage",
            data: {
                'filter': $('#filter').val(),
                'pageNumber': grid.pageNumber,
                'fieldSortBy': grid.fieldSortBy,
                'sortASC': grid.asc
            },
            success: function (data) {
                $("#container").html(data);
            },
            error: errorFunc
        });
    }

 
    $('#login').click(function () {

        var email = $('#email').val();

        var regex = /\S+@\S+\.\S+/;
        if (regex.test(email) == false) {
            alert("The email you've entered is wrong.");
            $('#email').val('');
            return;
        }

        $.ajax({
            type: "POST",
            url: "/Home/RememberUser",
            data: { 
                'email': email
            },
            success: function (data, status) {
                location.reload(true);
            },
            error: errorFunc
        });
    });


    function errorFunc(xhr, ajaxOptions, throwError) {
        switch (xhr.status) {
            case 400: alert("Client error." + throwError); break;
            case 403: alert("Log in, please. Unput your email."); location.reload(true); break;
            case 500: alert("Internal server error. Try again later."); break;
        }
    }

    //Searches and returns the element with attribute name = 'attrName' and value 'attrVal' in the row which contains 'e' element.
    function findElement(e, attrName, attrVal) {

        var tableRow = e;

        //searches parent <tr> tag of 'e' element
        while (tableRow.nodeName !== "TR") {
            tableRow = tableRow.parentNode;
        }

        var elems = tableRow.getElementsByTagName("*");
        var elemToFind;

        for (var i = 0; i < elems.length; i++) {
            var el = elems[i];

            if (el.nodeType == 1 && el.getAttribute(attrName) == attrVal) {
                elemToFind = el;
                break;
            }
        }

        return elemToFind;
    }


    window.takeBook = function (e, bookId) {        //'e' - element which intended event (button)

        var qtyElem = findElement(e, "class", 'quantity');
        var qtyElemOldVal = +qtyElem.innerHTML
        var qtyElemNewVal = qtyElemOldVal > 0 ? qtyElemOldVal - 1 : 0;

        $.ajax({
            type: "POST",
            url: "/Home/TakeBook",
            data: {
                'bookId': bookId
            },
            success: function () {
                if (qtyElemNewVal <= 0) {
                    e.setAttribute("disabled", "true");
                }
                qtyElem.innerHTML = qtyElemNewVal;      //changing book quantity after been taken
                alert("You have taken the book. Notification was sent to your email.");
            },
            error: errorFunc
            
        });


    }

    window.changeQuantity = function (e, bookId) {

        var qtyElem = findElement(e, "class", 'quantity');
        var qtyElemOldVal = qtyElem.innerHTML;

        qtyElem.setAttribute('contenteditable', true);

        var saveButton = document.createElement("Button");
        var node = document.createTextNode("Save changes");
        saveButton.appendChild(node);
        saveButton.addEventListener("click", function () {
            saveButton.remove();
            e.style.display = "inline-block";
            qtyElem.setAttribute('contenteditable', false);

            var qtyElemNum = +qtyElem.innerHTML;

            if (isNaN(qtyElemNum) || qtyElemNum !== parseInt(qtyElemNum, 10) || qtyElemNum < 0) {
                alert("Incorrect book quantity value: " + qtyElem.innerHTML);
                qtyElem.innerHTML = qtyElemOldVal;
                return;
            }

            $.ajax({
                type: "POST",
                url: "/Home/ChangeQuantity",
                data: {
                    'quantity': qtyElem.innerHTML,
                    'bookId': bookId
                },
                success: function (){
                    var qtyElemVal = +qtyElem.innerHTML;
                    var takeBookElem = findElement(e, "class", 'button-take'); 

                    if (takeBookElem != null) {

                        if (qtyElemVal > 0) {
                            takeBookElem.removeAttribute('disabled');
                        }
                        else {
                            takeBookElem.setAttribute('disabled', true);
                        }

                    }
                },
                error: function (xhr, ajaxOptions, throwError) {
                    qtyElem.innerHTML = qtyElemOldVal;
                    switch (xhr.status) {
                        case 400: alert("Client error." + throwError); break;
                        case 500: alert("Internal server error. Try again later."); break;
                    }
                }
            });

        })

        e.after(saveButton);
        e.style.display = "none";
        qtyElem.focus();

    }

    window.bookDetails = function (bookId) {

        $.ajax({
            type: "POST",
            url: "/Home/BookDetails",
            data: {
                'bookId': bookId
            },
            success: function (data) {
                $("#details").html(data);
            },
            error: errorFunc
        });
    }

    window.deleteBook = function (bookId) {

        $.ajax({
            type: "POST",
            url: "/Home/DeleteBook",
            data: {
 //               'rowsInPage': rowsInPage.value,
                'bookId': bookId
            },
            success: function (data) {
                $("#container").html(data);
                grid.pageNumber = 0;
                grid.pageSiz = 3;
                grid.fieldSortBy = 'title';
                grid.asc = true;
            },
            error: errorFunc
        });
    }
    
});

