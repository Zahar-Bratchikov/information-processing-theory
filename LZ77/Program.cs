using System;
using System.Collections.Generic;
using System.Text;

class LZ77
{
    private const int WindowSize = 20; // Размер окна поиска (размер истории, где ищем совпадения)
    private const int LookaheadBufferSize = 10; // Размер буфера предпросмотра (максимальная длина совпадения)

    // Метод сжатия строки с использованием алгоритма LZ77
    public static List<(int, int, char)> Compress(string input)
    {
        List<(int, int, char)> compressed = new List<(int, int, char)>(); // Список для хранения сжатых данных
        int index = 0; // Индекс текущего символа во входной строке

        while (index < input.Length)
        {
            int bestMatchDistance = 0, bestMatchLength = 0; // Переменные для хранения лучшего совпадения (смещение и длина)

            // Определяем буфер поиска (часть строки перед текущим индексом)
            string searchBuffer = input.Substring(Math.Max(0, index - WindowSize), Math.Min(index, WindowSize));

            // Поиск наилучшего совпадения в буфере поиска
            for (int matchLength = 1; matchLength <= LookaheadBufferSize && index + matchLength <= input.Length; matchLength++)
            {
                string match = input.Substring(index, matchLength); // Текущая подстрока для поиска совпадений
                int foundIndex = searchBuffer.LastIndexOf(match); // Поиск подстроки в буфере поиска
                if (foundIndex >= 0)
                {
                    bestMatchDistance = searchBuffer.Length - foundIndex; // Смещение относительно конца буфера
                    bestMatchLength = matchLength; // Длина совпадения
                }
                else
                {
                    break; // Если нет совпадения, выходим из цикла
                }
            }

            // Следующий символ после совпадения (если он есть)
            char nextChar = index + bestMatchLength < input.Length ? input[index + bestMatchLength] : '\0';

            // Добавляем к сжатому результату (смещение, длина совпадения, следующий символ)
            compressed.Add((bestMatchDistance, bestMatchLength, nextChar));

            // Перемещаем индекс на длину совпадения + 1 (чтобы обработать следующий символ)
            index += bestMatchLength + 1;
        }

        return compressed; // Возвращаем список сжатых данных
    }

    // Метод декомпрессии строки с использованием алгоритма LZ77
    public static string Decompress(List<(int, int, char)> compressed)
    {
        StringBuilder decompressed = new StringBuilder(); // Буфер для восстановленной строки

        foreach (var (distance, length, nextChar) in compressed)
        {
            int start = decompressed.Length - distance; // Начальная позиция совпадающей строки в буфере

            // Восстанавливаем повторяющуюся последовательность из уже разжатой части
            for (int i = 0; i < length; i++)
            {
                decompressed.Append(decompressed[start + i]);
            }

            // Добавляем следующий символ, если он не является нулевым
            if (nextChar != '\0')
                decompressed.Append(nextChar);
        }

        return decompressed.ToString(); // Возвращаем восстановленную строку
    }

    // Метод для вывода результата в формате таблицы
    public static void PrintCompressedData(List<(int, int, char)> compressed, string input)
    {
        Console.WriteLine("Шаг | Флаг | Последовательность | d | l | Кодовая последовательность");
        Console.WriteLine("-----------------------------------------------------------");
        int index = 0;
        for (int i = 0; i < compressed.Count; i++)
        {
            var (d, l, nextChar) = compressed[i];
            string sequence = input.Substring(index, l) + (nextChar != '\0' ? nextChar.ToString() : "");
            string flag = l > 0 ? "1" : "0";
            string codeSeq = l > 0 ? $"{d}({index}) {l} {Convert.ToString(nextChar, 2)}" : $"0 bin({nextChar})";
            Console.WriteLine($"{i,2} | {flag,3} | {sequence,-20} | {d,2} | {l,2} | {codeSeq}");
            index += l + 1;
        }
    }

    // Основной метод программы (тестирование алгоритма)
    static void Main()
    {
        string input = "early_to_bed_and_early_to_rise_makes_a_man_wise"; // Исходная строка

        // Выполняем сжатие
        var compressedData = Compress(input);

        // Выводим результат в виде таблицы
        PrintCompressedData(compressedData, input);

        // Выполняем разархивирование
        string decompressed = Decompress(compressedData);
        Console.WriteLine("Decompressed String: " + decompressed);
    }
}