using Furni.UI.Entities;
using Furni.UI.Models;
using Furni.UI.Models.ViewModel.Admin.Coupons;
using Furni.UI.Models.ViewModel.Admin.Dishes;
using Furni.UI.Models.ViewModel.Admin.Emails;
using Furni.UI.Models.ViewModel.Admin.Orders;
using Furni.UI.Models.ViewModel.Admin.Users;
using Furni.UI.Models.ViewModel.Profile;
using Furni.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Furni.UI.Controllers
{
	public class AdminController : Controller
	{
		private readonly HttpClient _httpClient;
		public AdminController(IHttpClientFactory httpClientFactory)
		{
			_httpClient = httpClientFactory.CreateClient("APIHTTP");
		}


		public async Task<IActionResult> Users()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			UsersViewModel viewModel = new UsersViewModel();

			string url = "User/GetAll";
			viewModel.Users = await _httpClient.GetFromJsonAsync<List<User>>(url);
			viewModel.UserName = Request.Cookies["UserName"];

			return View(viewModel);
		}
		public IActionResult AddUser()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			UsersAddViewModel viewModel = new UsersAddViewModel();
			viewModel.UserName = Request.Cookies["UserName"];

			return View(viewModel);
		}

		[HttpPost]

		public async Task<IActionResult> AddUser(UsersAddViewModel viewModel)
		{
			bool number = Regex.IsMatch(viewModel.Number, RegexPatterns.PhoneNumberRegexPatten);
			bool password = Regex.IsMatch(viewModel.Password, RegexPatterns.PasswordRegexPattern);
			bool lastName = Regex.IsMatch(viewModel.LastName, RegexPatterns.NameRegexPattern);
			bool firstName = Regex.IsMatch(viewModel.FirstName, RegexPatterns.NameRegexPattern);

			viewModel.FirstNameMessage = string.Empty;
			viewModel.LastNameMessage = string.Empty;
			viewModel.NumberMessage = string.Empty;
			viewModel.EmailMessage = string.Empty;
			viewModel.PasswordMessage = string.Empty;


			string endpoint = $"User/GetByEmail?email={viewModel.Email}";
			var data = await _httpClient.GetFromJsonAsync<User>(endpoint);


			if (number && password && lastName && firstName && data.Email == null)
			{

				User user = new User(TypeUser.Client)
				{
					StringId = "",
					Email = viewModel.Email,
					Number = viewModel.Number,
					Password = viewModel.Password,
					FirstName = viewModel.FirstName,
					LastName = viewModel.LastName,
					Products = new List<string>(),
					Orders = new List<string>()
				};

				endpoint = "User/Add";

				var response = await _httpClient.PostAsJsonAsync(endpoint, user);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToAction("Users", "Admin");
				}

			}


			viewModel.NumberMessage = RegexPatterns.AddMessage(number, "Phone number must be 10-15 digits long and start with 0");
			viewModel.PasswordMessage = RegexPatterns.AddMessage(password, "Password must be 8+ chars and use EN lang, include uppercase and number");
			viewModel.FirstNameMessage = RegexPatterns.AddMessage(firstName, "First Name must be 2-13 chars");
			viewModel.LastNameMessage = RegexPatterns.AddMessage(lastName, "Last Name must be 2-13 chars");
			viewModel.EmailMessage = RegexPatterns.AddMessage(data.Email == null, "This email already register");

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> EditUser(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			var id = collection["userId"];
			string url = $"User/GetById?id={id}";
			var data = await _httpClient.GetFromJsonAsync<User>(url);


			UsersEditViewModel viewModel = new UsersEditViewModel()
			{
				UserName = Request.Cookies["UserName"],
				TypeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client,
				FirstName = data.FirstName,
				LastName = data.LastName,
				Email = data.Email,
				Number = data.Number,
				StringId = data.StringId,
				Password = data.Password

			};

			return View(viewModel);

		}

		[HttpPost]
		public async Task<IActionResult> UpdateUser(UsersEditViewModel viewModel)
		{

			viewModel.UserName = Request.Cookies["UserName"];
			viewModel.TypeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (viewModel.UserName == null)
			{
				return RedirectToAction("Login", "Account");
			}



			bool number = Regex.IsMatch(viewModel.Number, RegexPatterns.PhoneNumberRegexPatten);
			bool password = Regex.IsMatch(viewModel.Password, RegexPatterns.PasswordRegexPattern);
			bool lastName = Regex.IsMatch(viewModel.LastName, RegexPatterns.NameRegexPattern);
			bool firstName = Regex.IsMatch(viewModel.FirstName, RegexPatterns.NameRegexPattern);

			viewModel.FirstNameMessage = string.Empty;
			viewModel.LastNameMessage = string.Empty;
			viewModel.NumberMessage = string.Empty;
			viewModel.EmailMessage = string.Empty;
			viewModel.PasswordMessage = string.Empty;


			if (number && password && lastName && firstName)
			{
				var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;

				User user = new User(typeUser)
				{
					StringId = viewModel.StringId,
					Email = viewModel.Email,
					Number = viewModel.Number,
					Password = viewModel.Password,
					FirstName = viewModel.FirstName,
					LastName = viewModel.LastName,
					Products = new List<string>(),
					Orders = new List<string>()
				};

				var endpoint = "User/Update";

				var response = await _httpClient.PutAsJsonAsync(endpoint, user);

				if (response.IsSuccessStatusCode)
				{
					return RedirectToAction("Users", "Admin");
				}

			}

			viewModel.NumberMessage = RegexPatterns.AddMessage(number, "Phone number must be 10-15 digits long and start with 0");
			viewModel.PasswordMessage = RegexPatterns.AddMessage(password, "Password must be 8+ chars and use EN lang, include uppercase and number");
			viewModel.FirstNameMessage = RegexPatterns.AddMessage(firstName, "First Name must be 2-13 chars");
			viewModel.LastNameMessage = RegexPatterns.AddMessage(lastName, "Last Name must be 2-13 chars");

			return View(viewModel);
		}
		[HttpPost]

		public async Task<IActionResult> DeleteUser(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}


			var id = collection["userId"];
			var adminId = Request.Cookies["Login"];

			string url = $"User/Delete?id={id}";

			await _httpClient.DeleteAsync(url);

			if (id == adminId)
			{
				return RedirectToAction("Loguot", "Home");
			}
			return RedirectToAction("Users", "Admin");
		}
		public async Task<IActionResult> Products()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			ProductsViewModel viewModel = new ProductsViewModel();

			viewModel.UserName = Request.Cookies["UserName"];
			var url = "Product/GetAll";
			viewModel.Product = await _httpClient.GetFromJsonAsync<List<Product>>(url);

			return View(viewModel);
		}
		public IActionResult AddProduct()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			ProductAddViewModel viewModel = new ProductAddViewModel();
			viewModel.UserName = Request.Cookies["UserName"];

			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> AddProduct(ProductAddViewModel viewModel, IFormFile Image)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			if (Image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				Image.OpenReadStream().CopyTo(memoryStream);

				Product product = new Product()
				{
					Title = viewModel.Title,
					Price = viewModel.Price,
					Email = "",
					StringId = "",
					Image = Convert.ToBase64String(memoryStream.ToArray()),
					Count = 1
				};

				string url = "Product/Add";
				await _httpClient.PostAsJsonAsync(url, product);

				return RedirectToAction("Products", "Admin");
			}
			else
			{
				return View();
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditProduct(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			var id = collection["Id"];
			string url = $"Product/Get?id={id}";
			var data = await _httpClient.GetFromJsonAsync<Product>(url);

			ProductEditViewModel viewModel = new ProductEditViewModel()
			{
				Title = data.Title,
				Price = data.Price,
				Image = data.Image,
				UserName = Request.Cookies["UserName"],
				StringId = data.StringId
			};

			//viewModel.

			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> UpdateProduct(ProductEditViewModel viewModel, IFormFile Image)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			if (Image != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				Image.OpenReadStream().CopyTo(memoryStream);

				Product product = new Product()
				{
					Title = viewModel.Title,
					Price = viewModel.Price,
					Email = "",
					StringId = viewModel.StringId,
					Image = Convert.ToBase64String(memoryStream.ToArray()),
					Count = 1
				};

				string url = "Product/Update";
				await _httpClient.PutAsJsonAsync(url, product);

				return RedirectToAction("Products", "Admin");
			}
			else
			{
				return View(viewModel);
			}
		}

		[HttpPost]

		public async Task<IActionResult> DeleteProduct(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			var id = collection["Id"];

			string url = $"Product/Delete?id={id}";
			await _httpClient.DeleteAsync(url);

			return RedirectToAction("Products", "Admin");
		}
		public IActionResult EditOrder()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			return View();
		}

		public async Task<IActionResult> Orders()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			OrdersViewModel viewModel = new OrdersViewModel();
			viewModel.UserName = Request.Cookies["UserName"];

			var url = "Order/GetAll";
			viewModel.Orders = await _httpClient.GetFromJsonAsync<List<Order>>(url);

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteOrder(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			var id = collection["Id"];

			string url = $"Order/Delete?id={id}";
			await _httpClient.DeleteAsync(url);

			return RedirectToAction("Orders", "Admin");
		}

		public async Task<IActionResult> Coupons()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			CouponsViewModel viewModel = new CouponsViewModel();

			string url = "Coupon/GetAll";
			viewModel.Coupons = await _httpClient.GetFromJsonAsync<List<Coupon>>(url);
			viewModel.UserName = Request.Cookies["UserName"];

			return View(viewModel);
		}
		public IActionResult AddCoupon()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			CouponsAddViewModel viewModel = new CouponsAddViewModel();
			viewModel.UserName = Request.Cookies["UserName"];

			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> AddCoupon(CouponsAddViewModel viewModel)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			viewModel.UserName = Request.Cookies["UserName"];

			var procent = 100;
			if (viewModel.Procent > procent)
			{
				return View(viewModel);
			}

			Coupon coupon = new Coupon()
			{
				StringId = "",
				CouponCode = viewModel.Coupon,
				Procent = viewModel.Procent,
				Email = ""
			};

			string url = "Coupon/Add";
			await _httpClient.PostAsJsonAsync(url, coupon);


			return RedirectToAction("Coupons", "Admin");
		}
		[HttpPost]
		public async Task<IActionResult> DeleteCoupon(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			var id = collection["id"];
			string url = $"Coupon/Delete?id={id}";

			await _httpClient.DeleteAsync(url);

			return RedirectToAction("Coupons", "Admin");
		}


		public IActionResult SendMoreMessage()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			EmailDataViewModel viewModel = new EmailDataViewModel()
			{
				UserName = Request.Cookies["UserName"]
			};
			return View(viewModel);
		}

		public async Task<IActionResult> Messages()
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			string url = "Message/GetAll";
			var data = await _httpClient.GetFromJsonAsync<List<Message>>(url);

			MessagesViewModel viewModel = new MessagesViewModel()
			{
				Messages = data,
				UserName = Request.Cookies["UserName"]
			};


			return View(viewModel);
		}

		[HttpPost]
		public IActionResult SendAnswear(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			EmailDataViewModel viewModel = new EmailDataViewModel()
			{
				StringId = collection["Id"],
				Email = collection["Email"],
				UserName = Request.Cookies["UserName"]
			};

			return View(viewModel);
		}
		[HttpPost]
		public async Task<IActionResult> SendAnswearUser(EmailDataViewModel viewModel)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			EmailData model = new EmailData()
			{
				Email = viewModel.Email,
				Message = viewModel.Message,
				Subject = "Furni - We give you answear"
			};

			var url = "Email/SendOne";
			await _httpClient.PostAsJsonAsync<EmailData>(url, model);

			url = $"Message/Delete?id={viewModel.StringId}";
			await _httpClient.DeleteAsync(url);

			 return RedirectToAction("Messages", "Admin");
		}

		[HttpPost]
		public async Task<IActionResult> DeleteMessage(IFormCollection collection)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}

			var url = $"Message/Delete?id={collection["Id"]}";
			await _httpClient.DeleteAsync(url);

			return RedirectToAction("Messages", "Admin");
		}
		[HttpPost]
		public async Task<IActionResult> SendMoreMessage(EmailData model)
		{
			var typeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
			if (typeUser != TypeUser.Admin)
			{
				return RedirectToAction("Error", "Home");
			}
			model.Email = "";

			string url = "Email/SendMore";
			await _httpClient.PostAsJsonAsync(url, model);

			return RedirectToAction("Messages", "Admin");
		}
	}
}
