using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class BEOKey : IEnumerable
    {
        private List<BEOKeyColumn> _List;

        public BEOKey()
        {
            _List = new List<BEOKeyColumn>();
        }


        public void Add(BEOKeyColumn aKeyColumn)
        {
            if (this.Contains(aKeyColumn.Name))
            {
                this.Remove(aKeyColumn.Name);
            }
            _List.Add(aKeyColumn);
        }

        public void Remove(string aColumnName)
        {
            foreach (BEOKeyColumn col in _List)
            {
                if (col.Name.Equals(aColumnName))
                {
                    _List.Remove(col);
                    break;
                }
            }
        }




        //public bool Equals(BEOKey key)
        //{
        //    if (key.Count != this.Count)
        //        return false;

        //    foreach(BEOKeyColumn col in key)
        //    {
        //        bool found = false;
        //        foreach (BEOKeyColumn col2 in this)
        //        {
        //            if (col.Equals(col2))
        //            {
        //                found = true;
        //                break;
        //            }
        //        }
        //        if (!found)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public bool Equals(object[] keyValues)
        {
            if (this.Count != keyValues.Length)
                return false;

            for (int i = 0; i < this.Count; i++)
            {
                if (!this[i].Equals(keyValues[i]))
                    return false;
            }
            return true;
        }

        public bool Equals(BEOKey key)
        {
            if (key.Count != this.Count)
                return false;

            var myCols = this.GetEnumerator();
            var extCols = key.GetEnumerator();

            while (myCols.MoveNext() && extCols.MoveNext())
            {
                if (!compare(myCols.Current, extCols.Current))
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is BEOKey)
                return Equals((BEOKey)obj);

            return base.Equals(obj);
        }

        public bool Contains(BEOKeyColumn keycol)
        {
            bool ret = false;
            foreach (BEOKeyColumn col in _List)
            { 
                if (col.Equals(keycol))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        public bool Contains(string aColumnName)
        {
            bool ret = false;
            foreach (BEOKeyColumn col in _List)
            {
                if (col.Name.Equals(aColumnName))
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        public void AddNew(string aName, string aDataType, object aValue)
        {
            BEOKeyColumn col = new BEOKeyColumn();
            col.Name = aName;
            col.DataType = aDataType;
            col.Value = aValue;

            this.Add(col);
        }
        public bool isMultiColumn
        {
            get
            {
                if (_List.Count > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public BEOKeyColumn this[int aIndex]
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


        private bool compare(object col1, object col2)
        {
            if (!(col1 is BEOKeyColumn) || !(col2 is BEOKeyColumn))
                return false;

            return ((BEOKeyColumn)col1).Equals((BEOKeyColumn)col2);
        }

        private bool compare(BEOKeyColumn col1, BEOKeyColumn col2)
        {
            return col1.Equals(col2);
        }
    }
}
