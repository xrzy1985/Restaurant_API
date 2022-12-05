using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using Restaurant_API.response;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // @TODO: Implement JWT creation, JWT Validation, JWT Revalidation
        IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public ActionResult<object> Login(Login login)
        {
            try {
                ParameterCheck test = new ParameterCheck();
                if (test.IsMalicious(login.Email) && test.IsMalicious(login.Password))
                {
                    return new ErrorResponse(500, "There was an error with the login information provided.");
                }
                string sqlString = $"select uuid from users where email = '{login.Email}';";
                DataTable dt = new GetQuery(sqlString, _config).GetDataTable();
                if (dt != null && dt.Rows.Count > 0)
                {
                    string uuid = Convert.ToString(dt.Rows[0]["Uuid"]);
                    sqlString = $"select u.email,l.password from users as u inner join login as l on u.uuid='{uuid}' and l.uuid='{uuid}';";
                    try
                    {
                        DataTable data = new GetQuery(sqlString, _config).GetDataTable();
                        if (data != null && data.Rows.Count > 0)
                        {
                            if (Convert.ToString(data.Rows[0]["password"]) == login.Password)
                            {
                                return new Dictionary<string, object>()
                            {
                                { "status", 200 },
                                { "message", "Login was successful" },
                                { "token", "create the token to return here" }
                            };
                            }
                            else
                            {
                                return new Dictionary<string, object>()
                            {
                                { "status", 401 },
                                { "message", "Login was unsuccessful, Invalid password provided" }
                            };
                            }
                        }
                        else
                        {
                            return new Dictionary<string, object>()
                        {
                            { "status", 404 },
                            { "message", "Login was unsuccessful, no user data found" }
                        };
                        }
                    } catch (Exception ex)
                    {
                        return new ErrorResponse(500, ex.Message);
                    }
                } else
                {
                    return new Dictionary<string, object>()
                    {
                        { "status", 404 },
                        { "message", "Login Unsuccessful, no user data found" }
                    };
                }
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }
    }
}
