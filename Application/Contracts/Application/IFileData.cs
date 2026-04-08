namespace Application.Contracts.Application
{
    public interface IFileData
    {
        string FileName { get; }
        string FileExtension { get; }
        string ContentType { get; }
        Stream FileStream { get; }
    }
}