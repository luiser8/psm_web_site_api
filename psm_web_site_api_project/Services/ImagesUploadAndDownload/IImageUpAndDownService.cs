namespace psm_web_site_api_project.Repository.ImageUpAndDown;
public interface IImageUpAndDownService
{
    Task<byte[]> SelectImageUpAndDownService(string idImage);
    Task<string> PostImageUpAndDownService(IFormFile image);
    Task<bool> PutImageUpAndDownService(string idImage, IFormFile image);
    Task<bool> DeleteImageUpAndDownService(string idImage);
}