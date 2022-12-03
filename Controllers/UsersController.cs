using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.response;
using Restaurant_API.Queries;
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
                DataTable dataTable = new Query("select * from users", _config).GetDataTable();
                List<Users> usersList = new List<Users>();
                if (dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow data = dataTable.Rows[i];
                        if (data != null)
                        {
                            usersList.Add(
                                new Users(
                                    Convert.ToInt32(data["id"]),
                                    Convert.ToString(data["uuid"]),
                                    Convert.ToString(data["name"]),
                                    Convert.ToString(data["email"]),
                                    (DateTime)data["dob"]
                                )
                            );
                        }
                    }
                }
                return new Response(200, usersList);
            } catch(Exception ex)
            {
                return new Error(500, ex.Message);
            }
        }

    }
}
