using System;

class Program
{
    static void Main(string[] args)
    {
        // Создаем упорядоченную таблицу записей
        var records = new Record[]
        {
            new Record { Key = 10, Value = "Запись 1" },
            new Record { Key = 20, Value = "Запись 2" },
            new Record { Key = 30, Value = "Запись 3" },
            new Record { Key = 40, Value = "Запись 4" },
            new Record { Key = 50, Value = "Запись 5" },
            new Record { Key = 60, Value = "Запись 6" },
            new Record { Key = 70, Value = "Запись 7" },
            // Добавляем фиктивную запись с ключом +∞
            new Record { Key = int.MaxValue, Value = "Фиктивная запись" }
        };

        Console.WriteLine("Введите ключ для поиска:");
        if (!int.TryParse(Console.ReadLine(), out int searchKey))
        {
            Console.WriteLine("Некорректный ввод. Ожидается целое число.");
            return;
        }

        var result = SequentialSearch(records, searchKey);
        
        if (result != null)
        {
            Console.WriteLine($"Найдена запись: {result.Value} с ключом {result.Key}");
        }
        else
        {
            Console.WriteLine("Запись с указанным ключом не найдена.");
        }
    }

    // Алгоритм T: Последовательный поиск в упорядоченной таблице
    static Record? SequentialSearch(Record[] records, int searchKey)
    {
        // T1. [Инициализация.] Установить i ← 1.
        int i = 0; // Индексация в C# начинается с 0

        // T2. [Сравнение.] Если K ≤ Ki, перейти к шагу T4.
        // T3. [Продолжение.] Увеличить i на 1 и перейти к шагу T2.
        while (searchKey > records[i].Key)
        {
            i++;
        }

        // T4. [Равенство?] Если K = Ki, алгоритм заканчивается успешно.
        // В противном случае — неудачное завершение алгоритма.
        if (searchKey == records[i].Key)
        {
            return records[i];
        }
        else
        {
            return null;
        }
    }
}

// Класс, представляющий запись в таблице
class Record
{
    public int Key { get; set; }
    public string Value { get; set; }
}
