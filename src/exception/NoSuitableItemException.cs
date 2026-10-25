/**
Nguyen Phuc Truong An
202419022
*/
namespace LabW07.Exceptions;

public class NoSuitableItemException : Exception
{
    public NoSuitableItemException()
        : base("Không có ứng viên phù hợp để lựa chọn.")
    {
    }

    public NoSuitableItemException(string message)
        : base(message)
    {
    }
}
