namespace Masa.Stack.Components.Configs;

internal class ProjectAppOptions
{
    public MasaStackProject Project { get; init; }

    public string? ServiceVersion { get; init; }

    public ProjectAppOptions(MasaStackProject project, string? serviceVersion = null)
    {
        Project = project;
        ServiceVersion = serviceVersion;
    }
}
