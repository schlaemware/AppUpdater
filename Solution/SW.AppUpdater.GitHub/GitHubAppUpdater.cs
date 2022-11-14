using Octokit;
using SW.AppUpdater.Abstractions.Contracts;
using SW.Framework.Extensions;

namespace SW.AppUpdater.GitHub {
  public class GitHubAppUpdater: IAppUpdater {
    public event EventHandler<Abstractions.Models.Release>? ReleaseFound;

    public async void CheckForUpdates(string organization, string application, Version? installedVersion = null, bool includePreReleases = false) {
      GitHubClient client = new GitHubClient(new ProductHeaderValue(organization));
      IReadOnlyList<Repository> repositories = await client.Repository.GetAllForOrg(organization);
      if (repositories.FirstOrDefault(x => x.Name.ToLower() == application.ToLower()) is Repository repository) {
        IEnumerable<Release> releases = (await client.Repository.Release.GetAll(repository.Id)).Where(x => Version.TryParse(x.TagName, out Version? version)
          && (installedVersion == null || version > installedVersion) && (includePreReleases || !x.Prerelease));

        foreach (Release release in releases) {
          RaiseReleaseFound(new Abstractions.Models.Release() {
            HasInstaller = await HasInstallers(client, repository, release),
            ID = release.Id,
            IsPreRelease = release.Prerelease,
            Name = release.Name,
            Organization = organization,
            RepositoryID = repository.Id,
            Version = Version.Parse(release.TagName),
          });
        }
      }
    }

    public async Task<string> LoadInstaller(Abstractions.Models.Release release) {
      GitHubClient client = new(new ProductHeaderValue(release.Organization));
      IReadOnlyList<ReleaseAsset> assets = await client.Repository.Release.GetAllAssets(release.RepositoryID, release.ID);

      string filePath = string.Empty;

      if (assets.FirstOrDefault(x => x.Name.EndsWith(".msi") || x.Name.EndsWith(".exe")) is ReleaseAsset asset) {
        filePath = $"C:\\Temporary\\{asset.Name}";
        using HttpClient httpClient = new();
        await httpClient.DownloadFileAsync(new Uri(asset.BrowserDownloadUrl), filePath);
      }

      return filePath;
    }

    private async Task<bool> HasInstallers(GitHubClient client, Repository repository, Release release) {
      IReadOnlyList<ReleaseAsset> assets = await client.Repository.Release.GetAllAssets(repository.Id, release.Id);
      return assets.Any(x => Path.GetExtension(x.Name).Contains("msi") || Path.GetExtension(x.Name).Contains("exe"));
    }

    private void RaiseReleaseFound(Abstractions.Models.Release release) {
      ReleaseFound?.Invoke(this, release);
    }
  }
}