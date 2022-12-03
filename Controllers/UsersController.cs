using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.response;
using Restaurant_API.queries;
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
                DataTable dataTable = new GetQuery("select * from users", _config).GetDataTable();
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
                return new GetResponse(200, new Users(usersList));
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }

    }
}
