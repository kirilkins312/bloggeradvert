namespace Instadvert.CZ.Data.ViewModels
{
    public class BlogVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public IFormFile CoverPhoto { get; set; }
        public string CoverImageUrl { get; set; }

    }
}
