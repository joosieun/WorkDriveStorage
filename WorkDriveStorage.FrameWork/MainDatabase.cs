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

            existTalbe = _db.TableExists("TB_QUERY");
            if (existTalbe != null && existTalbe == false)
            {
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryCreateQuery);
                _db.ExecuteNonQuery(GetQueryString.SQLite.GetQueryInsertQueryData);
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

        public DataTable GetData(string queryName, string version, Dictionary<string, object> parameters)
        {
            Dictionary<string, object> queryParam = new Dictionary<string, object>();
            queryParam.Add("Name", queryName);
            queryParam.Add("Version", version);
            DataTable dt = _db.GetDataTable(GetQueryString.SQLite.GetQueryString, queryParam);

            if (dt.Rows.Count > 0)
            {
                string queryStr = dt.Rows[0]["Query"].ToString();
                return _db.GetDataTable(queryStr, parameters);
            }
            else
            {
                throw new Exception("The item could not be found. [ " + queryName + "( " + version + " ) ]");
            }
        }

        public DataTable GetData(string queryName, string version)
        {
            Dictionary<string, object> queryParam = new Dictionary<string, object>();
            queryParam.Add("Name", queryName);
            queryParam.Add("Version", version);
            DataTable dt = _db.GetDataTable(GetQueryString.SQLite.GetQueryString, queryParam);

            if (dt.Rows.Count > 0)
            {
                string queryStr = dt.Rows[0]["Query"].ToString();
                return _db.GetDataTable(queryStr);
            }
            else
            {
                throw new Exception("The item could not be found. [ " + queryName + "( " + version + " ) ]");
            }
        }

        public bool SetData(string queryName, string version, Dictionary<string, object> parameters)
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
    }
}
