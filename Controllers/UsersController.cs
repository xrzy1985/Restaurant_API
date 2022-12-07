using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IConfiguration _config;
        public UsersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<object> GetUsers()
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select * from users", new SqlConnection(_config.GetConnectionString("Restaurant").ToString()));
                da.Fill(dataTable);
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
