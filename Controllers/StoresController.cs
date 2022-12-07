using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/stores")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        public readonly IConfiguration _config;
        private SqlConnection _conn;
        public StoresController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
        }

        [HttpGet]
        public ActionResult<object> GetStores()
        {
            try
            {
                DataTable dataTable = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter("select storeId, storeName, address1, address2, city, state, postalCode from stores", _conn);
                da.Fill(dataTable);
                List<Store> stores = new List<Store>();
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        DataTable storeHoursDataTable = new DataTable();
                        da = new SqlDataAdapter("select * from storeHours where storeId=@StoreId", _conn);
                        da.SelectCommand.Parameters.Add("@StoreId", SqlDbType.NVarChar, 2000).Value = Convert.ToString(dataTable.Rows[i]["storeId"]);
                        da.Fill(storeHoursDataTable);
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
                            Convert.ToString(dataTable.Rows[i]["storeId"]),
                            Convert.ToString(dataTable.Rows[i]["storeName"]),
                            Convert.ToString(dataTable.Rows[i]["address1"]),
                            Convert.ToString(dataTable.Rows[i]["address2"]),
                            Convert.ToString(dataTable.Rows[i]["city"]),
                            Convert.ToString(dataTable.Rows[i]["state"]).ToUpper(),
                            Convert.ToInt16(dataTable.Rows[i]["postalCode"]),
                            hours
                        ));
                    }
                    return Ok(new Dictionary<string, object> {
                        { "status", StatusCodes.Status200OK },
                        { "data", stores }
                    });
                } else
                {
                    return BadRequest(new Dictionary<string, object>(){
                        { "status", StatusCodes.Status400BadRequest },
                        { "message", "There was an issue fetching the stores data." }
                    });
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
