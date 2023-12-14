using Furni.Api.Entities;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IGenericRepository<Order> _userRepository;

		public OrderController(IGenericRepository<Order> userRepository)
		{
			_userRepository = userRepository;
		}
		[HttpPost]
		[Route("Add")]
		public void Add(Order user)
		{
			_userRepository.Add(user);
		}
		[HttpGet]
		[Route("GetAll")]
		public List<Order> GetAll()
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
		public async Task<Order> Get(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));
			if (data != null)
			{
				data.StringId = data._id.ToString();
			}
			return data ?? new Order();
		}

		[HttpDelete]
		[Route("Delete")]
		public void Delete(string id)
		{
			_userRepository.Delete(ObjectId.Parse(id));
		}

		[HttpPut]
		[Route("Update")]
		public void Update(Order user)
		{
			_userRepository.Update(user);
		}
	}
}
