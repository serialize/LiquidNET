using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class BEOKeyColumn
    {
        private string _Name;
        private string _DataType;
        private object _Value;

        public string Name
        {
            get
            {
                return _Name;
            }
            internal set
            {
                _Name = value;
            }
        }
        public string DataType
        {
            get
            {
                return _DataType;
            }
            internal set
            {
                _DataType = value;
            }
        }
        public Object Value
        {
            get
            {
                return _Value;
            }
            internal set
            {
                _Value = value;
            }
        }

        public bool Equals(BEOKeyColumn col)
        {
            if (!col.Name.Equals(_Name))
                return false;

            if (!col.DataType.Equals(_DataType))
                return false;

            if (!col.Value.Equals(_Value))
                return false;

            return true;
        }

        public override bool Equals(object obj)
        {
            return _Value.Equals(obj); ;
        }

        public DbType AsDbType()
        {
            switch (_DataType)
            {
                case "System.Int32":
                case "DBType.Int":
                    return DbType.Int32;
                case "System.String":
                case "DBType.NVarChar":
                case "DBType.NVarCharMax":
                    return DbType.String;
                case "System.Byte":
                case "DBType.TinyInt":
                    return DbType.Byte;
                case "System.Boolean":
                case "DBType.Bit":
                    return DbType.Boolean;
                case "DBType.Float":
                    return DbType.Double;
                case "System.DateTime":
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
            switch (_DataType)
            {
                case "System.Int32":
                case "DBType.Int":
                    return SqlDbType.Int;
                case "System.String":
                case "DBType.NVarChar":
                    return SqlDbType.NVarChar;
                case "DBType.NVarCharMax":
                    return SqlDbType.NText;
                case "System.Byte":
                case "DBType.TinyInt":
                    return SqlDbType.TinyInt;
                case "System.Boolean":
                case "DBType.Bit":
                    return SqlDbType.Bit;
                case "DBType.Float":
                    return SqlDbType.Decimal;
                case "System.DateTime":
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
