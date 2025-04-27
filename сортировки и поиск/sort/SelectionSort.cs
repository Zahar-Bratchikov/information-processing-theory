using System;

namespace SortingAlgorithms
{
    public class SelectionSort
    {
        /// <summary>
        /// Метод сортировки массива целых чисел посредством простого выбора
        /// с выводом промежуточных шагов
        /// </summary>
        /// <param name="array">Массив для сортировки</param>
        /// <param name="verbose">Флаг для вывода подробной информации во время сортировки</param>
        public static void Sort(int[] array, bool verbose = false)
        {
            if (array == null || array.Length <= 1)
                return;

            int n = array.Length;
            int[] workingArray = (int[])array.Clone();
            
            if (verbose)
            {
                Console.WriteLine("Таблица 1");
                Console.WriteLine("СОРТИРОВКА ПОСРЕДСТВОМ ПРОСТОГО ВЫБОРА");
                Console.WriteLine();
                
                // Вывод исходного массива
                PrintArrayFormatted(workingArray, -1, -1);
            }

            // Цикл по j
            for (int j = 0; j < n - 1; j++)
            {
                // Шаг S1: Выполнить шаги S2 и S3 при j = 0, N-1, ..., 2
                
                // Шаг S2: Найти max(K1, ..., Kj)
                // Найти наибольший из ключей Kj, Kj+1, ..., Kn
                int maxIndex = j;
                for (int i = j + 1; i < n; i++)
                {
                    if (workingArray[i] > workingArray[maxIndex])
                    {
                        maxIndex = i;
                    }
                }

                // Шаг S3: Поменять местами c Kj
                // Взаимно переставить записи Ri ↔ Rj
                if (maxIndex != j)
                {
                    int temp = workingArray[j];
                    workingArray[j] = workingArray[maxIndex];
                    workingArray[maxIndex] = temp;
                }
                
                // Выводим текущее состояние массива после перестановки
                if (verbose)
                {
                    PrintArrayFormatted(workingArray, j, maxIndex);
                }
            }
            
            // Копируем результат обратно в исходный массив
            Array.Copy(workingArray, array, n);
        }
        
        /// <summary>
        /// Метод для форматированного вывода массива с выделением элементов
        /// </summary>
        /// <param name="array">Массив для вывода</param>
        /// <param name="positionJ">Позиция j (будет выделена)</param>
        /// <param name="maxIndex">Позиция максимального элемента (будет выделена)</param>
        private static void PrintArrayFormatted(int[] array, int positionJ, int maxIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                // Форматируем числа по 3 знака с ведущими нулями
                string numStr = array[i].ToString("D3");
                
                // Если элемент на позиции j или maxIndex, выделяем его
                if (i == positionJ || i == maxIndex)
                {
                    // ANSI-код для яркого полужирного шрифта и цветного выделения
                    Console.Write($"\u001b[1;30;47m{numStr}\u001b[0m ");
                }
                else
                {
                    Console.Write($"{numStr} ");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Демонстрация работы алгоритма сортировки выбором
        /// </summary>
        public static void Demo()
        {
            // Пример массива из таблицы в задании
            int[] array = { 503, 087, 512, 061, 008, 170, 897, 275, 653, 426, 154, 509, 612, 677, 765, 703 };
            
            Sort(array, true); // Включаем подробный вывод
            
            Console.WriteLine("\nФинальный отсортированный массив (по убыванию):");
            PrintArray(array);
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