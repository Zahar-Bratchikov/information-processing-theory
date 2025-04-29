using System;

namespace MergeInsertionSort
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демонстрация алгоритма сортировки посредством вставок и слияния");
            Console.WriteLine("==========================================================================");
            Console.WriteLine();

            // Демонстрация работы алгоритма на небольшом массиве
            DemonstrateSort();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void DemonstrateSort()
        {
            Console.WriteLine("Сортировка массива:");
            int[] array = { 7, 3, 9, 1, 5, 8, 2, 6, 4, 0 };

            Console.WriteLine("Исходный массив:");
            PrintArray(array);

            int comparisons = MergeInsertionSort.SortAndCount(array);

            Console.WriteLine("Отсортированный массив:");
            PrintArray(array);
            Console.WriteLine($"Количество сравнений: {comparisons}");
            Console.WriteLine();

            // Демонстрация на массиве другого размера
            int[] array2 = { 11, 3, 5, 7, 9, 2, 4, 6, 8, 10, 1 };
            Console.WriteLine("Сортировка массива из 11 элементов:");
            Console.WriteLine("Исходный массив:");
            PrintArray(array2);

            int comparisons2 = MergeInsertionSort.SortAndCount(array2);

            Console.WriteLine("Отсортированный массив:");
            PrintArray(array2);
            Console.WriteLine($"Количество сравнений: {comparisons2}");
            Console.WriteLine();
        }

        // Вывод массива на экран
        static void PrintArray(int[] array)
        {
            Console.Write("[");
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i]);
                if (i < array.Length - 1)
                    Console.Write(", ");
            }
            Console.WriteLine("]");
        }
    }
}
