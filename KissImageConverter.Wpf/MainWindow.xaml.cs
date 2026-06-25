using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace KissImageConverter.Wpf;

public partial class MainWindow : Window
{
    public ObservableCollection<FileItem> FileItems { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        TextBlockVersion.Text =
            ApplicationInfo.GetApplicationVersion();

        TextBoxDestino.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "Converted");
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        ProgressBar.IsIndeterminate = true;

        try
        {
            string destinationFolder = TextBoxDestino.Text.Trim();

            if (FileItems.Count == 0)
            {
                MessageBox.Show(
                    "Please add at least one HEIC file.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            if (string.IsNullOrWhiteSpace(destinationFolder))
            {
                MessageBox.Show(
                    "Please select a destination folder.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Directory.CreateDirectory(destinationFolder);

            var tasks = FileItems.Select(sourceFile =>
            Task.Run(() =>
            {
                try
                {
                    var imageConverter = new Core.ImageConverter();

                    string targetFile = Path.Combine(
                        destinationFolder,
                        Path.GetFileNameWithoutExtension(sourceFile.FilePath) + ".jpg");

                    imageConverter.ConvertHeicToJpg(sourceFile.FilePath, targetFile);
                    sourceFile.Status = "Converted";
                }
                catch
                {
                    sourceFile.Status = "Error";
                }
            })
            );

            await Task.WhenAll(tasks);

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
        finally
        {
            ProgressBar.IsIndeterminate = false;
        }
    }

    private void AddFiles(IEnumerable<string> files)
    {
        foreach (string file in files)
        {
            if (!Path.GetExtension(file)
                .Equals(".heic", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var fileItem = new FileItem(file);

            if (!FileItems.Any(x =>
                string.Equals(x.FilePath,
                              fileItem.FilePath,
                              StringComparison.OrdinalIgnoreCase)))
            {
                FileItems.Add(fileItem);
            }
        }
    }

    private void DropArea_Click(object sender, MouseButtonEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select HEIC files",
            Filter = "HEIC files (*.heic)|*.heic",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            AddFiles(dialog.FileNames);
        }
    }

    private void DropArea_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void DropArea_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        e.Handled = true;
    }

    private void DropArea_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

        AddFiles(files);

        e.Handled = true;
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        FileItems.Clear();
    }

    private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();

        if (Directory.Exists(TextBoxDestino.Text))
        {
            dialog.InitialDirectory = TextBoxDestino.Text;
        }

        if (dialog.ShowDialog() == true)
        {
            TextBoxDestino.Text = dialog.FolderName;
        }
    }

    private void ButtonOpenFolder_Click(object sender, RoutedEventArgs e)
    {
        string folder = TextBoxDestino.Text.Trim();

        if (string.IsNullOrWhiteSpace(folder))
        {
            MessageBox.Show(
                "Please select a destination folder first.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        Directory.CreateDirectory(folder);

        Process.Start(new ProcessStartInfo
        {
            FileName = folder,
            UseShellExecute = true
        });
    }
}