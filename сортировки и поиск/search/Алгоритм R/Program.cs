using System;

namespace HashTableAlgorithms
{
    // Класс для реализации алгоритма R (Удаление при линейном исследовании)
    class Program
    {
        // Константа, определяющая размер хеш-таблицы
        private const int M = 10;
        
        // Класс для элементов таблицы
        class TableItem
        {
            public int Key { get; set; }
            public bool IsEmpty { get; set; } = true;
            
            public TableItem() { }
            
            public TableItem(int key)
            {
                Key = key;
                IsEmpty = false;
            }
        }
        
        // Хеш-функция
        static int HashFunction(int key, int tableSize)
        {
            return key % tableSize;
        }
        
        // Основной метод демонстрации алгоритма R
        static void Main(string[] args)
        {
            // Создаем хеш-таблицу с записями
            var table = new Record[]
            {
                new Record { Key = 10, Value = "Запись 1", IsEmpty = false },
                new Record { Key = 20, Value = "Запись 2", IsEmpty = false },
                new Record { Key = 30, Value = "Запись 3", IsEmpty = false },
                new Record { Key = 40, Value = "Запись 4", IsEmpty = false },
                new Record { Key = 50, Value = "Запись 5", IsEmpty = false },
                new Record { Key = 60, Value = "Запись 6", IsEmpty = false },
                new Record { Key = 70, Value = "Запись 7", IsEmpty = false },
                new Record { Key = 0, Value = "", IsEmpty = true },
                new Record { Key = 0, Value = "", IsEmpty = true },
                new Record { Key = 0, Value = "", IsEmpty = true }
            };

            // Выводим исходное состояние таблицы
            Console.WriteLine("Исходная таблица:");
            PrintTable(table);

            // Запрашиваем у пользователя выбор способа удаления
            Console.WriteLine("\nВыберите способ удаления записи:");
            Console.WriteLine("1 - Удалить по позиции в таблице");
            Console.WriteLine("2 - Удалить по ключу записи");
            
            if (!int.TryParse(Console.ReadLine(), out int choice) || (choice != 1 && choice != 2))
            {
                Console.WriteLine("Некорректный выбор. Завершение программы.");
                return;
            }

            int positionToDelete = -1;

            if (choice == 1)
            {
                // Удаление по позиции
                Console.WriteLine("\nВведите позицию для удаления (0-9):");
                
                if (!int.TryParse(Console.ReadLine(), out positionToDelete) || 
                    positionToDelete < 0 || positionToDelete >= table.Length)
                {
                    Console.WriteLine("Некорректная позиция. Завершение программы.");
                    return;
                }

                if (table[positionToDelete].IsEmpty)
                {
                    Console.WriteLine($"Позиция {positionToDelete} пуста. Нечего удалять.");
                    return;
                }
            }
            else
            {
                // Удаление по ключу
                Console.WriteLine("\nВведите ключ записи для удаления:");
                
                if (!int.TryParse(Console.ReadLine(), out int keyToDelete))
                {
                    Console.WriteLine("Некорректный ключ. Завершение программы.");
                    return;
                }

                // Ищем позицию по ключу
                positionToDelete = -1;
                for (int i = 0; i < table.Length; i++)
                {
                    if (!table[i].IsEmpty && table[i].Key == keyToDelete)
                    {
                        positionToDelete = i;
                        break;
                    }
                }

                if (positionToDelete == -1)
                {
                    Console.WriteLine($"Запись с ключом {keyToDelete} не найдена.");
                    return;
                }
            }

            Console.WriteLine($"\nУдаление записи из позиции {positionToDelete} (ключ: {table[positionToDelete].Key})");

            // Вызываем алгоритм R
            AlgorithmR(table, positionToDelete);

            // Выводим таблицу после удаления
            Console.WriteLine("\nТаблица после удаления:");
            PrintTable(table);
        }
        
        // Реализация алгоритма R (Удаление при линейном исследовании)
        static void AlgorithmR(Record[] table, int position)
        {
            int M = table.Length; // Размер таблицы

            // R1. [Освобождение ячейки.] Пометить TABLE[i] как пустую и установить j ← i.
            int i = position;
            table[i].IsEmpty = true;
            table[i].Key = 0;
            table[i].Value = "";
            int j = i;
            
            while (true)
            {
                // R2. [Уменьшение j.] Установить j ← j − 1; если j < 0, установить j ← j + M.
                j = (j - 1 + M) % M;
                
                // R3. [Проверить TABLE[j].] Если TABLE[j] пуст, алгоритм завершается.
                if (table[j].IsEmpty)
                {
                    Console.WriteLine($"Позиция {j} пуста, алгоритм завершается");
                    break;
                }
                
                // В противном случае установить r ← h(KEY[j]) — первоначальный хеш-адрес ключа, который
                // хранится в позиции j.
                int r = HashFunction(table[j].Key, M);
                Console.WriteLine($"Проверка ячейки {j}: ключ = {table[j].Key}, хеш = {r}");
                
                // Если i ≤ r < j или если r < j < i или j < i ≤ r
                // (другими словами, если r лежит циклически между i и j), вернуться к шагу R1.
                bool shouldMove = false;
                if (i <= r && r < j) shouldMove = true;
                else if (r < j && j < i) shouldMove = true;
                else if (j < i && i <= r) shouldMove = true;
                
                if (!shouldMove)
                {
                    Console.WriteLine($"Хеш {r} не находится циклически между {i} и {j}, продолжаем поиск");
                    continue;
                }
                
                // R4. [Переместить запись.] Установить TABLE[i] ← TABLE[j] и вернуться к шагу R1.
                Console.WriteLine($"Перемещение записи с ключом {table[j].Key} из позиции {j} в позицию {i}");
                table[i].Key = table[j].Key;
                table[i].Value = table[j].Value;
                table[i].IsEmpty = false;
                
                table[j].Key = 0;
                table[j].Value = "";
                table[j].IsEmpty = true;
                
                i = j;
            }
        }
        
        // Метод для вывода содержимого таблицы
        static void PrintTable(Record[] table)
        {
            Console.WriteLine("Индекс\tКлюч\tЗначение\tСтатус");
            for (int i = 0; i < table.Length; i++)
            {
                string status = table[i].IsEmpty ? "Пусто" : "Занято";
                Console.WriteLine($"{i}\t{table[i].Key}\t{table[i].Value}\t{status}");
            }
        }
    }

    // Класс, представляющий запись в таблице
    class Record
    {
        public int Key { get; set; }
        public string Value { get; set; } = "";
        public bool IsEmpty { get; set; } = true;
    }
} 