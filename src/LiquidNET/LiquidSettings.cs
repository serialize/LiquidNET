using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Serialize.LiquidNET
{
    public sealed class LiquidSettings : ConfigurationSection
    {
        private const string connectionStringNameProperty = "connectionString";
        private const string businessDelegateTypeNameProperty = "businessDelegate";
        private const string dataDelegateTypeNameProperty = "dataDelegate";

        public const string SectionName = "serialize.liquidNET";


        public static LiquidSettings GetConfiguration()
        {
            return (LiquidSettings)ConfigurationManager.GetSection(SectionName);
        }

        public String ConnectionString
        {
            get 
            {
                ConnectionStringSettings connectionConfig = ConfigurationManager.ConnectionStrings[ConnectionStringName];
                if (connectionConfig == null)
                    return String.Empty;

                return connectionConfig.ConnectionString;
            }
        }

        [ConfigurationProperty(connectionStringNameProperty, IsRequired=true)]
        public String ConnectionStringName
        {
            get
            {
                return (string)this[connectionStringNameProperty];
            }
            set
            {
                this[connectionStringNameProperty] = value;
            }
        }

        [ConfigurationProperty(businessDelegateTypeNameProperty, IsRequired = true)]
        public String BusinessDelegateTypeName
        {
            get
            {
                return (string)this[businessDelegateTypeNameProperty];
            }
            set
            {
                this[businessDelegateTypeNameProperty] = value;
            }
        }

        [ConfigurationProperty(dataDelegateTypeNameProperty, IsRequired = true)]
        public String DataDelegateTypeName 
        {
            get
            {
                return (string)this[dataDelegateTypeNameProperty];
            }
            set
            {
                this[dataDelegateTypeNameProperty] = value;
            }
        }

    }
}
