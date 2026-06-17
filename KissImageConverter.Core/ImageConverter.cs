using ImageMagick;

namespace KissImageConverter.Core;

public class ImageConverter
{
    public void ConvertHeicToJpg(string sourcePath, string destinationPath)
    {
        using var image = new MagickImage(sourcePath);
        image.Quality = 90;
        image.Write(destinationPath, MagickFormat.Jpeg);
    }
}