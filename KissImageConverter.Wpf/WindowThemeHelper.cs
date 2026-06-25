using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KissImageConverter.Wpf;

public static class WindowThemeHelper
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int attribute,
        ref int pvAttribute,
        int cbAttribute);

    public static void UseDarkMode(Window window)
    {
        if (window == null) return;

        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero) return;
        int darkMode = 1;

        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
    }

    public static void ApplySystemTheme(Window window)
    {
        if (window == null) return;

        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero) return;
        int darkMode = IsSystemDarkMode() ? 1 : 0;

        DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkMode, sizeof(int));
    }

    private static bool IsSystemDarkMode()
    {
        const string key = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        using var regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(key);
        return (int?)regKey?.GetValue("AppsUseLightTheme") == 0;
    }
}