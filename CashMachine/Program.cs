using System;
using System.Collections.Generic;

namespace CashMachine
{
    class Program
    {
        private static List<int> _newCombination = new List<int>();
        private static int[] _arrayOfNumbersInclusion;
        private static int[] _maxInclusion;

        private static bool NextCombination(int index)
        {
            if (index == 0 && _arrayOfNumbersInclusion[0] == _maxInclusion[0])
            {
                return true;
            }

            var tmp = 1;
            while (true)
            {
                if (index - tmp == 0 && _arrayOfNumbersInclusion[0] == _maxInclusion[0])
                {
                    return true;
                }

                if (_arrayOfNumbersInclusion[index - tmp] + 1 <= _maxInclusion[index - tmp])
                {
                    _arrayOfNumbersInclusion[index - tmp]++;
                    for (var i = index - tmp + 1; i < _arrayOfNumbersInclusion.Length; i++)
                    {
                        _arrayOfNumbersInclusion[i] = 0;
                    }

                    break;
                }

                tmp++;
            }

            return false;
        }

        private static List<int> AddCombination(IReadOnlyList<int> exchange)
        {
            _newCombination = new List<int>();
            for (var i = _arrayOfNumbersInclusion.Length - 1; i >= 0; i--)
            {
                for (var j = 0; j < _arrayOfNumbersInclusion[i]; j++)
                {
                    _newCombination.Add(exchange[i]);
                }
            }

            return _newCombination;
        }

        private static List<int> CheckTrivialCombination(int banknote, IReadOnlyList<int> exchange)
        {
            _newCombination = new List<int>();
            if (exchange[0] != exchange[exchange.Count - 1]) return null;

            for (var i = 0; i < banknote / exchange[0]; i++)
            {
                _newCombination.Add(exchange[0]);
            }

            return _newCombination;
        }

        private static void FindMaxInclusion(int banknote, int[] exchange)
        {
            for (int i = 0; i < exchange.Length; i++)
            {
                _maxInclusion[i] = banknote / exchange[i];
            }
        }

        private static List<List<int>> FindCombinations(int banknote, IReadOnlyList<int> exchange)
        {
            var result = new List<List<int>>();
            var isEnd = false;
            while (!isEnd)
            {
                var sum = 0;
                var isChange = false;
                for (var i = 0; i < exchange.Count; i++)
                {
                    sum += _arrayOfNumbersInclusion[i] * exchange[i];
                    if (sum == banknote)
                    {
                        result.Add(AddCombination(exchange));
                        isEnd = NextCombination(i);
                        isChange = true;
                        break;
                    }

                    if (i >= exchange.Count - 1 ||
                        banknote - sum >= _arrayOfNumbersInclusion[i + 1] * exchange[i + 1])
                    {
                        continue;
                    }

                    isEnd = NextCombination(i + 1);
                    isChange = true;
                    break;
                }

                if (!isChange)
                {
                    isEnd = NextCombination(_arrayOfNumbersInclusion.Length);
                }
            }

            return result;
        }

        private static List<List<int>> ExchangeSumToCoins(int banknote, int[] exchange)
        {
            var result = new List<List<int>>();
            Array.Sort(exchange);
            var trivialCombination = CheckTrivialCombination(banknote, exchange);
            if (trivialCombination != null)
            {
                result.Add(trivialCombination);
                return result;
            }

            _arrayOfNumbersInclusion = new int[exchange.Length];
            _maxInclusion = new int[exchange.Length];
            FindMaxInclusion(banknote, exchange);
            return FindCombinations(banknote, exchange);
        }

        private static void CheckInputValue(int value)
        {
            if (value <= 0)
            {
                throw new FormatException();
            }
        }

        private static void PrintResult(IReadOnlyCollection<List<int>> result)
        {
            if (result != null)
            {
                Console.WriteLine("Найдено " + result.Count + " комбинаций");
                foreach (var combination in result)
                {
                    foreach (var note in combination)
                    {
                        Console.Write(note + " ");
                    }

                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("Найдено 0 комбинаций");
            }
        }

        private static int Main(string[] args)
        {
            Console.WriteLine("Введите банкноту");
            int banknote;
            try
            {
                banknote = Convert.ToInt32(Console.ReadLine());
                CheckInputValue(banknote);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Введено недопустимое значение");
                return 1;
            }

            var exchange = new List<int>();

            Console.WriteLine("Введите купюры размена");
            try
            {
                while (true)
                {
                    var next = Console.ReadLine();
                    if (next.Equals("0"))
                    {
                        break;
                    }
                    CheckInputValue(Convert.ToInt32(next));
                    exchange.Add(Convert.ToInt32(next));
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine("Введено недопустимое значение");
                return 1;
            }

            Console.WriteLine();

            PrintResult(ExchangeSumToCoins(banknote, exchange.ToArray()));
            return 0;
        }
    }
}