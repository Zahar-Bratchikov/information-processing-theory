using System.Text;

public class HammingEncoderDecoder
{
    // Метод для преобразования текста в последовательность битов
    public static List<int> TextToBits(string text)
    {
        List<int> bits = new List<int>();
        foreach (byte b in Encoding.UTF8.GetBytes(text)) // Кодируем каждый символ текста в байт (UTF-8)
        {
            string binary = Convert.ToString(b, 2).PadLeft(8, '0'); // Преобразуем байт в двоичную строку (8 бит)
            foreach (char bit in binary) // Перебираем каждый бит в двоичной строке
            {
                bits.Add(int.Parse(bit.ToString())); // Добавляем бит (как целое число) в список битов
            }
        }
        return bits;
    }

    // Метод для преобразования последовательности битов в текст
    public static string BitsToText(List<int> bits)
    {
        bits = bits.Take(bits.Count - bits.Count % 8).ToList(); // Обрезаем биты до ближайшего кратного 8 (1 байт = 8 бит)
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < bits.Count; i += 8) // Перебираем биты блоками по 8
        {
            string byteStr = string.Join("", bits.GetRange(i, 8)); // Собираем 8 битов в строку
            bytes.Add(Convert.ToByte(byteStr, 2)); // Преобразуем двоичную строку в байт
        }
        return Encoding.UTF8.GetString(bytes.ToArray()); // Преобразуем массив байтов в строку (UTF-8)
    }

    // Метод для кодирования Хэмминга
    public static List<int> HammingEncode(List<int> dataBits)
    {
        int m = dataBits.Count; // Длина исходных данных (в битах)
        int r = 0; // Количество контрольных битов
        while (Math.Pow(2, r) < (m + r + 1)) // Вычисляем минимальное количество контрольных битов
        {
            r++;
        }

        int totalLength = m + r; // Общая длина кодированного сообщения
        List<int> hammingCode = Enumerable.Repeat(0, totalLength).ToList(); // Создаем список для кодированного сообщения, заполняем нулями

        int j = 0; // Индекс для данных
        for (int i = 1; i <= totalLength; i++) // Перебираем все позиции в кодированном сообщении
        {
            if ((i & (i - 1)) == 0) // Пропускаем позиции, являющиеся степенями двойки (позиции контрольных битов)
            {
                continue;
            }
            hammingCode[i - 1] = dataBits[j]; // Копируем биты данных в кодированное сообщение
            j++;
        }

        for (int i = 0; i < r; i++) // Вычисляем значения контрольных битов
        {
            int parityPos = (int)Math.Pow(2, i) - 1; // Позиция контрольного бита
            int parityValue = 0; // Значение контрольного бита (четность)
            for (int k = parityPos; k < totalLength; k += (int)Math.Pow(2, i + 1)) // Перебираем биты, которые контролируются данным контрольным битом
            {
                for (int l = k; l < Math.Min(k + (int)Math.Pow(2, i), totalLength); l++) // Суммируем биты в блоке
                {
                    parityValue ^= hammingCode[l]; // XOR для вычисления четности
                }
            }
            hammingCode[parityPos] = parityValue % 2; // Устанавливаем значение контрольного бита
        }

        return hammingCode; // Возвращаем кодированное сообщение
    }

    // Метод для декодирования Хэмминга и исправления ошибок
    public static List<int> HammingDecode(List<int> receivedBits)
    {
        int totalLength = receivedBits.Count; // Длина полученного сообщения
        int r = 0; // Количество контрольных битов
        while (Math.Pow(2, r) < totalLength + 1) // Вычисляем количество контрольных битов
        {
            r++;
        }

        int errorPos = 0; // Позиция ошибки
        for (int i = 0; i < r; i++) // Проверяем контрольные биты
        {
            int parityPos = (int)Math.Pow(2, i) - 1; // Позиция контрольного бита
            int parityValue = 0; // Значение контрольного бита
            for (int j = parityPos; j < totalLength; j += (int)Math.Pow(2, i + 1)) // Перебираем контролируемые биты
            {
                for (int k = j; k < Math.Min(j + (int)Math.Pow(2, i), totalLength); k++) // Суммируем биты в блоке
                {
                    parityValue ^= receivedBits[k]; // XOR для вычисления четности
                }
            }
            if (parityValue % 2 != 0) // Если четность не совпадает, добавляем позицию к ошибке
            {
                errorPos += (parityPos + 1);
            }
        }

        if (errorPos > 0) // Если есть ошибка, исправляем ее
        {
            receivedBits[errorPos - 1] ^= 1; // Инвертируем бит в позиции ошибки
        }

        List<int> decodedBits = new List<int>(); // Список для декодированных бит
        for (int i = 1; i <= totalLength; i++) // Перебираем биты
        {
            if ((i & (i - 1)) == 0) // Пропускаем контрольные биты
            {
                continue;
            }
            decodedBits.Add(receivedBits[i - 1]); // Добавляем бит данных в декодированное сообщение
        }

        return decodedBits; // Возвращаем декодированное сообщение
    }

    public static void Main(string[] args)
    {
        Console.Write("Введите слово для кодирования: ");
        string text = Console.ReadLine(); // Читаем слово из консоли

        Console.Write("Введите длину блока (в битах): ");
        int blockSize = int.Parse(Console.ReadLine()); // Читаем длину блока из консоли

        List<int> dataBits = TextToBits(text); // Преобразуем текст в биты
        List<int> decodedBitsAccumulated = new List<int>(); // Список для хранения всех декодированных битов

        for (int i = 0; i < dataBits.Count; i += blockSize) // Перебираем биты блоками
        {
            List<int> blockBits = dataBits.GetRange(i, Math.Min(blockSize, dataBits.Count - i)); // Получаем текущий блок
            Console.WriteLine("\nБлок: " + string.Join("", blockBits)); // Выводим блок

            List<int> encoded = HammingEncode(blockBits); // Кодируем блок кодом Хэмминга
            Console.WriteLine("Закодированная последовательность Хемминга: " + string.Join("", encoded)); // Выводим закодированную последовательность

            List<int> receivedBits = new List<int>(encoded); // Копируем закодированную последовательность
            Random random = new Random(); // Создаем генератор случайных чисел
            int errorIndex = random.Next(0, receivedBits.Count); // Генерируем случайную позицию для ошибки
            receivedBits[errorIndex] ^= 1; // Вносим ошибку (инвертируем бит)
            Console.WriteLine($"Принятая последовательность (с ошибкой в позиции {errorIndex + 1}): " + string.Join("", receivedBits)); // Выводим последовательность с ошибкой

            List<int> decodedBits = HammingDecode(receivedBits); // Декодируем блок
            decodedBitsAccumulated.AddRange(decodedBits); // Добавляем декодированные биты к общему списку
        }

        string decodedText = BitsToText(decodedBitsAccumulated); // Преобразуем биты обратно в текст
        Console.WriteLine("\nПолностью декодированное слово: " + decodedText); // Выводим декодированное слово

    }
}