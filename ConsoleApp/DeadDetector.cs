using AForge.Imaging;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ConsoleApp;

internal class DeadDetector
{
    private const float deathDetectionThreshold = 0.9f;

    private readonly int _gameboyWidth;
    private readonly int _gameboyHeight;
    private readonly int _gameboyX;
    private readonly int _gameboyY;

    private readonly Bitmap _deadMarioInLevel = new("Images/dead_mario_face.bmp");
    private readonly Bitmap _deadMarioInOverworld = new("Images/dead_mario_overworld.bmp");

    internal delegate void MarioDiedCallback();
    public event MarioDiedCallback? MarioDied;

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public DeadDetector()
    {
        var rect = new User32.Rect();
        User32.GetWindowRect(App.GameBoyHandle, ref rect);

        _gameboyWidth = rect.right - rect.left;
        _gameboyHeight = rect.bottom - rect.top;
        _gameboyX = rect.left;
        _gameboyY = rect.top;
    }

    public void Start()
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        _ = Task.Run(async () =>
        {
            while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                DetectIfMarioDied();
            }
        });
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
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

        ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(deathDetectionThreshold);

        if (tm.ProcessImage(sourceImage, _deadMarioInLevel).Any())
        {
            MarioDied?.Invoke();
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
