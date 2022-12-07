using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores/dispositions")]
    [ApiController]
    public class DispositionsController : ControllerBase
    {
        public readonly IConfiguration _config;
        private DataTable _dt;
        public DispositionsController(IConfiguration config)
        {
            _config = config;
            _dt = new DataTable();
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetDispositions(string storeId)
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter("select * from storesDispositions where storeId = @StoreId;",
                    new SqlConnection(_config.GetConnectionString("Restaurant").ToString()));
                da.SelectCommand.Parameters.Add("@StoreId", SqlDbType.NVarChar, 2000).Value = storeId;
                da.Fill(_dt);
                if (_dt.Rows.Count > 0)
                {
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "data", new Disposition(
                            Convert.ToString(_dt.Rows[0]["StoreId"]),
                            Convert.ToBoolean(_dt.Rows[0]["Curbside"]),
                            Convert.ToBoolean(_dt.Rows[0]["Delivery"]),
                            Convert.ToBoolean(_dt.Rows[0]["Pickup"])
                        )}
                    });
                } else
                {
                    return NotFound(new Dictionary<string, object>() {
                        { "status", StatusCodes.Status404NotFound },
                        { "message", "No records were found." }
                    });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
