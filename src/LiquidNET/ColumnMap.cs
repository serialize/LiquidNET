using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class ColumnMap : IEnumerable
    {
        private List<ColumnDef> _List;

        internal ColumnMap()
        {
            _List = new List<ColumnDef>();
        }

        public ColumnMap PrimaryKeyColumns
        {
            get
            {
                ColumnMap map = new ColumnMap();

                foreach (ColumnDef def in this)
                {
                    if (def.KeyType == KeyType.PrimaryKey)
                    {
                        map.AddColumnDef(def.ColumnIndex, def.ColumnName, def.DBType, def.SystemType, def.KeyType);
                    }
                }
                return map;
            }
        }


        internal void AddColumnDef(
            int aColumnIndex,
            string aColumnName)
        {
            ColumnDef def = new ColumnDef(aColumnIndex,
                                                aColumnName);
            _List.Add(def);
        }
        internal void AddColumnDef(
            int aColumnIndex,
            string aColumnName,
            KeyType aKeyType)
        {
            ColumnDef def = new ColumnDef(aColumnIndex,
                                                aColumnName,
                                                aKeyType);
            _List.Add(def);
        }
        internal void AddColumnDef(
            int aColumnIndex,
            string aColumnName,
            string aDBType,
            string aSystemType,
            KeyType aKeyType)
        {
            ColumnDef def = new ColumnDef(aColumnIndex,
                                                aColumnName,
                                                aDBType,
                                                aSystemType,
                                                aKeyType);
            _List.Add(def);
        }

        public ColumnDef fromIndex(int aIndex)
        {
            int idx = 0;
            ColumnDef def = null;
            foreach (ColumnDef _def in this)
            {
                if (idx == aIndex)
                {
                    def = _def;
                    break;
                }
                idx++;
            }
            return def;
        }
        public ColumnDef fromName(string aName)
        {
            ColumnDef def = null;
            foreach (ColumnDef _def in this)
            {
                if (def.ColumnName == aName)
                {
                    def = _def;
                    break;
                }
            }
            return def;
        }
        public ColumnDef this[int aIndex]
        {
            get
            {
                return _List[aIndex];
            }
        }

        public int Count
        {
            get
            {
                return _List.Count;
            }
        }
        public IEnumerator GetEnumerator()
        {
            return _List.GetEnumerator();
        }


    }
}
