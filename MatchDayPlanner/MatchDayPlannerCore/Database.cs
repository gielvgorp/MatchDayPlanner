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

        public void Insert(string query)
        {
            try
            {
                using (SqlConnection conneciton = new SqlConnection(connectionString))
                {
                    conneciton.Open();
                    using (SqlCommand command = new SqlCommand(query, conneciton))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption" + ex.ToString());
                throw;
            }
        }

        public void Delete(string query)
        {
            try
            {
                using (SqlConnection conneciton = new SqlConnection(connectionString))
                {
                    conneciton.Open();
                    using (SqlCommand command = new SqlCommand(query, conneciton))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exeption" + ex.ToString());
                throw;
            }
        }
    }
}
