using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using Restaurant_API.response;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores/dispositions")]
    [ApiController]
    public class DispositionsController : ControllerBase
    {
        public readonly IConfiguration _config;
        public DispositionsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetDispositions(string storeId)
        {
            if (new ParameterCheck().IsMalicious(storeId))
            {
                return new ErrorResponse(500, "There was an error with the storeId parameter.");
            }
            string dispositionsSqlString = $"select * from storesDispositions where storeId = '{storeId}';";
            try
            {
                DataTable dt = new GetQuery(dispositionsSqlString, _config).GetDataTable();
                if (dt.Rows.Count > 0)
                {
                    DataRow data = dt.Rows[0];
                    return new Dictionary<string, object>()
                    {
                        { "status", 200 },
                        { "data", new Disposition(
                            Convert.ToString(data["StoreId"]),
                            Convert.ToBoolean(data["Curbside"]),
                            Convert.ToBoolean(data["Delivery"]),
                            Convert.ToBoolean(data["Pickup"])
                        )}
                    };
                }
            } catch(Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
            return storeId;
        }
    }
}
