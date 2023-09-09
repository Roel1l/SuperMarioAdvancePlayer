namespace ConsoleApp.Genetic;

internal class Action
{
    private int ActionLength { get; set; } = 100;
    private byte Input { get; init; }

    public string InputName { get; init; } = string.Empty;
    public bool Executed { get; private set; }

    private Action() { }

    public static Action FromRandom()
    {
        var randomNumber = App.Random.Next(1, 101);

        if (randomNumber < 30)
        {
            return new Action { Input = KeyConstants.Left, InputName = "Left" };
        }

        return randomNumber < 65 ?
            new Action { Input = KeyConstants.Right, InputName = "Right" } : 
            new Action { Input = KeyConstants.Jump, InputName = "Jump" };
    }

    public static Action FromAction(Action action)
    {
        return new Action { Input = action.Input, InputName = action.InputName };
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        //Console.WriteLine($"Holding {InputName} for {ActionLength} milliseconds");
        await App.HoldKeyAsync(Input, ActionLength, cancellationToken);
        Executed = true;
    }

    public bool IsWalkRightAction()
    {
        return Input == KeyConstants.Right;
    }
}
