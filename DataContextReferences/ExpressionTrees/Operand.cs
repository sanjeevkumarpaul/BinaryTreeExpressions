using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Ex.Common.Extension;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Ex.Audit.ExternalSources.DataContextReferences.ExpressionTrees
{
    public sealed partial class GenericExpression<T>
    {
        private class Operand<O>
        {
            internal LogicalOperand Logic { get; private set; }
            internal List<Fraction> Fractions { get; private set; }
            internal Expression OperandExp { get; set; }

            internal bool HasValue { get { return Fractions.Count > 0; } }

            internal Operand(string fractionstr, ParameterExpression paramTypeExp, GenericExpressionOptions options)
            {
                ParseFractions(fractionstr, paramTypeExp);
                if (HasValue) BuildExpressions(paramTypeExp, options);
            }


            private void ParseFractions(string fractionstr, ParameterExpression paramTypeExp)
            {
                Fractions = new List<Fraction>();
                Logic = fractionstr.ContainsIgnoreCase(" and ") ? LogicalOperand.AND : LogicalOperand.OR;

                fractionstr = Regex.Replace(fractionstr, string.Format(" {0} ", Logic.ToString().ToLower()), " AND ", RegexOptions.IgnoreCase);

                foreach (string innerPartf in fractionstr.Split(new string[] { Logic.ToString() }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var predicates = innerPartf.SplitEx(' ');

                    if (predicates.Length != 3) continue;

                    Fractions.Add(new Fraction(predicates[0], predicates[2], predicates[1]));
                }
            }

            private void BuildExpressions(ParameterExpression paramTypeExp, GenericExpressionOptions options)
            {
                var typeProperties = typeof(O).PublicFields();

                foreach (var fraction in this.Fractions)
                {
                    if (!fraction.IsValid) continue;

                    string leftHandFieldName = fraction.LeftFractionType == FractionType.EXPRESSION ? typeProperties.ClosestFieldName(fraction.LeftHand) : "";
                    string rightHandFieldName = fraction.RightFractionType == FractionType.EXPRESSION ? typeProperties.ClosestFieldName(fraction.RightHand): "";

                    var fieldProperty = typeProperties.Find(P => P.Name.Equals(leftHandFieldName)) ?? typeProperties.Find(P => P.Name.Equals(rightHandFieldName));
                    
                    Expression leftExpression = BuildSingleExpression(paramTypeExp, fieldProperty, fraction.LeftHand, fraction.LeftFractionType, options);
                    Expression rightExpression = BuildSingleExpression(paramTypeExp, fieldProperty, fraction.RightHand, fraction.RightFractionType, options);

                    Expression compareExp = CombineExpressions(leftExpression, rightExpression, fraction.Operator);
                    if (compareExp != null)
                        OperandExp = OperandExp == null ? compareExp : Expression.AndAlso(OperandExp, compareExp);
                }

            }

            private Expression BuildSingleExpression(ParameterExpression paramTypeExp, PropertyInfo field, string fractionValue, FractionType fType, GenericExpressionOptions options)
            {
                Expression expression = null;

                if (!field.Null())
                {
                    if (fType == FractionType.EXPRESSION)
                        expression = Expression.Property(paramTypeExp, field.Name);                                            
                    else
                        expression = Expression.Constant(field.ConvertToPropertyType(fractionValue));

                    expression = PutStringMethodCalls(expression, field, options);                        
                }
                else expression = Expression.Constant(fractionValue.ToString());

                return expression;
            }

            private Expression PutStringMethodCalls(Expression propertyExp, PropertyInfo field, GenericExpressionOptions options)
            {
                if (field.IsString())
                {
                    if (options.StringCompareWithTrim)                                           
                        propertyExp = propertyExp.InjectStringMethod("Trim", null);                    

                    if (options.IgnoreCase)                    
                        propertyExp = propertyExp.InjectStringMethod("ToLower", null);                    
                }

                return propertyExp;
            }

            private Expression CombineExpressions(Expression leftHandExp, Expression rightHandExp, ExpressionType fracOperator)
            {
                switch (fracOperator)
                {
                    case ExpressionType.Equal: return Expression.Equal(leftHandExp, rightHandExp);
                    case ExpressionType.LessThan: return Expression.LessThan(leftHandExp, rightHandExp); 
                    case ExpressionType.LessThanOrEqual: return Expression.LessThanOrEqual(leftHandExp, rightHandExp); 
                    case ExpressionType.GreaterThan: return Expression.GreaterThan(leftHandExp, rightHandExp); 
                    case ExpressionType.GreaterThanOrEqual: return Expression.GreaterThanOrEqual(leftHandExp, rightHandExp);

                    default: return null;
                }
            }
        }
    }
}