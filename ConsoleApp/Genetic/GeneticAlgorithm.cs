namespace ConsoleApp.Genetic;

internal class GeneticAlgorithm
{
    private readonly List<Run> _runs = new();

    public GeneticAlgorithm()
    {
        _runs.Add(Run.FromRandom());
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            App.LoadSaveState();

            await _runs[^1].ExecuteAsync(cancellationToken);

            GenerateNewRun();
        }
    }
    
    private void GenerateNewRun()
    {
        if (_runs.Count < 2)
        {       
            _runs.Add(Run.FromRandom());
            return;
        }

        var runsOrderedByFitness = _runs.OrderByDescending(run => run.Fitness()).ToList();

        var mom = runsOrderedByFitness[0];
        var dad = runsOrderedByFitness[1];

        _runs.Add(Run.FromParents(mom, dad));
    }
}
