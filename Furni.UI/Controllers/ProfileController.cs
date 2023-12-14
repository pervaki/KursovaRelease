using Furni.UI.Entities;
using Furni.UI.Models.ViewModel.Profile;
using Furni.UI.Settings;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Furni.UI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;
        public ProfileController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("APIHTTP");
        }
        public async Task<IActionResult> Orders()
        {
            FrofileOrdersViewModel viewModel = new FrofileOrdersViewModel();
            viewModel.UserName = Request.Cookies["UserName"];
            viewModel.TypeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client;
            if (viewModel.UserName == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var url = "Order/GetAll";
            var data = await _httpClient.GetFromJsonAsync<List<Order>>(url);

            viewModel.Orders = new List<Order>();

            var id = Request.Cookies["Login"];
            foreach(var item in data)
            {
                if(item.IdUser == id)
                {
                    viewModel.Orders.Add(item);
				}
            }

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

			return RedirectToAction("Orders", "Profile");
		}

		public async Task<IActionResult> Settings()
        {
    
            var id = Request.Cookies["Login"];

            if (id == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string url = $"User/GetById?id={id}";
            var data = await _httpClient.GetFromJsonAsync<User>(url);


            ProfileSettingsViewModel viewModel = new ProfileSettingsViewModel()
            {
                UserName = Request.Cookies["UserName"],
                TypeUser = Request.Cookies["UserType"] == "Admin" ? TypeUser.Admin : TypeUser.Client,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                Number = data.Number,
                StringId = data.StringId
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(ProfileSettingsViewModel viewModel)
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
                if (viewModel.Password == viewModel.PasswordConfirm)
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

                        CookieOptions cookie = new CookieOptions();
                        cookie.Expires = DateTime.Now.AddMinutes(60);

                        Response.Cookies.Append("Login", user.StringId, cookie);
                        Response.Cookies.Append("UserName", $"{user.FirstName} {user.LastName}", cookie);
                        Response.Cookies.Append("UserType", user.TypeUser.ToString(), cookie);

                        return RedirectToAction("Orders", "Profile");
                    }

                }
            }

            viewModel.NumberMessage = RegexPatterns.AddMessage(number, "Phone number must be 10-15 digits long and start with 0");
            viewModel.PasswordMessage = RegexPatterns.AddMessage(password, "Password must be 8+ chars and use EN lang, include uppercase and number");
            viewModel.FirstNameMessage = RegexPatterns.AddMessage(firstName, "First Name must be 2-13 chars");
            viewModel.LastNameMessage = RegexPatterns.AddMessage(lastName, "Last Name must be 2-13 chars");
			viewModel.PasswordMessage = RegexPatterns.AddMessage(viewModel.Password == viewModel.PasswordConfirm, " ") == "" ? viewModel.PasswordMessage : "Passwords do not match";

			return View(viewModel);
        }

    }
}
