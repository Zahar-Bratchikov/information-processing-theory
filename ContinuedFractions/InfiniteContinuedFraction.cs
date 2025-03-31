using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ContinuedFractions
{
    /// <summary>
    /// Класс для работы с бесконечными цепными дробями
    /// </summary>
    public class InfiniteContinuedFraction
    {
        private readonly int _a0;                // Начальный элемент
        private readonly Func<int, int> _getAn;  // Функция для получения n-го элемента
        private readonly int? _period;           // Период (для периодических цепных дробей)
        
        /// <summary>
        /// Создает бесконечную цепную дробь с заданной функцией генерации элементов
        /// </summary>
        /// <param name="a0">Начальный элемент</param>
        /// <param name="getAn">Функция, возвращающая n-й элемент (индексация с 1)</param>
        /// <param name="period">Период последовательности (если дробь периодическая)</param>
        public InfiniteContinuedFraction(int a0, Func<int, int> getAn, int? period = null)
        {
            _a0 = a0;
            _getAn = getAn;
            _period = period;
        }
        
        /// <summary>
        /// Создает периодическую бесконечную цепную дробь
        /// </summary>
        /// <param name="a0">Начальный элемент</param>
        /// <param name="periodicPart">Периодическая часть</param>
        public InfiniteContinuedFraction(int a0, int[] periodicPart)
        {
            if (periodicPart == null || periodicPart.Length == 0)
                throw new ArgumentException("Периодическая часть должна содержать хотя бы один элемент");
                
            _a0 = a0;
            _period = periodicPart.Length;
            _getAn = n => periodicPart[(n - 1) % periodicPart.Length];
        }
        
        /// <summary>
        /// Возвращает начальный элемент
        /// </summary>
        public int InitialElement => _a0;
        
        /// <summary>
        /// Возвращает период (если дробь периодическая)
        /// </summary>
        public int? Period => _period;
        
        /// <summary>
        /// Получает n-й элемент цепной дроби
        /// </summary>
        /// <param name="n">Индекс элемента (начиная с 1)</param>
        public int GetElement(int n)
        {
            if (n == 0)
                return _a0;
            return _getAn(n);
        }
        
        /// <summary>
        /// Возвращает приближение бесконечной цепной дроби, используя указанное число элементов
        /// </summary>
        /// <param name="elementsCount">Количество элементов для приближения</param>
        public ContinuedFraction GetApproximation(int elementsCount)
        {
            if (elementsCount <= 0)
                throw new ArgumentException("Количество элементов должно быть положительным");
                
            int[] elements = new int[elementsCount];
            elements[0] = _a0;
            
            for (int i = 1; i < elementsCount; i++)
            {
                elements[i] = _getAn(i);
            }
            
            return new ContinuedFraction(elements);
        }
        
        /// <summary>
        /// Вычисляет подходящую дробь для заданного числа элементов
        /// </summary>
        /// <param name="elementsCount">Количество элементов</param>
        public (BigInteger Numerator, BigInteger Denominator) GetConvergent(int elementsCount)
        {
            return GetApproximation(elementsCount).ToRational();
        }
        
        /// <summary>
        /// Возвращает десятичное приближение на основе указанного числа элементов
        /// </summary>
        /// <param name="elementsCount">Количество элементов для приближения</param>
        public double ToDouble(int elementsCount)
        {
            return GetApproximation(elementsCount).ToDouble();
        }
        
        /// <summary>
        /// Возвращает строковое представление бесконечной цепной дроби
        /// </summary>
        public override string ToString()
        {
            // Ограничим вывод первыми несколькими элементами
            const int MaxElements = 10;
            
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(_a0);
            sb.Append("; ");
            
            for (int i = 1; i <= MaxElements; i++)
            {
                sb.Append(_getAn(i));
                if (i < MaxElements)
                    sb.Append(", ");
            }
            
            sb.Append(", ...]");
            
            if (_period.HasValue)
            {
                sb.Append(" (период ");
                sb.Append(_period.Value);
                sb.Append(")");
            }
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Создает бесконечную цепную дробь для числа √n
        /// </summary>
        /// <param name="n">Число под корнем</param>
        public static InfiniteContinuedFraction CreateForSquareRoot(int n)
        {
            if (n <= 0)
                throw new ArgumentException("Число под корнем должно быть положительным");
                
            if (IsSquare(n))
                throw new ArgumentException($"Число {n} является точным квадратом");
                
            // Начальное значение - целая часть √n
            int a0 = (int)Math.Floor(Math.Sqrt(n));
            
            // Для периодической цепной дроби √n используем алгоритм вычисления
            // Начальные значения
            int m0 = 0;
            int d0 = 1;
            int a = a0;
            
            List<int> periodicPart = new List<int>();
            
            // Множество для обнаружения периода
            HashSet<(int, int, int)> visited = new HashSet<(int, int, int)>();
            
            int m = m0;
            int d = d0;
            
            while (true)
            {
                m = d * a - m;
                d = (n - m * m) / d;
                a = (a0 + m) / d;
                
                // Запоминаем элемент периодической части
                periodicPart.Add(a);
                
                // Проверяем, не появлялся ли уже такой набор (m, d, a)
                var state = (m, d, a);
                if (visited.Contains(state))
                    break;
                    
                visited.Add(state);
            }
            
            return new InfiniteContinuedFraction(a0, periodicPart.ToArray());
        }
        
        // Проверяет, является ли число точным квадратом
        private static bool IsSquare(int n)
        {
            int sqrt = (int)Math.Sqrt(n);
            return sqrt * sqrt == n;
        }
    }
} 