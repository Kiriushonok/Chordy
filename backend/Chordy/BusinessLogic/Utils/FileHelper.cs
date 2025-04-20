namespace Chordy.BusinessLogic.Utils
{
    public static class FileHelper
    {
        public static async Task<string?> SaveAvatarAsync(IFormFile? file, string authorName, string folder = "wwwroot/avatars", CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            var safeName = string.Concat(authorName.Where(char.IsLetterOrDigit)).ToLower();
            var fileName = $"{safeName}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }
            // Возвращаем путь, по которому файл будет доступен из браузера
            return $"/avatars/{fileName}";
        }

        public static void DeleteAvatar(string? avatarPath, string folder = "wwwroot")
        {
            if (string.IsNullOrEmpty(avatarPath))
            {
                return;
            }

            var filePath = Path.Combine(folder, avatarPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
