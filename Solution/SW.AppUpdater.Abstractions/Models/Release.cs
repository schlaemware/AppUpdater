namespace SW.AppUpdater.Abstractions.Models {
  public readonly struct Release {
    public bool HasInstaller { get; init; }
    public int ID { get; init; }
    public bool IsPreRelease { get; init; }
    public string Name { get; init; }
    public string Organization { get; init; }
    public long RepositoryID { get; init; }
    public Version Version { get; init; }
  }
}
