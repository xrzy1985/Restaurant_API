using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores/mealTypes")]
    [ApiController]
    public class StoreMealTypesController : ControllerBase
    {
        public readonly IConfiguration _config;
        private string _sqlString;
        public StoreMealTypesController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetStoreMealTypes(string storeId)
        {
            if (new ParameterCheck().IsMalicious(storeId))
            {
                return Unauthorized("There was an error with the storeId parameter.");
            }
            try
            {
                _sqlString = $"select type, time from storeMealTypes where storeId = '{storeId}';";
                DataRowCollection data = new GetQuery(_sqlString, _config).GetDataTable().Rows;
                List<StoreMealType> mealTypes = new List<StoreMealType>();
                if (data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (data[i] != null)
                        {
                            mealTypes.Add(new StoreMealType(
                                Convert.ToString(data[i]["type"]),
                                Convert.ToString(data[i]["time"])
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
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "storeId", storeId },
                        { "data", mealTypes }
                    });
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
