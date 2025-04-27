using System;
using System.Diagnostics;

namespace SortingAlgorithms
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Сравнение алгоритмов сортировки");
            Console.WriteLine("==============================");
            Console.WriteLine();
            
            while (true)
            {
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1. Запустить сортировку выбором");
                Console.WriteLine("2. Запустить сортировку вставками и слиянием (Форд-Джонсон)");
                Console.WriteLine("3. Сравнить алгоритмы на случайных массивах разной длины");
                Console.WriteLine("4. Выход");
                Console.WriteLine();
                
                Console.Write("Ваш выбор: ");
                string choice = Console.ReadLine();
                Console.WriteLine();
                
                switch (choice)
                {
                    case "1":
                        RunSelectionSort();
                        break;
                    case "2":
                        RunMergeInsertSort();
                        break;
                    case "3":
                        CompareAlgorithms();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
                
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        
        private static void RunSelectionSort()
        {
            Console.WriteLine("Запуск демонстрации сортировки выбором");
            Console.WriteLine("====================================\n");
            
            SelectionSort.Demo();
        }
        
        private static void RunMergeInsertSort()
        {
            Console.WriteLine("Запуск демонстрации сортировки вставками и слиянием (Форд-Джонсон)");
            Console.WriteLine("===========================================================\n");
            
            MergeSortInsert.Demo();
        }
        
        private static void CompareAlgorithms()
        {
            Console.WriteLine("Сравнение алгоритмов сортировки на случайных массивах");
            Console.WriteLine("=================================================\n");
            
            int[] sizes = { 10, 50, 100, 500, 1000, 5000 };
            
            Console.WriteLine("|  Размер массива  |  Время сортировки выбором (мс)  |  Время сортировки Форда-Джонсона (мс)  |");
            Console.WriteLine("|------------------|----------------------------------|----------------------------------------|");
            
            foreach (int size in sizes)
            {
                // Генерируем случайный массив
                int[] array = GenerateRandomArray(size);
                int[] arrayCopy1 = (int[])array.Clone();
                int[] arrayCopy2 = (int[])array.Clone();
                
                // Измеряем время для сортировки выбором (без вывода информации)
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                SelectionSort.Sort(arrayCopy1, false);
                sw1.Stop();
                long selectionTime = sw1.ElapsedMilliseconds;
                
                // Измеряем время для сортировки Форда-Джонсона (без вывода информации)
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                MergeSortInsert.Sort(arrayCopy2, false);
                sw2.Stop();
                long fordJohnsonTime = sw2.ElapsedMilliseconds;
                
                // Проверка корректности сортировки
                bool selectionSorted = IsSorted(arrayCopy1, true); // По убыванию как в SelectionSort
                bool fordJohnsonSorted = IsSorted(arrayCopy2, true); // По убыванию как в MergeSortInsert
                
                // Выводим результаты
                Console.WriteLine($"|  {size,-16}  |  {selectionTime,-32}  |  {fordJohnsonTime,-38}  |");
                
                if (!selectionSorted || !fordJohnsonSorted)
                {
                    Console.WriteLine("ВНИМАНИЕ: Обнаружена ошибка сортировки!");
                    if (!selectionSorted) Console.WriteLine("- Сортировка выбором дала неверный результат");
                    if (!fordJohnsonSorted) Console.WriteLine("- Сортировка Форда-Джонсона дала неверный результат");
                }
            }
            
            Console.WriteLine("\nПримечание: Сортировка Форда-Джонсона теоретически более эффективна с точки зрения");
            Console.WriteLine("количества сравнений, но может быть медленнее из-за дополнительных операций и сложности.");
            Console.WriteLine("Сортировка выбором имеет сложность O(n²), в то время как сортировка Форда-Джонсона ближе к O(n log n).");
        }
        
        private static int[] GenerateRandomArray(int size)
        {
            int[] array = new int[size];
            Random rand = new Random();
            
            for (int i = 0; i < size; i++)
            {
                array[i] = rand.Next(1, 1000);
            }
            
            return array;
        }
        
        private static bool IsSorted(int[] array, bool descending = false)
        {
            if (array.Length <= 1)
                return true;
                
            for (int i = 1; i < array.Length; i++)
            {
                if (descending)
                {
                    if (array[i] > array[i - 1])
                        return false;
                }
                else
                {
                    if (array[i] < array[i - 1])
                        return false;
                }
            }
            
            return true;
        }
    }
} 