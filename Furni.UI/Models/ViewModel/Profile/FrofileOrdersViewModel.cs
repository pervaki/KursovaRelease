using Furni.UI.Entities;

namespace Furni.UI.Models.ViewModel.Profile
{
    public class FrofileOrdersViewModel
    {
        public string UserName { get; set; }
        public TypeUser TypeUser { get; set; }
        public List<Order> Orders { get; set; }
    }
}
