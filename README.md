# Lab W07 – Quản Lý & Cấp Phát Thiết Bị Dạy Học

---

## 1. Quy tắc nghiệp vụ thiết bị

| Thiết bị | Điều kiện sẵn sàng (`IsReadyForUse`) | Điểm ưu tiên mặc định (`GetDefaultPriorityScore`) |
| :--- | :--- | :--- |
| **ProjectorRemote** | `Status == Available` && `BatteryLevel >= 20` | `BatteryLevel` |
| **AirConditionerRemote** | `Status == Available` | 100 nếu có phòng tương thích, ngược lại 0 |
| **WirelessMicrophone** | `Status == Available` && `BatteryLevel >= 30` && `SignalQuality >= 40` | `(BatteryLevel + SignalQuality) / 2.0` |

- **Mượn thiết bị (`Borrow`)**: Chỉ thành công khi `IsReadyForUse() == true`, trạng thái chuyển thành `Borrowed`.
- **Trả thiết bị (`Return`)**: Chỉ thành công khi đang ở trạng thái `Borrowed`, chuyển về `Available`.

---

## 2. Các thành phần Generic & Đa hình

- **`IRepository<T> where T : IIdentifiable`**:
  - Quản lý lưu trữ in-memory tổng quát, tra cứu theo `Id` qua ràng buộc `IIdentifiable`.
  - Không phụ thuộc vào kiểu thiết bị cụ thể.
- **`ISelectionStrategy<T>`**:
  - Áp dụng Strategy Pattern để chọn thiết bị tối ưu.
  - Hỗ trợ `HighestPriorityStrategy<T>` (theo điểm số) và `FirstAvailableStrategy<T>` (theo thứ tự đầu tiên).
- **`AllocationService<T> where T : TeachingDevice`**:
  - Điều phối cấp phát: lọc thiết bị sẵn sàng -> áp dụng điều kiện bổ sung -> chọn theo chiến lược -> gọi `Borrow()`.
  - Cho phép thay đổi chiến lược linh hoạt lúc runtime qua `ChangeStrategy()`.

---

---

## 3. Hướng dẫn chạy kiểm thử

```powershell
# Chạy chương trình
dotnet run
```
