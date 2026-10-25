/**
Nguyen Phuc Truong An
202419022
*/

using LabW07.Enums;
using LabW07.Exceptions;
using LabW07.Interfaces;

namespace LabW07.Models;

public abstract class TeachingDevice : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string StorageLocation { get; }
    public DeviceStatus Status { get; protected set; }

    protected TeachingDevice(string id, string name, string storageLocation)
    {
        Id = ValidateText(id, nameof(id));
        Name = ValidateText(name, nameof(name));
        StorageLocation = ValidateText(storageLocation, nameof(storageLocation));
        Status = DeviceStatus.Available;
    }

    public abstract bool IsReadyForUse();

    public abstract double GetDefaultPriorityScore();

    public void Borrow()
    {
        if (!IsReadyForUse())
        {
            throw new DeviceNotReadyException(Id, Status,
                $"Thiết bị '{Id}' chưa sẵn sàng để cấp phát (trạng thái: {Status}).");
        }

        Status = DeviceStatus.Borrowed;
    }

    public void Return()
    {
        if (Status != DeviceStatus.Borrowed)
        {
            throw new DeviceNotReadyException(Id, Status,
                $"Thiết bị '{Id}' không ở trạng thái Borrowed nên không thể trả (trạng thái: {Status}).");
        }

        Status = DeviceStatus.Available;
    }

    public void SendToMaintenance()
    {
        if (Status != DeviceStatus.Available)
        {
            throw new DeviceNotReadyException(Id, Status,
                $"Chỉ có thể bảo trì thiết bị đang ở kho (trạng thái hiện tại: {Status}).");
        }

        Status = DeviceStatus.UnderMaintenance;
    }

    public void CompleteMaintenance()
    {
        if (Status != DeviceStatus.UnderMaintenance)
        {
            throw new DeviceNotReadyException(Id, Status,
                $"Thiết bị '{Id}' không trong trạng thái bảo trì (trạng thái hiện tại: {Status}).");
        }

        Status = DeviceStatus.Available;
    }

    public override string ToString()
        => $"[{GetType().Name}] {Id} - {Name} @ {StorageLocation} | {Status} | Score={GetDefaultPriorityScore():0.##}";

    // ---------- Helpers dùng chung cho lớp dẫn xuất ----------

    protected static string ValidateText(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Giá trị không được null hoặc rỗng.", paramName);
        }

        return value.Trim();
    }

    protected static int ValidatePercentage(int value, string paramName)
    {
        if (value < 0 || value > 100)
        {
            throw new ArgumentOutOfRangeException(paramName, value, "Giá trị phải nằm trong khoảng 0 đến 100.");
        }

        return value;
    }
}
