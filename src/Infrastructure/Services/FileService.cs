using Application.Abstractions.Services;
using Domain.Exceptions.Files;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

internal class FileService : IFileService
{
    private const string MainFilePath = "/var/app/capstone-backend";

    public void Delete(string fileName)
    {
        string filePath = Path.Combine(MainFilePath, "Upload", "File", fileName);

        if (!File.Exists(filePath))
        {
            throw new Domain.Exceptions.Files.FileNotFoundException();
        }

        File.Delete(filePath);
    }

    public async Task<byte[]> GetFile(string fileName)
    {
        string filePath = Path.Combine(MainFilePath, "Upload", "File", fileName);

        if (!File.Exists(filePath))
        {
            throw new Domain.Exceptions.Files.FileNotFoundException();
        }

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

        return fileBytes;
    }

    public async Task<string> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new UploadFileException("File is null");
        }

        var fileName = string.Empty;
        try
        {
            var extension = Path.GetExtension(file.FileName);
            fileName = $"{DateTime.Now.Ticks}{extension}";

            var uploadPath = Path.Combine(MainFilePath, "Upload", "File");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new UploadFileException(ex.Message);
        }
    }
}
