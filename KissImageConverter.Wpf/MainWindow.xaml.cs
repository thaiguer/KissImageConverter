using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace KissImageConverter.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        TextBlockVersion.Text =
            $"{ApplicationInfo.GetApplicationVersion()}";

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

            if (ListBoxFiles.Items.Count == 0)
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
        finally
        {
            ProgressBar.IsIndeterminate = false;
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

        foreach (string file in files)
        {
            if (!Path.GetExtension(file)
                .Equals(".heic", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!ListBoxFiles.Items.Contains(file))
            {
                ListBoxFiles.Items.Add(file);
            }
        }

        e.Handled = true;
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        ListBoxFiles.Items.Clear();
    }

    private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new VistaFolderBrowserDialog();

        if (Directory.Exists(TextBoxDestino.Text))
        {
            dialog.SelectedPath = TextBoxDestino.Text;
        }

        if (dialog.ShowDialog(this) == true)
        {
            TextBoxDestino.Text = dialog.SelectedPath;
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