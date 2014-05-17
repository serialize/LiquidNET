using System;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    public interface IDataDelegate
    {
        IDLO createDLO(int aContextID);
        int getContextID(string aContext);
    }
}
