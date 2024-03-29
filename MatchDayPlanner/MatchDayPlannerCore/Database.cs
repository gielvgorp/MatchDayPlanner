using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace MatchDayPlannerCore
{
    public class Database
    {
        string connectionString = "Server=DESKTOP-SML637F;Database=MatchDayPlanner;Trusted_Connection=True;TrustServerCertificate=True;";

        public List<T> Query<T>(string query) where T : new()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            List<T> result = ConvertDataTableToList<T>(dt);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption: " + ex.ToString());
                throw;
            }
        }
        private List<T> ConvertDataTableToList<T>(DataTable dataTable) where T : new()
        {
            List<T> list = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();

                foreach (DataColumn col in dataTable.Columns)
                {
                    typeof(T).GetProperty(col.ColumnName)?.SetValue(obj, row[col.ColumnName]);
                }

                list.Add(obj);
            }

            return list;
        }

        public void Insert<T>(List<T> items, string table)
        {
            string insertQuery = $"INSERT INTO {table} VALUES ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var item in items)
                {
                    string values = "";
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.GetValue(item) != null)
                        {
                            values += $"'{property.GetValue(item)}',";
                        }
                      
                    }
                    values = values.TrimEnd(',');

                    string query = $"{insertQuery} ({values})";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
