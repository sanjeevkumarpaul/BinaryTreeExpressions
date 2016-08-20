sing System;
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
using Ex.Audit.ExternalSources.DataContextReferences._SQLManagementObject;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.StoredProcedures.Models;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses
{
    public abstract class _BaseStoredProcedure : _BaseSQLExecutor, IStoredProcedure
    {
        public abstract object Execute(DbContext context  );   //ABSTRACT METHOD...
        public abstract ObjectResult<T> Execute<T>(DbContext context) where T: class;

        public IStoredProcedure SetParameters(params DbFunctionParam[] parameters)
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

        protected ObjectResult<T> ExecuteSet<T>(DbContext context, string functionName) where T: class
        {
            string sqlQuery = string.Format("{0}", functionName);       //objParamPlaceHolder.TrimEx(",")

            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            try
            {                
                List<ObjectParameter> paramList = new List<ObjectParameter>();
                using (var smo = new SMO(context))
                {
                    var _pName = smo.StoredProcedureParams(functionName);
                    
                    for(int ind = 0 ; ind < this.objParametes.Count; ind++)
                    {
                        if (ind < _pName.Length) 
                            paramList.Add( new ObjectParameter(_pName[ind], this.objParametes[ind].Value ?? this.objParametes[ind].ParameterType ));
                    }
                }
                                                
                var _T = objContext.ExecuteFunction<T>(sqlQuery, paramList.ToArray()); //must use named parameters.
                return _T;
            }
            catch
            {
                return null;
            }
        }

    }
}