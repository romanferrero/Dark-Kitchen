using DarkKitchen.IBusinessLogic.IServices;

namespace DarkKitchen.BusinessLogic.Services;

public sealed class ImageFileReader : IImageFileReader
{
    private const string JpegDataUriPrefix = "data:image/jpeg;base64,";

    public string ReadAsDataUri(string path)
    {
        try
        {
            var bytes = File.ReadAllBytes(path);
            return $"{JpegDataUriPrefix}{Convert.ToBase64String(bytes)}";
        }
        catch(Exception ex)
        {
            throw new ArgumentException($"Could not read image '{path}': {ex.Message}");
        }
    }
}
