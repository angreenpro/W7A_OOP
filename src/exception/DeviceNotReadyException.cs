/**
Nguyen Phuc Truong An
202419022
*/
using LabW07.Enums;

namespace LabW07.Exceptions;

public class DeviceNotReadyException : Exception
{
    public string DeviceId { get; }
    public DeviceStatus CurrentStatus { get; }

    public DeviceNotReadyException(string deviceId, DeviceStatus currentStatus, string message)
        : base(message)
    {
        DeviceId = deviceId;
        CurrentStatus = currentStatus;
    }
}
