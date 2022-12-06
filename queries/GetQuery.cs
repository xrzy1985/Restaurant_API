using System.Data;
using Microsoft.Data.SqlClient;

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
