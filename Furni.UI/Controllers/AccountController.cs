using Furni.UI.Entities;
using Furni.UI.Models.ViewModel.Account;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Furni.UI.Settings;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace Furni.UI.Controllers
{
	public class AccountController : Controller
	{
		private readonly HttpClient httpClient;
		public AccountController(IHttpClientFactory httpClientFactory)
		{
			httpClient = httpClientFactory.CreateClient("APIHTTP");
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(AccountRegisterViewModel viewModel)
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
			var data = await httpClient.GetFromJsonAsync<User>(endpoint);


			if (number && password && lastName && firstName && data.Email == null)
			{
				if (viewModel.Password == viewModel.PasswordConfirm)
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

					var response = await httpClient.PostAsJsonAsync(endpoint, user);

					if (response.IsSuccessStatusCode)
					{
						endpoint = $"User/GetByEmail?email={user.Email}";
						response = await httpClient.GetAsync(endpoint);
						data = await response.Content.ReadFromJsonAsync<User>();

						CookieOptions cookie = new CookieOptions();
						cookie.Expires = DateTime.Now.AddMinutes(60);

						Response.Cookies.Append("Login", data.StringId, cookie);
						Response.Cookies.Append("UserName", $"{data.FirstName} {data.LastName}", cookie);
						Response.Cookies.Append("UserType", data.TypeUser.ToString(), cookie);

						return RedirectToAction("Orders", "Profile");
					}

				}
			}

			viewModel.NumberMessage = RegexPatterns.AddMessage(number, "Phone number must be 10-15 digits long and start with 0");
			viewModel.PasswordMessage = RegexPatterns.AddMessage(password, "Password must be 8+ chars and use EN lang, include uppercase and number");
			viewModel.FirstNameMessage = RegexPatterns.AddMessage(firstName, "First Name must be 2-13 chars");
			viewModel.LastNameMessage = RegexPatterns.AddMessage(lastName, "Last Name must be 2-13 chars");
            viewModel.PasswordMessage = RegexPatterns.AddMessage(viewModel.Password == viewModel.PasswordConfirm, " ") == "" ? viewModel.PasswordMessage : "Passwords do not match";
            viewModel.EmailMessage = RegexPatterns.AddMessage(data.Email == null, "This email already register");

			return View(viewModel);
		}

		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(AccountLoginViewModel viewModel)
		{
			viewModel.EmailMessage = string.Empty;
			viewModel.PasswordMessage = string.Empty;


			string endpoint = $"User/GetByEmail?email={viewModel.Email}";
			var data = await httpClient.GetFromJsonAsync<User>(endpoint);


			if (data.Email == null)
			{
				viewModel.EmailMessage = "This email not register";

				return View(viewModel);
			}

			if (viewModel.Password == data.Password)
			{
				CookieOptions cookie = new CookieOptions();
				cookie.Expires = DateTime.Now.AddMinutes(60);

				Response.Cookies.Append("Login", data.StringId, cookie);
				Response.Cookies.Append("UserName", $"{data.FirstName} {data.LastName}", cookie);
				Response.Cookies.Append("UserType", data.TypeUser.ToString(), cookie);

				return RedirectToAction("Orders", "Profile");
			}

			viewModel.PasswordMessage = RegexPatterns.AddMessage(viewModel.Password == data.Password, "Wrong password");

			return View(viewModel);

		}

		[HttpPost]
		public IActionResult Logout()
		{
			Response.Cookies.Delete("Login");
			Response.Cookies.Delete("UserName");
			Response.Cookies.Delete("UserType");
			return Redirect("/");
		}

	}



}
