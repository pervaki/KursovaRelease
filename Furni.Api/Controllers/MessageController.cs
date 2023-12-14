using Furni.Api.Entities;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessageController : ControllerBase
	{
		private readonly IGenericRepository<Message> _userRepository;

		public MessageController(IGenericRepository<Message> userRepository)
		{
			_userRepository = userRepository;
		}
		[HttpPost]
		[Route("Add")]
		public void Add(Message user)
		{
			_userRepository.Add(user);
		}
		[HttpGet]
		[Route("GetAll")]
		public List<Message> GetAll()
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
		public async Task<Message> Get(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));
			return data ?? new Message();
		}

		[HttpDelete]
		[Route("Delete")]
		public void Delete(string id)
		{
			_userRepository.Delete(ObjectId.Parse(id));
		}

		[HttpPut]
		[Route("Update")]
		public void Update(Message user)
		{
			_userRepository.Update(user);
		}
	}
}
