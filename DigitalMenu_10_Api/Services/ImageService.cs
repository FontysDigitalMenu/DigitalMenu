namespace DigitalMenu_10_Api.Services;

public class ImageService(IWebHostEnvironment webHostEnvironment)
{
    public async Task<string> SaveImageAsync(IFormFile menuItemFile)
    {
        ValidateImage(menuItemFile);
        return await GenerateThumbnailNameAsync(menuItemFile);
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

    private async Task<string> GenerateThumbnailNameAsync(IFormFile menuItemFile)
    {
        Guid myUuid = Guid.NewGuid();

        string menuItemImageName = new string(Path.GetFileNameWithoutExtension(menuItemFile.FileName).Take(10).ToArray()).Replace(' ', '-');
        menuItemImageName = menuItemImageName + DateTime.Now.ToString("yymmssfff") + myUuid + Path.GetExtension(menuItemFile.FileName);
        string thumbnailPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", menuItemImageName);

        using (FileStream fileStream = new(thumbnailPath, FileMode.Create))
        {
            await menuItemFile.CopyToAsync(fileStream);
        }

        return menuItemImageName;
    }
}
