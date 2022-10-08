// See https://aka.ms/new-console-template for more information
using ExtractStory;
using ExtractStory.Model;

Console.Title = "Extract Youtube Storyboards";

HTML _HTML = new();
Scrape manager = new();

while (true)
{
    var url = AskForUrl();
    Console.WriteLine();

    var results = await manager.Run(url);

    YoutubeModel input = CreateModel(url, results);

    _HTML.Write(input);

    AsktoExit();
}

void AsktoExit()
{
    Console.WriteLine($"{Environment.NewLine}");
    Console.WriteLine("Do you want to exit? yes/no (no)");

    var answer = Console.ReadLine() ?? String.Empty;

    if(answer != null && (answer.Trim().ToLower() == "yes" || answer.Trim().ToLower() == "y"))
    {
        Environment.Exit(0);
    }

    Console.WriteLine("...resuming...");
}

YoutubeModel CreateModel(string url, List<string> results)
{
    YoutubeModel input = new()
    {
        Url = url
    };

    int counter = 1;
    foreach (var result in results)
    {
        Image image = new()
        {
            Title = $"story-{counter}",
            Url = result
        };

        input.Images.Add(image);

        counter++;
    }

    return input;
}

static string AskForUrl()
{
    Console.WriteLine($"{Environment.NewLine}");
    Console.WriteLine("Enter Youtube embed url:");

    var url = Console.ReadLine() ?? String.Empty;
    
    return url;
}





