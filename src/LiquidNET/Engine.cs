using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public sealed class Engine
    {
        private static Engine instance = new Engine();
        private static int cmdTimeout = 30;


        public static int CommandTimeout
        {
            get { return cmdTimeout; }
            set { cmdTimeout = value; }
        }


        public static Engine Instance
        {
            get
            {
                return instance;
            }
        }
        public static void Init(IDataDelegate aDelegate, String aConnectionString)
        {
            instance.Initialize(aDelegate, aConnectionString);
        }
        public static void Shutdown()
        {
            instance = null;
        }

        public static BCO createBCO(int aContextID, RetrieveContext aRContext)
        {
            return instance.CreateBCO(aContextID, aRContext);
        }
        public static BCO createBCO(string aContext, RetrieveContext aRContext)
        {
            return instance.CreateBCO(aContext, aRContext);
        }

        public static BCO createBCODefinition(int aContextID)
        {
            return instance.CreateBCODefinition(aContextID);
        }
        public static BCO createBCODefinition(string aContext)
        {
            return instance.CreateBCODefinition(aContext);
        }

        public static int nextSequence(string aSequenceName)
        {
            return instance.NextSequence(aSequenceName);
        }

        public static object executeDataset(string commandText) 
        {
            return instance.ExecuteDataSet(commandText);
        }
        public static object executeDataset(string commandText, SqlParameter[] sqlparams)
        {
            return instance.ExecuteDataSet(commandText, sqlparams);
        }
        public static object executeScalar(string commandText)
        {
            return instance.ExecuteScalar(commandText);
        }
        public static object executeScalar(string commandText, SqlParameter[] sqlparams)
        {
            return instance.ExecuteScalar(commandText, sqlparams);
        }
        public static object executeNonQuery(string commandText)
        {
            return instance.ExecuteNonQuery(commandText);
        }
        public static object executeNonQuery(string commandText, SqlParameter[] sqlparams)
        {
            return instance.ExecuteNonQuery(commandText, sqlparams);
        }


        private ServiceFactory _Factory;
        private IBusinessDelegate _Delegate;
        private String _ConnectionString;

        public ServiceFactory Factory
        {
            get
            {
                return _Factory;
            }
        }
        public String ConnectionString 
        {
            get
            {
                return _ConnectionString;
            }
        }


        private Engine()
        {
            LiquidSettings config = LiquidSettings.GetConfiguration();
            if (config == null)
                return;

            _ConnectionString = config.ConnectionString;

            Type dataDelegateType = Type.GetType(config.DataDelegateTypeName);
            Type businessDelegateType = Type.GetType(config.BusinessDelegateTypeName);

            IDataDelegate dataDelegate = (IDataDelegate)Activator.CreateInstance(dataDelegateType);
            _Factory = new ServiceFactory(dataDelegate);

            _Delegate = (IBusinessDelegate)Activator.CreateInstance(businessDelegateType);
        }

        internal void Initialize(IDataDelegate aDelegate, String aConnectionString)
        {
            _ConnectionString = aConnectionString;
            _Factory = new ServiceFactory(aDelegate);
        }
        public void AddBusinessDelegate(IBusinessDelegate aDelegate)
        {
            _Delegate = aDelegate;
        }

        public object ExecuteNonQuery(string commandText)
        {
            object ret = null;

            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            conn.Open();
            try
            {

                ret = cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteNonQuery", ex);
            }

            return ret;
        }
        public object ExecuteNonQuery(string commandText, SqlParameter[] sqlparams)
        {
            object ret = null;

            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            foreach (SqlParameter param in sqlparams)
            {
                cmd.Parameters.Add(param);
            }

            conn.Open();
            try
            {

                ret = cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteNonQuery", ex);
            }

            return ret;
        }
        public object ExecuteNonQuery(SqlCommand cmd)
        {
            object ret = null;
            cmd.Connection.Open();
            try
            {
                ret = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State != ConnectionState.Closed)
                    cmd.Connection.Close();
                throw new Exception("Exception in Engine::ExceuteNonQuery", ex);
            }
            return ret;
        }

        public object ExecuteScalar(string commandText)
        {
            object ret = null;

            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            conn.Open();
            try
            {

                ret = cmd.ExecuteScalar();
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteScalar", ex);
            }

            return ret;
        }
        public object ExecuteScalar(string commandText, SqlParameter[] sqlparams)
        {
            object ret = null;

            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            foreach (SqlParameter param in sqlparams)
            {
                cmd.Parameters.Add(param);
            }

            conn.Open();
            try
            {

                ret = cmd.ExecuteScalar();
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteScalar", ex);
            }

            return ret; 
        }
        public object ExecuteScalar(SqlCommand cmd)
        {
            object ret = null;
            cmd.Connection.Open();
            try
            {
                ret = cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                if (cmd.Connection.State != ConnectionState.Closed)
                    cmd.Connection.Close();
                throw new Exception("Exception in Engine::ExceuteScalar", ex);
            }
            return ret;
        }

        public DataSet ExecuteDataSet(string commandText) 
        {
            DataSet ds = null;
            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            SqlDataAdapter adap = new SqlDataAdapter(cmd);

            conn.Open();
            try
            {

                adap.Fill(ds);
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteDataSet", ex);
            }
            
            return ds;
        }
        public DataSet ExecuteDataSet(string commandText, SqlParameter[] sqlparams) 
        {
            DataSet ds = null;
            SqlConnection conn = new SqlConnection(_ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = commandText;
            cmd.CommandTimeout = Engine.CommandTimeout;

            foreach (SqlParameter param in sqlparams)
            {
                cmd.Parameters.Add(param);
            }

            SqlDataAdapter adap = new SqlDataAdapter(cmd);

            conn.Open();
            try
            {

                adap.Fill(ds);
                conn.Close();

            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::ExceuteDataSet", ex);
            }

            return ds;
        }
        public DataSet ExecuteDataSet(SqlCommand cmd)
        {
            DataSet ds = null;
            SqlDataAdapter adap = new SqlDataAdapter(cmd);
            cmd.Connection.Open();
            try
            {

                adap.Fill(ds);
                cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                if (cmd.Connection.State != ConnectionState.Closed)
                    cmd.Connection.Close();

                throw new Exception("Exception in Engine::ExceuteDataSet", ex);
            }

            return ds;
        }
        
        
        internal BEO CreateBEO(int aContextID)
        {
            return (BEO)_Delegate.CreateBEO(aContextID);
        }

        public BCO CreateBCO(int aContextID)
        {
            return new BCO(this, _Factory.Retrieve(aContextID, null));
        }
        public BCO CreateBCO(int aContextID, RetrieveContext aRContext)
        {
            return new BCO(this, _Factory.Retrieve(aContextID, aRContext));
        }
        public BCO CreateBCO(string aContext, RetrieveContext aRContext)
        {
            return new BCO(this, _Factory.Retrieve(aContext, aRContext));
        }

        public BCO CreateBCODefinition(int aContextID)
        {
            return new BCO(this, _Factory.Definition(aContextID));
        }
        public BCO CreateBCODefinition(string aContext)
        {
            return new BCO(this, _Factory.Definition(aContext));
        }

        public int NextSequence(string aSequenceName)
        {

            SqlConnection conn = new SqlConnection(_ConnectionString);

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spNextSequence";
            cmd.CommandTimeout = Engine.CommandTimeout;

            SqlParameter param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "Seq";
            param.Value = aSequenceName;
            cmd.Parameters.Add(param);

            param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Output;
            param.ParameterName = "ActualID";
            param.Value = -1;
            cmd.Parameters.Add(param);

            conn.Open();
            try
            {
                cmd.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {

                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in Engine::NextSequence", ex);
                
            }

            return Convert.ToInt32(param.Value);
        }

        public void AddSqlParameter(SqlCommand cmd, string aName, object aValue, SqlDbType aSqlDbType, int aSize)
        {
            SqlParameter param = cmd.CreateParameter();
            param.ParameterName = aName;
            param.SqlDbType = aSqlDbType;
            param.Size = aSize;
            param.Value = aValue;

            cmd.Parameters.Add(param);
        }

        public void AddSqlParameter(SqlCommand cmd, string aName, object aValue)
        {
            SqlParameter param = cmd.CreateParameter();
            param.ParameterName = aName;
            param.Value = aValue;

            cmd.Parameters.Add(param);
        }

        public SqlCommand NewCommand()
        {
            return new SqlConnection(_ConnectionString).CreateCommand();
        }

        public void ExecuteSPCommand(SqlCommand cmd)
        {
            SqlConnection conn = cmd.Connection;

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in Engine::ExecuteSPCommand", ex);
            }
            finally
            {
                if (conn != null && conn.State != ConnectionState.Closed)
                    conn.Close();
            }
        }

        public SqlCommand NewSPCommand(string spName)
        {
            SqlCommand cmd = NewCommand();
            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = Engine.CommandTimeout;
            return cmd;
        }
        public void AddSPInputParameter(SqlCommand cmd, String name, Object value)
        {
            SqlParameter param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}
