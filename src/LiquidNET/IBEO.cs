using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    public interface IBEO
    {
        BEOKey Key { get; }
        bool IsDeleted { get; }
        bool IsNew { get; }
        bool IsModified { get; }

        //void AttachSource(BCO aBCO, int aRowIndex);
        void RegisterBusinessObject();
        bool SetSourceValue(string aPropertyName, object aValue);
        object GetSourceValue(string aPropertyName);
        object Save();
        object Delete();

        bool PreSave();
        bool PostSave();
        bool PreDelete();
        bool PostDelete();
        BEOKey RelationshipKey(string aRelation);
        bool Validate();
    }
}
