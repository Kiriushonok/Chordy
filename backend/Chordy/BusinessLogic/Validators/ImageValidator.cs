using SixLabors.ImageSharp;

namespace Chordy.BusinessLogic.Validators
{
    public static class ImageValidator
    {
        private static readonly string[] AllowedExtensions = [ ".jpeg", ".jpg", ".png", ".webp" ];
        private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        private const long MaxFileSize = 2 * 1024 * 1024;
        public static void ValidateImage(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Файл не выбран");
            }
            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException("Размер файла превышает 2 МБ");
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Недопустимый формат изображения. Разрешены только" + string.Join(",", AllowedExtensions));
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                throw new ArgumentException("Недопустимый тип файла");
            }

            try
            {
                using var image = Image.Load(file.OpenReadStream());

                if (image.Width > 2000 || image.Height > 2000)
                {
                    throw new ArgumentException("Изображение слишком большое. Максимум 2000х2000 пикселей");
                }
            }
            catch
            {
                throw new ArgumentException("Файл не является корректным изображением.");
            }
        }
    }
}