using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class ServiceFactory
    {
        private IDataDelegate _Delegate;
        internal ServiceFactory(IDataDelegate aDelegate)
        {
            _Delegate = aDelegate;
        }

        internal ArrayList Retrieve(string aContext, RetrieveContext aRContext)
        {
            return Retrieve(_Delegate.getContextID(aContext), aRContext);
        }
        internal ArrayList Retrieve(int aContextID, RetrieveContext aRContext)
        {
            IDLO dlo = Init(aContextID);
            dlo.Retrieve(aRContext);

            return Format(dlo);
        }

        internal ArrayList Definition(string aContext)
        {
            return Definition(_Delegate.getContextID(aContext));
        }
        internal ArrayList Definition(int aContextID)
        {
            IDLO dlo = Init(aContextID);
            return Format(dlo);
        }

        internal object Update(ArrayList aBCORef)
        {
            int contextid = Convert.ToInt32(aBCORef[0]);
            DataTable table = (DataTable)aBCORef[2];

            IDLO dlo = Init(contextid);
            int rowsAffected = (int) dlo.Update(table);

            return Format(dlo);
        }
        
        
        
        private IDLO Init(int aContextID)
        {
            IDLO dlo = _Delegate.createDLO(aContextID);
            dlo.RegisterDataSource();

            dlo.FinishRegistration();

            return dlo;
        }

        private ArrayList Format(IDLO dlo)
        {
            ArrayList list = new ArrayList();

            list.Add(dlo.ContextID);
            list.Add(dlo.TableName);
            list.Add(dlo.DataTable);
            list.Add(dlo.ColumnMap);
            list.Add(dlo.RetrieveContext);

            return list;
        }




    }
}
