using Furni.UI.Entities;

namespace Furni.UI.Models.ViewModel.Home
{
	public class HomeCartViewModel
	{
		public List<Product> Products { get; set; }
		public decimal TotalPrice { get; set; }
		public decimal SubTotal {  get; set; }
	}
}
