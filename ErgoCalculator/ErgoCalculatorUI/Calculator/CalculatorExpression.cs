using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ErgoCalculatorUI.Calculator
{
    public enum AngleMode
    {
        DEG,
        RAD
    }

    public class CalculatorExpression
    {
        private string[] mCalculation;

        public string[] Calculation
        {
            get { return mCalculation; }
            set { mCalculation = value; }
        }

        public override string ToString()
        {
            return String.Join("", Calculation);
        }
    }
}
