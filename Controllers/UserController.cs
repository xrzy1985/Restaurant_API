using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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

        [HttpGet("{id}")]
        public ActionResult<object> GetUser(int id)
        {
            try
            {
                DataTable dataTable = new GetQuery($"select uuid, name, email, dob from users where id = {id}", _config).GetDataTable();

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
                    return new ErrorResponse(400, "There was an error fetching the data.");
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
                    { "status", 200 },
                    { "message", "The user was created successfully." }
                };
            }
            catch (Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }
    }
}
