using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class BEOCollection : IEnumerable
    {
        private List<IBEO> _List;


        public BEOCollection()
        {
            _List = new List<IBEO>();
        }

        public IBEO this[int aIndex]
        {
            get
            {
                return _List[aIndex];
            }
        }

        public IBEO this[BEOKey key]
        {
            get
            {
                foreach (IBEO beo in _List)
                {
                    if (beo.Key.Equals(key))
                    {
                        return beo;
                    }
                }
                return null;
            }
        }

        public IBEO this[object[] keyValues]
        {
            get
            {
                foreach (IBEO beo in _List)
                {
                    if (beo.Key.Equals(keyValues))
                    {
                        return beo;
                    }
                }
                return null;
            }
        }

        internal List<IBEO> List
        {
            get
            {
                return _List;
            }
        }

        public int Count
        {
            get
            {
                return _List.Count;
            }
        }


        public void Add(IBEO beo)
        {
            _List.Add(beo);
        }

        public void Add(BEOCollection collection)
        {
            _List.InsertRange(_List.Count, collection.List);
        }


        public void Remove(IBEO beo)
        {
            if (_List.Contains(beo))
            {
                _List.Remove(beo);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _List.Count; i++)
            {
                _List[i] = null;
            }
            _List.Clear();
        }

        public bool Contains(IBEO beo)
        {
            foreach (IBEO beo2 in _List)
            {
                if (beo.Key.Equals(beo2.Key))
                    return true;
            }
            return false;
        }

        public bool Contains(object[] keyValues)
        {
            foreach (IBEO beo2 in _List)
            {
                if (beo2.Key.Equals(keyValues))
                    return true;
            }
            return false;
        }

        public void Insert(int index, IBEO beo)
        {
            _List.Insert(index, beo);
        }

        public void InsertRange(int index, BEOCollection collection)
        {
            _List.InsertRange(index, collection.List);
        }

        public IEnumerator GetEnumerator()
        {
            return _List.GetEnumerator();
        }

    }
}
