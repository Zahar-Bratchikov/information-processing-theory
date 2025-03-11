using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace FileCompressor
{
    public partial class MainWindow : Window
    {
        // Параметры алгоритма LZ77: устанавливаем большие значения для повторяющегося текста
        const int WindowSize = 4096; // Размер окна (window) - определяет, насколько далеко назад в тексте мы будем искать совпадения
        const int MinMatchLength = 4;   // Минимальная длина совпадения (match), которую мы будем учитывать
        const int MaxMatchLength = 255; // Максимальная длина совпадения, которую мы можем закодировать

        // Кодировка UTF-8 для корректной обработки русских символов
        private static readonly Encoding Utf8Encoding = Encoding.UTF8;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для записи логов в окно LogTextBox и в отладочный вывод
        private void Log(string message)
        {
            Debug.WriteLine(message); // Вывод в отладочную консоль Visual Studio
            LogTextBox.AppendText(message + Environment.NewLine); // Добавление сообщения в окно LogTextBox
        }

        // Обработчик нажатия кнопки "Browse" - открывает диалог выбора файла
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog(); // Создаем новый диалог открытия файла
            if (dlg.ShowDialog() == true) // Открываем диалог и проверяем, что пользователь выбрал файл
            {
                FilePathTextBox.Text = dlg.FileName; // Отображаем путь к выбранному файлу в TextBox
            }
        }

        // Обработчик нажатия кнопки "Compress" - сжимает выбранный файл
        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text; // Получаем путь к файлу из TextBox
            string outputName = OutputFileNameTextBox.Text; // Получаем имя выходного файла из TextBox
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputName)) // Проверяем, что путь к файлу и имя выходного файла указаны
            {
                MessageBox.Show("Please select a file and specify an output file name.");
                return;
            }

            string compressedPath = Path.ChangeExtension(outputName, ".lza"); // Формируем путь к сжатому файлу, добавляя расширение .lza
            try
            {
                string input = File.ReadAllText(filePath, Utf8Encoding); // Читаем содержимое файла, используя кодировку UTF-8
                Compress(input, compressedPath); // Вызываем метод сжатия
                MessageBox.Show($"File compressed to {compressedPath}"); // Отображаем сообщение об успешном сжатии

                // Выводим размеры файлов для сравнения
                FileInfo originalFile = new FileInfo(filePath); // Создаем объект FileInfo для исходного файла
                FileInfo compressedFile = new FileInfo(compressedPath); // Создаем объект FileInfo для сжатого файла
                Log($"Original size: {originalFile.Length} bytes"); // Выводим размер исходного файла
                Log($"Compressed size: {compressedFile.Length} bytes"); // Выводим размер сжатого файла
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during compression: " + ex.Message); // Отображаем сообщение об ошибке
            }
        }

        // Обработчик нажатия кнопки "Decompress" - разжимает выбранный файл
        private void DecompressButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text; // Получаем путь к файлу из TextBox
            string outputName = OutputFileNameTextBox.Text; // Получаем имя выходного файла из TextBox
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputName)) // Проверяем, что путь к файлу и имя выходного файла указаны
            {
                MessageBox.Show("Please select a file and specify an output file name.");
                return;
            }
            if (!filePath.EndsWith(".lza")) // Проверяем, что файл имеет расширение .lza
            {
                MessageBox.Show("Selected file is not a valid compressed file (.lza).");
                return;
            }
            try
            {
                string decompressed = Decompress(filePath); // Вызываем метод разжатия
                File.WriteAllText(outputName, decompressed, Utf8Encoding); // Записываем разжатое содержимое в файл, используя кодировку UTF-8
                MessageBox.Show($"File decompressed to {outputName}"); // Отображаем сообщение об успешном разжатии
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during decompression: " + ex.Message); // Отображаем сообщение об ошибке
            }
        }

        // Метод Compress - реализует алгоритм сжатия LZ77
        private void Compress(string input, string outputFilePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create), Utf8Encoding)) // Создаем BinaryWriter для записи сжатых данных в файл, используя кодировку UTF-8
            {
                int pos = 0; // Текущая позиция во входной строке
                while (pos < input.Length) // Пока не дошли до конца входной строки
                {
                    int matchLength = 0; // Длина найденного совпадения
                    int matchOffset = 0; // Смещение найденного совпадения
                    int windowStart = Math.Max(0, pos - WindowSize); // Определяем начало окна поиска (не может быть меньше 0)
                    for (int j = windowStart; j < pos; j++) // Ищем совпадение в окне поиска
                    {
                        int length = 0; // Длина текущего совпадения
                        while (pos + length < input.Length && // Пока не дошли до конца входной строки
                               (j + length) < pos && // Пока не вышли за пределы окна
                               input[j + length] == input[pos + length]) // Пока символы совпадают
                        {
                            length++; // Увеличиваем длину текущего совпадения
                            if (length == MaxMatchLength) // Если достигли максимальной длины, прекращаем поиск
                                break;
                        }
                        if (length > matchLength) // Если нашли более длинное совпадение, запоминаем его
                        {
                            matchLength = length; // Запоминаем длину совпадения
                            matchOffset = pos - j; // Запоминаем смещение
                        }
                    }

                    if (matchLength >= MinMatchLength) // Если длина совпадения больше или равна минимальной
                    {
                        writer.Write((byte)1); // Записываем флаг, указывающий на то, что это указатель (pointer)
                        writer.Write((ushort)matchOffset); // Записываем смещение

                        // Кодируем длину монотонным кодом и записываем
                        List<byte> monotoneCode = EncodeMonotone(matchLength);
                        foreach (byte b in monotoneCode)
                        {
                            writer.Write(b);
                        }

                        Log($"Ptr: pos={pos}, offset={matchOffset}, length={matchLength}"); // Записываем информацию в лог
                        pos += matchLength; // Перемещаем текущую позицию на длину совпадения
                    }
                    else // Если длина совпадения меньше минимальной
                    {
                        writer.Write((byte)0); // Записываем флаг, указывающий на то, что это литерал (literal)
                        writer.Write(input[pos]); // Записываем символ
                        Log($"Lit: pos={pos}, char='{input[pos]}'"); // Записываем информацию в лог
                        pos++; // Перемещаем текущую позицию на 1 символ
                    }
                }
            }
        }

        // Метод Decompress - реализует алгоритм разжатия LZ77
        private string Decompress(string inputFilePath)
        {
            StringBuilder output = new StringBuilder(); // Создаем StringBuilder для хранения разжатого текста
            using (BinaryReader reader = new BinaryReader(File.Open(inputFilePath, FileMode.Open), Utf8Encoding)) // Создаем BinaryReader для чтения сжатых данных из файла, используя кодировку UTF-8
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length) // Пока не дошли до конца файла
                {
                    byte flag = reader.ReadByte(); // Читаем флаг
                    if (flag == 0) // Если флаг равен 0, это литерал
                    {
                        char ch = reader.ReadChar(); // Читаем символ
                        output.Append(ch); // Добавляем символ в выходную строку
                        Log($"Decomp Lit: '{ch}'"); // Записываем информацию в лог
                    }
                    else if (flag == 1) // Если флаг равен 1, это указатель
                    {
                        ushort offset = reader.ReadUInt16(); // Читаем смещение

                        // Декодируем длину из монотонного кода
                        int length = DecodeMonotone(reader);

                        int start = output.Length - offset; // Вычисляем позицию начала совпадения
                        if (start < 0) // Проверяем, что смещение не выходит за пределы выходной строки
                        {
                            throw new InvalidDataException("Invalid offset during decompression.");
                        }
                        string match = output.ToString(start, length); // Извлекаем совпадение из выходной строки
                        output.Append(match); // Добавляем совпадение в выходную строку
                        Log($"Decomp Ptr: offset={offset}, length={length}, extracted='{match}'"); // Записываем информацию в лог
                    }
                    else // Если флаг неизвестен
                    {
                        throw new InvalidDataException("Unknown flag in compressed data.");
                    }
                }
            }
            return output.ToString(); // Возвращаем разжатый текст
        }


        // Метод для кодирования числа i монотонным кодом
        private List<byte> EncodeMonotone(int i)
        {
            // Шаг 1: Преобразуем число i в двоичное представление (строку)
            string binary = Convert.ToString(i, 2);
            // Например: если i = 5, то binary = "101"

            // Шаг 2: Вычисляем длину унарной части монотонного кода
            // Длина унарной части равна длине двоичного представления + 1
            int unaryLength = binary.Length + 1;
            // Например: если binary = "101", то unaryLength = 4

            // Шаг 3: Создаем унарную часть монотонного кода
            // Унарная часть состоит из unaryLength - 1 единиц, за которыми следует ноль
            string unary = new string('1', unaryLength - 1) + '0';
            // Например: если unaryLength = 4, то unary = "1110"

            // Шаг 4: Создаем список байтов для хранения результата
            List<byte> result = new List<byte>();

            // Шаг 5: Преобразуем унарную часть в байты (0 и 1) и добавляем в список
            for (int j = 0; j < unary.Length; j++)
            {
                // Преобразуем символ '1' в байт 1 и символ '0' в байт 0
                result.Add((byte)(unary[j] - '0'));
                // Например: если unary[j] = '1', то добавляем байт 1 в result
            }

            // Шаг 6: Преобразуем двоичную часть в байты (0 и 1) и добавляем в список
            for (int j = 0; j < binary.Length; j++)
            {
                // Преобразуем символ '1' в байт 1 и символ '0' в байт 0
                result.Add((byte)(binary[j] - '0'));
                // Например: если binary[j] = '1', то добавляем байт 1 в result
            }

            // Шаг 7: Возвращаем список байтов, представляющий монотонный код числа i
            return result;
        }

        // Метод для декодирования числа из монотонного кода
        private int DecodeMonotone(BinaryReader reader)
        {
            // Шаг 1: Инициализируем переменную для хранения длины унарной части
            int unaryLength = 0;

            // Шаг 2: Читаем унарную часть из потока, пока не встретим 0
            while (true)
            {
                // Шаг 2.1: Проверяем, не достигнут ли конец потока
                if (reader.BaseStream.Position == reader.BaseStream.Length)
                {
                    // Если достигнут конец потока, значит, монотонный код неполный, выбрасываем исключение
                    throw new InvalidDataException("Incomplete monotone code.");
                }

                // Шаг 2.2: Читаем один байт из потока
                byte bit = reader.ReadByte();

                // Шаг 2.3: Проверяем значение байта
                if (bit == 0)
                {
                    // Если байт равен 0, значит, унарная часть закончилась, выходим из цикла
                    break;
                }
                else if (bit == 1)
                {
                    // Если байт равен 1, значит, это часть унарной части, увеличиваем ее длину
                    unaryLength++;
                }
                else
                {
                    // Если байт не равен 0 и не равен 1, значит, код поврежден, выбрасываем исключение
                    throw new InvalidDataException("Invalid bit in monotone code.");
                }
            }

            // Шаг 3: Длина двоичной части равна длине унарной части
            int binaryLength = unaryLength;

            // Шаг 4: Инициализируем строку для хранения двоичной части
            string binary = "";

            // Шаг 5: Читаем двоичную часть из потока
            for (int i = 0; i < binaryLength; i++)
            {
                // Шаг 5.1: Проверяем, не достигнут ли конец потока
                if (reader.BaseStream.Position == reader.BaseStream.Length)
                {
                    // Если достигнут конец потока, значит, монотонный код неполный, выбрасываем исключение
                    throw new InvalidDataException("Incomplete monotone code.");
                }

                // Шаг 5.2: Читаем один байт из потока
                byte bit = reader.ReadByte();

                // Шаг 5.3: Проверяем значение байта
                if (bit == 0 || bit == 1)
                {
                    // Если байт равен 0 или 1, добавляем его в двоичную строку
                    binary += bit;
                }
                else
                {
                    // Если байт не равен 0 и не равен 1, значит, код поврежден, выбрасываем исключение
                    throw new InvalidDataException("Invalid bit in monotone code.");
                }
            }

            // Шаг 6: Преобразуем двоичную строку в целое число
            if (binary.Length > 0)
            {
                // Если двоичная строка не пустая, преобразуем ее в число по основанию 2
                return Convert.ToInt32(binary, 2);
            }
            else
            {
                // Если двоичная строка пустая, значит, i = 0
                return 0;
            }
        }
    }
}