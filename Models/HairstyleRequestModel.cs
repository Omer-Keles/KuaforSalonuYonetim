public class HairstyleRequestModel
{
    public IFormFile ImageFile { get; set; }
    public string TextPrompt { get; set; }
    public string ResultImageUrl { get; set; }
}