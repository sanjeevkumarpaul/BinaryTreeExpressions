using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ex.Common.Extension;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.Functions.Models;
using System.Data;

using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq.Expressions;
using Ex.Audit.ExternalSources.DataContextReferences.ExpressionTrees;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.DBParameterType.Interface;
using Ex.Audit.ExternalSources.DataContextReferences.Programmability.Functions;

namespace Ex.Audit.ExternalSources.DataContextReferences.Programmability.Extensions
{
    public static class _UserDefinedFunctionExtensions
    {
        public static string FN_AUDIT_FindProfileIDQueryFromCurrentEntity(this DbContext context, string tableName, string filterCondition)
        {

            var res = new Fn_Audit_FindProfileIDQueryFromCurrentEntity()
                            .SetParameters(new DbFunctionParam() { Value = tableName }, new DbFunctionParam() { Value = filterCondition })
                            .Execute(context);
            
            return res.ToStringExt();

            //MethodBase.GetCurrentMethod().DeclaringType.Name
        }

        public static IQueryable<ExReport> FN_Ex_GetExReportExcel_TRY(this DbContext context, string filterCondition)
        {

            var res = new FN_Ex_GetExReportExcel_TRY()
                            .SetParameters(new DbFunctionParam(), 
                                           new DbFunctionParam(),
                                           new DbFunctionParam() { DbType = DbParameterType.INT, Value = 0 }, 
                                           new DbFunctionParam() { Value = "ANYTHING" })
                            .Execute <ExReport>(context);

            var _condition = new GenericExpression<ExReport>("stateCode = 'ga' And pendingCount > 1", 
                                                                      new GenericExpressionOptions() { IgnoreCase = true }); //this condition will be filterCondition
            res = res.Where(_condition.Lambda);

            return res;
        }

    }
}