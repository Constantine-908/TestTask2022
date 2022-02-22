using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWebLoad
{
    public interface IDAL:IDisposable
    {
        string ConnectionStringTemplate { get; }
        string ConnectionString { get; set; }
        List<T> GetListFromTable<T>(DataTable tbl) where T : new();
        T CreateItemFromRow<T>(DataRow row) where T : new();
        void SetItemFromRow<T>(T item, DataRow row) where T : new();
        SqlCommand GetCommandFromClass<T>(T paramClass,string spName) where T: new();
        DataSet GetDatasetFromClass<T>(T paramClass, string spName) where T : new();
        Task<DataTable> GetTableFromClassAsync<T>(T paramClass, string spName) where T : new();
        DataSet GetDatasetFromSqlText(string sqlText);
        Task<DataTable> GetTableFromSqlTextAsync(string sqlText);
        Task<string> GetScalarFromSqlTextAsync(string sqlText);
        string GetScalarFromSqlText(string sqlText);
        DataSet GetDatasetFromCommand(SqlCommand cmd);
        Task<DataTable> GetTableFromCommandAsync(SqlCommand cmd);

    }

    public class DAL : IDAL
    {

        public string ConnectionStringTemplate { get; } = "Server=tcp:{0},1433;Initial Catalog={1};Persist Security Info=False;Integrated Security=true;Encrypt=True;" +
                                                          "TrustServerCertificate=True;Connection Timeout=30; Application Name={2};Pooling=true;Max Pool Size=1000;";
        public string ConnectionString { get; set; }
        private readonly SqlConnection _con = new SqlConnection();
        private readonly StringBuilder _sqlMessages =new StringBuilder() ;
        private readonly int _connectionTimeout = 30;
        public  DAL(string server="localhost",string defaultdb="master",int connectionTimeout =6000)
        {
            this.ConnectionString = string.Format(this.ConnectionStringTemplate, server,defaultdb, Assembly.GetExecutingAssembly().GetName().Name);
            this._connectionTimeout = connectionTimeout;
            _con.InfoMessage += new SqlInfoMessageEventHandler(myConnection_InfoMessage);
        }



        private void myConnection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            _sqlMessages.AppendLine(e.Message);
        }
        public List<T> GetListFromTable<T>(DataTable tbl) where T : new()
        {
            return (from DataRow r in tbl.Rows select CreateItemFromRow<T>(r)).ToList();
          
        }

    
        public  T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            T item = new T();
            SetItemFromRow(item, row);
            return item;
        }

        public  void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
    
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);
                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, Convert.ChangeType(row[c],p.PropertyType), null);
                }
            }
        }
        public SqlCommand GetCommandFromClass<T>(T paramClass,string spName) where T: new()
        { //return command from class of parameters /name of parameters and class propertys should be same 
            //нужно ли сделать async
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = spName;
            foreach (var prop in paramClass.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(DateTime) )
                {
                   var p=sqlCmd.Parameters.Add("@" + prop.Name, SqlDbType.DateTime);
                   if (Convert.ToDateTime(prop.GetValue(paramClass, null)) == DateTime.MinValue)
                        p.Value = DBNull.Value;
                   else
                        p.Value = prop.GetValue(paramClass, null);
                }
                else
                {
                    sqlCmd.Parameters.AddWithValue("@" + prop.Name, prop.GetValue(paramClass, null) == null ? "" : prop.GetValue(paramClass, null).ToString());
                }
            }
            return sqlCmd;
        }

        public DataSet GetDatasetFromClass<T>(T paramClass, string spName) where T : new()
        {
            var sqlCmd = GetCommandFromClass(paramClass, spName);
            return GetDatasetFromCommand(sqlCmd);

        }
        public Task<DataTable> GetTableFromClassAsync<T>(T paramClass, string spName) where T : new()
        {
            var sqlCmd = GetCommandFromClass(paramClass, spName);
            return GetTableFromCommandAsync(sqlCmd);

        }

        public DataSet GetDatasetFromSqlText(string sqlText)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
          
            if (_con.State == ConnectionState.Closed)
            {
                _con.ConnectionString = ConnectionString;
                _con.Open();
            }
            cmd = new SqlCommand(sqlText, _con);
            cmd.CommandTimeout = _connectionTimeout;
            cmd.CommandType = CommandType.Text;
            da.SelectCommand = cmd;
            da.Fill(ds);
            _con.Close();
            return ds;
        }

        public async Task<DataTable> GetTableFromSqlTextAsync(string sqlText)
        {
           SqlCommand cmd = new SqlCommand();
            if (_con.State == ConnectionState.Closed)
            {
                _con.ConnectionString = ConnectionString;
               await _con.OpenAsync();
            }
            cmd = new SqlCommand(sqlText, _con);
            cmd.CommandTimeout = _connectionTimeout;
            cmd.CommandType = CommandType.Text;
            var dt = new DataTable();
            var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            _con.Close();
            return dt;
        }

        public async Task<string> GetScalarFromSqlTextAsync(string sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();

            if (_con.State == ConnectionState.Closed)
            {
                _con.ConnectionString = ConnectionString;
                _con.Open();
            }
            cmd = new SqlCommand(sqlText, _con);
            cmd.CommandTimeout = _connectionTimeout;
            cmd.CommandType = CommandType.Text;
            await cmd.ExecuteNonQueryAsync();
            _con.Close();
            return _sqlMessages.ToString();
        }
        public string GetScalarFromSqlText(string sqlText)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
           
            if (_con.State == ConnectionState.Closed) 
            {
                _con.ConnectionString = ConnectionString;
                _con.Open();
            }  
            cmd = new SqlCommand(sqlText, _con);
            cmd.CommandTimeout = _connectionTimeout;
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            _con.Close();
            return _sqlMessages.ToString();
        }

        public DataSet GetDatasetFromCommand(SqlCommand cmd)
        { //return dataset from command
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();
            if (_con.State == ConnectionState.Closed)
            {
                _con.ConnectionString = ConnectionString;
                _con.Open();
            }
            cmd.Connection = _con;
            cmd.CommandTimeout = _connectionTimeout;
            da.SelectCommand = cmd;
            da.Fill(ds);
            _con.Close();
            return ds;
        }

        public async Task<DataTable> GetTableFromCommandAsync(SqlCommand cmd)
        { 
            if (_con.State == ConnectionState.Closed)
            {
                _con.ConnectionString = ConnectionString;
                _con.Open();
            }
            cmd.Connection = _con;
            cmd.CommandTimeout = _connectionTimeout;
            var dt = new DataTable();
            var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            _con.Close();
            return dt;
        }


        public void Dispose()
        {
            _con?.Dispose();
        }
    }


}
