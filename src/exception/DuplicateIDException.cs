/**
Nguyen Phuc Truong An
202419022
*/
namespace LabW07.Exceptions;

public class DuplicateIdException : Exception
{
    public string DuplicateId { get; }

    public DuplicateIdException(string duplicateId)
        : base($"Mã '{duplicateId}' đã tồn tại trong kho.")
    {
        DuplicateId = duplicateId;
    }
}
