/**
Nguyen Phuc Truong An
202419022
*/
using LabW07.Exceptions;
using LabW07.Interfaces;

namespace LabW07.Repositories;

public class InMemoryRepository<T> : IRepository<T>
    where T : IIdentifiable
{
    private readonly Dictionary<string, T> index = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<T> items = new();

    public int Count => items.Count;

    public void Add(T item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item), "Không được thêm phần tử null vào kho.");
        }

        if (index.ContainsKey(item.Id))
        {
            throw new DuplicateIdException(item.Id);
        }

        index.Add(item.Id, item);
        items.Add(item);
    }

    public bool Remove(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !index.Remove(id.Trim(), out T? removed))
        {
            return false;
        }

        items.Remove(removed!);
        return true;
    }

    public T? FindById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return default;
        }

        return index.TryGetValue(id.Trim(), out T? item) ? item : default;
    }

    public IReadOnlyList<T> GetAll()
    {
        return items.ToList().AsReadOnly();
    }

    public IReadOnlyList<T> FindAll(Predicate<T> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        return items.FindAll(condition).AsReadOnly();
    }
}
