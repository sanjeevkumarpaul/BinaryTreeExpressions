using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Ex.Common.Extension;
using System.Text.RegularExpressions;

namespace Ex.Audit.ExternalSources.DataContextReferences.ExpressionTrees
{
    public sealed partial class GenericExpression<T>
    {       
        private class Fraction
        {
            internal string LeftHand { get; private set; }
            internal string RightHand { get; private set; }
            internal ExpressionType Operator { get; private set; }
            internal FractionType LeftFractionType { get; private set; }
            internal FractionType RightFractionType { get; private set; }

            internal bool IsValid { get; private set; }

            internal Fraction() { }
            internal Fraction(string left, string right, string opr) 
            { 
                LeftHand = DetermineFractionType(left);
                RightHand = DetermineFractionType(right, false); 
                IsValid = DetermineOperator(opr); 
            }
            
            private string DetermineFractionType(string fractionValue, bool isLeft = true)
            {
                var _isConst = Regex.IsMatch(fractionValue, @"^[0-9]+$");                   //Check if its a number. "^\d$", will match ther digit characters like the Eastern Arabic numerals ٠١٢٣٤٥٦٧٨٩
                if (!_isConst) _isConst = Regex.IsMatch(fractionValue, @"[^']\w*[$']");     //Check if its a String. (Datetime should also determine as string)

                FractionType _fType = (_isConst) ? FractionType.CONSTANT : FractionType.EXPRESSION;

                if (isLeft) LeftFractionType = _fType; else RightFractionType = _fType;

                return fractionValue.TrimEx("'");
            }

            private bool DetermineOperator(string opr)
            {
                switch (opr.ToLower().Replace("-",""))
                {
                    case "==":
                    case "equals":
                    case "equal":
                    case "eq":
                    case "=": Operator = ExpressionType.Equal; break;

                    case "less":                    
                    case "lessthan":
                    case "lt":
                    case "<": Operator = ExpressionType.LessThan; break;

                    case "greater":                    
                    case "greaterthan":
                    case "gt":
                    case ">": Operator = ExpressionType.GreaterThan; break;

                    case "lessthanequals":
                    case "lessthanequal":
                    case "lessthanequalsto":
                    case "lessthanequalto":
                    case "lte":
                    case "<=": Operator = ExpressionType.LessThanOrEqual; break;

                    case "greaterthanequals":
                    case "greaterthanequal":
                    case "greaterthanequalsto":
                    case "greaterthanequalto":
                    case "gte":
                    case ">=": Operator = ExpressionType.GreaterThanOrEqual; break;

                    case "notequals":                    
                    case "notequalto":
                    case "notequalsto":
                    case "ne":
                    case "<>":
                    case "!=": Operator = ExpressionType.NotEqual; break;

                    default: return false;
                }

                return true;
            }
        }
    }
}