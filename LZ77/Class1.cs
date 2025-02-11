using System;
using System.Text;

class Program
{
    // Функция поиска наибольшей подпоследовательности в буфере
    static (string, int) FindMaxSubstr(int charNumber, string buffer, string sourceSequence)
    {
        string resultSequence = sourceSequence[charNumber].ToString();

        if (buffer.Contains(resultSequence))
        {
            while (charNumber + 1 < sourceSequence.Length && buffer.Contains(resultSequence + sourceSequence[charNumber + 1]))
            {
                charNumber++;
                resultSequence += sourceSequence[charNumber];
            }
            return (resultSequence, charNumber);
        }
        return ("", charNumber);
    }

    // Генерация унарного кода длины последовательности
    static string GetMon(int matchLen)
    {
        if (matchLen == 0) return "0"; // Для длины 0 унарный код "0"
        string matchLenBin = Convert.ToString(matchLen, 2);
        return new string('1', matchLenBin.Length - 1) + "0" + matchLenBin.Substring(1);
    }

    // Проверка, является ли число степенью двойки
    static bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }

    // Генерация бинарного представления смещения
    static string GetBinSeq(int matchLenSymbolsInBuffer, int offset)
    {
        int seqLen = IsPowerOfTwo(matchLenSymbolsInBuffer) ? Convert.ToString(matchLenSymbolsInBuffer, 2).Length - 1 : Convert.ToString(matchLenSymbolsInBuffer, 2).Length;
        string offsetBin = Convert.ToString(offset, 2).PadLeft(seqLen, '0');
        return offsetBin;
    }

    // Основной алгоритм
    static void Algorithm(string sourceSequence, int winSize)
    {
        int charNumber = 0; // Текущий символ
        int step = 0;       // Шаги алгоритма
        string buffer = ""; // Буфер
        int allBitsCount = 0; // Счётчик всех битов

        Console.WriteLine("{0,-5} | {1,-4} | {2,-20} | {3,-10} | {4,-5} | {5,-25} | {6,-5}",
                          "Шаг", "Флаг", "Последовательность букв", "d", "l", "Кодовая последовательность", "Биты");
        Console.WriteLine(new string('-', 90));

        while (charNumber < sourceSequence.Length)
        {
            var (match, newCharNumber) = FindMaxSubstr(charNumber, buffer, sourceSequence);
            charNumber = newCharNumber;

            int flag, matchLen, offset, matchLenSymbolsInBuffer;
            string binarySequence, unar;

            if (match.Length > 0)
            {
                flag = 1;
                matchLen = match.Length;
                offset = charNumber - matchLen - buffer.IndexOf(match);
                unar = GetMon(matchLen);
                buffer += match;
                matchLenSymbolsInBuffer = buffer.LastIndexOf(match);
                binarySequence = GetBinSeq(matchLenSymbolsInBuffer, offset);
            }
            else
            {
                flag = 0;
                match = sourceSequence[charNumber].ToString();
                matchLen = 0;
                offset = -1;
                matchLenSymbolsInBuffer = -1;
                binarySequence = "";
                unar = Convert.ToString(match[0], 2).PadLeft(8, '0');
                buffer += match;
            }

            // Подсчёт битов
            int bitsCount = 1 + binarySequence.Length + unar.Length;
            allBitsCount += bitsCount;

            // Форматирование offset (d): если offset == -1, заменяем на "-"
            string offsetStr = (offset == -1) ? "-" : $"{offset}({matchLenSymbolsInBuffer})";

            Console.WriteLine("{0,-5} | {1,-4} | {2,-20} | {3,-10} | {4,-5} | {5,-25} | {6,-5}",
                              step, flag, match, offsetStr, matchLen,
                              flag + " " + binarySequence + " " + unar, bitsCount);

            charNumber++;
            step++;

            // Ограничение размера буфера
            if (buffer.Length > winSize)
                buffer = buffer.Substring(buffer.Length - winSize);
        }

        Console.WriteLine(new string('-', 90));
        Console.WriteLine($"Итого битов: {allBitsCount}");
    }

    static void Main()
    {
        string sourceSequence = "who_chatters_to_you_will_chatter_about_you";
        int winSize = 100; // Размер окна
        Algorithm(sourceSequence, winSize);
    }
}