using System.Runtime.InteropServices;
namespace ConsoleApp;

internal static class App
{
    [DllImport("user32.dll")]
    public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    const uint KEYEVENTF_KEYUP = 0x0002;
    const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

    public static IntPtr GameBoyHandle { get; set; }

    public static Random Random { get; } = new Random();

    public static Task HoldKeyAsync(byte key, int amountOfFrames = 1, CancellationToken cancellationToken = default) => 
        HoldKeysAsync(new [] { key }, amountOfFrames, cancellationToken);

    public static async Task HoldKeysAsync(byte[] keys, int amountOfFrames = 1, CancellationToken cancellationToken = default)
    {
        foreach (var key in keys)
        {
            PressKey(key);
        }

        await Task.Delay(amountOfFrames, cancellationToken);

        foreach (var key in keys)
        {
            ReleaseKey(key);
        }
    }

    public static void LoadSaveState()
    {
        SendKeys.SendWait(KeyConstants.F2);
    }

    private static void PressKey(byte key)
    {
        keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY, 0);
    }

    private static void ReleaseKey(byte key)
    {
        keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }
}
