
using ConsoleApp;
using System.Diagnostics;
using System.Runtime.InteropServices;

[DllImport("User32.dll")]
static extern int SetForegroundWindow(IntPtr point);

if (Process.GetProcessesByName("VisualBoyAdvance-M").FirstOrDefault() is not Process gameboy)
{
    return;
}

SetForegroundWindow(gameboy.MainWindowHandle);

//App.LoadSaveState();

var cancellationToken = new CancellationTokenSource().Token;

var deadDetector = new DeadDetector(gameboy);
await deadDetector.StartAsync(cancellationToken);

//await App.HoldKeyForFramesAsync(KeyConstants.Right, 5000);

