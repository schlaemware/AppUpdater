using SW.AppUpdater.Abstractions.Contracts;
using SW.AppUpdater.GitHub;

internal class Program {
  private static void Main(string[] args) {
    string organization = "Schlaemware";
    string application = "TestRepo";

    Console.WriteLine($"AppUpdater on {organization}/{application}\n");

    IAppUpdater updater = new GitHubAppUpdater();
    updater.ReleaseFound += new EventHandler<SW.AppUpdater.Abstractions.Models.Release>((sender, release) => {
      string filePath = release.HasInstaller ? updater.LoadInstaller(release).Result : string.Empty;

      Console.WriteLine($"\tRelease {release.Version}");
      Console.WriteLine($"\t\tIs Prerelease: {release.IsPreRelease}");
      Console.WriteLine($"\t\tHas installer: {release.HasInstaller}");
      Console.WriteLine();

      if (release.HasInstaller) {
        Console.WriteLine($"\t\tLoad installer: {(File.Exists(filePath) ? "Success" : "Failed")}");
      } else {
        Console.WriteLine($"\t\tLoad installer: No installer...");
      }

      Console.WriteLine();
    });
    updater.CheckForUpdates(organization, application, null, true);

    Task.Delay(10000).Wait();
  }
}