namespace DentistQueue.Client.Services;

public interface IStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
    Task ClearAsync();
    Task<bool> ContainKeyAsync(string key);
}

public interface IFileStorageService
{
    Task<string> UploadFileAsync(byte[] fileData, string fileName, string contentType);
    Task<bool> DeleteFileAsync(string fileUrl);
    Task<byte[]?> DownloadFileAsync(string fileUrl);
}
