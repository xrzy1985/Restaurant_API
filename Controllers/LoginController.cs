using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        // @TODO: Implement JWT creation, JWT Validation, JWT Revalidation
        private SqlConnection _conn;
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
        }

        [HttpPost]
        public ActionResult<object> Login(Login login)
        {
            try {
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter("select uuid from users where email = @Email;", _conn);
                dataAdapter.SelectCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 2000).Value = login.Email;
                dataAdapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    dataAdapter = new SqlDataAdapter("select u.email,l.password from users as u inner join login as l on u.uuid=@Uuid and l.uuid=@Uuid;", _conn);
                    dataAdapter.SelectCommand.Parameters.Add("@Uuid", SqlDbType.NVarChar, 2000).Value = Convert.ToString(dt.Rows[0]["Uuid"]);
                    dt.Clear();
                    dataAdapter.Fill(dt);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (Convert.ToString(dt.Rows[0]["password"]).Equals(login.Password))
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
                                { "message", "Login was unsuccessful, an invalid password provided." }
                            });
                        }
                    }
                    else
                    {
                        return NotFound(new Dictionary<string, object>()
                        {
                            { "status", StatusCodes.Status404NotFound },
                            { "message", "Login was unsuccessful, no user was data found." }
                        });
                    }
                } else
                {
                    return NotFound(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "Login Unsuccessful, no user was data found." }
                    });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
