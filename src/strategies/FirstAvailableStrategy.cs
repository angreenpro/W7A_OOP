/**
Nguyen Phuc Truong An
202419022
*/

using LabW07.Exceptions;
using LabW07.Interfaces;

namespace LabW07.Strategies;

public class FirstAvailableStrategy<T> : ISelectionStrategy<T>
{
    public T Select(IReadOnlyList<T> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        if (candidates.Count == 0)
        {
            throw new NoSuitableItemException("Danh sách ứng viên rỗng, không có phần tử đầu tiên để chọn.");
        }

        return candidates[0];
    }

    public override string ToString() => "FirstAvailableStrategy";
}
