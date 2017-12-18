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
using System.Collections.ObjectModel;

namespace ErgoCalculatorUI
{

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private List<String> calculation = new List<String>();
        private ObservableCollection<CalculatorExpression> pastCalculations = new ObservableCollection<CalculatorExpression>();
        private double lastResult = 0;

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

            btnDegRad.Content = Enum.GetName(typeof(AngleMode), ExpressionEvaluator.AngleMode);

            listViewCalculationHistory.ItemsSource = pastCalculations;
            listViewCalculationHistory.MouseDoubleClick += ListViewCalculationHistory_MouseDoubleClick;
            listViewCalculationHistory.KeyDown += ListViewCalculationHistory_KeyDown;
        }

        private void ListViewCalculationHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            HistoryItemToCalculation();
        }

        private void ListViewCalculationHistory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            HistoryItemToCalculation();
        }

        private void HistoryItemToCalculation()
        {
            if (listViewCalculationHistory.SelectedItem == null)
                return;

            calculation = ((CalculatorExpression)listViewCalculationHistory.SelectedItem).Calculation.ToList<string>();
            UpdateCalculationText();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            calculation.Add(((Button)sender).Content.ToString());
            UpdateCalculationText();
        }

        private void btnDegRad_Click(object sender, RoutedEventArgs e)
        {
            int nextMode = ((int)ExpressionEvaluator.AngleMode + 1) % Enum.GetValues(typeof(AngleMode)).Length;
            ExpressionEvaluator.AngleMode = (AngleMode)nextMode;
            Enum.GetName(typeof(AngleMode), ExpressionEvaluator.AngleMode);
            btnDegRad.Content = Enum.GetName(typeof(AngleMode), ExpressionEvaluator.AngleMode);
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
                else
                {
                    switch (token)
                    {
                        case "ANS":
                            preParse.Add(lastResult.ToString().Replace(",", "."));
                            break;
                        case "EXP":
                            preParse.Add("*10^");
                            break;
                        case "𝞹":
                            preParse.Add(Math.PI.ToString().Replace(",", "."));
                            break;
                        case "×":
                            preParse.Add("*");
                            break;
                        case "÷":
                            preParse.Add("/");
                            break;
                        case ",":
                            preParse.Add(".");
                            break;
                        default:
                            preParse.Add(token);
                            break;
                    }

                    pos = preParse.Count;
                }
            }
            var parsedOutputString = string.Join("", preParse.ToArray());
            var expression = new org.mariuszgromada.math.mxparser.Expression(parsedOutputString);
            double result = ExpressionEvaluator.EvaluateStringExpression(parsedOutputString);
            lastResult = result;

            pastCalculations.Add(new CalculatorExpression { Calculation = calculation.ToArray() });
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
