using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DataTable dt = new DataTable();
        public readonly IConfiguration _config;
        private SqlConnection _conn;
        public UserController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
        }

        [HttpGet("{uuid}")]
        public ActionResult<object> GetUser(string uuid)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select uuid, name, email, dob from users where uuid=@Uuid", _conn);
                da.SelectCommand.Parameters.Add("@Uuid", SqlDbType.NVarChar, 2000).Value = uuid;
                da.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return Ok(new Dictionary<string, object> {
                        { "status", StatusCodes.Status200OK },
                        { "data", new User(
                            Convert.ToString(dt.Rows[0]["uuid"]),
                            Convert.ToString(dt.Rows[0]["name"]),
                            Convert.ToString(dt.Rows[0]["email"]),
                            (DateTime)dt.Rows[0]["dob"]
                        )}
                    });
                } else
                {
                    return NotFound(new Dictionary<string, object>() {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "There was an error fetching the data." }
                    });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<object> PostUser(NewUser user)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            try
            {
                _conn.Open();
                command = new SqlCommand("insert into users (uuid, name, email, dob) values (@Uuid, @Name, @Email, @Dob)", _conn);
                string userUuid = Guid.NewGuid().ToString();
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                    DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new SqlCommand("insert into login (uuid, password) values(@Uuid, @Password)", _conn);
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = user.Password;
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                _conn.Close();
                return Created("The user was created successfully.", null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public ActionResult<object> PutUser(ExistingUser user)
        {
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                dt.Clear();
                SqlCommand command = new SqlCommand("select uuid, name, email, dob from users where uuid=@Uuid;", _conn);
                adapter = new SqlDataAdapter(command);
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = user.Uuid;
                adapter.Fill(dt);
                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    _conn.Open();
                    command = new SqlCommand("update users set name=@Name,email=@Email,dob=@Dob WHERE uuid=@Uuid;", _conn);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                    command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                        DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                    command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = user.Uuid;
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    _conn.Close();
                    return StatusCode(StatusCodes.Status200OK, new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "message", "The user was successfully updated." }
                    });
                }
                else
                {
                    return NotFound("No user exists with that UUID.");
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{uuid}")]
        public ActionResult<object> DeleteUser(string uuid)
        {
            try
            {
                dt.Clear();
                SqlDataAdapter da = new SqlDataAdapter("select * from users where uuid=@Uuid;", _conn);
                da.SelectCommand.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = uuid;
                da.Fill(dt);
                DataRow data = dt.Rows[0];
                if (data != null)
                {
                    _conn.Open();
                    dt.Clear();
                    SqlCommand command = new SqlCommand("delete from users where uuid=@Uuid", _conn);
                    command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = uuid;
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    command = new SqlCommand("delete from users where uuid=@Uuid", _conn);
                    command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = uuid;
                    da = new SqlDataAdapter(command);
                    da.Fill(dt);
                    if (dt.Rows.Count < 1)
                    {
                        _conn.Close();
                        return StatusCode(StatusCodes.Status202Accepted, new Dictionary<string, object>()
                            {
                                { "status", StatusCodes.Status202Accepted },
                                { "message", "The user was successfully deleted." }
                            }
                        );
                    }
                    else
                    {
                        _conn.Close();
                        return BadRequest("There was an issue removing the user.");
                    }
                }
                else
                {
                    return NotFound(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "No user exists with that UUID." }
                    });
                }
            } catch(Exception ex)
            {
                _conn.Close();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
