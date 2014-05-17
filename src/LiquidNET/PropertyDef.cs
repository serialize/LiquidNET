using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{

    public enum KeyType
    {
        PrimaryKey,
        ForeignKey,
        NoKey
    }


    [Serializable()]
    public class PropertyDef
    {
        private string _PropertyName;
        private int _ColumnIndex;
        private string _ColumnName;
        private string _DBType;
        private string _SystemType;
        private KeyType _KeyType;
        private string _FKField;
        private string _FKObject;
        private string _FKRelation;

        internal PropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName)
        {
            _PropertyName = String.Empty;
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = "DbType.Int32";
            _SystemType = "System.Int32";
            _KeyType = KeyType.NoKey;
            _FKField = String.Empty;
            _FKObject = String.Empty;
            _FKRelation = String.Empty;
        }
        internal PropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            KeyType aKeyType)
        {
            _PropertyName = String.Empty;
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = "DbType.Int32";
            _SystemType = "System.Int32";
            _KeyType = aKeyType;
            _FKField = String.Empty;
            _FKObject = String.Empty;
            _FKRelation = String.Empty;
        }

        internal PropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {
            _PropertyName = String.Empty;
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = aDBType;
            _SystemType = aSystemType;
            _KeyType = aKeyType;
            _FKField = String.Empty;
            _FKObject = String.Empty;
            _FKRelation = String.Empty;

        }
        internal PropertyDef(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType,
            string aFKField,
            string aFKObject,
            string aFKRelation)
        {
            _PropertyName = String.Empty;
            _ColumnIndex = aColumnIndex;
            _ColumnName = aColumnName;
            _DBType = aDBType;
            _SystemType = aSystemType;
            _KeyType = aKeyType;
            _FKField = aFKField;
            _FKObject = aFKObject;
            _FKRelation = aFKRelation;

        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Public Properties
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            internal set
            {
                _PropertyName = value;
            }
        }
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
            internal set
            {
                _SystemType = value;
            }
        }
        public KeyType KeyType
        {
            get
            {
                return _KeyType;
            }
        }
        public string FKObject
        {
            get
            {
                return _FKObject;
            }
            internal set
            {
                _FKObject = value;
            }
        }
        public string FKField
        {
            get
            {
                return _FKField;
            }
            internal set
            {
                _FKField = value;
            }
        }
        public string FKRelation
        {
            get
            {
                return _FKRelation;
            }
            internal set
            {
                _FKRelation = value;
            }
        }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    }
}
