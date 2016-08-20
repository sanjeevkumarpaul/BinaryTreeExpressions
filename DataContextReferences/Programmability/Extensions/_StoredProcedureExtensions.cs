using Ex.Audit.ExternalSources.DataContextReferences.Programmability.DBParameterType.Interface;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.StoredProcedures;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.StoredProcedures.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.Extensions
{
    public static class _StoredProcedureExtensions
    {

        public static ObjectResult<ReportAuditHistory> SP_GetAuditHistory(this DbContext context)
        {
            var res = new SP_GetAuditHistory()
                            .SetParameters(new DbFunctionParam() { DbType = DbParameterType.DATE, Value = new DateTime(2016, 3, 23) },
                                           new DbFunctionParam() { DbType = DbParameterType.DATE, Value = new DateTime(2016, 3, 23) },                                           
                                           new DbFunctionParam() { Value = "NBKOELO" },
                                           new DbFunctionParam() { Value = "Insurance" })
                            .Execute<ReportAuditHistory>(context);

            return res;
        }

    }
}