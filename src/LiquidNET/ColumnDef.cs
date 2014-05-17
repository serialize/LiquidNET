using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class ColumnDef
    {
        private int _ColumnIndex;
        private string _ColumnName;
        private string _DBType;
        private string _SystemType;
        private KeyType _KeyType;

        internal ColumnDef(
            int aColumnIndex,
            string aColumnName)
        {
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = "DBType.Int";
            _SystemType = "System.Int32";
            _KeyType = KeyType.NoKey;
        }
        internal ColumnDef(
            int aColumnIndex,
            string aColumnName,
            KeyType aKeyType)
        {
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = "DBType.Int";
            _SystemType = "System.Int32";
            _KeyType = aKeyType;
        }
        internal ColumnDef(
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = aDBType;
            _SystemType = aSystemType;
            _KeyType = aKeyType;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Public Properties
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public int ColumnIndex
        {
            get
            {
                return _ColumnIndex;
            }
        }
        public string ColumnName
        {
            get
            {
                return _ColumnName;
            }
        }
        public string DBType
        {
            get
            {
                return _DBType;
            }
        }
        public string SystemType
        {
            get
            {
                return _SystemType;
            }
        }
        public KeyType KeyType
        {
            get
            {
                return _KeyType;
            }
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public DbType AsDbType()
        {
            switch (_DBType)
            {
                case "DBType.Int":
                    return DbType.Int32;
                case "DBType.NVarChar":
                case "DBType.NVarCharMax":
                case "DBType.NText":
                    return DbType.String;
                case "DBType.TinyInt":
                    return DbType.Byte;
                case "DBType.Bit":
                    return DbType.Boolean;
                case "DBType.Float":
                    return DbType.Double;
                case "DBType.DateTime":
                    return DbType.DateTime;
                case "DBType.VarBinaryMax":
                    return DbType.Object;
                default:
                    return DbType.Int32;
            }
        }

        public SqlDbType AsSqlDbType()
        {
            switch (_DBType)
            {
                case "DBType.Int":
                    return SqlDbType.Int;
                case "DBType.NVarChar":
                    return SqlDbType.NVarChar;
                case "DBType.NVarCharMax":
                case "DBType.NText":
                    return SqlDbType.NText;
                case "DBType.TinyInt":
                    return SqlDbType.TinyInt;
                case "DBType.Bit":
                    return SqlDbType.Bit;
                case "DBType.Float":
                    return SqlDbType.Decimal;
                case "DBType.DateTime":
                    return SqlDbType.DateTime;
                case "DBType.VarBinaryMax":
                    return SqlDbType.Image;
                default:
                    return SqlDbType.Int;
            }
        }
        

    }

        
}
