using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Serialize.LiquidNET
{
    public interface IDLO
    {
        string TableName { get; }
        string TableOwner { get; }
        int ContextID { get; }
        DataTable DataTable { get; }
        ColumnMap ColumnMap { get; }
        RetrieveContext RetrieveContext { get; }

        string SelectCMD { get; set; }
        System.Data.CommandType SelectCMDType { get; set; }
        string UpdateCMD { get; set; }
        string InsertCMD { get; set; }
        string DeleteCMD { get; set; }

        bool PreRetrieve(RetrieveContext aRContext);
        bool PostRetrieve();
        bool PreUpdate();
        bool PostUpdate();

        System.Data.DataTable Retrieve(RetrieveContext aRContext);
        object Update(System.Data.DataTable aDataTable);

        void RegisterDataSource();
        void FinishRegistration();


    }
}
