using Furni.Api.Entities;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController : ControllerBase
	{
		private readonly IGenericRepository<Product> _userRepository;

		public ProductController(IGenericRepository<Product> userRepository)
		{
			_userRepository = userRepository;
		}
		[HttpPost]
		[Route("Add")]
		public void Add(Product user)
		{
			_userRepository.Add(user);
		}
		[HttpGet]
		[Route("GetAll")]
		public List<Product> GetAll()
		{
			var data = _userRepository.GetAll().ToList();
			foreach (var item in data)
			{
				item.StringId = item._id.ToString();
			}
			return data;
		}

		[HttpGet]
		[Route("Get")]
		public async Task<Product> Get(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));
			if (data != null)
			{
				data.StringId = data._id.ToString();
			}
			return data ?? new Product();
		}

		[HttpDelete]
		[Route("Delete")]
		public void Delete(string id)
		{
			_userRepository.Delete(ObjectId.Parse(id));
		}

		[HttpPut]
		[Route("Update")]
		public void Update(Product user)
		{
			user._id = ObjectId.Parse(user.StringId);
			_userRepository.Update(user);
		}
	}
}
