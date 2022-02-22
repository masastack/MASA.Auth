namespace MASA.Auth.Sdk.ApiParameter.Subjects.Platform;

public class DeletePlatformRequest
{
    public Guid PlatformId { get; set; }

    public DeletePlatformRequest(Guid platformId)
    {
        PlatformId = platformId;
    }

    public static implicit operator DeletePlatformRequest(PlatformItemResponse request)
    {
        return new DeletePlatformRequest(request.Id);
    }
}

