using Microsoft.AspNetCore.Mvc;
using Restaurant_API.models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Restaurant_API.Controllers
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        public readonly IConfiguration _config;
        private SqlConnection _conn;
        private DataTable _dt;
        public StoreController(IConfiguration config)
        {
            _config = config;
            _conn = new SqlConnection(_config.GetConnectionString("Restaurant").ToString());
            _dt = new DataTable();
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetStore(string storeId)
        {
            try
            {
                SqlDataAdapter dataAdapter =
                    new SqlDataAdapter("select storeId,storeName,address1,address2,city,state,postalCode from stores where storeId=@StoreId;", _conn);
                dataAdapter.SelectCommand.Parameters.Add("@StoreId", SqlDbType.NVarChar, 2000).Value = storeId;
                dataAdapter.Fill(_dt);
                if (_dt.Rows.Count > 0)
                {
                    Dictionary<string, List<string>> hours = new Dictionary<string, List<string>>();
                    DataTable storeHoursDataTable = new DataTable();
                    dataAdapter = new SqlDataAdapter("select * from storeHours where storeId=@StoreId", _conn);
                    dataAdapter.SelectCommand.Parameters.Add("@StoreId", SqlDbType.NVarChar, 2000).Value = storeId;
                    dataAdapter.Fill(storeHoursDataTable);
                    DataRowCollection storeHoursCollection = storeHoursDataTable.Rows;
                    if (storeHoursCollection != null)
                    {
                        for (int i = 0; i <  storeHoursCollection.Count; i++)
                        {
                            DataRow storeHoursData = storeHoursCollection[i];
                            if (storeHoursData != null) {
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
                            }
                        }
                    }
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "data", new Store(
                                Convert.ToString(_dt.Rows[0]["storeId"]),
                                Convert.ToString(_dt.Rows[0]["storeName"]),
                                Convert.ToString(_dt.Rows[0]["address1"]),
                                Convert.ToString(_dt.Rows[0]["address2"]),
                                Convert.ToString(_dt.Rows[0]["city"]),
                                Convert.ToString(_dt.Rows[0]["state"]),
                                Convert.ToInt16(_dt.Rows[0]["postalCode"]),
                                hours
                            )
                        }
                    });
                }
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return NoContent();
        }
    }
}
