using System.Text.Json;

namespace ConsoleApp.Genetic;

internal class GeneticAlgorithm
{
    private const int AmountOfRunsPerGeneration = 10;
    private const bool LoadLatestGeneration = true;
    private const string fileName = "save.txt";

    public Run[] Runs { get; set; } = new Run[AmountOfRunsPerGeneration];

    public GeneticAlgorithm()
    {
        if (LoadLatestGeneration)
        {
            Load();
            return;
        }

        for (int i = 0; i < AmountOfRunsPerGeneration; i++) 
        {
            Runs[i] = Run.FromRandom();
        }

        Save();
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < AmountOfRunsPerGeneration && !cancellationToken.IsCancellationRequested; i++) 
            {
                App.LoadSaveState();
                await Runs[i].ExecuteAsync(cancellationToken);
            }

            GenerateNewGeneration();
            Save();
        }
    }
    
    private void GenerateNewGeneration()
    {
        var runsOrderedByFitness = Runs.OrderByDescending(run => run.Fitness()).ToList();

        var mom = runsOrderedByFitness[0];
        var dad = runsOrderedByFitness[1];

        var newGeneration = new Run[AmountOfRunsPerGeneration];

        for (int i = 0; i < AmountOfRunsPerGeneration; i++)
        {
            newGeneration[i] = Run.FromParents(mom, dad);
        }

        Runs = newGeneration;
    }

    private void Load()
    {
        string path = "C:\\Users\\roelg\\source\\repos\\ConsoleApp\\ConsoleApp\\Resources";
        string readText = File.ReadAllText($"{path}\\{fileName}");
        
        if (JsonSerializer.Deserialize<Run[]>(readText) is Run[] runs)
        {
            Runs = runs;
        }
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(Runs);
        var path = "C:\\Users\\roelg\\source\\repos\\ConsoleApp\\ConsoleApp\\Resources";
        File.WriteAllText($"{path}\\{fileName}", json);
    }
}
