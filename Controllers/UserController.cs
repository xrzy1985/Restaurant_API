using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using Restaurant_API.queries;
using Restaurant_API.response;
using System;
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

        [HttpGet("{uuid}")]
        public ActionResult<object> GetUser(string uuid)
        {
            try
            {
                if (new ParamaterCheck().IsMalicious(uuid))
                {
                    return new ErrorResponse(500, "There was an error with the uuid parameter.");
                }
                DataTable dataTable = new GetQuery($"select uuid, name, email, dob from users where uuid = '{uuid}'", _config).GetDataTable();
                if (dataTable != null)
                {
                    DataRow data = dataTable.Rows[0];
                    return new GetResponse(200, new User(
                        Convert.ToString(data["uuid"]),
                        Convert.ToString(data["name"]),
                        Convert.ToString(data["email"]),
                        (DateTime)data["dob"]
                    ));
                } else
                {
                    return new ErrorResponse(404, "There was an error fetching the data.");
                }
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<object> PostUser(NewUser user)
        {
            string userSql = $"insert into users (uuid, name, email, dob) values (@Uuid, @Name, @Email, @Dob)";
            string passSql = $"insert into login (uuid, password) values(@Uuid, @Password)";
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection connection = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
            SqlCommand command = new SqlCommand();
            try
            {
                connection.Open();
                command = new SqlCommand(userSql, connection);
                string userUuid = Guid.NewGuid().ToString();
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                    DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new SqlCommand(passSql, connection);
                command.Parameters.Add("@Uuid", SqlDbType.VarChar, 50).Value = userUuid;
                command.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = user.Password;
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                connection.Close();
                return new Dictionary<string, object>()
                {
                    { "status", 201 },
                    { "message", "The user was created successfully." }
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }

        [HttpPut]
        public ActionResult<object> PutUser(ExistingUser user)
        {
            try
            {
                if (Convert.ToString(user.Uuid).Contains(";") ||
                    Convert.ToString(user.Uuid).Contains("drop") ||
                    string.IsNullOrEmpty(Convert.ToString(user.Uuid)))
                {
                    return new ErrorResponse(500, "There was an error with the uuid parameter.");
                }
                DataTable dataTable = 
                    new GetQuery($"select uuid, name, email, dob from users where uuid = '{user.Uuid}';", _config).GetDataTable();
                DataRow data = dataTable.Rows[0];
                if (data != null)
                {
                    string updateSqlString = $"update users set name=@Name,email=@Email,dob=@Dob WHERE uuid='{user.Uuid}';";
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    SqlConnection connection = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
                    SqlCommand command = new SqlCommand();
                    connection.Open();
                    command = new SqlCommand(updateSqlString, connection);
                    command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = user.Name;
                    command.Parameters.Add("@Email", SqlDbType.VarChar, 50).Value = user.Email;
                    command.Parameters.Add("@Dob", SqlDbType.VarChar, 50).Value =
                        DateTime.Parse(DateOnly.FromDateTime(user.Dob).ToString()).ToString("yyyy-MM-dd");
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    connection.Close();
                    return new Dictionary<string, object>()
                    {
                        { "status", 204 },
                        { "message", "The user was successfully updated." }
                    };
                }
                else
                {
                    return new Dictionary<string, object>()
                    {
                        { "status", 404 },
                        { "message", "No user exists with that UUID." }
                    };
                }
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }

        [HttpDelete("{uuid}")]
        public ActionResult<object> DeleteUser(string uuid)
        {
            try
            {
                if (uuid.Contains(";") || uuid.Contains("drop") || string.IsNullOrEmpty(uuid))
                {
                    return new ErrorResponse(500, "There was an error with the uuid parameter.");
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
                                return new Dictionary<string, object>()
                                {
                                    { "status", 202 },
                                    { "message", "The user was successfully deleted." }
                                };
                            } else
                            {
                                return new Dictionary<string, object>()
                                {
                                    { "status", 400 },
                                    { "message", "There was an issue removing the user." }
                                };
                            }
                        } catch(Exception ex)
                        {
                            return new ErrorResponse(500, ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ErrorResponse(500, ex.Message);
                    }
                }
                else
                {
                    return new Dictionary<string, object>()
                    {
                        { "status", 404 },
                        { "message", "No user exists with that UUID." }
                    };
                }
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }
    }
}
