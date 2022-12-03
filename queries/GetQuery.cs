using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Restaurant_API.models;
using Restaurant_API.response;

namespace Restaurant_API.queries
{
    public class GetQuery
    {
        public DataTable dt { get; set; }
        public GetQuery() { }
        public GetQuery(string sql, IConfiguration config)
        {
            dt = new DataTable();
            new SqlDataAdapter(sql,
                new SqlConnection(config.GetConnectionString("Restaurant").ToString())).Fill(dt);
        }
        public DataTable GetDataTable() { return dt; }
    }
}
