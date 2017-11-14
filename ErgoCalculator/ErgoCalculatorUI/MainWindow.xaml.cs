using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ErgoCalculatorUI.Calculator;

namespace ErgoCalculatorUI
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        List<String> calculation = new List<String>();
        AngleMode currentAngleMode = AngleMode.RAD;
        ShuntingYardParser parser = new ShuntingYardParser();

        public MainWindow()
        {
            InitializeComponent();
            
            foreach(Button btn in FindVisualChildren<Button>(buttonGrid))
            {
                btn.Click += Btn_Click;
            }
            btnDEL.Click -= Btn_Click;
            btnAC.Click -= Btn_Click;
            btnEquals.Click -= Btn_Click;

            btnDegRad.Content = Enum.GetName(typeof(AngleMode), currentAngleMode);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            calculation.Add(((Button)sender).Content.ToString());
            UpdateCalculationText();
        }

        private void btnDegRad_Click(object sender, RoutedEventArgs e)
        {
            int nextMode = ((int)currentAngleMode + 1) % Enum.GetValues(typeof(AngleMode)).Length;
            currentAngleMode = (AngleMode)nextMode;
            Enum.GetName(typeof(AngleMode), currentAngleMode);
            btnDegRad.Content = Enum.GetName(typeof(AngleMode), currentAngleMode);
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Open Settings Window
        }

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            // Parse String to calculate
            List<string> preParse = new List<string>();
            int pos = 0;
            int output;
            foreach (var token in calculation)
            {
                if (Int32.TryParse(token, out output))
                {
                    if (preParse.Count == pos) preParse.Add(token);
                    else preParse[pos] += token;
                }
                else if (token == ",")
                {
                    preParse[pos] += ".";
                }
                else
                {
                    preParse.Add(token);
                    pos = preParse.Count;
                }
            }

            string[] parsedOutput = parser.Parse(preParse.ToArray());
            decimal result = RPNEvaluator.Evaluate(string.Join(" ", parsedOutput));

            // TODO: save in history
            calculation.Clear();
            calculation.Add(result.ToString());
            UpdateCalculationText();

        }

        private void btnAC_Click(object sender, RoutedEventArgs e)
        {
            // Delete calculation
            calculation.Clear();
            UpdateCalculationText();
        }

        private void btnDEL_Click(object sender, RoutedEventArgs e)
        {
            // Delete last Operation
            if (calculation.Count == 0)
                return;

            calculation.RemoveAt(calculation.Count - 1);
            UpdateCalculationText();
        }

        private void UpdateCalculationText()
        {
            StringBuilder sb = new StringBuilder();

            if (calculation.Count == 0)
                sb.Append(0);
            else
                calculation.ForEach(x => sb.Append(x));

            txtCalculation.Text = sb.ToString();
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
