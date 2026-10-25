/**
Nguyen Phuc Truong An
202419022
*/
namespace LabW07.Interfaces;

public interface ISelectionStrategy<T>
{
    T Select(IReadOnlyList<T> candidates);
}
