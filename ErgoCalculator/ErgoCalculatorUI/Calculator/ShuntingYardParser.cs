using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgoCalculatorUI.Calculator
{
    public enum AngleMode
    {
        DEG,
        RAD
    }

    struct Precedence
    {
        public Precedence(int precedence, bool associativity)
        {
            this.precedence = precedence;
            this.isLeft = associativity;
        }

        public int precedence;
        public bool isLeft;
    }
    
    public class ShuntingYardParser
    {
        private Dictionary<string, Precedence> operators = new Dictionary<string, Precedence>()
        {
            { "^", new Precedence(4, true) },
            { "×", new Precedence(3, false) },
            { "÷", new Precedence(3, false) },
            { "+", new Precedence(2, false) },
            { "-", new Precedence(2, false) },
        };

        

        public string[] Parse(string[] array)
        {
            Queue<String> output = new Queue<String>();
            Stack<String> stack = new Stack<String>();

            double result = 0;
            Precedence topOperator, currentOperator;

            foreach (var token in array)
            {
                if (Double.TryParse(token, out result))
                    output.Enqueue(token);
                else if (operators.ContainsKey(token))
                {
                    if (stack.Count > 0)
                    {
                        operators.TryGetValue(stack.Peek(), out topOperator);
                        operators.TryGetValue(token, out currentOperator);

                        // TODO: check if currentOperator is correct
                        while (topOperator.precedence >= currentOperator.precedence && !topOperator.isLeft)
                        {
                            output.Enqueue(stack.Pop());
                            if (stack.Count == 0)
                                break;
                            operators.TryGetValue(stack.Peek(), out topOperator);
                        }
                    }

                    stack.Push(token);
                }
                else if (token == "(")
                    stack.Push(token);
                else if (token == ")")
                {
                    while (stack.Peek() != "(")
                    {
                        output.Enqueue(stack.Pop());
                    }
                    stack.Pop();
                }

                Console.WriteLine(token);
               
            }
            while (stack.Count > 0)
            {
                output.Enqueue(stack.Pop());
            }

            return output.ToArray();
        }
    }
}
