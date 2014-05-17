using System;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    public enum SQLOperator
    {
        Equal,
        Greater,
        GreaterEqual,
        Lower,
        LowerEqual,
        NotEqual,
        Like
    }

    [Serializable()]
    public class SQLFactory
    {
        #region Constants
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        
        private const string coSELECT           = "SELECT";
        private const string coINSERT           = "INSERT INTO";
        private const string coUPDATE           = "UPDATE";
        private const string coDELETE           = "DELETE";
        private const string coFROM             = "FROM";
        private const string coSET              = "SET";
        private const string coVALUES           = "VALUES";
        private const string coWHERE            = "WHERE";
        private const string coLIKE             = "LIKE";
        private const string coORDER            = "ORDER BY";
        private const string coASC              = "ASC";
        private const string coDESC             = "DESC";
        private const string coAND              = "AND";
        private const string coOR               = "OR";
        private const string coSQLStr           = "'";
        private const string coBLANK            = " ";
        private const string coPOINT            = ".";
        private const string coCOMMA            = ",";
        private const string coLEFTBRACKET      = "(";
        private const string coRIGHTBRACKET     = ")";
        private const string coLEFTSQRBRACKET   = "[";
        private const string coRIGHTSQRBRACKET  = "]";
        private const string coQUESTIONMARK     = "?";
        private const string coEQUAL            = "=";
        private const string coNOTEQUAL         = "!=";
        private const string coGREATER          = ">";
        private const string coGREATEREQUAL     = ">=";
        private const string coLOWER            = "<";
        private const string coLOWEREQUAL       = "<=";
        private const string coDATE             = "#";
        private const string coPARAMETER        = "@";
        private const string coDBLPOINT         = ":";

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
                
        #region Private Variables
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private string _TableOwner;
        private string _TableName;
        private ColumnMap _ColumnMap;
        private bool _UseNamedParameter = true;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region Constructor
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        internal SQLFactory(string aTableName, ColumnMap aColumnMap)
        {
            _TableOwner = String.Empty;
            _TableName = aTableName;
            _ColumnMap = aColumnMap;
            _UseNamedParameter = true;
        }
        internal SQLFactory(string aTableName, ColumnMap aColumnMap, bool aUseNamedParameter)
        {
            _TableOwner = String.Empty;
            _TableName = aTableName;
            _ColumnMap = aColumnMap;
            _UseNamedParameter = aUseNamedParameter;
        }
        internal SQLFactory(string aTableOwner, string aTableName, ColumnMap aColumnMap)
        {
            _TableOwner = aTableOwner;
            _TableName = aTableName;
            _ColumnMap = aColumnMap;
            _UseNamedParameter = true;
        }
        internal SQLFactory(string aTableOwner, string aTableName, ColumnMap aColumnMap, bool aUseNamedParameter)
        {
            _TableOwner = aTableOwner;
            _TableName = aTableName;
            _ColumnMap = aColumnMap;
            _UseNamedParameter = aUseNamedParameter;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        

        #region Public Methods
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public string buildSQLString(object aString2build)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(coSQLStr);
            sb.Append(aString2build.ToString());
            sb.Append(coSQLStr);

            return sb.ToString();
        }

        public string buildSQLDate(object aDate2build) 
        {

            System.DateTime l_date = (System.DateTime)aDate2build;

            StringBuilder sb = new StringBuilder();

            sb.Append(coSQLStr);
            sb.Append(l_date.Year.ToString());
            sb.Append(coPOINT);
            sb.Append(l_date.Month.ToString());
            sb.Append(coPOINT);
            sb.Append(l_date.Day.ToString());
            sb.Append(coSQLStr);

            return sb.ToString();
        }
        public string buildSQLDateTime(object aDate2build)
        {
            System.DateTime l_date = (System.DateTime)aDate2build;

            StringBuilder sb = new StringBuilder();

            sb.Append(coSQLStr);
            sb.Append(l_date.Year.ToString());
            sb.Append(coPOINT);
            sb.Append(l_date.Month.ToString());
            sb.Append(coPOINT);
            sb.Append(l_date.Day.ToString());
            sb.Append(coBLANK);
            sb.Append(l_date.Hour.ToString());
            sb.Append(coDBLPOINT);
            sb.Append(l_date.Minute.ToString());
            sb.Append(coDBLPOINT);
            sb.Append(l_date.Second.ToString());
            sb.Append(coSQLStr);

            return sb.ToString();
        }
        public string buildSELECT() 
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(coSELECT);
            sb.Append(coBLANK);

            bool l_flag = false;

            foreach (ColumnDef def in _ColumnMap) 
            {
                if (l_flag)
                    sb.Append(coCOMMA + coBLANK);
                else 
                    l_flag = true;

                appendFullColumnName(sb, def.ColumnName, false);
            }
            sb.Append(coBLANK);

            appendFromFullTableName(sb);
            return sb.ToString();
        }
        public string buildUPDATE(bool aUseTable)
        {
            StringBuilder sbfields = new StringBuilder();
            StringBuilder sbconditions = new StringBuilder();
            StringBuilder l_filterValue = new StringBuilder();
            bool l_flag = false;
            bool l_filterFlag = false;

            appendValue(sbfields, coUPDATE);
            appendFullTableName(sbfields);
            appendValue(sbfields, coSET);

            bool firstCol = true;
            bool firstParam = true;
            foreach (ColumnDef def in _ColumnMap)
            {
                if (def.KeyType != KeyType.PrimaryKey)
                {
                    if (!firstCol)
                        appendComma(sbfields);

                    appendColumnCondition(sbfields, def.ColumnName);
                    firstCol = false;
                }
                else 
                {
                    sbconditions.Append(coBLANK);

                    if (firstParam)
                        appendWHERE(sbconditions);
                    else
                        appendAND(sbconditions);

                    appendColumnCondition(sbconditions, def.ColumnName);
                    appendBlank(sbconditions);

                    firstParam = false;
                }
            }

            sbfields.Append(sbconditions);

            return sbfields.ToString();
        }
        public string buildINSERT(bool aUseTable)
        {
            StringBuilder sbfields = new StringBuilder();
            StringBuilder sbvalues = new StringBuilder();

            appendValue(sbfields, coINSERT);
            appendFullTableName(sbfields);

            sbfields.Append(coBLANK);

            sbfields.Append(coLEFTBRACKET);
            sbvalues.Append(coLEFTBRACKET);

            bool l_flag = false;

            foreach (ColumnDef def in _ColumnMap)
            {
                if (l_flag)
                {
                    appendComma(sbfields);
                    appendComma(sbvalues);
                }
                else
                    l_flag = true;

                appendFullColumnName(sbfields, def.ColumnName, false);
                appendParamter(sbvalues, def.ColumnName);
            }

            sbfields.Append(coRIGHTBRACKET);
            sbvalues.Append(coRIGHTBRACKET);

            sbfields.Append(coBLANK);
            sbfields.Append(coVALUES);
            sbfields.Append(coBLANK);

            sbfields.Append(sbvalues.ToString());


            return sbfields.ToString();
        }

        public string buildDELETE()
        {
            bool l_filterFlag = false;
            StringBuilder sb = initBuilder(false);

            appendValue(sb, coDELETE);
            appendFromFullTableName(sb);


            bool first = true;
            foreach (ColumnDef def in _ColumnMap.PrimaryKeyColumns) 
            { 
                if (first)
                    appendWHERE(sb);
                else
                    appendAND(sb);

                appendColumnCondition(sb, def.ColumnName);
                appendBlank(sb);

                first = false;
            }
            return sb.ToString();
        }
        public string buildCondition(string column, SQLOperator op)
        {
            StringBuilder sb = new StringBuilder();
            appendColumnCondition(sb, column, op);
            return sb.ToString();
        }
        public string buildCondition(string column, SQLOperator op, object val) 
        {
            StringBuilder sb = new StringBuilder();
            appendColumnCondition(sb, column, op, val);
            return sb.ToString();
        }

        public string buildWHERE(string column, SQLOperator op)
        {
            return this.buildWHERE(column, op, coPARAMETER + column);
        }
        public string buildWHERE(string column, SQLOperator op, object val)
        {
            StringBuilder sb = initBuilder();
            appendWHERE(sb);
            appendColumnCondition(sb, column, op, val);
            return sb.ToString();
        }



        public string buildAND(string column, SQLOperator op)
        {
            return this.buildAND(column, op, coQUESTIONMARK + column);
        }
        public string buildAND(string column, SQLOperator op, object val)
        {
            StringBuilder sb = initBuilder();
            appendAND(sb);
            appendColumnCondition(sb, column, op, val);
            return sb.ToString();
        }



        public string buildOR(string column, SQLOperator op)
        {
            return this.buildOR(column, op, coQUESTIONMARK + column);
        }
        public string buildOR(string column, SQLOperator op, object val)
        {
            StringBuilder sb = initBuilder();
            appendOR(sb);
            appendColumnCondition(sb, column, op, val);
            return sb.ToString();
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region Private Methods
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        private StringBuilder initBuilder()
        {
            return initBuilder(true);
        }
        private StringBuilder initBuilder(bool appendBlank)
        {
            if (appendBlank)
                return new StringBuilder(coBLANK);
            else
                return new StringBuilder();
        }

        private string FormatValueWithColumnName(string column, object val) 
        {
            string sb;
            ColumnDef def = _ColumnMap.fromName(column);

            switch (def.DBType)
            {
                case ("DBType.NVarChar"):
                case ("DBType.NVarCharMax"):
                    sb = buildSQLString(val);
                    break;

                case ("DBType.Date"):
                    sb = buildSQLDate(val);
                    break;

                case ("DBType.DateTime"):
                    sb = buildSQLDateTime(val);
                    break;

                default:
                    sb = val.ToString();
                    break;
            }

            return sb;
        }

        private void appendComma(StringBuilder sb)
        {
            sb.Append(coCOMMA);
            sb.Append(coBLANK);
        }
        private void appendColumnCondition(StringBuilder sb, string column)
        {
            appendColumnCondition(sb, column, SQLOperator.Equal);
        }
        private void appendColumnCondition(StringBuilder sb, string column, SQLOperator op)
        {
            appendFullColumnName(sb, column);
            appendSqlOperator(sb, op);
            appendParamter(sb, column);
        }

        private void appendColumnCondition(StringBuilder sb, string column, SQLOperator op, object value)
        {
            appendColumnCondition(sb, column, op, value, true);
        }

        private void appendColumnCondition(StringBuilder sb, string column, SQLOperator op, object value, bool appendTrailingBlank)
        {
            appendFullColumnName(sb, column);
            appendSqlOperator(sb, op);

            if ((value == null) || (value.ToString().Equals(String.Empty)))
                sb.Append(0);
            else
                sb.Append(value);

            if (appendTrailingBlank)
                sb.Append(coBLANK);
        }


        private void appendFromFullTableName(StringBuilder sb)
        {
            sb.Append(coFROM);
            sb.Append(coBLANK);
            appendFullTableName(sb);
        }

        private void appendFullTableName(StringBuilder sb)
        {
            appendFullTableName(sb, false);
        }

        private void appendFullTableName(StringBuilder sb, bool appendTrailingDot)
        {
            if (_TableOwner != String.Empty)
            {
                appendSchemaName(sb, _TableOwner);
            }
            appendSchemaName(sb, _TableName, appendTrailingDot);
        }

        private void appendFullColumnName(StringBuilder sb, string column)
        {
            appendFullTableName(sb, true);
            appendSchemaName(sb, column, false);
        }

        private void appendFullColumnName(StringBuilder sb, string column, bool appendTrailingBlank)
        {
            appendFullTableName(sb, true);
            if (appendTrailingBlank)
                appendSchemaName(sb, column, String.Empty);
            else
                appendSchemaName(sb, column, coBLANK);
        }

        private void appendWHERE(StringBuilder sb)
        {
            sb.Append(coWHERE);
            sb.Append(coBLANK);
        }
        private void appendAND(StringBuilder sb)
        {
            sb.Append(coAND);
            sb.Append(coBLANK);
        }
        private void appendOR(StringBuilder sb)
        {
            sb.Append(coOR);
            sb.Append(coBLANK);
        }

        private void appendSqlOperator(StringBuilder sb, SQLOperator op)
        {
            appendSqlOperator(sb, op, true);
        }

        private void appendSqlOperator(StringBuilder sb, SQLOperator op, bool appendTrailingBlank)
        {
            sb.Append(getSQLOperatorString(op));
            if (appendTrailingBlank)
                sb.Append(coBLANK);
        }

        private string getSQLOperatorString(SQLOperator op) 
        {

            string ret = String.Empty;

            switch (op)
            {
                case SQLOperator.Equal:
                    ret = coEQUAL;
                    break;
                case SQLOperator.NotEqual:
                    ret = coNOTEQUAL;
                    break;
                case SQLOperator.Greater:
                    ret = coGREATER;
                    break;
                case SQLOperator.GreaterEqual:
                    ret = coGREATEREQUAL;
                    break;
                case SQLOperator.Lower:
                    ret = coLOWER;
                    break;
                case SQLOperator.LowerEqual:
                    ret = coLOWEREQUAL;
                    break;
                case SQLOperator.Like:
                    ret = coLIKE;
                    break;
                default:
                    ret = coEQUAL;
                    break;
            }
            return ret;
        }

        private void appendDot(StringBuilder sb)
        {
            sb.Append(coPOINT);
        }
        private void appendBlank(StringBuilder sb)
        {
            sb.Append(coBLANK);
        }
        private void appendValue(StringBuilder sb, string value)
        {
            appendValue(sb, value, true);
        }
        private void appendValue(StringBuilder sb, string value, bool addTrailingBlank)
        {
            if (addTrailingBlank)
                appendValue(sb, value, coBLANK);
            else
                appendValue(sb, value, String.Empty);

        }
        private void appendParamter(StringBuilder sb, string name)
        {
            if (_UseNamedParameter)
            {
                sb.Append("@");
                sb.Append(name);
            }
            else
            {
                sb.Append(coQUESTIONMARK);
            }
        }

        private void appendValue(StringBuilder sb, string value, string trailing)
        {
            sb.Append(value);
            sb.Append(coBLANK);
            sb.Append(trailing);
        }
        private void encapsulateWithBlank(StringBuilder sb, string value)
        {
            appendBlank(sb);
            appendValue(sb, value);
        }
        private void encapsulateSquareBrackets(StringBuilder sb, string value)
        {
            encapsulateSquareBrackets(sb, value, true);
        }
        private void encapsulateSquareBrackets(StringBuilder sb, string value, bool addTrailingBlank)
        {
            if (addTrailingBlank)
                sb.Append(coBLANK);
        }
        private void encapsulateSquareBrackets(StringBuilder sb, string value, string trailing)
        {
            sb.Append(coLEFTSQRBRACKET);
            sb.Append(value);
            sb.Append(coRIGHTSQRBRACKET);
            sb.Append(trailing);
        }
        private void appendSchemaName(StringBuilder sb, string value)
        {
            appendSchemaName(sb, value, true);
        }
        private void appendSchemaName(StringBuilder sb, string value, bool addDot)
        {
            if (addDot)
                appendSchemaName(sb, value, coPOINT);
            else
                appendSchemaName(sb, value, coBLANK);
        }
        private void appendSchemaName(StringBuilder sb, string value, string trailing)
        {
            encapsulateSquareBrackets(sb, value, trailing);
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion

        #region Temp
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion



    }
}
