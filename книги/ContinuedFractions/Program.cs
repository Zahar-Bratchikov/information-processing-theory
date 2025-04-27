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
        
        Console.WriteLine("Введите дробь в формате числитель/знаменатель или десятичное число:");
        string input = Console.ReadLine();
        
        double number;
        
        if (input.Contains("/"))
        {
            string[] parts = input.Split('/');
            if (parts.Length == 2 && 
                BigInteger.TryParse(parts[0], out BigInteger numerator) && 
                BigInteger.TryParse(parts[1], out BigInteger denominator) &&
                denominator != 0)
            {
                // Создаем цепную дробь из дроби
                number = (double)numerator / (double)denominator;
            }
            else
            {
                Console.WriteLine("Ошибка: Неверный формат дроби");
                Console.ReadKey();
                return;
            }
        }
        else if (double.TryParse(input, out number))
        {
            // Просто используем введенное число
        }
        else
        {
            Console.WriteLine("Ошибка: Введите корректное число или дробь");
            Console.ReadKey();
            return;
        }
        
        // Создаем цепную дробь из введенного числа
        var cf = ContinuedFraction.FromDouble(number, 10);
        
        Console.WriteLine($"\nЦепная дробь: {cf}");
        Console.WriteLine(ContinuedFractionVisualizer.Visualize(cf));
        
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}