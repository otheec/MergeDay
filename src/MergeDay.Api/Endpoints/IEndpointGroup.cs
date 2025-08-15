namespace MergeDay.Api.Endpoints;

public interface IEndpointGroup
{
    string GroupName { get; }
    void ConfigureGroup(RouteGroupBuilder group);
}
