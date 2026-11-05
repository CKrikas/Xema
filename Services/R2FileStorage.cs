
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace xema.Services;

public class R2FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3;
    private readonly R2Options _options;

    public R2FileStorage(
        IAmazonS3 s3,
        IOptions<R2Options> options)
    {
        _s3 = s3;
        _options = options.Value;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileName,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };

        await _s3.PutObjectAsync(request, cancellationToken);

        return $"{_options.PublicUrl.TrimEnd('/')}/{fileName}";
    }

    public async Task DeleteAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var key = GetObjectKey(fileName);

        await _s3.DeleteObjectAsync(
            new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = key
            },
            cancellationToken);
    }

    private string GetObjectKey(string fileUrl)
    {
        if (!Uri.TryCreate(
                _options.PublicUrl,
                UriKind.Absolute,
                out var publicBase) ||
            !Uri.TryCreate(
                fileUrl,
                UriKind.Absolute,
                out var imageUri))
        {
            throw new InvalidOperationException(
                "The configured public URL or image URL is invalid.");
        }

        var basePath = publicBase.AbsolutePath.TrimEnd('/');

        if (!string.Equals(publicBase.Scheme, imageUri.Scheme,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(publicBase.Host, imageUri.Host,
                StringComparison.OrdinalIgnoreCase) ||
            publicBase.Port != imageUri.Port)
        {
            throw new InvalidOperationException(
                "The image URL does not belong to the configured media domain.");
        }

        var prefix = basePath + "/";

        if (!imageUri.AbsolutePath.StartsWith(
                prefix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "The image URL is outside the configured media path.");
        }

        var key = Uri.UnescapeDataString(
            imageUri.AbsolutePath[prefix.Length..]);

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Could not determine the R2 object key.");
        }

        return key;
    }
}
