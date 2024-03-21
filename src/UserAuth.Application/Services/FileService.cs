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

    public async Task<string> Upload(IFormFile arquivo, EUploadPath uploadPath, EPathAccess pathAccess = EPathAccess.Private, int urlLimitLength = 255)
    {
        var fileName = GenerateNewFileName(arquivo.FileName, pathAccess, uploadPath, urlLimitLength);
    }

    public string ObterPath(Uri uri)
    {
        throw new NotImplementedException();
    }

    public bool Apagar(Uri uri)
    {
        throw new NotImplementedException();
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

        return Path.Combine(path, uploadPath.ToDescriptionString(),fileName);
    }
}