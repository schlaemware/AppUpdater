using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW.AppUpdater.Abstractions.Models;

namespace SW.AppUpdater.Abstractions.Contracts {
  public interface IAppUpdater {
    public event EventHandler<Release> ReleaseFound;

    public void CheckForUpdates(string organization, string application, Version? installedVersion, bool includePreReleases);
  }
}
