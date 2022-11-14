using SW.AppUpdater.Abstractions.Models;

namespace SW.AppUpdater.Abstractions.Contracts {
  public interface IAppUpdater {
    public event EventHandler<Release> ReleaseFound;

    public void CheckForUpdates(string organization, string application, Version? installedVersion, bool includePreReleases);

    public Task<string> LoadInstaller(Release release);
  }
}
