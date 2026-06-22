namespace KissImageConverter.Core;

public class FileItem
{
    public string FilePath { get; set; }
    public string Status { get; set; } = string.Empty;

    public FileItem(string filePath)
    {
        FilePath = filePath;
    }
}