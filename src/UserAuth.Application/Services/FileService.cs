using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using UserAuth.Application.Contracts;
using UserAuth.Core.Enums;
using UserAuth.Core.Extensions;
using UserAuth.Core.Settings;

namespace UserAuth.Application.Services;

public class FileService : IFileService
{
    private readonly AppSettings _appSettings;
    private readonly UploadSettings _uploadSettings;

    public FileService(IOptions<AppSettings> appSettings, IOptions<UploadSettings> uploadSettings)
    {
        _appSettings = appSettings.Value;
        _uploadSettings = uploadSettings.Value;
    }

    public async Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath,
        EPathAccess pathAccess = EPathAccess.Private, int urlLimitLength = 255)
    {
        var fileName = GenerateNewFileName(arquivo.FileName, pathAccess, uploadPath, urlLimitLength);
        var filePath = MountFilePath(fileName, pathAccess, uploadPath);

        try
        {
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }
        catch (DirectoryNotFoundException)
        {
            var file = new FileInfo(filePath);
            file.Directory?.Create();
            await File.WriteAllBytesAsync(filePath, ConvertFileInByteArray(arquivo));
        }

        return GetFileUrl(fileName, pathAccess, uploadPath);
    }

    public string ObterPath(Uri uri)
    {
        return GetFilePath(uri);
    }

    public bool Apagar(Uri uri)
    {
        try
        {
            var filePath = GetFilePath(uri);

            new FileInfo(filePath).Delete();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private string GenerateNewFileName(string fileName, EPathAccess pathAccess, EUploadPath uploadPath, int limit = 255)
    {
        var guid = Guid.NewGuid().ToString("N");
        var newFileName = fileName.Replace("-", "");
        var url = GetFileUrl($"{guid}_{newFileName}", pathAccess, uploadPath);

        if (url.Length <= limit)
        {
            return newFileName;
        }

        var remove = url.Length - limit;
        newFileName = newFileName.Remove(newFileName.Length - remove - Path.GetExtension(newFileName).Length, remove);

        return $"{guid}_{newFileName}";
    }

    private string MountFilePath(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        var path = pathAccess == EPathAccess.Private ? _uploadSettings.PrivateBasePath : _uploadSettings.PublicBasePath;
        return Path.Combine(path, uploadPath.ToDescriptionString(), fileName);
    }

    private string GetFileUrl(string fileName, EPathAccess pathAccess, EUploadPath uploadPath)
    {
        return Path.Combine(_appSettings.UrlApi, pathAccess.ToDescriptionString(), uploadPath.ToDescriptionString(),
            fileName);
    }

    private string GetBaseFileUrl(EPathAccess pathAccess, EUploadPath uploadPath)
    {
        return Path.Combine(_appSettings.UrlApi, pathAccess.ToDescriptionString(), uploadPath.ToDescriptionString());
    }

    private string GetFilePath(Uri uri)
    {
        var filePath = uri.AbsolutePath;
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var pathAccessStr = filePath.Split("/")[1];
        var pathAccess = Enum.Parse<EPathAccess>(pathAccessStr, true);
        filePath = filePath.Remove(0, pathAccess.ToDescriptionString().Length);
        if (filePath.StartsWith("/"))
        {
            filePath = filePath.Remove(0, 1);
        }

        var basePath = pathAccess == EPathAccess.Private
            ? _uploadSettings.PrivateBasePath
            : _uploadSettings.PublicBasePath;

        return Path.Combine(basePath, filePath);
    }

    private static byte[] ConvertFileInByteArray(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}