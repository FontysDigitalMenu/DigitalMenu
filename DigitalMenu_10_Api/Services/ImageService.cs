using DigitalMenu_20_BLL.Interfaces.Services;

namespace DigitalMenu_10_Api.Services;

public class ImageService(IWebHostEnvironment webHostEnvironment, ITimeService timeService)
{
    public async Task<string> SaveImageAsync(IFormFile menuItemFile)
    {
        ValidateImage(menuItemFile);
        return await GenerateImageNameAsync(menuItemFile);
    }

    private static void ValidateImage(IFormFile menuItemFile)
    {
        if (menuItemFile == null || menuItemFile.Length == 0)
        {
            throw new Exception("No file provided or file is empty.");
        }

        string extension = Path.GetExtension(menuItemFile.FileName).ToLower();
        if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
        {
            throw new Exception("Invalid file format. Only JPG, JPEG, PNG, and GIF files are allowed.");
        }
    }

    private async Task<string> GenerateImageNameAsync(IFormFile menuItemFile)
    {
        Guid myUuid = Guid.NewGuid();

        string menuItemImageName =
            new string(Path.GetFileNameWithoutExtension(menuItemFile.FileName).Take(10).ToArray()).Replace(' ', '-');
        menuItemImageName = menuItemImageName + timeService.GetNow().ToString("yymmssfff") + myUuid +
                            Path.GetExtension(menuItemFile.FileName);
        string imagePath = Path.Combine(webHostEnvironment.ContentRootPath, "api/Images", menuItemImageName);

        using (FileStream fileStream = new(imagePath, FileMode.Create))
        {
            await menuItemFile.CopyToAsync(fileStream);
        }

        return menuItemImageName;
    }
}