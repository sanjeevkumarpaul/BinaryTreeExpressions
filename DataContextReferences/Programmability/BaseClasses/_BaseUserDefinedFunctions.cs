using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ex.Common.Extension;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

using Ex.Audit.ExternalSources.DataContextReferences.Programmability.DBParameterType.Interface;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses.Interface;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses
{
    public abstract class _BaseUserDefinedFunctions : _BaseSQLExecutor, IUserDefinedFunction
    {
        public abstract object Execute(DbContext context  );   //ABSTRACT METHOD...
        public abstract IQueryable<T> Execute<T>(DbContext context) where T: class;

        public IUserDefinedFunction SetParameters(params DbFunctionParam[] parameters)
        {
            base.SetParams(parameters);

            return this;
        }
        
        protected string ExecuteScalar(DbContext context, string functionName)
        {
            string sqlQuery = string.Format("SELECT [dbo].[{0}] ( {1} )", functionName,  paramPlaceHolder.TrimEx(",") ); ;

            try
            {
                return context
                        .Database
                        .SqlQuery<string>( sqlQuery
                                          ,parameters.ToArray())
                        .FirstOrDefault()
                        .ToString()
                        .ToNull();
            }
            catch
            {
                return "";
            }
        }

        protected IQueryable<T> ExecuteSet<T>(DbContext context, string functionName) where T: class
        {
            string sqlQuery = string.Format("[{0}].[{1}] ( {2} )", context.GetType().Name, functionName, objParamPlaceHolder.TrimEx(",")); ;

            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            try
            {
                var _T = objContext.CreateQuery<T>(sqlQuery, this.objParametes.ToArray());
                return _T;
            }
            catch
            {
                return null;
            }
        }

    }
}