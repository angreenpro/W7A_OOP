/**
Nguyen Phuc Truong An
202419022
*/

using LabW07.Exceptions;
using LabW07.Interfaces;

namespace LabW07.Strategies;

public class HighestPriorityStrategy<T> : ISelectionStrategy<T>
{
    private readonly Func<T, double> scoreSelector;

    public HighestPriorityStrategy(Func<T, double> scoreSelector)
    {
        this.scoreSelector = scoreSelector ?? throw new ArgumentNullException(nameof(scoreSelector));
    }

    public T Select(IReadOnlyList<T> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);

        if (candidates.Count == 0)
        {
            throw new NoSuitableItemException("Danh sách ứng viên rỗng, không thể chọn phần tử có điểm cao nhất.");
        }
        
        T best = candidates[0];
        double bestScore = scoreSelector(best);

        for (int i = 1; i < candidates.Count; i++)
        {
            double score = scoreSelector(candidates[i]);
            if (score > bestScore)
            {
                best = candidates[i];
                bestScore = score;
            }
        }

        return best;
    }

    public override string ToString() => "HighestPriorityStrategy";
}
