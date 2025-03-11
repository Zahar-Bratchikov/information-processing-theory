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
        // Параметры алгоритма LZ77:
        const int WindowSize = 4096;
        const int MinMatchLength = 4;
        const int MaxMatchLength = 255;

        // Кодировка UTF-8 для корректной обработки русских символов
        private static readonly Encoding Utf8Encoding = Encoding.UTF8;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Log(string message)
        {
            Debug.WriteLine(message);
            LogTextBox.AppendText(message + Environment.NewLine);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                FilePathTextBox.Text = dlg.FileName;
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;
            string outputName = OutputFileNameTextBox.Text;
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(outputName))
            {
                MessageBox.Show("Please select a file and specify an output file name.");
                return;
            }

            string compressedPath = Path.ChangeExtension(outputName, ".lza");
            try
            {
                string input = File.ReadAllText(filePath, Utf8Encoding);
                MessageBox.Show($"File compressed to {compressedPath}");

                // Выводим размеры файлов для сравнения
                FileInfo originalFile = new FileInfo(filePath);
                FileInfo compressedFile = new FileInfo(compressedPath);
                Log($"Original size: {originalFile.Length} bytes");
                Log($"Compressed size: {compressedFile.Length} bytes");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during compression: " + ex.Message);
            }
        }

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
                File.WriteAllText(outputName, decompressed, Utf8Encoding);
                MessageBox.Show($"File decompressed to {outputName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during decompression: " + ex.Message);
            }
        }

        private void Compress(string input, string outputFilePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFilePath, FileMode.Create), Utf8Encoding))
            {
                int pos = 0;
                while (pos < input.Length)
                {
                    int matchLength = 0;
                    int matchOffset = 0;
                    int windowStart = Math.Max(0, pos - WindowSize);
                    for (int j = windowStart; j < pos; j++)
                    {
                        int length = 0;
                        while (pos + length < input.Length &&
                               (j + length) < pos &&
                               input[j + length] == input[pos + length])
                        {
                            length++;
                            if (length == MaxMatchLength)
                                break;
                        }
                        if (length > matchLength)
                        {
                            matchLength = length;
                            matchOffset = pos - j;
                        }
                    }

                    if (matchLength >= MinMatchLength)
                    {
                        writer.Write((byte)1);
                        writer.Write((ushort)matchOffset);
                        writer.Write((byte)matchLength);
                        Log($"Ptr: pos={pos}, offset={matchOffset}, length={matchLength}");
                        pos += matchLength;
                    }
                    else
                    {
                        writer.Write((byte)0);
                        writer.Write(input[pos]);
                        Log($"Lit: pos={pos}, char='{input[pos]}'");
                        pos++;
                    }
                }
            }
        }

        private string Decompress(string inputFilePath)
        {
            StringBuilder output = new StringBuilder();
            using (BinaryReader reader = new BinaryReader(File.Open(inputFilePath, FileMode.Open), Utf8Encoding)) // Указываем кодировку
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    byte flag = reader.ReadByte();
                    if (flag == 0)
                    {
                        char ch = reader.ReadChar();
                        output.Append(ch);
                        Log($"Decomp Lit: '{ch}'");
                    }
                    else if (flag == 1)
                    {
                        ushort offset = reader.ReadUInt16();
                        int length = reader.ReadByte();
                        int start = output.Length - offset;
                        if (start < 0)
                        {
                            throw new InvalidDataException("Invalid offset during decompression.");
                        }
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