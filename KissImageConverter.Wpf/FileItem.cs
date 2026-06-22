namespace KissImageConverter.Wpf;

public class FileItem : ObservableObject
{
    private string _filePath;
    private string _status;

    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public FileItem(string filePath)
    {
        _filePath = filePath;
        _status = string.Empty;
    }
}