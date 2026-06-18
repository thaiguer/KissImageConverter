using System.Reflection;

namespace KissImageConverter.Wpf;

internal class ApplicationInfo
{
    internal static string GetApplicationVersion()
    {
        return Assembly
            .GetExecutingAssembly()
            .GetName()
            .Version?
            .ToString() ?? string.Empty;
    }
}