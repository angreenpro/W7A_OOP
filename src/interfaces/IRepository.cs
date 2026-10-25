/**
Nguyen Phuc Truong An
202419022
*/
namespace LabW07.Interfaces;

public interface IRepository<T>
    where T : IIdentifiable
{
    void Add(T item);

    bool Remove(string id);

    T? FindById(string id);

    IReadOnlyList<T> GetAll();

    IReadOnlyList<T> FindAll(Predicate<T> condition);
}
