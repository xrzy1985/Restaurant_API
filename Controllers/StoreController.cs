﻿using Microsoft.AspNetCore.Mvc;
using Restaurant_API.queries;
using Restaurant_API.models;
using System.Data;

namespace Restaurant_API.Controllers
{
    [Route("api/store")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        public readonly IConfiguration _config;
        private string _sqlString;
        public StoreController(IConfiguration config)
        {
            _config = config;
            _sqlString = "";
        }

        [HttpGet("{storeId}")]
        public ActionResult<object> GetStore(string storeId)
        {
            if (new ParameterCheck().IsMalicious(storeId))
            {
                return Unauthorized("There was an error with the disposition parameter.");
            }
            _sqlString = $"select storeId,storeName,address1,address2,city,state,postalCode from stores where storeId='{storeId}';";
            try
            {
                DataTable dt = new GetQuery(_sqlString, _config).GetDataTable();
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, List<string>> hours = new Dictionary<string, List<string>>();
                    try
                    {
                        string hoursSql = $"select * from storeHours where storeId = '{storeId}'";
                        DataTable storeHoursDataTable = new GetQuery(hoursSql, _config).GetDataTable();
                        DataRowCollection storeHoursCollection = storeHoursDataTable.Rows;
                        if (storeHoursCollection != null)
                        {
                            for(int i = 0; i <  storeHoursCollection.Count; i++)
                            {
                                DataRow storeHoursData = storeHoursCollection[i];
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
                    catch (Exception ex)
                    {
                        StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                    }
                    return Ok(new Dictionary<string, object>()
                    {
                        { "status", StatusCodes.Status200OK },
                        { "data", new Store(
                                Convert.ToString(dt.Rows[0]["storeId"]),
                                Convert.ToString(dt.Rows[0]["storeName"]),
                                Convert.ToString(dt.Rows[0]["address1"]),
                                Convert.ToString(dt.Rows[0]["address2"]),
                                Convert.ToString(dt.Rows[0]["city"]),
                                Convert.ToString(dt.Rows[0]["state"]),
                                Convert.ToInt16(dt.Rows[0]["postalCode"]),
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
