using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KissImageConverter.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        ProgressBar.IsIndeterminate = true;

        try
        {
            string destinationFolder = TextBoxDestino.Text.Trim();

            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                MessageBox.Show(
                    "Informe a pasta de destino.",
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Directory.CreateDirectory(destinationFolder);

            await Task.Run(() =>
            {
                var imageConverter = new Core.ImageConverter();

                foreach (string sourceFile in ListBoxFiles.Items)
                {
                    string targetFile = Path.Combine(
                        destinationFolder,
                        Path.GetFileNameWithoutExtension(sourceFile) + ".jpg");

                    imageConverter.ConvertHeicToJpg(sourceFile, targetFile);
                }
            });

            MessageBox.Show(
                "Conversion finished.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        ProgressBar.IsIndeterminate = false;
    }

    private void DropArea_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effects = DragDropEffects.Copy;
        else
            e.Effects = DragDropEffects.None;
    }

    private void DropArea_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        foreach (string file in files)
        {
            if (Path.GetExtension(file)
                    .Equals(".heic", StringComparison.OrdinalIgnoreCase))
            {
                ListBoxFiles.Items.Add(file);
            }
        }
    }
}