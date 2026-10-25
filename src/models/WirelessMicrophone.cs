/**
Nguyen Phuc Truong An
202419022
*/

using LabW07.Enums;

namespace LabW07.Models;

public class WirelessMicrophone : TeachingDevice
{
    public const int MinBatteryLevel = 30;
    public const int MinSignalQuality = 40;

    public int BatteryLevel { get; }
    public int SignalQuality { get; }

    public WirelessMicrophone(string id, string name, string storageLocation, int batteryLevel, int signalQuality)
        : base(id, name, storageLocation)
    {
        BatteryLevel = ValidatePercentage(batteryLevel, nameof(batteryLevel));
        SignalQuality = ValidatePercentage(signalQuality, nameof(signalQuality));
    }

    public override bool IsReadyForUse()
        => Status == DeviceStatus.Available
           && BatteryLevel >= MinBatteryLevel
           && SignalQuality >= MinSignalQuality;

    public override double GetDefaultPriorityScore()
        => (BatteryLevel + SignalQuality) / 2.0;
}
