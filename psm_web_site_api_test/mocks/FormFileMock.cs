using System.Text;

namespace psm_web_site_api_test.mocks;

public class FormFileMock : IFormFile
{
    private readonly MemoryStream _stream;
    private readonly string _contentType;
    private readonly string _fileName;
    private readonly IHeaderDictionary _headers;
    private readonly string _contentDisposition;

    public FormFileMock(string content, string fileName, string contentType)
    {
        if (string.IsNullOrEmpty(content))
            content = string.Empty;

        _stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        _fileName = fileName ?? "unnamed.file";
        _contentType = contentType ?? "application/octet-stream";
        _contentDisposition = $"form-data; name=\"file\"; filename=\"{_fileName}\"";

        _headers = new HeaderDictionary
        {
            {"Content-Type", _contentType},
            {"Content-Disposition", _contentDisposition}
        };
    }

    public string ContentType => _contentType;
    public string ContentDisposition => _contentDisposition;
    public IHeaderDictionary Headers => _headers;
    public long Length => _stream?.Length ?? 0;
    public string Name => "file";
    public string FileName => _fileName;

    public void CopyTo(Stream target)
    {
        if (_stream == null) throw new InvalidOperationException("Stream is null");
        _stream.CopyTo(target);
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        if (_stream == null) throw new InvalidOperationException("Stream is null");
        return _stream.CopyToAsync(target, cancellationToken);
    }

    public Stream OpenReadStream()
    {
        if (_stream == null) throw new InvalidOperationException("Stream is null");
        _stream.Position = 0;
        return _stream;
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }
}