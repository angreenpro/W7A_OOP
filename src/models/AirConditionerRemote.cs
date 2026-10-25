/**
Nguyen Phuc Truong An
202419022
*/
using LabW07.Enums;

namespace LabW07.Models;

public class AirConditionerRemote : TeachingDevice
{
    private readonly HashSet<string> compatibleRooms;

    /// <summary>Tập phòng tương thích (chỉ đọc, so sánh không phân biệt hoa/thường).</summary>
    public IReadOnlySet<string> CompatibleRooms => compatibleRooms;

    public AirConditionerRemote(string id, string name, string storageLocation, IEnumerable<string> compatibleRooms)
        : base(id, name, storageLocation)
    {
        ArgumentNullException.ThrowIfNull(compatibleRooms);

        this.compatibleRooms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string room in compatibleRooms)
        {
            this.compatibleRooms.Add(ValidateText(room, nameof(compatibleRooms)));
        }
    }

    public bool IsCompatibleWith(string roomId)
        => !string.IsNullOrWhiteSpace(roomId) && compatibleRooms.Contains(roomId.Trim());

    public override bool IsReadyForUse()
        => Status == DeviceStatus.Available;

    public override double GetDefaultPriorityScore()
        => compatibleRooms.Count > 0 ? 100 : 0;

    /// <summary>Điểm ưu tiên theo phòng cụ thể: 100 nếu tương thích, ngược lại 0.</summary>
    public double GetPriorityScoreForRoom(string roomId)
        => IsCompatibleWith(roomId) ? 100 : 0;

    public override string ToString()
        => $"{base.ToString()} | Rooms={{{string.Join(", ", compatibleRooms)}}}";
}
