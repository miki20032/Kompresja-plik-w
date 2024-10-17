using System;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.Win32;

namespace szyfrowanie_pliku
{
    public partial class MainWindow : Window
    {
        private string customSavePath = null;
        private string selectedFilePath = null; // Przechowywanie ścieżki wybranego pliku

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                selectedFilePath = openFileDialog.FileName;
                FileNameText.Text = $"Wybrany plik: {Path.GetFileName(selectedFilePath)}";
                FilePathText.Text = $"Ścieżka: {Path.GetDirectoryName(selectedFilePath)}";
            }
        }

        private void ChooseLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                customSavePath = dialog.SelectedPath;
                MessageBox.Show($"Wybrana lokalizacja zapisu: {customSavePath}");
            }
        }

        private void CompressButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath))
            {
                MessageBox.Show("Najpierw wybierz plik do kompresji.");
                return;
            }

            string compressedPath = selectedFilePath + ".zip";

            // Użycie wybranej przez użytkownika ścieżki zapisu, jeśli została ustawiona
            if (!string.IsNullOrEmpty(customSavePath))
            {
                compressedPath = Path.Combine(customSavePath, Path.GetFileName(compressedPath));
            }

            // Zmiana nazwy pliku na nową nazwę z TextBox
            string newFileName = NewFileNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                compressedPath = Path.Combine(Path.GetDirectoryName(compressedPath), newFileName + ".zip");
            }

            try
            {
                using (FileStream originalFileStream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                using (FileStream compressedFileStream = File.Create(compressedPath))
                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                {
                    originalFileStream.CopyTo(compressionStream);
                }

                // Pobranie rozmiaru pliku
                FileInfo fileInfo = new FileInfo(compressedPath);
                string fileInfoText = $"Nazwa Pliku: {fileInfo.Name}\nLokalizacja: {fileInfo.DirectoryName}\nRozmiar: {fileInfo.Length / 1024.0:F2} KB";
                FileInfoText.Text = fileInfoText;

                MessageBox.Show("Kompresja zakończona sukcesem!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas kompresji: " + ex.Message);
            }
        }
    }
}
