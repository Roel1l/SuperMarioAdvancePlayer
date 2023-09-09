using ConsoleApp;
using ConsoleApp.Genetic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

[DllImport("User32.dll")]
static extern int SetForegroundWindow(IntPtr point);

if (Process.GetProcessesByName("VisualBoyAdvance-M").FirstOrDefault() is not Process gameboy)
{
    return;
}

App.GameBoyHandle = gameboy.MainWindowHandle;
SetForegroundWindow(App.GameBoyHandle);

var cancellationTokenSource = new CancellationTokenSource();

await new GeneticAlgorithm().ExecuteAsync(cancellationTokenSource.Token);

Console.ReadKey();

cancellationTokenSource.Cancel();

