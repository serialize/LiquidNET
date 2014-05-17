using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public abstract class DLO : IDLO
    {
        private string _TableName;
        private string _TableOwner;
        private int _ContextID;
        private DataTable _DataTable;
        private ColumnMap _ColumnMap;
        private static bool _UseNamedParameter = true;
        private RetrieveContext _RetrieveContext;

        private SQLFactory _SQLFactory;

        private string _SelectCMD;
        private CommandType _SelectCMDType = CommandType.Text;
        private string _UpdateCMD;
        private string _InsertCMD;
        private string _DeleteCMD;



        public string TableOwner
        {
            get
            {
                return _TableOwner;
            }
            protected set
            {
                _TableOwner = value;
            }
        }
        public string TableName
        {
            get
            {
                return _TableName;
            }
            protected set
            {
                _TableName = value;
            }
        }
        public string SelectCMD
        {
            get
            {
                return _SelectCMD;
            }
            set
            {
                _SelectCMD = value;
            }
        }
        public CommandType SelectCMDType
        {
            get
            {
                return _SelectCMDType;
            }
            set
            {
                _SelectCMDType = value;
            }
        }
        public string UpdateCMD
        {
            get
            {
                return _UpdateCMD;
            }
            set
            {
                _UpdateCMD = value;
            }
        }
        public string InsertCMD
        {
            get
            {
                return _InsertCMD;
            }
            set
            {
                _InsertCMD = value;
            }
        }
        public string DeleteCMD
        {
            get
            {
                return _DeleteCMD;
            }
            set
            {
                _DeleteCMD = value;
            }
        }
        public static bool UseNamedParameter
        {
            get
            {
                return _UseNamedParameter;
            }
            set
            {
                _UseNamedParameter = value;
            }
        }

        public int ContextID 
        {
            get
            {
                return _ContextID;
            }
            protected set
            {
                _ContextID = value;
            }
        }

        public DataTable DataTable
        {
            get
            {
                return _DataTable;
            }
        }
        public ColumnMap ColumnMap
        {
            get
            {
                return _ColumnMap;
            }
        }

        protected SQLFactory SQLFactory
        {
            get
            {
                return _SQLFactory;
            }
        }

        public RetrieveContext RetrieveContext 
        {
            get
            {
                return _RetrieveContext;
            }
        }




        public DLO()
        {
            //System.Diagnostics.Debug.WriteLine("DLO CTOR()");

            _ColumnMap = new ColumnMap();
            _DataTable = new DataTable();

        }
        private void initSQL()
        {
            _SQLFactory = new SQLFactory(_TableName, _ColumnMap);


            this.SelectCMD = _SQLFactory.buildSELECT();
            this.UpdateCMD = _SQLFactory.buildUPDATE(false);
            this.InsertCMD = _SQLFactory.buildINSERT(false);
            this.DeleteCMD = _SQLFactory.buildDELETE();
        }




        public DataTable Retrieve(RetrieveContext aRContext)
        {
            this.PreRetrieve(aRContext);
            _RetrieveContext = aRContext;

            SqlConnection conn = new SqlConnection(Engine.Instance.ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = _SelectCMDType;
            cmd.CommandText = _SelectCMD;
            cmd.CommandTimeout = Engine.CommandTimeout;

            // build Params from BEOKey
            if (aRContext != null)
            {
                for (int i = 0; i < _RetrieveContext.Key.Count; i++)
                {
                    SqlParameter param = cmd.CreateParameter();
                    param.ParameterName = _RetrieveContext.Key[i].Name;
                    param.SqlDbType = _RetrieveContext.Key[i].AsSqlDbType();
                    param.Value = _RetrieveContext.Key[i].Value;

                    cmd.Parameters.Add(param);
                }
            }

            SqlDataAdapter adap = new SqlDataAdapter(cmd);

            conn.Open();

            try
            {

                adap.Fill(_DataTable);

                conn.Close();
            }
            catch (Exception ex)
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();

                throw new Exception("Exception in DLO::Retrieve " + this._TableName, ex);
            }

            this.PostRetrieve();
            //AssignTableName();
            //AssignPrimaryKey2Table();

            return _DataTable;
        }
        public object Update(System.Data.DataTable aDataTable)
        {
            _DataTable = aDataTable;

            this.PreUpdate();

            SqlConnection conn = new SqlConnection(Engine.Instance.ConnectionString);
            SqlDataAdapter adapt = new SqlDataAdapter();

            SqlCommand cmd = this.CreateInsertCommand(this.InsertCMD);
            cmd.Connection = conn;
            adapt.InsertCommand = cmd;

            cmd = this.CreateUpdateCommand(this.UpdateCMD);
            cmd.Connection = conn;
            adapt.UpdateCommand = cmd;

            cmd = this.CreateDeleteCommand(this.DeleteCMD);
            cmd.Connection = conn;
            adapt.DeleteCommand = cmd;

            
            int rowsAffected = -1;
            try
            {
                rowsAffected = adapt.Update(_DataTable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            this.PostUpdate();
            
            return rowsAffected;
        }


        public void RegisterColumn(int aColumnIndex, string aColumnName)
        {
            _ColumnMap.AddColumnDef(aColumnIndex, aColumnName);
        }
        public void RegisterColumn(int aColumnIndex, string aColumnName, KeyType aKeyType)
        {
            _ColumnMap.AddColumnDef(aColumnIndex, aColumnName, aKeyType);
        }
        public void RegisterColumn(int aColumnIndex, string aColumnName, string aDBType, string aSystemType, KeyType aKeyType)
        {
            _ColumnMap.AddColumnDef(aColumnIndex, aColumnName, aDBType, aSystemType, aKeyType);
        }

        public void FinishRegistration()
        {
            AssignTableName();
            AddColumns2Table();
            AssignPrimaryKey2Table();

            initSQL();
        }

        private void AssignTableName()
        {
            _DataTable.TableName = _TableName;
        }

        private void AddColumns2Table()
        {
            foreach (ColumnDef def in _ColumnMap)
            {
                _DataTable.Columns.Add(def.ColumnName, Type.GetType(def.SystemType));
            }
        }
        private void AssignPrimaryKey2Table()
        {

            //System.Diagnostics.Debug.WriteLine("DLO.AssignPrimaryKey2Table Name:" + _TableName);

            ColumnMap pmap = _ColumnMap.PrimaryKeyColumns;

            if (pmap.Count == 0)
            {
                throw new ArgumentOutOfRangeException(String.Format("DLO {0} has no PrimaryKey Registration", this.GetType().ToString()));
            }

            DataColumn[] pcols = new DataColumn[pmap.Count];
            for (int i = 0; i < pmap.Count; i++)
            {
                pcols[i] = _DataTable.Columns[pmap[i].ColumnName];
            }

            _DataTable.PrimaryKey = pcols;
        }

        public abstract void RegisterDataSource();
        public abstract bool PreRetrieve(RetrieveContext aRContext);
        public abstract bool PostRetrieve();
        public abstract bool PreUpdate();
        public abstract bool PostUpdate();


        private SqlCommand CreateSelectCommand(string aSelectCMD)
        {

            SqlCommand cmd = new SqlCommand(aSelectCMD);
            cmd.CommandTimeout = Engine.CommandTimeout;

            
            return cmd;
        }
        private SqlCommand CreateUpdateCommand(string aUpdateCMD)
        {
            SqlCommand cmd = new SqlCommand(aUpdateCMD);
            cmd.CommandTimeout = Engine.CommandTimeout;

            if (_UseNamedParameter)
            {
                AddParametersAll(cmd);
            }
            else
            {
                AddParametersNoPrimaryCols(cmd);
                AddParametersPrimaryCols(cmd);
            }
            return cmd;
        }
        private SqlCommand CreateInsertCommand(string aInsertCMD)
        {
            SqlCommand cmd = new SqlCommand(aInsertCMD);
            cmd.CommandTimeout = Engine.CommandTimeout;

            AddParametersAll(cmd);

            return cmd;

        }
        private SqlCommand CreateDeleteCommand(string aDeleteCMD)
        {
            SqlCommand cmd = new SqlCommand(aDeleteCMD);
            cmd.CommandTimeout = Engine.CommandTimeout;

            AddParametersPrimaryCols(cmd);

            return cmd;
        }


        private void AddParametersAll(SqlCommand cmd)
        {
            SqlParameter param = null;
            int idx = 1;
            foreach (ColumnDef def in _ColumnMap)
            {
                param = cmd.CreateParameter();
                if (_UseNamedParameter)
                {
                    param.ParameterName = "@" + def.ColumnName;
                }
                else
                {
                    param.ParameterName = "@p" + idx.ToString();
                    idx++;
                }
                param.SqlDbType = def.AsSqlDbType();
                param.SourceColumn = def.ColumnName;

                cmd.Parameters.Add(param);
            }
        }
        private void AddParametersPrimaryCols(SqlCommand cmd)
        {
            SqlParameter param = null;
            int idx = 1;
            foreach (ColumnDef def in _ColumnMap.PrimaryKeyColumns)
            {
                param = cmd.CreateParameter();
                if (_UseNamedParameter)
                {
                    param.ParameterName = "@" + def.ColumnName;
                }
                else
                {
                    param.ParameterName = "@p" + idx.ToString();
                    idx++;
                }
                param.SqlDbType = def.AsSqlDbType();
                param.SourceColumn = def.ColumnName;

                cmd.Parameters.Add(param);
            }
        }
        private void AddParametersNoPrimaryCols(SqlCommand cmd)
        {
            SqlParameter param = null;
            int idx = 1;
            foreach (ColumnDef def in _ColumnMap.PrimaryKeyColumns)
            {
                if (def.KeyType != KeyType.PrimaryKey)
                {
                    param = cmd.CreateParameter();
                    if (_UseNamedParameter)
                    {
                        param.ParameterName = "@" + def.ColumnName;
                    }
                    else
                    {
                        param.ParameterName = "@p" + idx.ToString();
                        idx++;
                    }

                    param.SqlDbType = def.AsSqlDbType();
                    param.SourceColumn = def.ColumnName;

                    cmd.Parameters.Add(param);
                }
            }
        }
    }
}
