using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkDriveStorage.FrameWork.Database;

namespace WorkDriveStorage.FrameWork
{
    public class MainDatabase
    {
        private SQLiteManager _db;

        public MainDatabase()
        {
            try
            {
                if (_db == null)
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    string name = "WorkDriveStorage";
                    _db = new SQLiteManager(path, name);
                    CheckColumns(GetQueryString.SQLite.GetQueryCreateDocument);
                    CheckColumns(GetQueryString.SQLite.GetQueryCreateProject);
                    CheckColumns(GetQueryString.SQLite.GetQueryCreateSource);
                    CheckColumns(GetQueryString.SQLite.GetQueryCreateWorkList);
                    CheckColumns(GetQueryString.SQLite.GetQueryCreateMemo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateTable()
        {
            bool? existTalbe = _db.TableExists("TB_DOCUMENT");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateDocument);
            }

            existTalbe = _db.TableExists("TB_PROJECT");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateProject);
            }

            existTalbe = _db.TableExists("TB_SOURCE");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateSource);
            }

            existTalbe = _db.TableExists("TB_WORKLIST");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateWorkList);
            }

            existTalbe = _db.TableExists("TB_MEMO");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateMemo);
            }
        }

        public bool CreateDatabase()
        {
            try
            {
                string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\WorkDriveStorage.db";
                if (!System.IO.File.Exists(filePath))
                {
                    SQLiteConnection.CreateFile(filePath);
                }

                CreateTable();

                return true;
            }
            catch
            {
                return false;
            }
        }

        //public DataTable GetDataByName(string queryName, string version, Dictionary<string, object> parameters)
        //{
        //    Dictionary<string, object> queryParam = new Dictionary<string, object>();
        //    queryParam.Add("Name", queryName);
        //    queryParam.Add("Version", version);
        //    DataTable dt = _db.GetDataTable(GetQueryString.SQLite.GetQueryString, queryParam);

        //    if (dt.Rows.Count > 0)
        //    {
        //        string queryStr = dt.Rows[0]["Query"].ToString();
        //        return _db.GetDataTable(queryStr, parameters);
        //    }
        //    else
        //    {
        //        throw new Exception("The item could not be found. [ " + queryName + "( " + version + " ) ]");
        //    }
        //}

        //public DataTable GetDataByName(string queryName, string version)
        //{
        //    Dictionary<string, object> queryParam = new Dictionary<string, object>();
        //    queryParam.Add("Name", queryName);
        //    queryParam.Add("Version", version);
        //    DataTable dt = _db.GetDataTable(GetQueryString.SQLite.GetQueryString, queryParam);

        //    if (dt.Rows.Count > 0)
        //    {
        //        string queryStr = dt.Rows[0]["Query"].ToString();
        //        return _db.GetDataTable(queryStr);
        //    }
        //    else
        //    {
        //        throw new Exception("The item could not be found. [ " + queryName + "( " + version + " ) ]");
        //    }
        //}

        public DataTable GetData(string queryStr)
        {
            return _db.GetDataTable(queryStr);
        }

        public DataTable GetData(string queryStr, Dictionary<string, object> parameters)
        {
            return _db.GetDataTable(queryStr, parameters);
        }

        public bool SetDataByName(string queryName, string version, Dictionary<string, object> parameters)
        {
            Dictionary<string, object> queryParam = new Dictionary<string, object>();
            queryParam.Add("Name", queryName);
            queryParam.Add("Version", version);
            DataTable dt = _db.GetDataTable(GetQueryString.SQLite.GetQueryString, queryParam);

            if (dt.Rows.Count > 0)
            {
                string queryStr = dt.Rows[0]["Query"].ToString();
                int result = _db.ExecuteNonQuery(queryStr, parameters);
                return result > 0 ? true : false;
            }
            else
            {
                throw new Exception("The item could not be found. [ " + queryName + "( " + version + " ) ]");
            }

        }

        public bool SetData(string queryStr, Dictionary<string, object> parameters)
        {
            int result = _db.ExecuteNonQuery(queryStr, parameters);
            return result > 0 ? true : false;
        }

        private void CheckColumns(string query)
        {
            string[] columns = query.Split('\n');

            string tableName = string.Empty;

            foreach (string col in columns)
            {
                string value = col.Replace("\r", "");
                if (string.IsNullOrEmpty(value) == false)
                {
                    if (value.Contains("CREATE TABLE"))
                    {
                        tableName = value.Replace("\t", "").Split('\"')[1];
                    }
                    else
                    {
                        if (value.Contains("PRIMARY KEY") == false && value.Contains(");") == false)
                        {
                            string columnName = value.Replace("\t", "").Split('\"')[1];

                            Dictionary<string, object> parameters = new Dictionary<string, object>();
                            parameters.Add("TableName", tableName);
                            parameters.Add("Value", "%"+ columnName + "%");
                            DataTable dt = GetData(GetQueryString.SQLite.GetColumnCheck, parameters);

                            if (dt.Rows.Count == 0)
                            {
                                Dictionary<string, object> addParameters = new Dictionary<string, object>();
                                addParameters.Add("TableName", tableName);
                                addParameters.Add("ColumName", columnName);
                                SetData(GetQueryString.SQLite.GetColumnAdd, addParameters);
                            }
                        }
                    }
                }
            }
        }

    }
}
