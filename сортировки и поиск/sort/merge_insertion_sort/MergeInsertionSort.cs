using System;
using System.Collections.Generic;

namespace MergeInsertionSort
{
    public class MergeInsertionSort
    {
        // Основной метод сортировки посредством вставок и слияния (Ford-Johnson)
        public static void Sort(int[] array)
        {
            if (array == null || array.Length <= 1)
                return;
            
            // Создаем копию массива
            int[] result = new int[array.Length];
            Array.Copy(array, result, array.Length);
            
            // Запускаем сортировку
            FordJohnsonSort(result);
            
            // Копируем результат обратно
            Array.Copy(result, array, array.Length);
        }
        
        // Метод для подсчета числа сравнений при сортировке
        public static int SortAndCount(int[] array)
        {
            if (array == null || array.Length <= 1)
                return 0;
            
            // Создаем копию массива
            int[] result = new int[array.Length];
            Array.Copy(array, result, array.Length);
            
            // Счетчик сравнений
            int comparisons = 0;
            
            // Запускаем сортировку с подсчетом сравнений
            FordJohnsonSortWithCount(result, ref comparisons);
            
            // Копируем результат обратно
            Array.Copy(result, array, array.Length);
            
            return comparisons;
        }
        
        // Основной алгоритм сортировки
        private static void FordJohnsonSort(int[] array)
        {
            int n = array.Length;
            
            // Базовый случай
            if (n <= 1) return;
            
            // i) Сравнить [n/2] непересекающихся пар элементов
            List<Pair> pairs = new List<Pair>();
            for (int i = 0; i < n/2; i++)
            {
                pairs.Add(new Pair(array[2*i], array[2*i+1]));
            }
            
            // Если n нечетно, последний элемент не участвует в сравнениях
            int unpaired = 0;
            bool hasUnpaired = false;
            if (n % 2 == 1)
            {
                unpaired = array[n-1];
                hasUnpaired = true;
            }
            
            // ii) Рассортировать [n/2] больших элементов пар
            int[] largerElements = new int[pairs.Count];
            for (int i = 0; i < pairs.Count; i++)
            {
                largerElements[i] = pairs[i].Larger;
            }
            
            // Рекурсивно сортируем большие элементы
            if (largerElements.Length > 1)
            {
                FordJohnsonSort(largerElements);
            }
            
            // Создаем словарь для быстрого сопоставления больших элементов с их парами
            Dictionary<int, int> smallerByLarger = new Dictionary<int, int>();
            for (int i = 0; i < pairs.Count; i++)
            {
                smallerByLarger[pairs[i].Larger] = pairs[i].Smaller;
            }
            
            // iii) Вставить остальные элементы b в главную цепочку
            // Сначала добавляем все большие элементы в результат
            List<int> mainChain = new List<int>(largerElements);
            
            // Теперь начинаем вставлять меньшие элементы в оптимальном порядке
            List<int> insertionOrder = GenerateInsertionOrder(pairs.Count);
            
            // Создаем список для результата
            List<int> result = new List<int>(mainChain);
            
            // Вставляем меньшие элементы в порядке, определенном в книге
            foreach (int idx in insertionOrder)
            {
                if (idx < 0 || idx >= mainChain.Count)
                    continue;
                
                int larger = mainChain[idx];
                if (!smallerByLarger.ContainsKey(larger))
                    continue;
                
                int smaller = smallerByLarger[larger];
                int insertPos = BinarySearch(result, smaller);
                result.Insert(insertPos, smaller);
            }
            
            // Вставляем непарный элемент, если он есть
            if (hasUnpaired)
            {
                int insertPos = BinarySearch(result, unpaired);
                result.Insert(insertPos, unpaired);
            }
            
            // Копируем результат обратно в исходный массив
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = result[i];
            }
        }
        
        // Алгоритм сортировки с подсчетом сравнений
        private static void FordJohnsonSortWithCount(int[] array, ref int comparisons)
        {
            int n = array.Length;
            
            // Базовый случай
            if (n <= 1) return;
            
            // i) Сравнить [n/2] непересекающихся пар элементов
            List<Pair> pairs = new List<Pair>();
            for (int i = 0; i < n/2; i++)
            {
                comparisons++; // Сравнение элементов в паре
                pairs.Add(new Pair(array[2*i], array[2*i+1]));
            }
            
            // Если n нечетно, последний элемент не участвует в сравнениях
            int unpaired = 0;
            bool hasUnpaired = false;
            if (n % 2 == 1)
            {
                unpaired = array[n-1];
                hasUnpaired = true;
            }
            
            // ii) Рассортировать [n/2] больших элементов пар
            int[] largerElements = new int[pairs.Count];
            for (int i = 0; i < pairs.Count; i++)
            {
                largerElements[i] = pairs[i].Larger;
            }
            
            // Рекурсивно сортируем большие элементы
            if (largerElements.Length > 1)
            {
                FordJohnsonSortWithCount(largerElements, ref comparisons);
            }
            
            // Создаем словарь для быстрого сопоставления больших элементов с их парами
            Dictionary<int, int> smallerByLarger = new Dictionary<int, int>();
            for (int i = 0; i < pairs.Count; i++)
            {
                smallerByLarger[pairs[i].Larger] = pairs[i].Smaller;
            }
            
            // iii) Вставить остальные элементы b в главную цепочку
            // Сначала добавляем все большие элементы в результат
            List<int> mainChain = new List<int>(largerElements);
            
            // Теперь начинаем вставлять меньшие элементы в оптимальном порядке
            List<int> insertionOrder = GenerateInsertionOrder(pairs.Count);
            
            // Создаем список для результата
            List<int> result = new List<int>(mainChain);
            
            // Вставляем меньшие элементы в порядке, определенном в книге
            foreach (int idx in insertionOrder)
            {
                if (idx < 0 || idx >= mainChain.Count)
                    continue;
                
                int larger = mainChain[idx];
                if (!smallerByLarger.ContainsKey(larger))
                    continue;
                
                int smaller = smallerByLarger[larger];
                int insertPos = BinarySearchWithCount(result, smaller, ref comparisons);
                result.Insert(insertPos, smaller);
            }
            
            // Вставляем непарный элемент, если он есть
            if (hasUnpaired)
            {
                int insertPos = BinarySearchWithCount(result, unpaired, ref comparisons);
                result.Insert(insertPos, unpaired);
            }
            
            // Копируем результат обратно в исходный массив
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = result[i];
            }
        }
        
        // Бинарный поиск позиции для вставки
        private static int BinarySearch(List<int> list, int value)
        {
            int low = 0;
            int high = list.Count - 1;
            
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (list[mid] < value)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            
            return low;
        }
        
        // Бинарный поиск с подсчетом сравнений
        private static int BinarySearchWithCount(List<int> list, int value, ref int comparisons)
        {
            int low = 0;
            int high = list.Count - 1;
            
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                comparisons++; // Увеличиваем счетчик сравнений
                if (list[mid] < value)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            
            return low;
        }
        
        // Генерация оптимального порядка вставки по формуле (13) из текста
        private static List<int> GenerateInsertionOrder(int n)
        {
            List<int> result = new List<int>();
            
            // Первые индексы в последовательности
            if (n > 0) result.Add(0);  // Первый индекс
            if (n > 1) result.Add(1);  // Второй индекс
            
            // Вычисляем t_k по формуле (13): t_k = (2^(k+1) + (-1)^k)/3
            List<int> tValues = new List<int> { 1 };  // t_1 = 1, t_0 считаем равным 0
            int k = 2;
            
            while (true)
            {
                int t_k = (int)((Math.Pow(2, k + 1) + Math.Pow(-1, k)) / 3);
                if (t_k >= n) break;
                
                tValues.Add(t_k);
                
                // Добавляем промежуточные индексы между t_(k-1) и t_k в правильном порядке
                for (int i = tValues[tValues.Count - 2] + 1; i <= t_k; i++)
                {
                    if (i < n) result.Add(i);
                }
                
                k++;
            }
            
            // Добавляем оставшиеся индексы
            for (int i = tValues[tValues.Count - 1] + 1; i < n; i++)
            {
                result.Add(i);
            }
            
            return result;
        }
        
        // Вспомогательный класс для хранения пар элементов
        private class Pair
        {
            public int Smaller { get; private set; }
            public int Larger { get; private set; }
            
            public Pair(int a, int b)
            {
                if (a <= b)
                {
                    Smaller = a;
                    Larger = b;
                }
                else
                {
                    Smaller = b;
                    Larger = a;
                }
            }
        }
    }
} 