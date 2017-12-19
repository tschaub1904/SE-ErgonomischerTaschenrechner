using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using org.mariuszgromada.math.mxparser;

namespace ErgoCalculatorUI.Calculator
{
    /// <summary>
    /// Diese Klasse dient zu Evaluierung von Rechenzeichenketten
    /// </summary>
    public class ExpressionEvaluator
    {
        private static AngleMode mAngleMode;
        public static AngleMode AngleMode { get { return mAngleMode; } set { mAngleMode = value; } }

        /// <summary>
        /// Errechnet anhand einer Zeichenkette ein Ergebnis. Bei fehlerhafter Eingabe gibt sie NaN
        /// </summary>
        /// <param name="expression">Die Rechenzeichenkette</param>
        /// <returns>Das Ergebnis der Rechnung</returns>
        public static double EvaluateStringExpression(string expression)
        {
            Expression exp = new Expression(expression);
            
            return exp.calculate();
        }
    }
}
