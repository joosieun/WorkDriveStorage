using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkDriveStorage.FrameWork.Database
{
    public class SQLiteManager
    {
        SQLiteConnection DBConn;

        #region SqliteManager 생성자.

        public SQLiteManager(string TargetFilePath, string name, bool ReadOnly = false)
        {
            string filePath = TargetFilePath + "\\" + name + ".db";

            DirectoryInfo di = new DirectoryInfo(TargetFilePath);

            if (!di.Exists)
            {
                di.Create();
            }

            if (!System.IO.File.Exists(filePath))
            {
                SQLiteConnection.CreateFile(filePath);
            }

            string connString = String.Empty;

            if (ReadOnly)
            {
                connString = "Data Source=" + TargetFilePath + "\\" + name + ".db; Mode=ReadOnlys";
            }
            else
            {
                connString = "Data Source=" + TargetFilePath + "\\" + name + ".db";
            }

            this.DBConn = new SQLiteConnection(connString);
        }

        #endregion

        #region SqliteManager 소멸자.

        public void Dispose()
        {
            if (DBConn != null)
            {
                this.DBConn.Close();
                DBConn = null;
            }
        }

        #endregion

        public bool? TableExists(string tableName)
        {
            string queryStr = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "';";

            DataTable dt = GetDataTable(queryStr);

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void CreateTable(string name, DataTable dt)
        {
            try
            {
                StringBuilder queryStr = new StringBuilder();
                queryStr.AppendLine("CREATE TABLE IF NOT EXISTS " + name + " (");

                int lastCount = 1;
                foreach (DataColumn col in dt.Columns)
                {
                    queryStr.Append(col.ColumnName + " " + GetTableType(col.DataType.ToString()));
                    if (dt.Columns.Count != lastCount)
                    {
                        queryStr.AppendLine(",");
                    }

                    lastCount++;
                }

                queryStr.AppendLine(")");

                ExecuteNonQuery(queryStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecuteNonQuery(string queryString)
        {
            int count = 0;
            using (SQLiteConnection conn = new SQLiteConnection(DBConn.ConnectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(queryString, conn);
                count = cmd.ExecuteNonQuery();
                conn.Close();
            }

            return count;
        }

        public int ExecuteNonQuery(string queryString, Dictionary<string, object> parameters)
        {
            int count = 0;
            queryString = MergeQuery(queryString, parameters);

            using (SQLiteConnection conn = new SQLiteConnection(DBConn.ConnectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(queryString, conn);
                count = cmd.ExecuteNonQuery();
                conn.Close();
            }

            return count;
        }

        public DataTable GetDataTable(string queryString)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(DBConn.ConnectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(queryString, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
            }
            return dt;
        }

        public DataTable GetDataTable(string queryString, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            queryString = MergeQuery(queryString, parameters);

            using (SQLiteConnection conn = new SQLiteConnection(DBConn.ConnectionString))
            {
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(queryString, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
                conn.Close();
            }
            return dt;
        }

        private string GetTableType(string type)
        {
            string TargetType = string.Empty;
            switch (type)
            {
                case "System.String":
                    TargetType = "TEXT";
                    break;
                case "System.Int32":
                    TargetType = "INTEGER";
                    break;
            }

            return TargetType;
        }

        private string MergeQuery(string queryString, Dictionary<string, object> parameters)
        {
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                if (queryString.IndexOf("@@" + parameter.Key.ToString()) > 0)
                {
                    queryString = queryString.Replace("@@" + parameter.Key.ToString(), parameter.Value.ToString());
                }
                else
                {
                    queryString = queryString.Replace("@" + parameter.Key.ToString(), "'" + parameter.Value.ToString() + "'");
                }
            }

            return queryString;
        }

        public DateTime? GetDateTime()
        {
            DateTime? dateTime;
            string queryString = "SELECT datetime('now','localtime') as DateTime;";
            DataTable dt = GetDataTable(queryString);
            dateTime = DateTime.Parse(dt.Rows[0]["DateTime"].ToString());
            return dateTime;
        }
    }
}
