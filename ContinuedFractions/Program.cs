using System;
using System.Numerics;
using ContinuedFractions;

/// <summary>
/// Демонстрационная программа для работы с конечными и бесконечными цепными дробями
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа в программу
    /// </summary>
    /// <param name="args">Аргументы командной строки (не используются)</param>
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Console.WriteLine("Программа для создания цепной дроби из числа");
        Console.WriteLine("Введите число:");
        
        if (double.TryParse(Console.ReadLine(), out double number))
        {
            // Создаем цепную дробь из введенного числа
            var cf = ContinuedFraction.FromDouble(number, 10);
            
            Console.WriteLine("\nРезультаты:");
            Console.WriteLine($"Цепная дробь: {cf}");
            Console.WriteLine($"В виде обыкновенной дроби: {cf.ToRational().Numerator}/{cf.ToRational().Denominator}");
            Console.WriteLine($"В десятичном виде: {cf.ToDouble():F8}");
            Console.WriteLine($"Погрешность: {Math.Abs(number - cf.ToDouble()):E6}");
            
            Console.WriteLine("\nВизуализация цепной дроби:");
            Console.WriteLine(ContinuedFractionVisualizer.Visualize(cf));
        }
        else
        {
            Console.WriteLine("Ошибка: Введите корректное число");
        }
        
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
} 