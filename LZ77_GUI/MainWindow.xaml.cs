using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace FileCompressor
{
    public partial class MainWindow : Window
    {
        // Параметры алгоритма LZ77
        // WindowSize - максимальное количество символов, которые будут храниться в истории (скользящее окно)
        const int WindowSize = 100;
        // MinMatchLength - минимальная длина совпадения, при которой вместо литерала будет записываться указатель
        const int MinMatchLength = 3;
        // MaxMatchLength - максимальная длина совпадения, которую можно записать (соответствует 1 байту)
        const int MaxMatchLength = 255;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для логирования отладочной информации как в окно LogTextBox
        private void Log(string message)
        {
            Debug.WriteLine(message);
            LogTextBox.AppendText(message + Environment.NewLine);
        }

        // Обработчик кнопки Browse: позволяет выбрать исходный файл для сжатия/разжатия
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                FilePathTextBox.Text = dlg.FileName;
            }
        }

        // Обработчик кнопки Compress: читает входной файл, вызывает метод компрессии и сохраняет результат
        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;
            string outputName = OutputFileNameTextBox.Text;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputName))
            {
                MessageBox.Show("Please select a file and specify an output file name.");
                return;
            }

            // Задаём расширение .lza для сжатого файла
            string compressedPath = Path.ChangeExtension(outputName, ".lza");
            try
            {
                string input = File.ReadAllText(filePath);
                Compress(input, compressedPath);
                MessageBox.Show($"File compressed to {compressedPath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during compression: " + ex.Message);
            }
        }

        // Обработчик кнопки Decompress: декомпрессирует выбранный файл и сохраняет результат
        private void DecompressButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;
            string outputName = OutputFileNameTextBox.Text;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputName))
            {
                MessageBox.Show("Please select a file and specify an output file name.");
                return;
            }
            if (!filePath.EndsWith(".lza"))
            {
                MessageBox.Show("Selected file is not a valid compressed file (.lza).");
                return;
            }
            try
            {
                string decompressed = Decompress(filePath);
                File.WriteAllText(outputName, decompressed);
                MessageBox.Show($"File decompressed to {outputName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during decompression: " + ex.Message);
            }
        }

        // Метод Compress реализует алгоритм LZ77.
        // Он читает входной текст и для каждой позиции ищет наибольшее совпадение в пределах скользящего окна.
        // Если найдено совпадение длиной не менее MinMatchLength, записывается указатель с флагом 1, 
        // затем один байт смещения и один байт длины.
        // Если совпадение не удовлетворяет условию, записывается литерал с флагом 0 и символ.
        private void Compress(string input, string outputFilePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create)))
            {
                int pos = 0;
                // Проходим по всему входному тексту посимвольно
                while (pos < input.Length)
                {
                    int matchLength = 0;
                    int matchOffset = 0;
                    // Определяем границы окна: начиная от максимально возможного назад, но не меньше 0
                    int windowStart = Math.Max(0, pos - WindowSize);
                    // Поиск наибольшего совпадения в окне для текущей позиции
                    for (int j = windowStart; j < pos; j++)
                    {
                        int length = 0;
                        // Сравнение символов между окном и текущей позицией
                        // Цикл продолжается, пока символы совпадают и мы не выходим за конец входного текста
                        while (pos + length < input.Length &&
                               // Следующее условие гарантирует, что сравнение всегда происходит внутри окна
                               (j + length) < pos &&
                               input[j + length] == input[pos + length])
                        {
                            length++;
                            if (length == MaxMatchLength)
                                break;
                        }
                        // Если найденное совпадение длиннее предыдущего, сохраняем его параметры
                        if (length > matchLength)
                        {
                            matchLength = length;
                            // Смещение вычисляется как разница между текущей позицией и позицией начала совпадения
                            matchOffset = pos - j;
                        }
                    }

                    // Если найдены совпадения, удовлетворяющие минимальной длине, записываем указатель
                    if (matchLength >= MinMatchLength)
                    {
                        // Записываем токен: сначала флаг 1, затем смещение и длину
                        writer.Write((byte)1);
                        writer.Write((byte)matchOffset);
                        writer.Write((byte)matchLength);
                        Log($"Ptr: pos={pos}, offset={matchOffset}, length={matchLength}");
                        pos += matchLength;  // Пропускаем обработанные символы
                    }
                    else
                    {
                        // Если совпадение слишком короткое, записываем литерал, то есть сам символ
                        writer.Write((byte)0); // Флаг литерала
                        writer.Write((byte)input[pos]);
                        Log($"Lit: pos={pos}, char='{input[pos]}'");
                        pos++;
                    }
                }
            }
        }

        // Метод Decompress осуществляет восстановление данных.
        // Он читает по одному байту из сжатого файла.
        // Если флаг равен 0, читает литерал и добавляет его к выходной строке.
        // Если флаг равен 1, читает смещение и длину, затем копирует соответствующую последовательность символов из уже декомпрессированного текста.
        private string Decompress(string inputFilePath)
        {
            StringBuilder output = new StringBuilder();
            using (BinaryReader reader = new BinaryReader(File.Open(inputFilePath, FileMode.Open)))
            {
                // Читаем сжатые данные, пока не достигнем конца файла
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    byte flag = reader.ReadByte();
                    if (flag == 0)
                    {
                        // Литерал: читаем один байт и добавляем его как символ
                        char ch = (char)reader.ReadByte();
                        output.Append(ch);
                        Log($"Decomp Lit: '{ch}'");
                    }
                    else if (flag == 1)
                    {
                        // Указатель: читаем смещение и длину,
                        int offset = reader.ReadByte();
                        int length = reader.ReadByte();
                        // Вычисляем позицию в уже декомпрессированном тексте
                        int start = output.Length - offset;
                        if (start < 0)
                        {
                            throw new InvalidDataException("Invalid offset during decompression.");
                        }
                        // Извлекаем строку с найденным совпадением и дописываем её к выходному тексту
                        string match = output.ToString(start, length);
                        output.Append(match);
                        Log($"Decomp Ptr: offset={offset}, length={length}, extracted='{match}'");
                    }
                    else
                    {
                        throw new InvalidDataException("Unknown flag in compressed data.");
                    }
                }
            }
            return output.ToString();
        }
    }
}