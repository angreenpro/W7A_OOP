/**
Nguyen Phuc Truong An
202419022
*/
using LabW07.Enums;

namespace LabW07.Models;

public class ProjectorRemote : TeachingDevice
{
    public const int MinBatteryLevel = 20;

    public int BatteryLevel { get; }

    public ProjectorRemote(string id, string name, string storageLocation, int batteryLevel)
        : base(id, name, storageLocation)
    {
        BatteryLevel = ValidatePercentage(batteryLevel, nameof(batteryLevel));
    }

    public override bool IsReadyForUse()
        => Status == DeviceStatus.Available && BatteryLevel >= MinBatteryLevel;

    public override double GetDefaultPriorityScore()
        => BatteryLevel;
}
