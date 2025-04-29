using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public class MergeSortInsert
    {
        /// <summary>
        /// Метод сортировки массива целых чисел посредством вставок и слияния (алгоритм Форда-Джонсона)
        /// </summary>
        /// <param name="array">Массив для сортировки</param>
        /// <param name="verbose">Флаг для вывода подробной информации во время сортировки</param>
        public static void Sort(int[] array, bool verbose = false)
        {
            if (array == null || array.Length <= 1)
                return;

            int[] result = FordJohnsonSort(array, verbose);
            Array.Copy(result, array, array.Length);
        }

        /// <summary>
        /// Реализация сортировка посредством вставок и слияния
        /// </summary>
        private static int[] FordJohnsonSort(int[] array, bool verbose = false)
        {
            int n = array.Length;
            int[] workingArray = (int[])array.Clone();
            
            if (verbose)
            {
                Console.WriteLine("Сортировка посредством вставок и слияния");
                Console.WriteLine("Исходный массив:");
                PrintArray(workingArray);
                Console.WriteLine();
            }

            // Шаг i: Сравнить [n/2] непересекающихся пар элементов
            List<Pair> pairs = new List<Pair>();
            
            if (verbose)
            {
                Console.WriteLine("Шаг i: Формирование пар элементов и их сравнение");
            }
            
            for (int i = 0; i < n / 2; i++)
            {
                int a = workingArray[i * 2];
                int b = i * 2 + 1 < n ? workingArray[i * 2 + 1] : int.MinValue;
                
                Pair pair = new Pair();
                
                // Сравниваем элементы и определяем больший/меньший
                if (b == int.MinValue)
                {
                    pair.Greater = a;
                    pair.Lesser = int.MinValue;
                }
                else if (a >= b)
                {
                    pair.Greater = a;
                    pair.Lesser = b;
                }
                else
                {
                    pair.Greater = b;
                    pair.Lesser = a;
                }
                
                pairs.Add(pair);
                
                if (verbose)
                {
                    Console.WriteLine($"Пара {i+1}: ({a}, {b}) -> Больший: {pair.Greater}, Меньший: {pair.Lesser}");
                }
            }
            
            // Шаг ii: Рассортировать большие элементы пар посредством вставок и слияния
            if (verbose)
            {
                Console.WriteLine("\nШаг ii: Сортировка больших элементов пар");
            }
            
            List<int> mainChain = new List<int>(); // Главная цепочка (a1, a2, ...)
            List<int> pendingElements = new List<int>(); // Элементы ожидающие вставки (b1, b2, ...)
            
            // Добавляем первый больший элемент в главную цепочку
            mainChain.Add(pairs[0].Greater);
            
            // Добавляем меньшие элементы в соответствующие списки
            for (int i = 0; i < pairs.Count; i++)
            {
                if (i > 0) // Пропускаем первую пару, т.к. уже добавили greater в mainChain
                {
                    if (pairs[i].Greater != int.MinValue)
                    {
                        // Вставка большего элемента в основную цепочку с помощью бинарного поиска
                        InsertIntoSortedList(mainChain, pairs[i].Greater);
                    }
                }
                
                if (pairs[i].Lesser != int.MinValue)
                {
                    pendingElements.Add(pairs[i].Lesser);
                }
            }
            
            if (verbose)
            {
                Console.WriteLine("\nПосле сортировки больших элементов:");
                Console.Write("Главная цепочка: ");
                foreach (var e in mainChain)
                {
                    Console.Write($"{e} ");
                }
                Console.WriteLine();
                
                Console.Write("Ожидающие вставки: ");
                foreach (var e in pendingElements)
                {
                    Console.Write($"{e} ");
                }
                Console.WriteLine("\n");
            }
            
            // Шаг iii: Вставка оставшихся элементов в главную цепочку
            if (verbose)
            {
                Console.WriteLine("Шаг iii: Вставка оставшихся элементов в главную цепочку");
            }
            
            // Строим последовательность Якобсталя (0, 1, 3, 5, 11, 21, 43, ...)
            List<int> jacobsthalSequence = GenerateJacobsthalSequence(pendingElements.Count);
            
            if (verbose)
            {
                Console.Write("Последовательность Якобсталя: ");
                foreach (var j in jacobsthalSequence)
                {
                    Console.Write($"{j} ");
                }
                Console.WriteLine("\n");
            }
            
            // Создаем порядок вставки согласно последовательности Якобсталя
            List<int> insertionOrder = new List<int>();
            int lastIndex = 0;
            
            // Вставляем элементы согласно индексам последовательности Якобсталя
            foreach (int jacobi in jacobsthalSequence)
            {
                if (jacobi >= pendingElements.Count)
                    break;
                    
                // Добавляем элемент по индексу Якоби
                insertionOrder.Add(jacobi);
                
                // Добавляем все пропущенные элементы в обратном порядке
                for (int i = jacobi - 1; i > lastIndex; i--)
                {
                    insertionOrder.Add(i);
                }
                
                lastIndex = jacobi;
            }
            
            // Добавляем оставшиеся элементы
            for (int i = lastIndex + 1; i < pendingElements.Count; i++)
            {
                insertionOrder.Add(i);
            }
            
            if (verbose)
            {
                Console.Write("Порядок вставки элементов: ");
                foreach (var idx in insertionOrder)
                {
                    Console.Write($"{idx} ");
                }
                Console.WriteLine("\n");
            }
            
            // Вставляем элементы в главную цепочку согласно вычисленному порядку
            foreach (int index in insertionOrder)
            {
                if (index < pendingElements.Count)
                {
                    int element = pendingElements[index];
                    InsertIntoSortedList(mainChain, element);
                    
                    if (verbose)
                    {
                        Console.WriteLine($"Вставка элемента {element} в главную цепочку:");
                        Console.Write("Главная цепочка: ");
                        foreach (var e in mainChain)
                        {
                            Console.Write($"{e} ");
                        }
                        Console.WriteLine("\n");
                    }
                }
            }
            
            return mainChain.ToArray();
        }
        
        /// <summary>
        /// Вставка элемента в отсортированный список с использованием бинарного поиска
        /// </summary>
        private static void InsertIntoSortedList(List<int> sortedList, int element)
        {
            int index = BinarySearch(sortedList, element);
            sortedList.Insert(index, element);
        }
        
        /// <summary>
        /// Бинарный поиск для определения позиции вставки (по убыванию)
        /// </summary>
        private static int BinarySearch(List<int> list, int element)
        {
            int left = 0;
            int right = list.Count - 1;
            
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                
                // Сортировка по убыванию (как в примере)
                if (list[mid] > element)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }
            
            return left;
        }
        
        /// <summary>
        /// Генерирует последовательность Якобсталя: j(0)=0, j(1)=1, j(n)=j(n-1)+2*j(n-2) при n≥2
        /// </summary>
        private static List<int> GenerateJacobsthalSequence(int maxSize)
        {
            List<int> sequence = new List<int> { 0, 1 };
            
            int i = 2;
            while (true)
            {
                // Формула: J(n) = J(n-1) + 2*J(n-2) при n≥2
                int next = sequence[i - 1] + 2 * sequence[i - 2];
                if (next >= maxSize)
                    break;
                    
                sequence.Add(next);
                i++;
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Структура для хранения пары элементов (больший и меньший)
        /// </summary>
        private class Pair
        {
            public int Greater { get; set; }
            public int Lesser { get; set; }
        }
        
        /// <summary>
        /// Вспомогательный метод для вывода массива
        /// </summary>
        private static void PrintArray(int[] array)
        {
            foreach (int num in array)
            {
                Console.Write($"{num} ");
            }
            Console.WriteLine();
        }
        
        /// <summary>
        /// Демонстрация работы алгоритма сортировки посредством вставок и слияния
        /// </summary>
        public static void Demo()
        {
            // Пример массива для демонстрации (21 элемент, как в примере из описания)
            int[] array = { 503, 87, 512, 61, 8, 170, 897, 275, 653, 426, 154, 509, 612, 677, 765, 703, 321, 444, 123, 999, 111 };
            
            Console.WriteLine("Исходный массив:");
            PrintArray(array);
            Console.WriteLine();
            
            Sort(array, true); // Включаем подробный вывод
            
            Console.WriteLine("\nФинальный отсортированный массив (по убыванию):");
            PrintArray(array);
            
            // Подсчет количества сравнений для массива размера n
            CalculateComparisons(array.Length);
        }
        
        /// <summary>
        /// Вычисление количества сравнений для сортировки массива определенного размера
        /// по формуле из материалов S(n) = [lg n!]
        /// </summary>
        private static void CalculateComparisons(int n)
        {
            Console.WriteLine("\nТеоретическая оценка количества сравнений:");
            
            // Оптимальное количество сравнений для n элементов равно [lg n!]
            double factorial = 0;
            for (int i = 1; i <= n; i++)
            {
                factorial += Math.Log2(i);
            }
            
            int optimalComparisons = (int)Math.Floor(factorial);
            
            Console.WriteLine($"Для сортировки {n} элементов оптимально требуется {optimalComparisons} сравнений");
            Console.WriteLine($"По формуле S(n) = [lg n!] = [{factorial:F2}] = {optimalComparisons}");
        }
        
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        public static void Main()
        {
            Demo();
            
            // Ожидание нажатия клавиши перед завершением программы
            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }
    }
} 