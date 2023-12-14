using Furni.Api.Entities;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CouponController : ControllerBase
	{
		private readonly IGenericRepository<Coupon> _userRepository;

		public CouponController(IGenericRepository<Coupon> userRepository)
		{
			_userRepository = userRepository;
		}
		[HttpPost]
		[Route("Add")]
		public async Task Add(Coupon user)
		{
			await _userRepository.Add(user);
		}
		[HttpGet]
		[Route("GetAll")]
		public List<Coupon> GetAll()
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
		public async Task<Coupon> Get(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));
			if (data != null)
			{
				data.StringId = data._id.ToString();
			}
			return data ?? new Coupon();
		}

		[HttpDelete]
		[Route("Delete")]
		public void Delete(string id)
		{
			_userRepository.Delete(ObjectId.Parse(id));
		}

		[HttpPut]
		[Route("Update")]
		public void Update(Coupon user)
		{
			_userRepository.Update(user);
		}
	}
}
