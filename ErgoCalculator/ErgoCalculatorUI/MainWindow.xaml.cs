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

namespace ErgoCalculatorUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> calculation = new List<String>();

        public MainWindow()
        {
            InitializeComponent();
            
            foreach(Button btn in FindVisualChildren<Button>(buttonGrid))
            {
                btn.Click += Btn_Click;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(((Button)sender).Content);
            calculation.Add(((Button)sender).Content.ToString());
            UpdateCalculationText();
        }

        private void btnDegRad_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = "RAD";
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            // Open Settings Window
        }

        private void btnEquals_Click(object sender, RoutedEventArgs e)
        {
            // Parse String to calculate
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
            calculation.RemoveAt(calculation.Count - 1);
        }

        private void UpdateCalculationText()
        {
            StringBuilder sb = new StringBuilder();

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
