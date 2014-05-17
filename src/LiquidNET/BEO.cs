using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public abstract class BEO : IBEO
    {

        #region private variables

        private BCO _BCO;
        private int _RowIndex;
        private bool _isDeleted;
        private bool _isNew;
        private bool _isModified;
        private BEOKey _internalKey;

        #endregion





        public BCO BCO
        {
            get
            {
                return _BCO;
            }
        }

        public BEOKey Key 
        {
            get
            {
                BEOKey key = new BEOKey();
                foreach (PropertyDef def in _BCO.Properties.PrimaryKeyProperties)
                {
                    key.AddNew(def.ColumnName, def.SystemType, this.GetSourceValue(def.ColumnName));
                }
                return key;
            }
        }


        public bool IsDeleted 
        {
            get
            {
                return _BCO.CheckDeleted(_RowIndex);
            }
        }
        public bool IsNew 
        {
            get
            {
                return _BCO.CheckAdded(_RowIndex);
            }
        }
        public bool IsModified
        {
            get
            {
                return _BCO.CheckModified(_RowIndex);
            }
        }

        public BEO()
        {
            //System.Diagnostics.Debug.WriteLine(String.Format("BEO.CTOR {0}", this.GetType().Name)); 
        }
        ~BEO()
        {
            //System.Diagnostics.Debug.WriteLine(String.Format("BEO.~CTOR {0} {1}", this.BCO.ContextID, _internalKey[0].Value));
            _BCO.Callback -= bcoCallback;
        }

        public int RegisterProperty(
            string aPropertyName,
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {

            foreach (PropertyDef def in _BCO.Properties)
            {
                if (def.ColumnName.Equals(aColumnName))
                {
                    def.PropertyName = aPropertyName;
                    return 1;
                }
            }

            _BCO.Properties.AddPropertyDef(
                aPropertyName,
                aColumnIndex,
                aColumnName,
                aDBType,
                aSystemType,
                aKeyType);

            return 0;

        }
        public int RegisterProperty(
            int aColumnIndex,
            string aPropertyName,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {

            foreach (PropertyDef def in _BCO.Properties)
            {
                if (def.ColumnName.Equals(aColumnName))
                {
                    def.PropertyName = aPropertyName;
                    return 1;
                }
            }

            _BCO.Properties.AddPropertyDef(
                aPropertyName,
                aColumnIndex,
                aColumnName,
                aDBType,
                aSystemType,
                aKeyType);

            return 0;

        }
        public int RegisterProperty(
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

            foreach (PropertyDef def in _BCO.Properties)
            {
                if ((def.ColumnName.Equals(aColumnName)) && (def.PropertyName.Equals(String.Empty)) && (def.FKRelation.Equals(String.Empty)))
                {
                    def.PropertyName = aPropertyName;
                    def.FKField = aFKField;
                    def.FKObject = aFKObject;
                    def.FKRelation = aFKRelation;
                    return 1;
                }
            }

            _BCO.Properties.AddPropertyDef(
                aPropertyName,
                aColumnIndex,
                aColumnName,
                aDBType,
                aSystemType,
                aKeyType,
                aFKField,
                aFKObject,
                aFKRelation);

            return 0;

        }
        public int RegisterProperty(
            int aColumnIndex,
            string aPropertyName,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType,
            string aFKField,
            string aFKObject,
            string aFKRelation)
        {

            foreach (PropertyDef def in _BCO.Properties)
            {
                if ((def.ColumnName.Equals(aColumnName)) && (def.PropertyName.Equals(String.Empty)) && (def.FKRelation.Equals(String.Empty)))
                {
                    def.PropertyName = aPropertyName;
                    def.FKField = aFKField;
                    def.FKObject = aFKObject;
                    def.FKRelation = aFKRelation;
                    return 1;
                }
            }

            _BCO.Properties.AddPropertyDef(
                aPropertyName,
                aColumnIndex,
                aColumnName,
                aDBType,
                aSystemType,
                aKeyType,
                aFKField,
                aFKObject,
                aFKRelation);

            return 0;

        }

        internal void AttachSource(BCO aBCO, int aRowIndex)
        {
            _BCO = aBCO;
            _RowIndex = aRowIndex;

            RegisterBusinessObject();

            _BCO.Callback += bcoCallback;

            //System.Diagnostics.Debug.WriteLine(String.Format("BEO.AttachSource() {0} {1}", aBCO.TableName, _RowIndex)); 

            _isDeleted = _BCO.CheckDeleted(_RowIndex);
            _isModified = _BCO.CheckModified(_RowIndex);
            _isNew = _BCO.CheckAdded(_RowIndex);

            _internalKey = Key;
        }

        public object GetSourceValue(string aPropertyName)
        {
            return _BCO.GetSourceValue(_RowIndex, aPropertyName);
        }
        public bool SetSourceValue(string aPropertyName, object aValue)
        {
            _isModified = true;
            bool ret = _BCO.SetSourceValue(_RowIndex, aPropertyName, aValue);
            PropertyDef def = _BCO.Properties.GetWithPropertyName(aPropertyName);
            
            if (def == null)
                throw new NullReferenceException("PropertyDef is null");

            if (def.KeyType == KeyType.PrimaryKey)
            {
                _internalKey = Key;
            }
            return ret;
        }

        internal void bcoCallback(object sender, EventArgs e)
        {
            if (_isDeleted)
            {
                _RowIndex = -1;
            }
            else
            {
                _RowIndex = _BCO.RowIndexFromKey(_internalKey);
            }
        }

        public object Save()
        {
            object ret = null;
            if (this.Validate() && this.PreSave())
            {
                if (!_isDeleted)
                    _internalKey = Key;
                ret = _BCO.Save();

                _isModified = false;
                _isNew = false;

                this.PostSave();
            }
            return ret;
        }

        public object Delete()
        {
            if (this.PreDelete())
            {
                _internalKey = Key;
                _BCO.Delete(_RowIndex);
                _isDeleted = true;
            }
            this.PostDelete();
            return true;
        }

        public abstract bool PreSave();
        public abstract bool PostSave();
        public abstract bool PreDelete();
        public abstract bool PostDelete();
        public abstract BEOKey RelationshipKey(string aRelation);
        public abstract bool Validate();
        public abstract void RegisterBusinessObject();


        public bool Equals(IBEO beo)
        {
            return this.Key.Equals(beo.Key);
        }

        public override bool Equals(object obj)
        {
            if (obj is IBEO)
                return Equals((IBEO)obj);

            return base.Equals(obj);
        }

    }
}
