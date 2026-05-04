namespace cinemaSystem.Interfaces
{
    namespace cinemaSystem.Interfaces
    {
        public interface IImageService
        {
            Task<string> SaveImageAsync(IFormFile file);
            void DeleteImage(string? imagePath);
        }
    }
    }
