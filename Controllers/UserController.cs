using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using Restaurant_API.queries;
using System;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IConfiguration _config;
        private string _sqlString;
        public UserController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpGet("{uuid}")]
        public ActionResult<object> GetUser(string uuid)
        {
            try
            {
                if (new ParameterCheck().IsMalicious(uuid))
                {
                    return Unauthorized("There was an error with the uuid parameter.");
                }
                DataTable dataTable = new GetQuery($"select uuid, name, email, dob from users where uuid = '{uuid}'", _config).GetDataTable();
                if (dataTable != null)
                {
                    DataRow data = dataTable.Rows[0];
                    return Ok(new Dictionary<string, object> {
                        { "status", StatusCodes.Status200OK },
                        { "data", new User(
                            Convert.ToString(data["uuid"]),
                            Convert.ToString(data["name"]),
                            Convert.ToString(data["email"]),
                            (DateTime)data["dob"]
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
            _sqlString = $"insert into users (uuid, name, email, dob) values (@Uuid, @Name, @Email, @Dob)";
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection connection = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
            SqlCommand command = new SqlCommand();
            try
            {
                connection.Open();
                command = new SqlCommand(_sqlString, connection);
                string userUuid = Guid.NewGuid().ToString();
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                    DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                _sqlString = $"insert into login (uuid, password) values(@Uuid, @Password)";
                command = new SqlCommand(_sqlString, connection);
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = user.Password;
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
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
                if (new ParameterCheck().IsMalicious(user.Uuid))
                {
                    return Unauthorized("There was an error with the uuid parameter.");
                }
                DataTable dataTable = 
                    new GetQuery($"select uuid, name, email, dob from users where uuid = '{user.Uuid}';", _config).GetDataTable();
                DataRow data = dataTable.Rows[0];
                if (data != null)
                {
                    _sqlString = $"update users set name=@Name,email=@Email,dob=@Dob WHERE uuid='{user.Uuid}';";
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlConnection connection = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
                    SqlCommand command = new SqlCommand();
                    connection.Open();
                    command = new SqlCommand(_sqlString, connection);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                    command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                        DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    connection.Close();
                    return StatusCode(StatusCodes.Status204NoContent, "The user was successfully updated.");
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
                if (new ParameterCheck().IsMalicious(uuid))
                {
                    return Unauthorized("There was an error with the uuid parameter.");
                }
                DataTable dataTable =
                    new GetQuery($"select * from users where uuid = '{uuid}';", _config).GetDataTable();
                DataRow data = dataTable.Rows[0];
                if (data != null)
                {
                    try
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        SqlConnection connection = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
                        SqlCommand command = new SqlCommand();
                        connection.Open();
                        command = new SqlCommand($"delete from users where uuid='{uuid}'", connection);
                        adapter.InsertCommand = command;
                        adapter.InsertCommand.ExecuteNonQuery();
                        connection.Close();
                        try
                        {
                            DataTable dt = new GetQuery($"select * from users where uuid = '{uuid}';", _config).GetDataTable();
                            if (dt.Rows.Count < 1)
                            {
                                return StatusCode(StatusCodes.Status202Accepted, new Dictionary<string, object>()
                                {
                                    { "status", StatusCodes.Status202Accepted },
                                    { "message", "The user was successfully deleted." }
                                });
                            } else
                            {
                                return BadRequest("There was an issue removing the user.");
                            }
                        } catch(Exception ex)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
