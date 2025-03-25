using System.Text;

//https://en.wikipedia.org/wiki/Repetition_code#:~:text=In%20coding%20theory%2C%20the%20repetition,repeat%20the%20message%20several%20times.
public class RepetitionCode
{
    // Метод для преобразования текста в последовательность битов
    public static List<int> TextToBits(string text)
    {
        List<int> bits = new List<int>();
        foreach (byte b in Encoding.UTF8.GetBytes(text))
        {
            string binary = Convert.ToString(b, 2).PadLeft(8, '0');
            foreach (char bit in binary)
            {
                bits.Add(int.Parse(bit.ToString()));
            }
        }
        return bits;
    }

    // Метод для преобразования последовательности битов в текст
    public static string BitsToText(List<int> bits)
    {
        bits = bits.Take(bits.Count - bits.Count % 8).ToList();
        List<byte> bytes = new List<byte>();
        for (int i = 0; i < bits.Count; i += 8)
        {
            string byteStr = string.Join("", bits.GetRange(i, 8));
            bytes.Add(Convert.ToByte(byteStr, 2));
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    // Метод для кодирования повторным кодом (Repetition Code) с коэффициентом повторения 3
    public static List<int> EncodeRepetition(List<int> dataBits)
    {
        List<int> encoded = new List<int>();
        foreach (int bit in dataBits)
        {
            // Кодируем каждый бит, повторяя его 3 раза, для обеспечения устойчивости к ошибкам
            encoded.Add(bit);
            encoded.Add(bit);
            encoded.Add(bit);
        }
        return encoded;
    }

    // Метод для декодирования повторного кода (Repetition Code) с коэффициентом повторения 3
    public static List<int> DecodeRepetition(List<int> receivedBits)
    {
        List<int> decoded = new List<int>();

        // Группируем биты по 3 и используем правило большинства для восстановления исходного бита
        for (int i = 0; i < receivedBits.Count; i += 3)
        {
            int countOnes = 0;
            int countZeros = 0;
            for (int j = 0; j < 3; j++)
            {
                if (receivedBits[i + j] == 1)
                    countOnes++;
                else
                    countZeros++;
            }
            // Если количество единиц больше, чем нулей, принимаем бит как 1, иначе как 0
            decoded.Add(countOnes > countZeros ? 1 : 0);
        }
        return decoded;
    }

    public static void Main(string[] args)
    {
        Console.Write("Введите слово для кодирования: ");
        string text = Console.ReadLine(); // Чтение слова для кодирования

        // Преобразуем текст в последовательность битов
        List<int> dataBits = TextToBits(text);
        Console.WriteLine("Исходная последовательность битов: " + string.Join("", dataBits));

        // Кодируем с помощью повторного кода
        List<int> encodedBits = EncodeRepetition(dataBits);
        Console.WriteLine("Закодированная последовательность (повторный код): " + string.Join("", encodedBits));

        // Симуляция ошибки: инвертируем случайный бит в закодированной последовательности
        Random random = new Random();
        int errorIndex = random.Next(0, encodedBits.Count);
        encodedBits[errorIndex] = encodedBits[errorIndex] == 0 ? 1 : 0;
        Console.WriteLine($"Принятая последовательность (с ошибкой в позиции {errorIndex + 1}): " + string.Join("", encodedBits));

        // Декодируем полученную последовательность с использованием правила большинства
        List<int> decodedBits = DecodeRepetition(encodedBits);
        Console.WriteLine("Декодированная последовательность битов: " + string.Join("", decodedBits));

        // Преобразуем декодированные биты обратно в текст
        string decodedText = BitsToText(decodedBits);
        Console.WriteLine("Полностью декодированное слово: " + decodedText);

        Console.ReadKey();
    }
}