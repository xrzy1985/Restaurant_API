using System.Data;
using Microsoft.Data.SqlClient;

namespace Restaurant_API.queries
{
    public class Query
    {
        public DataTable dt { get; set; }
        public Query(string sql, IConfiguration config)
        {
            dt = new DataTable();
            new SqlDataAdapter(sql,
                new SqlConnection(config.GetConnectionString("Restaurant").ToString())).Fill(dt);
        }
        public DataTable GetDataTable() { return dt; }
    }
}
