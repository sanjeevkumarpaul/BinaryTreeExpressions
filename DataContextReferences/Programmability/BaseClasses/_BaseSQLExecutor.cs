using Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses.Interface;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.DBParameterType.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.BaseClasses
{
    public class _BaseSQLExecutor 
    {
        protected List<Object> parameters { get; set; }
        protected List<ObjectParameter> objParametes { get; set; }

        protected string paramPlaceHolder = string.Empty;
        protected string objParamPlaceHolder = string.Empty;

        public _BaseSQLExecutor()
        {
            parameters = new List<object>();
            objParametes = new List<ObjectParameter>();
        }

        protected void SetParams(params DbFunctionParam[] parameters)
        {
            Int32 index = 0;
            for (index = 0; index < parameters.Length; index++)
            {
                var param = parameters[index];

                this.parameters.Add(param.Value);
                this.objParametes.Add(param.HasValue ? new ObjectParameter(string.Format("p{0}", index), param.Value) :
                                                      new ObjectParameter(string.Format("p{0}", index), param.GetParamType()));

                paramPlaceHolder += string.Format("{{{0}}},", index);
                objParamPlaceHolder += string.Format("@p{0},", index);
            }
        }
    }
}
