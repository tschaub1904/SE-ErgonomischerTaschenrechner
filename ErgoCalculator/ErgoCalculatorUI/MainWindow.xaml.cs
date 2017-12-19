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

        /// <summary>
        /// Einstiegspunkt für das ErgoCalc Fenster
        /// </summary>
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

        #region listViewHistoryControls
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

        /// <summary>
        /// Fügt das momentan ausgewählte Item der Historie der Rechenzeile hinzu.
        /// </summary>
        private void HistoryItemToCalculation()
        {
            if (listViewCalculationHistory.SelectedItem == null)
                return;

            calculation = ((CalculatorExpression)listViewCalculationHistory.SelectedItem).Calculation.ToList<string>();
            UpdateCalculationText();
        }
        #endregion

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /* Diese Funktion wird verwendet, um das Rechnen mithilfe des Ziffernblocks zu ermöglichen
             * Hier werden Tastendrücke abgefangen und weiterverarbeitet
             */
            if (
                Keyboard.IsKeyDown(Key.LeftCtrl)
                || Keyboard.IsKeyDown(Key.LeftAlt)
                || Keyboard.IsKeyDown(Key.LeftShift)
                )
                return;

            string keyContent = "";
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    keyContent = "0";
                    break;
                case Key.D1:
                case Key.NumPad1:
                    keyContent = "1";
                    break;
                case Key.D2:
                case Key.NumPad2:
                    keyContent = "2";
                    break;
                case Key.D3:
                case Key.NumPad3:
                    keyContent = "3";
                    break;
                case Key.D4:
                case Key.NumPad4:
                    keyContent = "4";
                    break;
                case Key.D5:
                case Key.NumPad5:
                    keyContent = "5";
                    break;
                case Key.D6:
                case Key.NumPad6:
                    keyContent = "6";
                    break;
                case Key.D7:
                case Key.NumPad7:
                    keyContent = "7";
                    break;
                case Key.D8:
                case Key.NumPad8:
                    keyContent = "8";
                    break;
                case Key.D9:
                case Key.NumPad9:
                    keyContent = "9";
                    break;
                case Key.OemPlus:
                case Key.Add:
                    keyContent = "+";
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    keyContent = "-";
                    break;
                case Key.Multiply:
                    keyContent = "×";
                    break;
                case Key.Divide:
                    keyContent = "÷";
                    break;
                case Key.Decimal:
                    keyContent = ",";
                    break;
                case Key.Back:
                    if (calculation.Count > 0)
                    {
                        calculation.RemoveAt(calculation.Count - 1);
                        UpdateCalculationText();
                    }
                    return;
                case Key.Delete:
                    calculation.Clear();
                    UpdateCalculationText();
                    return;
                default:
                    return;
            }

            calculation.Add(keyContent);
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

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Wandelt Sonderzeichen in der Rechenzeichenkette in für das Rechenframework lesbare ausdrücke um
        /// und schickt den umgewandelten String weiter an den ExpressionEvaluator.
        /// </summary>
        private void Calculate()
        {
            List<string> preParse = new List<string>();
            int pos = 0;
            double parsedToken = 0;
            foreach (var token in calculation)
            {
                if (double.TryParse(token, out parsedToken))
                {
                    preParse.Add(parsedToken.ToString().Replace(",", "."));
                }
                else
                {
                    switch (token)
                    {
                        case "ANS":
                            preParse.Add(lastResult.ToString().Replace(",", "."));
                            break;
                        case "EXP":
                            preParse.Add("E");
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
                }
                pos = preParse.Count;
            }

            var parsedOutputString = string.Join("", preParse.ToArray());
            var expression = new org.mariuszgromada.math.mxparser.Expression(parsedOutputString);
            double result = ExpressionEvaluator.EvaluateStringExpression(parsedOutputString);
            lastResult = result;

            pastCalculations.Add(new CalculatorExpression { Calculation = calculation.ToArray() });
            calculation.Clear();
            calculation.Add(result.ToString());
            UpdateCalculationText();
            txtCalculation.Focus();
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

        /// <summary>
        /// Wandelt die Liste der Eingaben in eine Zeichenkette um und zeigt diese in der Rechenzeile an.
        /// </summary>
        private void UpdateCalculationText()
        {
            StringBuilder sb = new StringBuilder();

            if (calculation.Count == 0)
                sb.Append(0);
            else
                calculation.ForEach(x => sb.Append(x));

            txtCalculation.Text = sb.ToString();
        }

        /// <summary>
        /// Erstellt eine Liste aller Kindelemente eines ausgewählten Typs
        /// </summary>
        /// <typeparam name="T">Der zu findende Kindtyp</typeparam>
        /// <param name="depObj">Das Elternelement</param>
        /// <returns></returns>
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

        #region Executed Commands
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void FocusCalculationTextCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtCalculation.Focus();
        }
        private void FocusOperatorsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnPlus.Focus();
        }
        private void FocusNumpadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnNumpad1.Focus();
        }
        private void FocusTrigonometricsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnSin.Focus();
        }
        private void FocusExponentsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnPow2.Focus();
        }

        private void FocusExtrasCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            btnOpenParanthesis.Focus();
        }
        private void CalculateCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Calculate();
        }
#endregion
    }

    /// <summary>
    /// Diese Statische Klasse enthält Kommandos, die von der ErgoCalcUI aus ausgelöst werden können
    /// </summary>
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
        (
            "Beenden",
            "Beenden",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                    new KeyGesture(Key.Q, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand FocusCalculationText = new RoutedUICommand
        (
            "Rechenzeile Fokussieren",
            "Rechenzeile Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad0, ModifierKeys.Control)
            }
        );
        public static readonly RoutedUICommand FocusOperators = new RoutedUICommand
        (
            "Operatoren Fokussieren",
            "Operatoren Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad2, ModifierKeys.Control)
            }
        );
        public static readonly RoutedUICommand FocusNumpad = new RoutedUICommand
        (
            "Ziffernblock Fokussieren",
            "Ziffernblock Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad1, ModifierKeys.Control)
            }
        );

        public static readonly RoutedUICommand FocusExponents = new RoutedUICommand
        (
            "Exponenten Fokussieren",
            "Exponenten Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad3, ModifierKeys.Control),
                new KeyGesture(Key.E, ModifierKeys.Alt)
            }
        );
        public static readonly RoutedUICommand FocusTrigonometrics = new RoutedUICommand
        (
            "Trigonometrische Funktionen Fokussieren",
            "Trigonometrische Funktionen Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad4, ModifierKeys.Control),
                new KeyGesture(Key.T, ModifierKeys.Alt)
            }
        );
        public static readonly RoutedUICommand FocusExtras = new RoutedUICommand
        (
            "Sonderzeichen Fokussieren",
            "Sonderzeichen Fokussieren",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.NumPad5, ModifierKeys.Control),
                new KeyGesture(Key.X, ModifierKeys.Alt)
            }
        );
        public static readonly RoutedUICommand CalculateCommand = new RoutedUICommand
        (
            "Berechnen",
            "Berechnen",
            typeof(CustomCommands),
            new InputGestureCollection()
            {
                new KeyGesture(Key.E, ModifierKeys.Control)
            }
        );
    }
}
