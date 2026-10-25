/**
Nguyen Phuc Truong An
202419022
*/

using LabW07.Exceptions;
using LabW07.Interfaces;
using LabW07.Models;

namespace LabW07.Services;


public class AllocationService<T>
    where T : TeachingDevice
{
    private readonly IRepository<T> repository;
    private ISelectionStrategy<T> strategy;

    public ISelectionStrategy<T> CurrentStrategy => strategy;

    public AllocationService(IRepository<T> repository, ISelectionStrategy<T> strategy)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public void ChangeStrategy(ISelectionStrategy<T> newStrategy)
    {
        strategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
    }
    /// Cấp phát một thiết bị:
    /// - Lấy các đối tượng từ repository.
    /// - Chỉ giữ thiết bị có IsReadyForUse() == true (gọi đa hình).
    /// - Áp dụng additionalCondition nếu được cung cấp.
    /// - Dùng chiến lược hiện tại để chọn (gọi đa hình qua interface).
    /// - Gọi Borrow() trên thiết bị được chọn.
    /// - Trả về thiết bị đã cấp phát.
    /// <exception cref="NoSuitableItemException">Không có thiết bị nào thỏa điều kiện.</exception>
    public T Allocate(Predicate<T>? additionalCondition = null)
    {
        IReadOnlyList<T> candidates = repository.FindAll(device =>
            device.IsReadyForUse()
            && (additionalCondition is null || additionalCondition(device)));

        if (candidates.Count == 0)
        {
            throw new NoSuitableItemException(
                $"Không có thiết bị {typeof(T).Name} nào sẵn sàng và thỏa điều kiện yêu cầu.");
        }

        T selected = strategy.Select(candidates);
        selected.Borrow();
        return selected;
    }

    public T Return(string deviceId)
    {
        T device = repository.FindById(deviceId)
                   ?? throw new KeyNotFoundException($"Không tìm thấy thiết bị có mã '{deviceId}'.");

        device.Return();
        return device;
    }
}
