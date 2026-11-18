namespace RussianNationalMessengerClient.Services.RNMApi;

public partial class ApiRNM
{
    public async Task<byte[]?> DownloadFile(string attachmentId)
    {
        try
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(5);

            var response = await _httpClient.GetAsync($"File/getfile/{attachmentId}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            return null;
        }
        finally
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
    }
    
    /*
    public async Task DownloadSaveBigFile(string messageId, string attachmentId, string filePath, IProgress<double>? progress = null, CancellationToken token = default)
    {
        var response = await _httpClient.GetAsync($"File/getfile/{attachmentId}", HttpCompletionOption.ResponseHeadersRead, token);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength;

        await using var contentStream = await response.Content.ReadAsStreamAsync(token);

        await using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true);

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, token)) > 0) 
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token);
            totalRead += bytesRead;

            if (totalBytes.HasValue)
            {
                double percent = totalRead / totalBytes.Value * 100;
                progress?.Report(Math.Round(percent, 2));
            }
        }
    }
    */
}
