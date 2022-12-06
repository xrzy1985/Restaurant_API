using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using System.Data;
using System.Data.SqlTypes;

namespace Restaurant_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IConfiguration _config;
        private string _sqlString;
        public UsersController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpGet]
        public ActionResult<object> GetUsers()
        {
            try
            {
                _sqlString = "select * from users";
                DataTable dataTable = new GetQuery(_sqlString, _config).GetDataTable();
                List<User> usersList = new List<User>();
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow data = dataTable.Rows[i];
                        if (data != null)
                        {
                            usersList.Add(
                                new User(
                                    Convert.ToString(data["uuid"]),
                                    Convert.ToString(data["name"]),
                                    Convert.ToString(data["email"]),
                                    (DateTime)data["dob"]
                                )
                            );
                        }
                    }
                }
                return Ok(new Dictionary<string, object>()
                {
                    { "status", StatusCodes.Status200OK },
                    { "data", usersList }
                });
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
