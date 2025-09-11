namespace EMS.API.Services.Reports;

public interface IReportGenerator
{
    Task<byte[]> GenerateAsync();
    string GetContentType();
    string GetFileExtension();
}
