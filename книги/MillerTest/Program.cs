using System;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Тест простоты Миллера");
        Console.WriteLine("Введите число для проверки:");
        
        if (!ulong.TryParse(Console.ReadLine(), out ulong n))
        {
            Console.WriteLine("Некорректный ввод. Введите положительное целое число.");
            return;
        }
        
        bool isPrime = IsPrimeMillerRabin(n, 10); // 10 раундов проверки
        Console.WriteLine($"Число {n} {(isPrime ? "простое" : "составное")}");
    }
    
    // Проверка, является ли число простым по тесту Миллера-Рабина
    static bool IsPrimeMillerRabin(ulong n, int k)
    {
        // Обработка особых случаев
        if (n <= 1) return false;
        if (n <= 3) return true;
        if (n % 2 == 0) return false;
        
        // Представление n-1 в виде 2^s * d, где d нечетное
        ulong d = n - 1;
        int s = 0;
        
        while (d % 2 == 0)
        {
            d /= 2;
            s++;
        }
        
        // Выполняем k тестов
        Random rand = new Random();
        for (int i = 0; i < k; i++)
        {
            // Выбираем случайное a в диапазоне [2, n-2]
            ulong a = 2;
            if (n > 4) // Убедимся, что у нас есть достаточно большой диапазон
            {
                a = (ulong)rand.NextInt64(2, (long)(n - 1));
            }
            
            // Если (a, n) != 1, то n не простое
            if (GCD(a, n) != 1)
                return false;
            
            // Проверяем условие критерия Миллера
            if (!MillerTest(a, n, d, s))
                return false;
        }
        
        return true; // Число простое
    }
    
    // Вычисление НОД с помощью алгоритма Евклида
    static ulong GCD(ulong a, ulong b)
    {
        while (b != 0)
        {
            ulong temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    
    // Тест Миллера для конкретного значения a
    static bool MillerTest(ulong a, ulong n, ulong d, int s)
    {
        // Вычисляем a^d mod n
        BigInteger x = BigInteger.ModPow(a, d, n);
        
        // Если a^d ≡ 1 (mod n) или a^d ≡ -1 (mod n)
        if (x.Equals(BigInteger.One) || x.Equals(n - 1))
            return true;
        
        // Проверяем условие a^(2^r * d) ≡ -1 (mod n) для всех r, 0 <= r < s-1
        for (int r = 1; r < s; r++)
        {
            // x = x^2 mod n
            x = BigInteger.ModPow(x, 2, n);
            
            // Если x ≡ -1 (mod n)
            if (x.Equals(n - 1))
                return true;
            
            // Если x ≡ 1 (mod n), то это точно не простое число
            if (x.Equals(BigInteger.One))
                return false;
        }
        
        return false; // Если не нашли подходящее значение, то число составное
    }
}
