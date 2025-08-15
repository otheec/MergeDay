namespace MergeDay.Api.Endpoints;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointGroupAttribute(string groupName) : Attribute
{
    public string GroupName { get; } = groupName;
}
