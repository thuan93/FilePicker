using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Windows.Storage;

namespace FilePicker;


public partial class MainPage : ContentPage
{
    int count = 0;

    public User User { get; set; }

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(
                 new Dictionary<DevicePlatform, IEnumerable<string>>
                 {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/json" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".json" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
                 });

            PickOptions options = new()
            {
                PickerTitle = "Please select a comic file",
                FileTypes = customFileType,
            };
            var result = await PickAndShow(options);
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await Microsoft.Maui.Storage.FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    var text = await new StreamReader(stream).ReadToEndAsync();
                    var user = JsonConvert.DeserializeObject<User>(text);
                    User = user;
                    OnPropertyChanged(nameof(User));
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
#if ANDROID

        var downloadsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        var filePath = Path.Combine(downloadsPath, "file.json");
        using var writer = new StreamWriter(filePath, false);
        var user = JsonConvert.SerializeObject(User);
        await writer.WriteAsync(user);
#elif WINDOWS
        var downloadsFolder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Documents);
        var downloadsPath = downloadsFolder.SaveFolder.Path;
        var filePath = Path.Combine(downloadsPath, "file.json");
        using var writer = new StreamWriter(filePath, false);
        var user = JsonConvert.SerializeObject(User);
        await writer.WriteAsync(user);
#endif
    }
}

public class Cars
{
    public string Car1 { get; set; }
    public string Car2 { get; set; }
    public string Car3 { get; set; }
}

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Cars Cars { get; set; }
}
