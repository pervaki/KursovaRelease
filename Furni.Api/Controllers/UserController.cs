using Furni.Api.Entities;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IGenericRepository<User> _userRepository;

        public UserController(IGenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost]
		[Route ("Add")]
		public void Add(User user)
		{
			_userRepository.Add(user);
		}
        [HttpGet]
        [Route("GetAll")]
        public List<User> GetAll()
        {
            var data = _userRepository.GetAll().ToList();
            foreach (var item in data)
            {
                item.StringId = item._id.ToString();
            }

            return data;
        }

		[HttpGet]
		[Route("GetById")]
		public async Task<User> GetById(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));
			if (data != null)
			{
				data.StringId = data._id.ToString();
			}
			return data ?? new User(TypeUser.Client);
		}

		[HttpGet]
        [Route("GetByEmail")]
        public async Task<User> GetByEmail(string email)
        {
            var data = await _userRepository.Get(email);
            if (data != null)
            {
                data.StringId = data._id.ToString();
            }
            return data ?? new User(TypeUser.Client);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task Delete(string id)
        {
            await _userRepository.Delete(ObjectId.Parse(id));
        }

        [HttpPut]
        [Route("Update")]
        public void Update(User user)
        {
            user._id = ObjectId.Parse(user.StringId);
            _userRepository.Update(user);
        }
    }
}
