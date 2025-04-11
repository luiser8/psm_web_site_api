namespace psm_web_site_api_project.Repository.ImageUpAndDown;
public interface IImageUpAndDownService
{
    Task<(byte[] content, string contentType)> SelectImageUpAndDownService(string imageName, string elementKey, string? extensionOrSede);
    Task<string> PostImageUpAndDownService(IFormFile? image, string elementKey, string? extensionOrSede);
    Task<string> PutImageUpAndDownService(IFormFile? image, string elementKey, string? extensionOrSede);
    Task<bool> DeleteImageUpAndDownService(string nameImage, string elementKey, string? extensionOrSede);
}