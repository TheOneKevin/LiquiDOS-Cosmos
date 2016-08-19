using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquiDOS
{
    public class MathParser
    {
        public const char END_ARG = ')';
        public const char START_ARG = '(';

        public static int getPriority(char action)
        {
            switch (action)
            {
                case '+': return 2;
                case '-': return 2;
                case '*': return 3;
                case '/': return 3;
                case '^': return 4;
            }
            return 0;
        }

        public static bool canMergeCell(Cell left, Cell right)
        {
            return getPriority(left.action) >= getPriority(right.action);
        }

        public static void mergeCells(Cell left, Cell right)
        {
            switch (left.action)
            {
                case '^': left.value = Math.Pow(left.value, right.value); break;
                case '*': left.value *= right.value; break;
                case '/': left.value /= right.value; break;
                case '+': left.value += right.value; break;
                case '-': left.value -= right.value; break;
            }
            left.action = right.action;
        }

        public static double Merge(Cell current, ref int index, List<Cell> listToMerge, bool mergeOnly = false)
        {
            while (index < listToMerge.Count)
            {
                Cell mergeNext = listToMerge[index++];
                while (!canMergeCell(current, mergeNext))
                    Merge(mergeNext, ref index, listToMerge, true);
                mergeCells(current, mergeNext);
                if (mergeOnly)
                    return current.value;
            }
            return current.value;
        }

        public static double loadAndCalc(string data, ref int from, char to = END_ARG)
        {
            if (from >= data.Length || data[from] == to) {
                Console.WriteLine("Loaded invalid data: " + data);
                return -1;
            }

            List<Cell> listToMerge = new List<Cell>(16);
            StringBuilder foo = new StringBuilder();
            do
            {
                char ch = data[from++];
                if(stillCollecting(foo.ToString(), ch, to))
                {
                    foo.Append(ch);
                    if (from < data.Length && data[from] != to)
                        continue;
                }
                ParserFunction func = new ParserFunction(data, ref from, foo.ToString(), ch);
                double value = func.GetValue(data, ref from);
                char action = ValidAction(ch) ? ch : updateAction(data, ref from, ch, to);
                listToMerge.Add(new Cell(value, action));
                foo.Clear();
            }
            while (from < data.Length && data[from] != to);
            if (from < data.Length && (data[from] == END_ARG || data[from] == to))
                from++;
            Cell baseC = listToMerge[0];
            int index = 1;
            return Merge(baseC, ref index, listToMerge);
        }

        public static char updateAction(string item, ref int from, char ch, char to)
        {
            return ' ';
        }

        public static bool stillCollecting(string item, char ch, char to)
        {
            char stopColl = (to == END_ARG) ? END_ARG : to;
            return (item.Length == 0 && (ch == '-' || ch == END_ARG)) ||
                !(ValidAction(ch) || ch == START_ARG || ch == stopColl);
        }

        public static bool ValidAction(char ch)
        {
            return ch == '*' || ch == '+' || ch == '-' || ch == '^';
        }
    }

    public class ParserFunction
    {
        private ParserFunction m_impl;
        private static Dictionary<string, ParserFunction> m_functions = new Dictionary<string, ParserFunction>();
        private static IdentityFunction s_idFunction = new IdentityFunction();
        private static StrtodFunction s_strtodFunction = new StrtodFunction();

        internal ParserFunction(string data, ref int from, string item, char ch)
        {
            if(item.Length == 0 && ch == MathParser.START_ARG)
            {
                m_impl = s_idFunction;
                return;
            }
            if (m_functions.TryGetValue(item, out m_impl))
                return;
            s_strtodFunction.Item = item;
            m_impl = s_strtodFunction;
        }

        public static void AddFunction(string name, ParserFunction function)
        {
            m_functions[name] = function;
        }

        public double GetValue(string data, ref int from)
        {
            return m_impl.Evaluate(data, ref from);
        }

        protected virtual double Evaluate(string data, ref int from)
        {
            // The real implementation will be in the derived classes.
            return 0;
        }

        public ParserFunction()
        {
            m_impl = this;
        }
    }

    class IdentityFunction : ParserFunction
    {
        protected override double Evaluate(string data, ref int from)
        {
            return MathParser.loadAndCalc(data, ref from, MathParser.END_ARG);
        }
    }

    class StrtodFunction : ParserFunction
    {
        protected override double Evaluate(string data, ref int from)
        {
            double num;
            if (!Double.TryParse(Item, out num))
            {
                throw new ArgumentException("Could not parse token [" + Item + "]");
            }
            return num;
        }
        public string Item { private get; set; }
    }

    public class Cell
    {
        internal Cell(double left, char opr)
        {
            value = left; action = opr;
        }

        internal double value { get; set; }
        internal char action { get; set; }
    }
}