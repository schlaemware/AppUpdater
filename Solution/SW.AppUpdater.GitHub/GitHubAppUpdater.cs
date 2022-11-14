using Octokit;
using SW.AppUpdater.Abstractions.Contracts;

namespace SW.AppUpdater.GitHub {
  public class GitHubAppUpdater : IAppUpdater {
    public event EventHandler<Abstractions.Models.Release>? ReleaseFound;

    public async void CheckForUpdates(string organization, string application, Version? installedVersion = null, bool includePreReleases = false) {
      GitHubClient client = new GitHubClient(new ProductHeaderValue(organization));
      IReadOnlyList<Repository> repositories = await client.Repository.GetAllForOrg(organization);
      if (repositories.FirstOrDefault(x => x.Name.ToLower() == application.ToLower()) is Repository repository) {
        IEnumerable<Release> releases = (await client.Repository.Release.GetAll(repository.Id)).Where(x => Version.TryParse(x.TagName, out Version? version)
          && (installedVersion == null || version > installedVersion) && (includePreReleases || !x.Prerelease));

        foreach (Release release in releases) {
          RaiseReleaseFound(new Abstractions.Models.Release() {
            Name = release.Name,
            Version = Version.Parse(release.TagName),
            IsPreRelease = release.Prerelease,
            HasInstaller = await HasInstallers(client, repository, release),
          });
        }
      }
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