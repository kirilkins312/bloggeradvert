using Instadvert.CZ.Models;

namespace Instadvert.CZ.Data.ViewModels
{
    public class FilterVM
    {

        public string? searchString {  get; set; }
        public int? searchStringBottom { get; set; }
        public int? searchStringTop { get; set; }

        public List<string> userRoles { get; set; }
        public List<BloggerUser> blogUsers { get; set; }
        public List<CompanyUser> compUsers { get; set; }
        public List<int>? SelectedCategories { get; set; }
        public virtual List<Message>? Messages { get; set; }
        public ICollection<Category>? CategoryList { get; set; }
    }
}
