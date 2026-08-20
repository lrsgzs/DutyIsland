using DutyIsland.Shared;

namespace DutyIsland.Models.DataSettings;

public class GetWorkersDataSettings
{
    public string JobGuid { get; set; } = GlobalConstants.Guids.NullTemplate.ToString();
}