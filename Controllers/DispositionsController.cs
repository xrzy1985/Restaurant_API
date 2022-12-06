using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores/dispositions")]
    [ApiController]
    public class DispositionsController : ControllerBase
    {
        public readonly IConfiguration _config;
        private string _sqlString;
        public DispositionsController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetDispositions(string storeId)
        {
            if (new ParameterCheck().IsMalicious(storeId))
            {
                return Unauthorized("There was an error with the storeId parameter.");
            }
            _sqlString = $"select * from storesDispositions where storeId = '{storeId}';";
            try
            {
                DataTable dt = new GetQuery(_sqlString, _config).GetDataTable();
                if (dt.Rows.Count > 0)
                {
                    DataRow data = dt.Rows[0];
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "data", new Disposition(
                            Convert.ToString(data["StoreId"]),
                            Convert.ToBoolean(data["Curbside"]),
                            Convert.ToBoolean(data["Delivery"]),
                            Convert.ToBoolean(data["Pickup"])
                        )}
                    });
                }
            } catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return storeId;
        }
    }
}
