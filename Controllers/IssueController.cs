using Library_Management_System.Entity;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
namespace Library_Management_System.Controllers
{
    [Route("api/[Controller]/[Action]")]
    [ApiController]
    public class IssueController :  Controller
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "Library2";
        public string ContainerName = "Issued Details";
        public Container container;

        public IssueController()
        {
            container = GetContainer();
        }
        [HttpPost]
        public async Task<ActionResult<IssueModel>> IssueBookEntity(IssueModel issueModel)
        {
            if (issueModel == null || string.IsNullOrEmpty(issueModel.BookId) || string.IsNullOrEmpty(issueModel.MemberId))
            {
                return BadRequest("Please provide a valid issue model with book and user IDs.");
            }
            //1. create obj of entity and mapp all the fields from model to entity
            IssuedBookEntity issuedBook = new IssuedBookEntity();
            issuedBook.UId = Guid.NewGuid().ToString();
            issuedBook.BookId = issueModel.BookId;
            issuedBook.MemberId = issueModel.MemberId;
            issuedBook.IsReturned = issueModel.IsReturned;
            issuedBook.IssueDate = issueModel.IssueDate;
            issuedBook.ReturnDate = issueModel.ReturnDate;
            issuedBook.LibrarianName = issueModel.LibrarianName;

            //2. Assign values to madatory fields
            issuedBook.Id = Guid.NewGuid().ToString();
            issuedBook.DocumentType = "issuedBook";

            issuedBook.CreatedBy = issuedBook.LibrarianName;
            issuedBook.CreatedOn = DateTime.Now;
            
            issuedBook.Version = 1;
            issuedBook.Active = true;
            issuedBook.Archived = false;

            //3. Add the data to database
            IssuedBookEntity response = await container.CreateItemAsync(issuedBook);

            //4. return the model
            IssueModel responseModel = new IssueModel();
            responseModel.UId = response.UId;
            responseModel.BookId = response.BookId;
            responseModel.MemberId = response.MemberId;
            responseModel.IssueDate = response.IssueDate;
            responseModel.ReturnDate = response.ReturnDate;
            responseModel.IsReturned = response.IsReturned;

            return Ok(responseModel);
        }

        [HttpGet]
        public async Task<ActionResult<IssueModel>> GetIssueById(string issueId)
        {
            if (string.IsNullOrEmpty(issueId))
            {
                return BadRequest("Please provide a valid issue ID.");
            }

            var issue = container.GetItemLinqQueryable<IssueModel>(true).Where(q => q.UId == issueId).FirstOrDefault();

            if (issue == null)
            {
                return NotFound("No issue found with the provided ID.");
            }

            return Ok(issue);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateIssue(IssueModel issueModel)
        {
            if (issueModel == null || string.IsNullOrEmpty(issueModel.UId))
            {
                return BadRequest("Please provide a valid issue model with an ID for update.");
            }


            //1. get the existing record by UId
            var existingIssue = container.GetItemLinqQueryable<IssuedBookEntity>(true).Where(q => q.UId == issueModel.UId && q.Active == true && q.Archived == false).FirstOrDefault();

            if (existingIssue == null)
            {
                return NotFound("No Member found with the provided ID.");
            }
            //2. Replace the records
            existingIssue.Archived = true;
            existingIssue.Active = false;

            //3. Assign the values for mandatory fields

            existingIssue.Id = Guid.NewGuid().ToString();
            existingIssue.UpdatedBy = existingIssue.LibrarianName ;
            existingIssue.UpdatedOn = DateTime.Now;
            existingIssue.Version = existingIssue.Version + 1;
            existingIssue.Active = true;
            existingIssue.Archived = false;

            //4. Assign the values to the fields which we will get from request obj
            existingIssue.BookId = issueModel.BookId;
            existingIssue.MemberId = issueModel.MemberId;
            existingIssue.ReturnDate = issueModel.ReturnDate;
            existingIssue.IssueDate = issueModel.IssueDate;
            existingIssue.IsReturned = issueModel.IsReturned;
            existingIssue.LibrarianName = issueModel.LibrarianName;

            //5. Add the data to database

            existingIssue = await container.CreateItemAsync(existingIssue);
            //6. Return

            IssueModel response = new IssueModel();
          

            response.BookId = existingIssue.BookId;
            response.MemberId = existingIssue.MemberId;
            response.ReturnDate = existingIssue.ReturnDate;
            response.IssueDate = existingIssue.IssueDate;
            response.IsReturned = existingIssue.IsReturned;
            response.LibrarianName = existingIssue.LibrarianName;

            return Ok(response);
        }
    
    private Container GetContainer()
        {
            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database database = cosmosClient.GetDatabase(DatabaseName);
            Container container = database.GetContainer(ContainerName);
            return container;

        }
    }
}
