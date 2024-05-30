using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Configuration;
using System.Linq;

namespace Library_Management_System.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public class BookController : Controller
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "Library2";
        public string ContainerName = "Books";
        public Container container;



        public BookController()
        {
            container = GetContainer();
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> AddBook(BookModel bookModel)
        {
            if (bookModel == null || string.IsNullOrEmpty(bookModel.Title))
            {
                return BadRequest("Please provide a valid book model with at least a title.");
            }
            bookModel.UId = Guid.NewGuid().ToString();

            var book = await container.CreateItemAsync(bookModel);
            return Ok(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookById(string bookId)
        {
            if (string.IsNullOrEmpty(bookId))
            {
                return BadRequest("Please provide a valid book ID.");
            }
            var book = container.GetItemLinqQueryable<BookModel>(true).Where(q => q.UId == bookId).FirstOrDefault();
            if (book == null)
            {
                return NotFound("No book found with the provided ID.");
            }

            return Ok(book);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = container.GetItemLinqQueryable<BookModel>(true).ToList();
            if (books == null || !books.Any())
            {
                return NotFound("No members found.");
            }
            return Ok(books);
        }

        [HttpGet]
        public async Task<IActionResult> GetIssuedBooks()
        {

            var issuedBooks = container.GetItemLinqQueryable<BookModel>(true).Where(q => q.IsIssued == true).ToList();

            if (issuedBooks.Any())
            {
                return Ok(issuedBooks);
            }
            else
            {
                return NotFound("No issued books found.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableBooks()
        {

            var availableBooks = container.GetItemLinqQueryable<BookModel>(true).Where(q => q.IsIssued == true).ToList();

            if (availableBooks.Any())
            {
                return Ok(availableBooks);
            }
            else
            {
                return NotFound("No available books found.");
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetBookByName(string bookName)
        {
            if (string.IsNullOrEmpty(bookName))
            {
                return BadRequest("Please provide a book name to search.");
            }



            var book = container.GetItemLinqQueryable<BookModel>(true).Where(q => q.Title.Contains(bookName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (book == null)
            {
                return NotFound("No book found with the provided name.");
            }

            return Ok(book);

        }
        [HttpPut]
        public async Task<IActionResult> UpdateBook(BookModel bookModel)
        {
            if (bookModel == null || string.IsNullOrEmpty(bookModel.UId))
            {
                return BadRequest("Please provide a valid book model with an ID for update.");
            }
            var existingBook = container.GetItemLinqQueryable<BookModel>(true).Where(q => q.UId == bookModel.UId).FirstOrDefault();
            if (existingBook == null)
            {
                return NotFound("No book found with the provided ID.");
            }
            existingBook.Title = bookModel.Title;
            existingBook.Author = bookModel.Author;
            existingBook.PublishedDate = bookModel.PublishedDate;
            existingBook.ISBN = bookModel.ISBN;
            existingBook.IsIssued = bookModel.IsIssued;

            await container.ReplaceItemAsync(existingBook, existingBook.UId);
            return Ok(existingBook);

        }
        
        [HttpPost]
        private Container GetContainer()
        {
            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database database = cosmosClient.GetDatabase(DatabaseName);
            Container container = database.GetContainer(ContainerName);
            return container;

        }

    }
}
