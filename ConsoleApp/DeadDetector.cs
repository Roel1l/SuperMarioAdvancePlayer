using AForge.Imaging;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApp;

internal class DeadDetector
{
    private const float deathDetectionThreshold = 0.9f;

    private readonly Process _gameboy;

    private readonly int _gameboyWidth;
    private readonly int _gameboyHeight;
    private readonly int _gameboyX;
    private readonly int _gameboyY;

    public DeadDetector(Process gameboy)
    {
        _gameboy = gameboy;

        var rect = new User32.Rect();
        User32.GetWindowRect(_gameboy.MainWindowHandle, ref rect);

        _gameboyWidth = rect.right - rect.left;
        _gameboyHeight = rect.bottom - rect.top;
        _gameboyX = rect.left;
        _gameboyY = rect.top;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            DetectIfMarioDied();
        }
    }

    private Bitmap TakeImageOfGameboy()
    {
        var bmp = new Bitmap(_gameboyWidth, _gameboyHeight, PixelFormat.Format24bppRgb);
        using (Graphics graphics = Graphics.FromImage(bmp))
        {
            graphics.CopyFromScreen(_gameboyX, _gameboyY, 0, 0, new Size(_gameboyWidth, _gameboyHeight), CopyPixelOperation.SourceCopy);
        }

        return bmp;
    }

    private void DetectIfMarioDied()
    {
        Bitmap sourceImage = TakeImageOfGameboy();
        Bitmap targetImage = new Bitmap("images/dead_mario_face.bmp");

        ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(deathDetectionThreshold);
        TemplateMatch[] matches = tm.ProcessImage(sourceImage, targetImage);

        if (matches.Any())
        {
            Console.WriteLine("Mario died!");
        }
    }

    private sealed class User32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
    }
}
