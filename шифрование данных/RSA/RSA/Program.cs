using System.Numerics;

/// <summary>
/// Класс, демонстрирующий работу алгоритма шифрования RSA
/// </summary>
class RSAExample
{
    /// <summary>
    /// Точка входа в программу
    /// </summary>
    static void Main()
    {
        try
        {
            // Генерируем пару ключей RSA: публичный и приватный
            var keys = GenerateKeys();
            var publicKey = keys.Item1;
            var privateKey = keys.Item2;

            // Запрашиваем у пользователя сообщение для шифрования
            Console.Write("Введите сообщение для шифрования: ");
            string message = Console.ReadLine();  // Исходное сообщение
            
            // Шифруем сообщение с использованием публичного ключа
            var ciphertext = Encrypt(message, publicKey);
            
            // Расшифровываем сообщение с использованием приватного ключа
            var decryptedMessage = Decrypt(ciphertext, privateKey);

            // Выводим результаты всех операций
            Console.WriteLine("Оригинальное сообщение: " + message);
            Console.WriteLine("Зашифрованное сообщение: " + string.Join(", ", ciphertext));
            Console.WriteLine("Расшифрованное сообщение: " + decryptedMessage);
        }
        catch (Exception ex)
        {
            // Обработка возможных ошибок при работе программы
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    /// <summary>
    /// Генерирует случайное простое число в заданном диапазоне
    /// </summary>
    /// <param name="start">Нижняя граница диапазона</param>
    /// <param name="end">Верхняя граница диапазона</param>
    /// <returns>Случайное простое число</returns>
    static BigInteger GeneratePrime(int start = 1000, int end = 5000)
    {
        Random random = new Random();
        while (true)
        {
            // Генерируем случайное число и проверяем его на простоту
            int num = random.Next(start, end);
            if (IsPrime(num))
            {
                return num;
            }
        }
    }

    /// <summary>
    /// Проверяет, является ли число простым
    /// </summary>
    /// <param name="number">Проверяемое число</param>
    /// <returns>true, если число простое; иначе false</returns>
    static bool IsPrime(int number)
    {
        // Числа меньше 2 не являются простыми
        if (number < 2) return false;
        
        // Проверяем делимость числа на все возможные делители до квадратного корня из числа
        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Вычисляет обратное по модулю число
    /// a * x ≡ 1 (mod m)
    /// </summary>
    /// <param name="a">Число, для которого ищется обратное</param>
    /// <param name="m">Модуль</param>
    /// <returns>Обратное по модулю число</returns>
    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        // Используем расширенный алгоритм Евклида для нахождения обратного по модулю
        BigInteger m0 = m, t, q;
        BigInteger x0 = 0, x1 = 1;
        
        if (m == 1) return 0;
        
        // Выполняем алгоритм до тех пор, пока 'a' не станет равным 1
        while (a > 1)
        {
            q = a / m;
            t = m;
            m = a % m;
            a = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }
        
        // Если x1 отрицательно, добавляем m0
        if (x1 < 0) x1 += m0;
        
        return x1;
    }

    /// <summary>
    /// Генерирует пару RSA ключей: публичный и приватный
    /// </summary>
    /// <returns>Кортеж, содержащий публичный и приватный ключи</returns>
    static Tuple<(BigInteger, BigInteger), (BigInteger, BigInteger)> GenerateKeys()
    {
        BigInteger p, q, n, phi_n, e, d;
        while (true)
        {
            try
            {
                // Генерируем два случайных простых числа
                p = GeneratePrime();
                q = GeneratePrime();
                
                Console.WriteLine(p.ToString());
                Console.WriteLine(q.ToString());
                // Вычисляем модуль n = p * q
                n = p * q;
                
                // Вычисляем функцию Эйлера φ(n) = (p-1) * (q-1)
                phi_n = (p - 1) * (q - 1);

                // Выбираем e
                // e должно быть взаимно простым с φ(n)
                e = 3; // Начинаем с малого значения
                while (e < phi_n && BigInteger.GreatestCommonDivisor(e, phi_n) != 1)
                {
                    e += 2;
                }

                // Вычисляем d
                // d * e ≡ 1 (mod φ(n))
                d = ModInverse(e, phi_n);
                break;
            }
            catch
            {
                // Если что-то пошло не так, повторяем генерацию заново
                continue;
            }
        }

        // Возвращаем публичный ключ (e, n) и приватный ключ (d, n)
        return Tuple.Create((e, n), (d, n));
    }

    /// <summary>
    /// Шифрует сообщение с использованием публичного ключа RSA
    /// </summary>
    /// <param name="message">Исходное сообщение</param>
    /// <param name="publicKey">Публичный ключ (e, n)</param>
    /// <returns>Список зашифрованных значений</returns>
    static List<BigInteger> Encrypt(string message, (BigInteger e, BigInteger n) publicKey)
    {
        var encrypted = new List<BigInteger>();
        foreach (char c in message)
        {
            // Для каждого символа применяем формулу шифрования: c^e mod n
            encrypted.Add(BigInteger.ModPow(c, publicKey.e, publicKey.n));
        }
        return encrypted;
    }

    /// <summary>
    /// Расшифровывает сообщение с использованием приватного ключа RSA
    /// </summary>
    /// <param name="ciphertext">Зашифрованное сообщение</param>
    /// <param name="privateKey">Приватный ключ (d, n)</param>
    /// <returns>Расшифрованное сообщение</returns>
    static string Decrypt(List<BigInteger> ciphertext, (BigInteger d, BigInteger n) privateKey)
    {
        var decrypted = new List<char>();
        foreach (var c in ciphertext)
        {
            // Для каждого зашифрованного значения применяем формулу расшифрования: c^d mod n
            decrypted.Add((char)BigInteger.ModPow(c, privateKey.d, privateKey.n));
        }
        return new string(decrypted.ToArray());
    }
}