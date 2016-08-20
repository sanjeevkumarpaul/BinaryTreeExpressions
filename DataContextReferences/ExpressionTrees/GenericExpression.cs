sing System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

using Ex.Common.Extension;
using System.Reflection;

namespace Ex.Audit.ExternalSources.DataContextReferences.ExpressionTrees
{
    public sealed partial class GenericExpression<T> where T : class   //Sealed - because it can not be inherited.
    {
        internal const string Expression_Lambda_Name = "Cond";
        internal readonly ParameterExpression paramTypeExp = Expression.Parameter(typeof(T), Expression_Lambda_Name);

        internal GenericExpressionOptions Options = null;
        
        public GenericExpression(string filter, GenericExpressionOptions options = null)
        {
            Options = options ?? new GenericExpressionOptions();
            Lambda = ParseFilter(filter);
        }

        public Expression<Func<T, bool>> Lambda { get; set; } 
    }


    public sealed partial class GenericExpression<T>
    {
        #region ^Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private Expression<Func<T, bool>> ParseFilter(string filter)
        {
            var Operands = SplitToFraction(filter);

            Expression MainExp = null;

            foreach (var _operand in Operands.Where(o => o.Value.HasValue).Select(o => o.Value))
            {
                MainExp = MainExp == null ? _operand.OperandExp :  
                                            ( Options.Logic == LogicalOperand.AND ?    Expression.AndAlso(MainExp, _operand.OperandExp) : 
                                                                                       Expression.OrElse(MainExp, _operand.OperandExp) 
                                            );
            }

            if (MainExp == null)
                MainExp = Expression.NotEqual(paramTypeExp, null);

            var lambda = Expression.Lambda<Func<T, bool>>(MainExp, paramTypeExp);

            return lambda;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private Dictionary<int, Operand<T>> SplitToFraction(string filter)
        {
            Dictionary<int, Operand<T>> dictFractions = new Dictionary<int, Operand<T>>();
            Int32 index = 0;
           
            foreach (string partf in SplitFilter(filter))
            {
                Operand<T> operand;
                if ((operand = new Operand<T>(partf, paramTypeExp, Options)).HasValue) dictFractions.Add(index++, operand);                
            }

            return dictFractions;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private string[] SplitFilter(string filter)
        {
            var filters = Regex.Matches(filter, @"\((.*?)\)").OfType<Match>().Select(m => m.Groups[1].Value.TrimStart(new char[] { '(' }).Trim()).ToArray();

            return filters.Length <= 0 ? new string[] { filter } : filters;
        }
        #endregion ~Private methods.

    }
}