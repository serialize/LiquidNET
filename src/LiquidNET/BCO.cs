using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public sealed class BCO : IEnumerable
    {
        private int _ContextID;
        private Engine _Engine;
        private System.Data.DataTable _DataTable;
        private BCOEnumerator _Enumerator;
        private string _TableName;

        private BEOCollection _beoCollection;

        private string _RootRelation;
        private IBEO _RootBEO;
        private PropertyMap _Properties;
        private RetrieveContext _RetrieveContext;

        public PropertyMap Properties
        {
            get
            {
                return _Properties;
            }
        }

        public IBEO RootBEO
        {
            get
            {
                return _RootBEO;
            }
            set
            {
                _RootBEO = value;
            }
        }

        public string RootRelation
        {
            get
            {
                return _RootRelation;
            }
            set
            {
                _RootRelation = value;
            }
        }


        public int ContextID
        {
            get
            {
                return _ContextID;
            }

        }
        public string TableName
        {
            get
            {
                return _TableName;
            }
        }
        public System.Data.DataTable DataTable
        {
            get
            {
                return _DataTable;
            }
        }
        public Engine Engine
        {
            get
            {
                return _Engine;
            }
        }
        public int RowCount
        {
            get
            {
                return this.DataTable.Rows.Count;
            }
        }

        internal BCO(Engine aEngine, ArrayList list)
        {
            _Engine = aEngine;
            assignSource(list);
        }

        ~BCO()
        {
            //System.Diagnostics.Debug.WriteLine("~BCO Destructor " + _TableName + " " + RowCount.ToString());
            _beoCollection.Clear();
        }

        public delegate void BCOCallback(object sender, EventArgs e);
        public event BCOCallback Callback;

        private void assignSource(ArrayList list)
        {
            _Properties = new PropertyMap();

            _ContextID = (int)list[0];
            _TableName = (string)list[1];
            _DataTable = (DataTable)list[2];
            ColumnMap map = (ColumnMap)list[3];
            _RetrieveContext = (RetrieveContext)list[4];

            if (_beoCollection != null)
            {
                _beoCollection.Clear();
                _beoCollection = null;
            }
            _beoCollection = new BEOCollection();

            foreach (ColumnDef def in map)
            {
                _Properties.AddPropertyDef(string.Empty, def.ColumnIndex, def.ColumnName, def.DBType, def.SystemType, def.KeyType);
            }
        }

        public void Refresh()
        {
            ArrayList list = _Engine.Factory.Retrieve(_ContextID, _RetrieveContext);
            assignSource(list);

            if (_Enumerator != null)
                _Enumerator.Reset();
        }

        public object Save()
        {
            ArrayList list = new ArrayList();

            DataTable tblChanged = _DataTable.GetChanges();

            if (tblChanged != null)
            {
                //tblChanged.RejectChanges();

                list.Add(_ContextID);
                list.Add(_TableName);
                list.Add(_DataTable);

                ColumnMap map = new ColumnMap();

                foreach (PropertyDef def in _Properties)
                {
                    map.AddColumnDef(def.ColumnIndex, def.ColumnName, def.DBType, def.SystemType, def.KeyType);
                }
                list.Add(map);
                list.Add(_RetrieveContext);

                list = (ArrayList) _Engine.Factory.Update(list);
                //this.assignSource(list);

                if (Callback != null) Callback(this, new EventArgs());

                //Console.WriteLine("DataTable Rows {0} tblChanges Rows {1}", _DataTable.Rows.Count, tblChanged.Rows.Count);
                //tblChanged = _DataTable.GetChanges();
                //if (tblChanged != null)
                //{
                //    Console.WriteLine("DataTable Rows {0} tblChanges Rows {1}", _DataTable.Rows.Count, tblChanged.Rows.Count);
                //}
                //_DataTable.AcceptChanges();

                return true;
            }
            return null;
        }
        public void DeleteAll()
        {
            int count = _DataTable.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataRow row = _DataTable.Rows[i];
                if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                {
                    int idx = _DataTable.Rows.IndexOf(row);
                    object[] keys = fetchKeys(row);
                    if (!_beoCollection.Contains(keys))
                    {
                        IBEO beo = _Engine.CreateBEO(_ContextID);
                        ((BEO)beo).AttachSource(this, idx);
                        beo.Delete();
                    }
                    else
                    {
                        IBEO beo = _beoCollection[keys];
                        _beoCollection.Remove(beo);
                        beo.Delete();
                    }
                }
            }

        }
        public T BEObyIndex<T>(int index)
        {
            return (T)BEObyIndex(index);
        }
        public IBEO BEObyIndex(int aIndex)
        {
            try
            {
                if (aIndex < 0 || _DataTable.Rows.Count <= aIndex)
                {
                    return null;
                }

                DataRow row = _DataTable.Rows[aIndex];

                if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                {
                    object[] keys = fetchKeys(row);

                    if (!_beoCollection.Contains(keys))
                    {
                        IBEO beo = _Engine.CreateBEO(_ContextID);
                        ((BEO)beo).AttachSource(this, aIndex);

                        _beoCollection.Add(beo);

                        return beo;
                    }
                    else
                    {
                        return _beoCollection[keys];
                    }

                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        public T BEObyPropertyValue<T>(string aProperty, object aValue)
        {
            try
            {
                // fetch DataType from Property
                PropertyDef def = _Properties.GetWithPropertyName(aProperty);
                if (def != null)
                {
                    foreach (DataRow row in _DataTable.Rows)
                    {
                        if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached && row[def.ColumnName].Equals(aValue))
                        {
                            int idx = _DataTable.Rows.IndexOf(row);
                            return BEObyIndex<T>(idx);
                        }
                    }
                }
                return default(T);

            }
            catch
            {
                throw;
            }
        }
        public IBEO BEObyPropertyValue(string aProperty, object aValue)
        {
            try
            {
                // fetch DataType from Property
                PropertyDef def = _Properties.GetWithPropertyName(aProperty);
                if (def != null)
                {
                    foreach (DataRow row in _DataTable.Rows)
                    {
                        if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached && row[def.ColumnName].Equals(aValue))
                        {
                            int idx = _DataTable.Rows.IndexOf(row);
                            return BEObyIndex(idx);
                        }
                    }
                }
                return null;

            }
            catch 
            {
                throw;
            }
        }


        public IBEO BEObyPrimaryKey(object aPrimaryKey)
        {
            try
            {
                DataColumn[] priCols = _DataTable.PrimaryKey;

                if (priCols.Length != 1)
                {
                    throw new ArgumentException("PrimaryKey columns count does not match");
                }

                string select = String.Empty;
                for (int i = 0; i < priCols.Length; i++)
                {
                    DataColumn col = priCols[i];
                    if (i > 0)
                        select += " AND ";

                    select += col.ColumnName + " = " + aPrimaryKey.ToString();
                }

                //if (rows.Length != 1)
                //{
                //    throw new InvalidOperationException("wrong row count by primarykey returned");
                //}

                //foreach (DataRow rowTemp in _DataTable.Rows)
                //{
                //    System.Diagnostics.Debug.WriteLine(_DataTable.Columns[0].ColumnName + ": " + rowTemp[0].ToString());
                //}

                DataRow[] rows = _DataTable.Select(select);
                if (rows.Length == 1)
                {
                    DataRow row = rows[0];
                    if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                    {
                        int rowID = _DataTable.Rows.IndexOf(row);

                        object[] keys = new object[] { aPrimaryKey };

                        if (!_beoCollection.Contains(keys))
                        {
                            IBEO beo = _Engine.CreateBEO(_ContextID);
                            ((BEO)beo).AttachSource(this, rowID);

                            _beoCollection.Add(beo);

                            return beo;
                        }
                        else
                        {
                            return _beoCollection[keys];
                        }

                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public IBEO BEObyPrimaryKey(object[] aPrimaryKeys)
        {
            try
            {
                DataRow row = _DataTable.Rows.Find(aPrimaryKeys);
                if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                {
                    int rowID = _DataTable.Rows.IndexOf(row);

                    if (!_beoCollection.Contains(aPrimaryKeys))
                    {
                        IBEO beo = _Engine.CreateBEO(_ContextID);
                        ((BEO)beo).AttachSource(this, rowID);

                        _beoCollection.Add(beo);

                        return beo;
                    }
                    else
                    {
                        return _beoCollection[aPrimaryKeys];
                    }
                }
                return null;
            }
            catch
            {
                throw;
            }
        }

        private object[] fetchKeys(DataRow row)
        {
            ArrayList list = new ArrayList();
            if (row != null)
            {
                PropertyMap map = this.Properties.PrimaryKeyProperties;
                for (int i = 0; i < map.Count; i++)
                {
                    PropertyDef def = map[i];
                    list.Add(row[def.ColumnName]);
                }
            }
            return list.ToArray();
        }

        private object[] TransformKey(BEOKey beokey)
        {
            ArrayList list = new ArrayList();
            int length;
            foreach (BEOKeyColumn col in beokey)
            {
                list.Add(col.Value);
            }
            return list.ToArray();
        }

        internal int RowIndexFromKey(BEOKey beokey)
        {
            ArrayList list = new ArrayList();
            foreach (BEOKeyColumn col in beokey)
            {
                list.Add(col.Value);
            }

            DataRow row = _DataTable.Rows.Find(list.ToArray());
            if (row != null && row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                return _DataTable.Rows.IndexOf(row);
            return -1;
        }

        public IBEO NewBEO()
        {
            return this.NewBEO(new object[] { Engine.nextSequence(this.TableName) });
        }

        public IBEO NewBEO(object aNewPrimaryKey)
        {
            object[] obj = new object[] { aNewPrimaryKey };

            return this.NewBEO(obj);
        }

        public IBEO NewBEO(object[] aNewPrimaryKeys)
        {
            try
            {
                // create beo
                IBEO beo = _Engine.CreateBEO(_ContextID);

                // create row
                DataRow dr = _DataTable.NewRow();

                // assignment of new primary key
                PropertyMap pmap = _Properties.PrimaryKeyProperties;
                if (aNewPrimaryKeys.Length != pmap.Count)
                {
                    throw new Exception("new PrimaryKey Count doesn't match the PrimaryKey Columns Count");
                }
                for (int i = 0; i < aNewPrimaryKeys.Length; i++)
                {
                    //dr[pmap[i].ColumnName] = Convert.ToInt32(aNewPrimaryKeys[i]);
                    dr[pmap[i].ColumnName] = aNewPrimaryKeys[i];
                }
                
                // add row
                _DataTable.Rows.Add(dr);

                ((BEO)beo).AttachSource(this, _DataTable.Rows.IndexOf(dr));
                //beo.RegisterBusinessObject();

                _beoCollection.Add(beo);

                // new beo from Definition
                if (_RetrieveContext == null)
                {
                    _RetrieveContext = new RetrieveContext("PrimaryKey", beo.Key); 
                }

                return beo;
            }
            catch
            {
                throw;
            }
        }

        public void CopyForeignKeys(IBEO source, IBEO target, string relation)
        {
            PropertyDef def = _Properties.GetRelationProperty(relation);
            try
            {
                if ((source != null) && (target != null))
                {
                    object val = source.GetSourceValue(def.FKField);
                    target.SetSourceValue(def.PropertyName, val);
                }
            }
            catch
            {
                throw new ArgumentException("relation is unkown");
            }
        }


        internal bool CheckDeleted(int aRowIndex)
        {
            try
            {
                if (aRowIndex < 0)
                    return true;

                DataRow row = _DataTable.Rows[aRowIndex];
                if (row != null)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        internal bool CheckAdded(int aRowIndex)
        {
            try
            {
                DataRow row = _DataTable.Rows[aRowIndex];
                if (row != null)
                {
                    if (row.RowState == DataRowState.Added)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal bool CheckModified(int aRowIndex)
        {
            try
            {
                DataRow row = _DataTable.Rows[aRowIndex];
                if (row != null)
                {
                    if (row.RowState == DataRowState.Modified)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        internal bool CheckDetached(int aRowIndex)
        {
            try
            {
                DataRow row = _DataTable.Rows[aRowIndex];
                if (row != null)
                {
                    if (row.RowState == DataRowState.Detached)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal object GetSourceValue(int aRowIndex, string aPropertyName)
        {
            string colName = _Properties.ColumnName(aPropertyName);
            if (colName == String.Empty)
                return null;

            DataRow row = _DataTable.Rows[aRowIndex];
            if (row != null && row.RowState != DataRowState.Deleted) 
            {
                if (row[colName] is DBNull)
                {
                    switch (_Properties.GetWithPropertyName(aPropertyName).SystemType)
                    {
                        case "System.Byte":
                            return default(System.Byte);
                        case "System.Int32":
                            return default(System.Int32);
                        case "System.Int64":
                            return default(System.Int64);
                        case "System.Double":
                            return default(System.Double);
                        case "System.DateTime":
                            return DateTime.Parse("01-01-1970");
                        case "System.Boolean":
                            return false;
                        case "System.Byte[]":
                            return new byte[] {};
                        default:
                            return String.Empty;
                    }
                }
                else
                    return _DataTable.Rows[aRowIndex][colName];
            }
            return null;
        }
        internal bool SetSourceValue(int aRowIndex, string aPropertyName, object aValue)
        {
            string colName = _Properties.ColumnName(aPropertyName);
            if (colName == String.Empty)
                return false;

            DataRow row = _DataTable.Rows[aRowIndex];
            if (row != null && row.RowState != DataRowState.Deleted) 
            {
                row[colName] = aValue;
                return true;
            }
            return false;
        }

        internal object Delete(int aRowIndex)
        {
            DataRow row = _DataTable.Rows[aRowIndex];
            if (row != null)
            {
                object[] keys = fetchKeys(row);
                if (_beoCollection.Contains(keys))
                {
                    IBEO beo = _beoCollection[keys];
                    _beoCollection.Remove(beo);
                    beo = null;
                } 
                row.Delete();
                return true;
            }
            return false;
        }

        public List<T> AsList<T>()
        {
            List<T> list = new List<T>();
            foreach (IBEO beo in this)
            {
                if (beo is T)
                    list.Add((T)beo);
            }
            return list;
        }

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            if (_Enumerator == null)
            {
                _Enumerator = new BCOEnumerator(this);
            }
            else
            {
                _Enumerator.Reset();
            }
            return _Enumerator;
        }

        public BCOEnumerator GetBCOEnumerator()
        {
            if (_Enumerator == null)
            {
                _Enumerator = new BCOEnumerator(this);
            }
            else
            {
                //_Enumerator.Reset();
            }
            return _Enumerator;
        }
        #endregion
    }
}
