using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class LZWArchiver
{
    public void Compress(string inputFile, string outputFile)
    {
        string text = File.ReadAllText(inputFile);
        Dictionary<string, int> dictionary = new();
        for (int i = 0; i < 256; i++)
            dictionary.Add(((char)i).ToString(), i);

        List<int> compressed = new();
        string current = "";
        int dictSize = 256;

        foreach (char c in text)
        {
            string temp = current + c;
            if (dictionary.ContainsKey(temp))
            {
                current = temp;
            }
            else
            {
                compressed.Add(dictionary[current]);
                dictionary[temp] = dictSize++;
                current = c.ToString();
            }
        }
        if (!string.IsNullOrEmpty(current))
            compressed.Add(dictionary[current]);

        using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
        {
            writer.Write(compressed.Count);
            foreach (int code in compressed)
                writer.Write(code);
        }
    }

    public void Decompress(string inputFile, string outputFile)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            List<int> compressed = new();
            for (int i = 0; i < count; i++)
                compressed.Add(reader.ReadInt32());

            Dictionary<int, string> dictionary = new();
            for (int i = 0; i < 256; i++)
                dictionary.Add(i, ((char)i).ToString());

            string current = dictionary[compressed[0]];
            string result = current;
            int dictSize = 256;

            for (int i = 1; i < compressed.Count; i++)
            {
                string entry = dictionary.ContainsKey(compressed[i]) ? dictionary[compressed[i]] : current + current[0];
                result += entry;
                dictionary[dictSize++] = current + entry[0];
                current = entry;
            }

            File.WriteAllText(outputFile, result);
        }
    }
}

public class ArchiverForm : Form
{
    private Button compressButton, decompressButton;
    private OpenFileDialog openFileDialog;
    private SaveFileDialog saveFileDialog;
    private LZWArchiver archiver = new LZWArchiver();

    public ArchiverForm()
    {
        Text = "LZW Архиватор";
        Size = new System.Drawing.Size(300, 200);
        compressButton = new Button { Text = "Сжать", Left = 50, Top = 50, Width = 100 };
        decompressButton = new Button { Text = "Распаковать", Left = 150, Top = 50, Width = 100 };
        compressButton.Click += (s, e) => ProcessFile(true);
        decompressButton.Click += (s, e) => ProcessFile(false);
        Controls.Add(compressButton);
        Controls.Add(decompressButton);
        openFileDialog = new OpenFileDialog();
        saveFileDialog = new SaveFileDialog();
    }

    private void ProcessFile(bool isCompression)
    {
        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            saveFileDialog.FileName = isCompression ? "compressed.lzw" : "decompressed.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (isCompression)
                    archiver.Compress(openFileDialog.FileName, saveFileDialog.FileName);
                else
                    archiver.Decompress(openFileDialog.FileName, saveFileDialog.FileName);
                MessageBox.Show("Операция завершена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}