using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ex.Audit.ExternalSources.DataContextReferences.ExpressionTrees
{
    public sealed class GenericExpressionOptions
    {
        public GenericExpressionOptions()
        {
            Logic = LogicalOperand.AND;
            IgnoreCase = false;
            StringCompareWithTrim = true;
        }

        public LogicalOperand Logic { internal get; set; }
        public bool IgnoreCase { get; set; }
        public bool StringCompareWithTrim { get; set; }
    }
}
