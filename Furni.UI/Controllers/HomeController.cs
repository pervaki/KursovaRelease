using Furni.UI.Entities;
using Furni.UI.Models;
using Furni.UI.Models.ViewModel.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Furni.UI.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly HttpClient _httpClient;

		public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			_httpClient = httpClientFactory.CreateClient("APIHTTP");//httpClientFactory.CreateClient("API");
		}

		public async Task<IActionResult> Index()
		{
			HomeIndexViewModel viewModel = new HomeIndexViewModel();
			string url = "Product/GetAll";
			viewModel.Products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

			return View(viewModel);

		}

		public async Task<IActionResult> Shop()
		{
			HomeShopViewModel viewModel = new HomeShopViewModel();

			string url = "Product/GetAll";
			viewModel.Products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

			viewModel.TypeProduct = new();



			if (viewModel.Products != null && viewModel.Products.Count > 0)
			{
				foreach (var item in viewModel.Products)
				{
					viewModel.TypeProduct.Add(item.Title.Split(' ')[0]);
				}

				for (int i = 0; i < viewModel.TypeProduct.Count; i++)
				{
					for (int j = 1; j < viewModel.TypeProduct.Count - 1; j++)
					{
						if (viewModel.TypeProduct[i] == viewModel.TypeProduct[j + 1] && i != j + 1)
						{
							viewModel.TypeProduct.Remove(viewModel.TypeProduct[j + 1]);
						}
					}
				}
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Shop(IFormCollection collection)
		{
			var id = collection["Id"];
			var user = Request.Cookies["Login"];

			if (user == null)
			{
				return RedirectToAction("Register", "Account");
			}

			string type = collection["typeProduct"];
			if (!String.IsNullOrEmpty(type))
			{
				HomeShopViewModel viewModel = new HomeShopViewModel();

				string urls = "Product/GetAll";
				var datas = await _httpClient.GetFromJsonAsync<List<Product>>(urls);

				viewModel.TypeProduct = new();
				viewModel.Products = new();
				if (datas != null && datas.Count > 0)
				{
					for (var i = 0; i < datas.Count(); i++)
					{
						viewModel.TypeProduct.Add(datas[i].Title.Split(' ')[0]);
						viewModel.Products.Add(datas[i]);
					}

					for (int i = 0; i < viewModel.TypeProduct.Count; i++)
					{
						for (int j = 1; j < viewModel.TypeProduct.Count - 1; j++)
						{
							if (viewModel.TypeProduct[i] == viewModel.TypeProduct[j + 1] && i != j + 1)
							{
								viewModel.TypeProduct.Remove(viewModel.TypeProduct[j + 1]);
							}
						}
					}
				}

				for (int i = 0; i < viewModel.Products.Count; i++)
				{
					if (viewModel.Products[i].Title.Split(' ')[0] != type)
					{
						viewModel.Products.Remove(viewModel.Products[i]);
						i--;
					}

				}

				return View(viewModel);
			}
			string url = $"User/GetById?id={user}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);



			bool isIdInData = false;
			for (int i = 0; i < data.Products.Count(); i++)
			{
				if (data.Products == id)
				{
					isIdInData = true;
				}
			}
			if (!isIdInData)
			{
				data.Products.Add(id);
			}

			url = $"User/Update";
			await _httpClient.PutAsJsonAsync<User>(url, data);

			return RedirectToAction("Shop", "Home");
		}

		public IActionResult About()
		{

			return View();
		}

		public IActionResult Blog()
		{
			return View();
		}

		public async Task<IActionResult> Cart()
		{
			var id = Request.Cookies["Login"];
			if (id == null)
			{
				return RedirectToAction("Register", "Account");
			}
			HomeCartViewModel viewModel = new HomeCartViewModel();

			string url = $"User/GetById?id={id}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);

			url = "Product/GetAll";
			List<Product> products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

			viewModel.Products = new List<Product>();

			for (int i = 0; i < products.Count(); i++)
			{
				for (int j = 0; j < data.Products.Count(); j++)
				{
					if (products[i].StringId == data.Products[j])
					{
						viewModel.Products.Add(products[i]);
						break;
					}
				}
			}

			for (int i = 0; i < viewModel.Products.Count; i++)
			{
				viewModel.TotalPrice += viewModel.Products[i].Price;
				viewModel.Products[i].Count = 1;
			}
			viewModel.SubTotal = viewModel.TotalPrice;
			var procentForCoupon = Request.Cookies["Coupon"];

			if (procentForCoupon != null)
			{
				decimal proccent = (Decimal.Parse(procentForCoupon) / 100);
				viewModel.TotalPrice -= viewModel.TotalPrice * proccent;
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Cart(IFormCollection collection)
		{
			var id = Request.Cookies["Login"];
			if (id == null)
			{
				return RedirectToAction("Register", "Account");
			}

			string url = $"User/GetById?id={id}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);

			url = "Product/GetAll";
			List<Product> products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

			HomeCartViewModel viewModel = new HomeCartViewModel();
			viewModel.Products = new List<Product>();
			for (int i = 0; i < products.Count(); i++)
			{
				for (int j = 0; j < data.Products.Count(); j++)
				{
					if (products[i].StringId == data.Products[j])
					{
						viewModel.Products.Add(products[i]);
						break;
					}
				}
			}

			var idProduct = collection["Id"];
			var incOrDesc = Int32.Parse(collection["QuantityChange"]);
			var count = Int32.Parse(collection["count"]);

			foreach (var item in viewModel.Products)
			{
				item.Count = 1;
				if (item.StringId == idProduct && item.Count >= 1)
				{
					item.Count = count + incOrDesc;
				}

			}

			foreach (var item in viewModel.Products)
			{
				viewModel.TotalPrice += item.Price * item.Count;
			}
			viewModel.SubTotal = viewModel.TotalPrice;
			var procentForCoupon = Request.Cookies["Coupon"];

			if (procentForCoupon != null)
			{
				decimal proccent = (Decimal.Parse(procentForCoupon) / 100);
				viewModel.TotalPrice -= viewModel.TotalPrice * proccent;
			}
			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> RemoveProduct(IFormCollection collection)
		{
			var id = Request.Cookies["Login"];
			if (id == null)
			{
				return RedirectToAction("Register", "Account");
			}

			var idProduct = collection["Id"];

			string url = $"User/GetById?id={id}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);

			data.Products.Remove(idProduct);

			url = $"User/Update";
			await _httpClient.PutAsJsonAsync<User>(url, data);

			return RedirectToAction("Cart", "Home");
		}

		[HttpPost]
		public async Task<IActionResult> UseCoupon(IFormCollection collection)
		{
			var coupon = collection["Coupon"];
			var url = "Coupon/GetAll";
			var data = await _httpClient.GetFromJsonAsync<List<Coupon>>(url);

			foreach (var item in data)
			{
				if (item.CouponCode == coupon)
				{
					Response.Cookies.Append("Coupon", item.Procent.ToString(), new CookieOptions { Expires = DateTime.Now.AddHours(1) });

					url = $"Coupon/Delete?id={item.StringId}";
					_httpClient.DeleteAsync(url);
				}
			}
			return RedirectToAction("Cart");
		}


		public IActionResult Contact()
		{
			return View();
		}
		[HttpPost]

		public IActionResult Contact(IFormCollection collection)
		{
			Message model = new Message()
			{
				FirstName = collection["FirstName"],
				LastName = collection["LastName"],
				Email = collection["Email"],
				MessageAboutProblem = collection["Message"],
				StringId = ""
			};

			string url = "Message/Add";
			_httpClient.PostAsJsonAsync(url, model);

			return RedirectToAction("Contact");
		}
		public IActionResult Services()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ThankYou(IFormCollection collection)
		{
			var id = Request.Cookies["Login"];
			if (id == null)
			{
				return RedirectToAction("Login", "Account");
			}


			string url = $"User/GetById?id={id}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);

			string list = collection["Products"];
			if (list != null)
			{
				List<string> dataList = list.Split(',').ToList();
				if (data.Products.Count != 0)
				{
					Order order = new Order()
					{
						Email = "",
						IdUser = data.StringId,
						Products = data.Products,
						CountProduct = new List<int>(),
						Total = Decimal.Parse(collection["Total"]),
						StringId = ""
					};


					foreach (var item in dataList)
					{
						order.CountProduct.Add(Int32.Parse(item));
					}

					url = "Order/Add";
					var response = await _httpClient.PostAsJsonAsync(url, order);

					if (response.IsSuccessStatusCode)
					{
						data.Products = new List<string>();

						url = "User/Update";
						response = await _httpClient.PutAsJsonAsync(url, data);
					}

					Response.Cookies.Delete("Coupon");
					return View();
				}
			}
			return RedirectToAction("Shop", "Home");
		}
		public IActionResult CheckOut()
		{
			return View();
		}

		public IActionResult Setting()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SaveEmail(IFormCollection collection)
		{
			AboutEmail data = new AboutEmail()
			{
				Name = collection["Name"],
				Email = collection["Email"]
			};

			string url = "Email/Add";
			await _httpClient.PostAsJsonAsync(url, data);

			url = "Email/SendOne";

			EmailData model = new EmailData()
			{
				Email = data.Email,
				Message = $"Thank you {data.Name} for subscribing to newsletter",
				Subject = "Furni"

			};
			_httpClient.PostAsJsonAsync<EmailData>(url, model);

			return RedirectToAction("Index", "Home");
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public IActionResult SetLang(string culture, string returnUrl)
		{
			Response.Cookies.Append(
				CookieRequestCultureProvider.DefaultCookieName,
				CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			return LocalRedirect(returnUrl);
		}
	}
}