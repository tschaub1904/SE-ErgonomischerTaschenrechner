using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.mariuszgromada.math.mxparser;

namespace ErgoCalculatorUI.Calculator
{
    public class ExpressionEvaluator
    {
        private static AngleMode mAngleMode;
        public static AngleMode AngleMode { get { return mAngleMode; } set { mAngleMode = value; } }

        public static double EvaluateStringExpression(string expression)
        {
            Expression exp = new Expression(expression);
            
            return exp.calculate();
        }
    }
}
