using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using Restaurant_API.response;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores/mealTypes")]
    [ApiController]
    public class StoreMealTypesController : ControllerBase
    {
        public readonly IConfiguration _config;
        public StoreMealTypesController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetStoreMealTypes(string storeId)
        {
            if (new ParamaterCheck().IsMalicious(storeId))
            {
                return new ErrorResponse(500, "There was an error with the storeId parameter.");
            }
            try
            {
                string mealTypeSqlString = $"select type, time from storeMealTypes where storeId = '{storeId}';";
                DataRowCollection data = new GetQuery(mealTypeSqlString, _config).GetDataTable().Rows;
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
                    return new Dictionary<string, object>()
                    {
                        { "status", 200 },
                        { "storeId", storeId },
                        { "data", mealTypes }
                    };
                } else
                {
                    return new Dictionary<string, object>()
                    {
                        { "status", 200 },
                        { "storeId", storeId },
                        { "data", mealTypes }
                    };
                }
            } catch (Exception ex)
            {
                return new ErrorResponse(500, ex.Message);
            }
        }
    }
}
