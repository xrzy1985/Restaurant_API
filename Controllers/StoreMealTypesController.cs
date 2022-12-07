using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("stores/mealTypes")]
    [ApiController]
    public class StoreMealTypesController : ControllerBase
    {
        public readonly IConfiguration _config;
        private SqlConnection _conn;
        public StoreMealTypesController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetStoreMealTypes(string storeId)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter("select type, time from storeMealTypes where storeId=@StoreId;", _conn);
                dataAdapter.SelectCommand.Parameters.Add("@StoreId", SqlDbType.NVarChar, 2000).Value = storeId;
                dataAdapter.Fill(dt);
                List<StoreMealType> mealTypes = new List<StoreMealType>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i] != null)
                        {
                            mealTypes.Add(new StoreMealType(
                                Convert.ToString(dt.Rows[i]["type"]),
                                Convert.ToString(dt.Rows[i]["time"])
                            ));
                        }
                    }
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "storeId", storeId },
                        { "data", mealTypes }
                    });
                } else
                {
                    return NotFound(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "No records were found." }
                    });
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
