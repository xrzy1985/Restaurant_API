using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using Restaurant_API.queries;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        public readonly IConfiguration _config;
        public StoresController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult<object> GetStores()
        {
            string storesSql = "select storeId, storeName, address1, address2, city, state, postalCode from stores";
            try
            {
                DataTable dataTable = new GetQuery(storesSql, _config).GetDataTable();
                List<Store> stores = new List<Store>();
                if (dataTable != null)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataRow data = dataTable.Rows[i];
                        if (new ParameterCheck().IsMalicious(Convert.ToString(data["storeId"])))
                        {
                            return Unauthorized("There was an error with the storeId parameter.");
                        }
                        string hoursSql = $"select * from storeHours where storeId = '{Convert.ToString(data["storeId"])}'";
                        DataTable storeHoursDataTable = new GetQuery(hoursSql, _config).GetDataTable();
                        Dictionary<string, List<string>> hours = new Dictionary<string, List<string>>();
                        for (int j = 0; j < storeHoursDataTable.Rows.Count; j++)
                        {
                            DataRow storeHoursData = storeHoursDataTable.Rows[j];
                            if (storeHoursData != null)
                            {
                                if (hours.ContainsKey(Convert.ToString(storeHoursData["dayOfWeek"])))
                                {
                                    List<string> existingList = new List<string>(hours[Convert.ToString(storeHoursData["dayOfWeek"])]);
                                    string newHours = Convert.ToString(storeHoursData["hours"]);
                                    if (!string.IsNullOrEmpty(newHours))
                                    {
                                        existingList.Add(Convert.ToString(storeHoursData["hours"]));
                                    }
                                    hours[Convert.ToString(storeHoursData["dayOfWeek"])] = existingList;
                                }
                                else
                                {
                                    List<string> newList = new List<string>();
                                    string newHours = Convert.ToString(storeHoursData["hours"]);
                                    if (!string.IsNullOrEmpty(newHours))
                                    {
                                        newList.Add(Convert.ToString(storeHoursData["hours"]));
                                    }
                                    hours[Convert.ToString(storeHoursData["dayOfWeek"])] = newList;
                                }
                            } else
                            {
                                return BadRequest("There was an issue fetching the stores data.");
                            }
                        }
                        stores.Add(new Store(
                            Convert.ToString(data["storeId"]),
                            Convert.ToString(data["storeName"]),
                            Convert.ToString(data["address1"]),
                            Convert.ToString(data["address2"]),
                            Convert.ToString(data["city"]),
                            Convert.ToString(data["state"]).ToUpper(),
                            Convert.ToInt16(data["postalCode"]),
                            hours
                        ));
                    }
                    return Ok(new Dictionary<string, object> {
                        { "status", StatusCodes.Status200OK },
                        { "data", stores }
                    });
                } else
                {
                    return BadRequest("There was an issue fetching the stores data.");
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
