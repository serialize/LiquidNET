using System;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    public interface IBusinessDelegate
    {
        IBEO CreateBEO(int aContextID);
    }
}
