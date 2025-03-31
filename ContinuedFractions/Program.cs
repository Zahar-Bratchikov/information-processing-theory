using System;
using System.Numerics;
using ContinuedFractions;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("Примеры работы с конечными цепными дробями\n");

        // Пример 1: Создание цепной дроби и преобразование её в рациональное число
        var cf1 = new ContinuedFraction(2, 3, 4, 5);
        Console.WriteLine($"Цепная дробь: {cf1}");
        var rational = cf1.ToRational();
        Console.WriteLine($"В виде обыкновенной дроби: {rational.Numerator}/{rational.Denominator}");
        Console.WriteLine($"В десятичном виде: {cf1.ToDouble():F8}");
        Console.WriteLine();

        // Пример 2: Создание цепной дроби из рационального числа
        BigInteger num = 355;
        BigInteger denom = 113;
        var cf2 = ContinuedFraction.FromRational(num, denom);
        Console.WriteLine($"Рациональное число {num}/{denom} в виде цепной дроби: {cf2}");
        Console.WriteLine($"Проверка: {cf2.ToRational().Numerator}/{cf2.ToRational().Denominator}");
        Console.WriteLine();

        // Пример 3: Создание цепной дроби из вещественного числа (π)
        double pi = Math.PI;
        var cf3 = ContinuedFraction.FromDouble(pi, 10);
        Console.WriteLine($"Число π ≈ {pi} в виде цепной дроби: {cf3}");
        Console.WriteLine($"Приближение π через цепную дробь: {cf3.ToDouble():F8}");
        Console.WriteLine($"Погрешность: {Math.Abs(pi - cf3.ToDouble()):E6}");
        Console.WriteLine();

        // Пример 4: Создание цепной дроби из корня из двух
        double sqrt2 = Math.Sqrt(2);
        var cf4 = ContinuedFraction.FromDouble(sqrt2, 10);
        Console.WriteLine($"Число √2 ≈ {sqrt2} в виде цепной дроби: {cf4}");
        Console.WriteLine($"Приближение √2 через цепную дробь: {cf4.ToDouble():F8}");
        Console.WriteLine($"Погрешность: {Math.Abs(sqrt2 - cf4.ToDouble()):E6}");
        Console.WriteLine();
        
        // Пример визуализации цепной дроби
        Console.WriteLine("Визуализация цепной дроби [2; 3, 4, 5]:");
        var cfVis = new ContinuedFraction(2, 3, 4, 5);
        Console.WriteLine(ContinuedFractionVisualizer.Visualize(cfVis));
        Console.WriteLine();
        
        Console.WriteLine("\nПримеры работы с бесконечными цепными дробями\n");
        
        // Пример 5: Бесконечная цепная дробь для √2
        var icf1 = InfiniteContinuedFraction.CreateForSquareRoot(2);
        Console.WriteLine($"Бесконечная цепная дробь для √2: {icf1}");
        
        // Выводим последовательные приближения
        Console.WriteLine("Последовательные приближения √2:");
        for (int i = 1; i <= 10; i++)
        {
            var convergent = icf1.GetConvergent(i);
            double approximation = (double)convergent.Numerator / (double)convergent.Denominator;
            Console.WriteLine($"  {icf1.GetApproximation(i)} = " +
                             $"{convergent.Numerator}/{convergent.Denominator} ≈ " +
                             $"{approximation:F10} (ошибка: {Math.Abs(sqrt2 - approximation):E10})");
        }
        Console.WriteLine();
        
        // Пример 6: Бесконечная цепная дробь для √3
        var icf2 = InfiniteContinuedFraction.CreateForSquareRoot(3);
        Console.WriteLine($"Бесконечная цепная дробь для √3: {icf2}");
        double sqrt3 = Math.Sqrt(3);
        
        // Выводим последовательные приближения
        Console.WriteLine("Последовательные приближения √3:");
        for (int i = 1; i <= 8; i++)
        {
            var convergent = icf2.GetConvergent(i);
            double approximation = (double)convergent.Numerator / (double)convergent.Denominator;
            Console.WriteLine($"  {icf2.GetApproximation(i)} = " +
                             $"{convergent.Numerator}/{convergent.Denominator} ≈ " +
                             $"{approximation:F10} (ошибка: {Math.Abs(sqrt3 - approximation):E10})");
        }
        Console.WriteLine();
        
        // Пример 7: Бесконечная цепная дробь для e (числа Эйлера)
        // e = [2; 1, 2, 1, 1, 4, 1, 1, 6, 1, 1, 8, ...]
        // Закономерность: [2; 1, 2, 1, 1, 4, 1, 1, 6, 1, 1, 8, ...]
        var icf3 = new InfiniteContinuedFraction(2, n => 
        {
            if (n % 3 == 0)
                return 2 * (n / 3);
            return 1;
        });
        Console.WriteLine($"Бесконечная цепная дробь для e: {icf3}");
        double e = Math.E;
        
        // Выводим последовательные приближения
        Console.WriteLine("Последовательные приближения e:");
        for (int i = 1; i <= 10; i++)
        {
            var convergent = icf3.GetConvergent(i);
            double approximation = (double)convergent.Numerator / (double)convergent.Denominator;
            Console.WriteLine($"  {icf3.GetApproximation(i)} = " +
                             $"{convergent.Numerator}/{convergent.Denominator} ≈ " +
                             $"{approximation:F10} (ошибка: {Math.Abs(e - approximation):E10})");
        }
        
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
    
    // Вспомогательный метод для получения строкового представления элементов бесконечной цепной дроби
    static string[] GetElementsString(InfiniteContinuedFraction icf, int count)
    {
        string[] elements = new string[count];
        for (int i = 0; i < count; i++)
        {
            elements[i] = icf.GetElement(i).ToString();
        }
        return elements;
    }
} 