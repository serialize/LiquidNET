using System;
using System.Collections.Generic;
using System.Text;

namespace Serialize.LiquidNET
{
    [Serializable()]
    public class RetrieveContext
    {
        /// <summary>
        /// Context property
        /// </summary>
        public string Context
        {
            get;
            private set;
        }

        /// <summary>
        /// BEOKey property
        /// </summary>
        public BEOKey Key
        {
            get;
            private set;
        }

        /// <summary>
        /// User property
        /// </summary>
        public IBEO User 
        { 
            get; 
            private set; 
        }

        public RetrieveContext(string context, BEOKey key)
            : this(context, key, null) { }
        public RetrieveContext(string context, BEOKey key, IBEO user)
        {
            Context = context;
            Key = key;
            User = user;
        }

    }
}
