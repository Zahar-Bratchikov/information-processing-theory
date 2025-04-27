using System.Text;

/// <summary>
/// Реализация повторного кода для обнаружения и исправления ошибок
/// при передаче данных. Повторный код - это один из простейших методов помехоустойчивого
/// кодирования, который заключается в повторении каждого бита информации несколько раз.
/// </summary>
/// <remarks>
/// Подробнее: https://en.wikipedia.org/wiki/Repetition_code
/// </remarks>
public class RepetitionCode
{
    /// <summary>
    /// Преобразует текстовую строку в последовательность битов (0 и 1)
    /// </summary>
    /// <param name="text">Входная строка для преобразования</param>
    /// <returns>Список целых чисел, представляющих биты (0 или 1)</returns>
    public static List<int> TextToBits(string text)
    {
        List<int> bits = new List<int>();
        // Преобразуем строку в массив байтов с использованием кодировки UTF-8
        foreach (byte b in Encoding.UTF8.GetBytes(text))
        {
            // Преобразуем каждый байт в 8-битную двоичную строку с ведущими нулями
            string binary = Convert.ToString(b, 2).PadLeft(8, '0');
            // Добавляем каждый бит в виде отдельного числа (0 или 1) в результирующий список
            foreach (char bit in binary)
            {
                bits.Add(int.Parse(bit.ToString()));
            }
        }
        return bits;
    }

    /// <summary>
    /// Преобразует последовательность битов обратно в текстовую строку
    /// </summary>
    /// <param name="bits">Список целых чисел, представляющих биты (0 или 1)</param>
    /// <returns>Восстановленная текстовая строка</returns>
    public static string BitsToText(List<int> bits)
    {
        // Убираем лишние биты, если их количество не кратно 8
        // (каждый символ кодируется 8 битами)
        bits = bits.Take(bits.Count - bits.Count % 8).ToList();
        List<byte> bytes = new List<byte>();
        
        // Преобразуем последовательность битов в байты, группируя по 8 бит
        for (int i = 0; i < bits.Count; i += 8)
        {
            // Объединяем 8 битов в строку и преобразуем её в байт
            string byteStr = string.Join("", bits.GetRange(i, 8));
            bytes.Add(Convert.ToByte(byteStr, 2));
        }
        
        // Преобразуем массив байтов в строку с использованием кодировки UTF-8
        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    /// <summary>
    /// Кодирует исходное сообщение с помощью повторного кода с коэффициентом 3.
    /// Каждый бит исходного сообщения повторяется трижды для обеспечения
    /// возможности исправления одиночных ошибок.
    /// </summary>
    /// <param name="dataBits">Исходная последовательность битов для кодирования</param>
    /// <returns>Закодированная последовательность битов</returns>
    public static List<int> EncodeRepetition(List<int> dataBits)
    {
        List<int> encoded = new List<int>();
        foreach (int bit in dataBits)
        {
            // Кодируем каждый бит, повторяя его 3 раза, для обеспечения устойчивости к ошибкам
            // Это позволяет исправить до 1 ошибки в каждой тройке битов
            encoded.Add(bit);
            encoded.Add(bit);
            encoded.Add(bit);
        }
        return encoded;
    }

    /// <summary>
    /// Декодирует сообщение, закодированное повторным кодом с коэффициентом 3.
    /// Использует принцип "большинства голосов" для восстановления исходного бита.
    /// </summary>
    /// <param name="receivedBits">Принятая (возможно с ошибками) последовательность битов</param>
    /// <returns>Декодированная последовательность битов</returns>
    public static List<int> DecodeRepetition(List<int> receivedBits)
    {
        List<int> decoded = new List<int>();

        // Группируем биты по 3 и используем правило большинства для восстановления исходного бита
        for (int i = 0; i < receivedBits.Count; i += 3)
        {
            int countOnes = 0;
            int countZeros = 0;
            
            // Подсчитываем количество нулей и единиц в тройке битов
            for (int j = 0; j < 3; j++)
            {
                if (i + j < receivedBits.Count) // Защита от выхода за границы массива
                {
                    if (receivedBits[i + j] == 1)
                        countOnes++;
                    else
                        countZeros++;
                }
            }
            
            // Если количество единиц больше, чем нулей, принимаем бит как 1, иначе как 0
            // Это правило большинства позволяет исправить одиночную ошибку в тройке битов
            decoded.Add(countOnes > countZeros ? 1 : 0);
        }
        return decoded;
    }

    /// <summary>
    /// Главный метод программы, демонстрирующий работу повторного кода.
    /// Выполняет следующие действия:
    /// 1. Получает входное сообщение от пользователя
    /// 2. Кодирует сообщение с помощью повторного кода
    /// 3. Имитирует ошибку передачи, искажая один случайный бит
    /// 4. Декодирует искаженное сообщение
    /// 5. Выводит результаты на экран
    /// </summary>
    public static void Main(string[] args)
    {
        // Запрашиваем у пользователя входное сообщение
        Console.Write("Введите слово для кодирования: ");
        string text = Console.ReadLine(); // Чтение слова для кодирования

        // Преобразуем текст в последовательность битов
        List<int> dataBits = TextToBits(text);
        Console.WriteLine("Исходная последовательность битов: " + string.Join("", dataBits));

        // Кодируем с помощью повторного кода (каждый бит повторяется 3 раза)
        List<int> encodedBits = EncodeRepetition(dataBits);
        Console.WriteLine("Закодированная последовательность (повторный код): " + string.Join("", encodedBits));

        // Симуляция ошибки передачи данных:
        // Инвертируем случайный бит в закодированной последовательности
        Random random = new Random();
        int errorIndex = random.Next(0, encodedBits.Count);
        encodedBits[errorIndex] = encodedBits[errorIndex] == 0 ? 1 : 0; // Инвертируем бит (0->1 или 1->0)
        int errorIndex1 = random.Next(0, encodedBits.Count);
        encodedBits[errorIndex] = encodedBits[errorIndex] == 0 ? 1 : 0; // Инвертируем бит (0->1 или 1->0)
        Console.WriteLine($"Принятая последовательность (с ошибкой в позиции {errorIndex + 1} и {errorIndex1 + 1}): " + string.Join("", encodedBits));

        // Декодируем полученную последовательность с использованием правила большинства
        // Благодаря повторному коду, одиночные ошибки будут исправлены
        List<int> decodedBits = DecodeRepetition(encodedBits);
        Console.WriteLine("Декодированная последовательность битов: " + string.Join("", decodedBits));

        // Преобразуем декодированные биты обратно в текст
        string decodedText = BitsToText(decodedBits);
        Console.WriteLine("Полностью декодированное слово: " + decodedText);

        // Ожидаем нажатия клавиши перед завершением программы
        Console.ReadKey();
    }
}