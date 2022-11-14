namespace SW.AppUpdater.Abstractions.Models {
  public readonly struct Release {
    public bool HasInstaller { get; init; }
    public bool IsPreRelease { get; init; }
    public string Name { get; init; }
    public int ReleaseID { get; init; }
    public long RepositoryID { get; init; }
    public Version Version { get; init; }
  }
}
