namespace ConsoleApp.Genetic;

internal class Run
{
    private const int AmountOfActionsPerRun = 400;

    private bool Dead { get; set; }

    public Action[] Actions { get; set; }

    private Run() 
    { 
        Actions = new Action[AmountOfActionsPerRun];
    }
    
    private Run(Action[] actions)
    {
        Actions = actions;
    }

    public static Run FromRandom()
    {
        Console.WriteLine("Generating random run");
        var run = new Run();

        for (int i = 0; i < AmountOfActionsPerRun; i++)
        {
            run.Actions[i] = Action.FromRandom();
        }

        return run;
    }

    public static Run FromParents(Run mom, Run dad)
    { 
        Console.WriteLine($"Creating new run with mom, fitnessscore: {mom.Fitness()} and dad, fitnessscore: {dad.Fitness()}");
        var actions = new Action[AmountOfActionsPerRun];

        for (int i = 0; i < AmountOfActionsPerRun; i++)
        {
            var randomNumber = App.Random.Next(1, 101);

            if (randomNumber < 49)
            {
                actions[i] = Action.FromAction(mom.Actions[i]);
            }
            else if (randomNumber < 98)
            {
                actions[i] = Action.FromAction(dad.Actions[i]);
            }
            else
            {
                actions[i] = Action.FromRandom();
                Console.WriteLine($"A mutation occured! {actions[i].InputName} was generated");
            }
        }

        return new Run(actions);
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var deadDetector = new DeadDetector();
        deadDetector.MarioDied += DeadDetector_MarioDied;
        deadDetector.Start();

        for (int i = 0; i < AmountOfActionsPerRun && !cancellationToken.IsCancellationRequested; i++)
        {
            await Actions[i].ExecuteAsync(cancellationToken);

            if (Dead)
            {
                break;
            }
        }

        deadDetector.Stop();
        deadDetector.MarioDied -= DeadDetector_MarioDied;
    }

    private void DeadDetector_MarioDied()
    {
        Console.WriteLine("Detected dead mario!");
        Dead = true;
    }

    public int Fitness()
    {
        var fitness = 0;

        fitness -= Dead ? 5 : 0;
        fitness += Actions.Count(action => action.Executed);
        fitness += Actions.Count(action => action.IsWalkRightAction()) * 2;

        return fitness;
    }
}
