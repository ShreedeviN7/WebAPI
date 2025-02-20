using Microsoft.Data.SqlClient;
using System.Data;

namespace WebAPITemplate.Services
{
    public class ClsDal
    {
        public static List<string> fnExecute(string strQry, string SqlDataSourseConnection, int IntQueryType = 0)
        {
            var ResultDictionary = new List<string>();
            var retScon = new SqlConnection();
            var retScmd = new SqlCommand();
            try
            {
                retScon.ConnectionString = SqlDataSourseConnection;
                retScmd.Connection = retScon;
                retScmd.CommandText = strQry;
                retScmd.CommandType = CommandType.Text;
                retScon.Open();
                retScmd.Transaction = retScon.BeginTransaction();
                switch (IntQueryType)
                {
                    case 1:
                        {
                            retScmd.ExecuteScalar();
                            retScmd.Transaction.Commit();
                            retScon.Close();
                            break;
                        }

                    default:
                        {
                            retScmd.ExecuteNonQuery();
                            retScmd.Transaction.Commit();
                            break;
                        }
                }

                retScon.Close();
                ResultDictionary = new List<string>();
                ResultDictionary.Add("Executed");
                return ResultDictionary;
            }
            catch (SystemException sx)
            {
                retScmd.Transaction.Rollback();
                retScon.Close();
                string StrResultDictionary = sx.Message.ToString();
                ResultDictionary.Add(StrResultDictionary);
                return ResultDictionary;
            }
            catch (Exception ex)
            {
                retScmd.Transaction.Rollback();
                retScon.Close();
                string ExceptionMessage = string.Empty;
                if (ex.InnerException == null)
                {
                    ExceptionMessage = Convert.ToString(ex.Message);
                }
                else
                {
                    ExceptionMessage = Convert.ToString(ex.InnerException.Message);
                }
                string StrResultDictionary = ExceptionMessage.ToString();
                ResultDictionary.Add(StrResultDictionary);
                return ResultDictionary;
            }
        }
        public static List<Dictionary<string, object>> GetDataSetFromSQL(string StrSQLQuery, string SqlDataSourseConnection)
        {
            DataTable tblResult = new DataTable();
            SqlDataReader DataReader;
            using (SqlConnection ConnStr = new SqlConnection(SqlDataSourseConnection))
            {
                ConnStr.Open();
                using (SqlCommand SqlCmd = new SqlCommand(StrSQLQuery, ConnStr))
                {
                    DataReader = SqlCmd.ExecuteReader();
                    tblResult.Load(DataReader);
                    DataReader.Close();
                    ConnStr.Close();
                }
            }
            List<Dictionary<string, object>> ResultDictionary = fnConvertDataTableToListDistionary(tblResult);
            return ResultDictionary;
        }
        private static List<Dictionary<string, object>> fnConvertDataTableToListDistionary(DataTable tblResult)
        {
            List<Dictionary<string, object>> ResultDictionary = new List<Dictionary<string, object>>();
            Dictionary<string, object> ResultRow;
            foreach (DataRow dr in tblResult.Rows)
            {
                ResultRow = new Dictionary<string, object>();
                foreach (DataColumn col in tblResult.Columns)
                {
                    ResultRow.Add(col.ColumnName, dr[col]);
                }
                ResultDictionary.Add(ResultRow);
            }
            return ResultDictionary;
        }
    }
}
