using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using Restaurant_API.response;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<object> GetUser()
        {
            try
            {
                DataTable dataTable = new Query("select uuid, name, email, dob from users where id = 1", _config).GetDataTable();

                if (dataTable != null)
                {
                    DataRow data = dataTable.Rows[0];
                    return new Response(200, new User(
                        Convert.ToString(data["uuid"]),
                        Convert.ToString(data["name"]),
                        Convert.ToString(data["email"]),
                        (DateTime)data["dob"]
                    ));
                } else
                {
                    return new Error(400, "There was an error fetching the data.");
                }
            } catch(Exception ex)
            {
                return new Error(500, ex.Message);
            }
        }
    }
}
