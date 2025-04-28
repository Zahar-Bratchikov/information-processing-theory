using System;
using System.Numerics;

namespace PollardsRho
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            Console.Write("Введите составное число N: ");
            if (!BigInteger.TryParse(Console.ReadLine(), out BigInteger N))
            {
                Console.WriteLine("Некорректный ввод. Пожалуйста, введите целое число.");
                return;
            }

            if (N <= 1)
            {
                Console.WriteLine("N должно быть больше 1.");
                return;
            }

            double epsilon = 0.5; // Параметр ε из алгоритма (можно изменить)
            BigInteger divisor = FindDivisor(N, epsilon);

            if (divisor == 1 || divisor == N)
            {
                Console.WriteLine($"Алгоритм не смог найти нетривиальный делитель для {N}.");
            }
            else
            {
                Console.WriteLine($"Найден делитель: {divisor}");
                Console.WriteLine($"{N} = {divisor} × {N / divisor}");
            }
            
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        /// <summary>
        /// Алгоритм ρ-Полларда для нахождения делителя числа N
        /// </summary>
        static BigInteger FindDivisor(BigInteger N, double epsilon)
        {
            Console.WriteLine($"Шаг 1. Вычисляем T₂ = ⌊√(2N·ln(1/ε))⌋ + 1");
            // Вычисляем T₂ = ⌊√(2N·ln(1/ε))⌋ + 1
            int T2 = (int)Math.Floor(Math.Sqrt(2 * (double)(N) * Math.Log(1 / epsilon))) + 1;
            Console.WriteLine($"T₂ = {T2}");

            // Создаем генератор случайных чисел
            Random random = new Random();

            while (true)
            {
                Console.WriteLine("\nШаг 2. Выбираем случайный многочлен f(x) ∈ Z_N[x]");
                // Создаем полином f(x) = x² + a (mod N)
                int a = random.Next(1, (int)Math.Min(int.MaxValue, (long)N));
                Console.WriteLine($"f(x) = x² + {a} (mod {N})");

                Console.WriteLine($"\nШаг 3. Выбираем случайно x₀ ∈ Z_N и вычисляем последовательность элементов");
                // Выбираем случайное начальное значение x₀ ∈ Z_N
                BigInteger x0 = random.Next(0, (int)Math.Min(int.MaxValue, (long)N));
                Console.WriteLine($"x₀ = {x0}");

                // Вычисляем последовательность x_i+1 = f(x_i) (mod N), 0 ≤ i ≤ T₂
                BigInteger[] sequence = new BigInteger[T2 + 1];
                sequence[0] = x0;

                for (int i = 0; i < T2; i++)
                {
                    // Вычисляем x_i+1 = f(x_i) (mod N) = (x_i² + a) mod N
                    sequence[i + 1] = (BigInteger.ModPow(sequence[i], 2, N) + a) % N;
                    Console.WriteLine($"x_{i+1} = {sequence[i+1]}");
                }

                Console.WriteLine($"\nШаг 4. Для каждого 0 ≤ k < i вычисляем d_k = (x_i - x_k, N)");
                // Для каждого 0 ≤ k < i вычисляем d_k = (x_i - x_k, N)
                for (int i = 1; i <= T2; i++)
                {
                    for (int k = 0; k < i; k++)
                    {
                        BigInteger diff = (sequence[i] - sequence[k]) % N;
                        if (diff < 0) diff += N; // Убедимся, что разность положительна

                        BigInteger gcd = BigInteger.GreatestCommonDivisor(diff, N);
                        Console.WriteLine($"d_{k} = НОД({sequence[i]} - {sequence[k]}, {N}) = НОД({diff}, {N}) = {gcd}");

                        if (gcd > 1 && gcd < N)
                        {
                            Console.WriteLine($"Найден нетривиальный делитель: {gcd}");
                            return gcd;
                        }
                        else if (gcd == N)
                        {
                            Console.WriteLine($"d_{k} = N = {N}, выбираем новое случайное значение x₀ и новый многочлен f(x)");
                            break;
                        }
                    }
                }

                Console.WriteLine("Делитель не найден, перезапускаем алгоритм с новым многочленом и новым начальным значением.");
            }
        }
    }
}
