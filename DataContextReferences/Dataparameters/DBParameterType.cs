using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.DBParameterType.Interface
{
    public enum DbParameterType
    {
        INT,
        STRING,
        DATE
    }

    public class DbFunctionParam
    {
        public DbFunctionParam()
        {
            DbType = DbParameterType.STRING;
        }

        public DbParameterType DbType { get; set; }
        public object Value { get; set; }

        public bool HasValue { get { return Value != null; } }

        public Type GetParamType()
        {
            switch(DbType)
            {
                case DbParameterType.INT: return typeof(int);
                case DbParameterType.STRING: return typeof(string);
                case DbParameterType.DATE: return typeof(DateTime);
            }

            return null;
        }
    }
}