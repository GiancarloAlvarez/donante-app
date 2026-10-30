namespace Donant_app.Services;

public static class PhotoStorage
{
    public static async Task<string?> PickAndSavePhotoAsync(string title)
    {
        IEnumerable<FileResult> results = await MediaPicker.Default.PickPhotosAsync(
            new MediaPickerOptions { Title = title });

        FileResult? selectedPhoto = results.FirstOrDefault();
        if (selectedPhoto is null)
        {
            return null;
        }

        string photosDirectory = Path.Combine(FileSystem.AppDataDirectory, "photos");
        Directory.CreateDirectory(photosDirectory);

        string extension = Path.GetExtension(selectedPhoto.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".jpg";
        }

        string destination = Path.Combine(photosDirectory, $"{Guid.NewGuid():N}{extension}");
        await using Stream source = await selectedPhoto.OpenReadAsync();
        await using FileStream target = File.Create(destination);
        await source.CopyToAsync(target);
        return destination;
    }
}
