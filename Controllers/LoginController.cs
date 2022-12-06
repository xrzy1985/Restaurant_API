using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // @TODO: Implement JWT creation, JWT Validation, JWT Revalidation
        public readonly IConfiguration _config;
        private string _sqlString;
        public LoginController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpPost]
        public ActionResult<object> Login(Login login)
        {
            try {
                ParameterCheck test = new ParameterCheck();
                if (test.IsMalicious(login.Email) && test.IsMalicious(login.Password))
                {
                    return Unauthorized("There was an error with the login information provided.");
                }
                _sqlString = $"select uuid from users where email = '{login.Email}';";
                DataTable dt = new GetQuery(_sqlString, _config).GetDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string uuid = Convert.ToString(dt.Rows[0]["Uuid"]);
                    _sqlString = $"select u.email,l.password from users as u inner join login as l on u.uuid='{uuid}' and l.uuid='{uuid}';";
                    DataTable data = new GetQuery(_sqlString, _config).GetDataTable();
                    if (data != null && data.Rows.Count > 0)
                    {
                        if (Convert.ToString(data.Rows[0]["password"]) == login.Password)
                        {
                            return Ok(new Dictionary<string, object>()
                            {
                                { "status", StatusCodes.Status200OK },
                                { "message", "Login was successful" },
                                { "token", "create the token to return here" }
                            });
                        }
                        else
                        {
                            return Unauthorized(new Dictionary<string, object>()
                            {
                                { "status", StatusCodes.Status401Unauthorized },
                                { "message", "Login was unsuccessful, Invalid password provided" }
                            });
                        }
                    }
                    else
                    {
                        return NotFound(new Dictionary<string, object>()
                        {
                            { "status", StatusCodes.Status404NotFound },
                            { "message", "Login was unsuccessful, no user data found" }
                        });
                    }
                } else
                {
                    return NotFound(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "Login Unsuccessful, no user data found" }
                    });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
