using System;
using System.Numerics;
using System.Collections.Generic;

class Program
{
    // Заранее подготовленный список простых чисел до 2^16
    private static readonly List<int> smallPrimes = GenerateSmallPrimes(65536);

    static void Main(string[] args)
    {
        Console.WriteLine("Алгоритм построения простого числа");
        
        Console.WriteLine("Введите число A:");
        if (!ulong.TryParse(Console.ReadLine(), out ulong A))
        {
            Console.WriteLine("Некорректный ввод для A.");
            return;
        }
        
        Console.WriteLine("Введите число B (B > A + 1):");
        if (!ulong.TryParse(Console.ReadLine(), out ulong B) || B <= A + 1)
        {
            Console.WriteLine("Некорректный ввод для B. Должно быть B > A + 1.");
            return;
        }
        
        ulong p = GeneratePrime(A, B);
        Console.WriteLine($"Сгенерированное простое число: {p}");
    }
    
    static ulong GeneratePrime(ulong A, ulong B)
    {
        // Шаг 1: Если B ≤ 2^16, выбрать простое число из таблицы
        if (B <= 65536)
        {
            Random rand = new Random();
            List<int> possiblePrimes = new List<int>();
            
            foreach (int prime in smallPrimes)
            {
                if (prime > (int)A && prime < (int)B)
                    possiblePrimes.Add(prime);
            }
            
            if (possiblePrimes.Count > 0)
            {
                int randomIndex = rand.Next(possiblePrimes.Count);
                return (ulong)possiblePrimes[randomIndex];
            }
        }
        
        // Шаг 2: Определить переменные
        ulong qA = (ulong)Math.Sqrt(B);
        ulong a = (B + A - 1) / (2 * A);
        ulong k1 = (ulong)Math.Ceiling((double)A / qA);
        ulong k2 = (ulong)Math.Floor((double)B / qA);
        
        // Шаг 3-13: Реализация алгоритма построения простого числа
        while (true)
        {
            // Шаг 3: Построить простое число q в интервале qA < q < [a * qA]
            ulong q = GeneratePrimeInRange(qA, (ulong)(a * qA));
            
            // Шаг 4: Выбрать случайное число k в интервале k1 < k < k2
            Random rand = new Random();
            ulong k;
            do
            {
                k = (ulong)rand.NextInt64((long)k1 + 1, (long)k2);
            } while (k == k1 || k == k2);
            
            if (k % 2 == 1)
                k = k - 1;
            
            ulong kStart = k;
            ulong p;
            
            // Первые 10 простых чисел
            int[] d = { 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 };
            int[] delta = new int[10];
            
            // Шаг 5: Определить остатки от деления числа p на простые числа d_i
            do
            {
                p = k * q + 1;
                
                for (int i = 0; i < 10; i++)
                {
                    delta[i] = (int)(p % (ulong)d[i]);
                }
                
                // Шаг 6-7: Увеличивать k на 2 и пересчитывать p и delta
                bool foundCandidate = false;
                while (k <= k2)
                {
                    bool passedAllTests = true;
                    
                    // Проверка, что p не делится на малые простые числа
                    for (int i = 0; i < 10; i++)
                    {
                        if (delta[i] == 0)
                        {
                            passedAllTests = false;
                            break;
                        }
                    }
                    
                    if (passedAllTests)
                    {
                        foundCandidate = true;
                        break;
                    }
                    
                    // Увеличиваем k на 2 и пересчитываем p и delta
                    k += 2;
                    p = k * q + 1;
                    
                    for (int i = 0; i < 10; i++)
                    {
                        delta[i] = (int)((delta[i] + 2 * (int)q) % d[i]);
                    }
                }
                
                if (!foundCandidate)
                {
                    // Если не нашли подходящего k, начинаем с новым q
                    break;
                }
                
                // Шаг 9: Применить тест Миллера-Рабина
                if (IsPrimeMillerRabin(p, 10))
                {
                    // Шаг 10-13: Дополнительные проверки
                    int c = 10;
                    while (c > 0)
                    {
                        ulong a_witness = 2; // Используем a = 2 как свидетеля
                        if (GCD(BigInteger.Pow(a_witness, (int)k) - 1, p) == 1 &&
                            BigInteger.ModPow(a_witness, p - 1, p) == 1)
                        {
                            // Число p признано простым
                            return p;
                        }
                        c--;
                    }
                }
                
                // Переходим к следующему потенциальному k
                k += 2;
                
            } while (k <= k2);
            
            // Если мы дошли до этой точки, нужно выбрать новое значение q и начать заново
        }
    }
    
    // Генерация простого числа в заданном диапазоне
    static ulong GeneratePrimeInRange(ulong min, ulong max)
    {
        Random rand = new Random();
        ulong candidate;
        
        do
        {
            candidate = (ulong)rand.NextInt64((long)min + 1, (long)max);
            if (candidate % 2 == 0) candidate++;  // Начинаем с нечетного числа
        } while (!IsPrimeMillerRabin(candidate, 10));
        
        return candidate;
    }
    
    // Тест Миллера-Рабина для проверки простоты числа
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
            
            if (!MillerTest(a, n, d, s))
                return false;
        }
        
        return true; // Число вероятно простое
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
    
    // Вычисление НОД с помощью алгоритма Евклида
    static ulong GCD(BigInteger a, ulong b)
    {
        BigInteger bb = new BigInteger(b);
        while (bb != 0)
        {
            BigInteger temp = bb;
            bb = a % bb;
            a = temp;
        }
        return (ulong)a;
    }
    
    // Генерация списка малых простых чисел методом решета Эратосфена
    static List<int> GenerateSmallPrimes(int limit)
    {
        bool[] isPrime = new bool[limit + 1];
        for (int i = 2; i <= limit; i++)
        {
            isPrime[i] = true;
        }
        
        for (int p = 2; p * p <= limit; p++)
        {
            if (isPrime[p])
            {
                for (int i = p * p; i <= limit; i += p)
                {
                    isPrime[i] = false;
                }
            }
        }
        
        List<int> primes = new List<int>();
        for (int p = 2; p <= limit; p++)
        {
            if (isPrime[p])
            {
                primes.Add(p);
            }
        }
        
        return primes;
    }
}
