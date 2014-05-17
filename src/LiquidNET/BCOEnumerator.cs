using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class BCOEnumerator : IEnumerator
    {

        private int _RowIndex;
        private BCO _BCO;
        //private int temp; //3367.22333

        internal BCOEnumerator(BCO aBCO)
        {
            _RowIndex = -1;
            _BCO = aBCO;
        }

        public bool MoveNext()
        {
            if (_RowIndex < (_BCO.RowCount - 1))
            {
                _RowIndex++;
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Reset()
        {
            _RowIndex = -1;
        }
        public object Current
        {
            get
            {
                if (_RowIndex == -1)
                {
                    _RowIndex = 0;
                }
                if (_RowIndex < _BCO.RowCount)
                {
                    return _BCO.BEObyIndex(_RowIndex);
                }
                else
                {
                    return null;
                }
            }
        }
        public IBEO GetFirst()
        {
            if (_BCO.RowCount > 0)
            {
                _RowIndex = 0;
                return _BCO.BEObyIndex(0);
            }
            else
            {
                return null;
            }
        }
        public IBEO GetNext()
        {
            if (this.MoveNext())
                return this.Current as IBEO;
            else
                return null;
        }
        public IBEO GetLast()
        {
            if (_BCO.RowCount > 0)
            {
                _RowIndex = _BCO.RowCount - 1;
                return _BCO.BEObyIndex(_RowIndex);
            }
            else
            {
                return null;
            }
        }

    }
}
