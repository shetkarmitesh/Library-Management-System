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
    public class MemberController :Controller
    {

        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "Library2";
        public string ContainerName = "Members";
        public Container container;

        public MemberController()
        {
            container = GetContainer();
        }

        [HttpPost]
        public async Task<ActionResult<MemberModel>> AddMember(MemberModel memberModel)
        {
            if (memberModel == null || string.IsNullOrEmpty(memberModel.Name))
            {
                return BadRequest("Please provide a valid book model with at least a title.");
            }
            memberModel.UId = Guid.NewGuid().ToString();

            var addedMember = await container.CreateItemAsync(memberModel);
            return Ok(addedMember);
        }
       

        [HttpGet]
        public async Task<ActionResult<MemberModel>> GetMemberById(string memberId)
        {
            if (string.IsNullOrEmpty(memberId))
            {
                return BadRequest("Please provide a valid member ID.");
            }

            var member = container.GetItemLinqQueryable<MemberModel>(true).Where(q => q.UId == memberId).FirstOrDefault();

            if (member == null)
            {
                return NotFound("No member found with the provided ID.");
            }

            return Ok(member);
        }


        [HttpGet]
        public async Task<ActionResult<List<MemberModel>>> GetAllMembers()
        {
            var members = container.GetItemLinqQueryable<MemberModel>(true).ToList();

            if (members == null || !members.Any())
            {
                return NotFound("No members found.");
            }


            return Ok(members);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMember(MemberModel memberModel)
        {
            if (memberModel == null || string.IsNullOrEmpty(memberModel.UId))
            {
                return BadRequest("Please provide a valid member model with an ID for update.");
            }

            var updatedMember = container.GetItemLinqQueryable<MemberModel>(true).Where(q => q.UId == memberModel.UId).FirstOrDefault();
            if (updatedMember == null)
            {
                return NotFound("No book found with the provided ID.");
            }

            updatedMember.Name = memberModel.Name;
            updatedMember.Email = memberModel.Email;
/*date of birth should be check*/
            updatedMember.DateOfBirth = memberModel.DateOfBirth;

            await container.ReplaceItemAsync(updatedMember, updatedMember.UId);

            return Ok(updatedMember);
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
