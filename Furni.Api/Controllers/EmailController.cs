using Furni.Api.Email;
using Furni.Api.Entities;
using Furni.Api.Model;
using Furni.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Furni.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmailController : ControllerBase
	{
		private readonly IEmailSender _emailSender;
		private readonly IGenericRepository<AboutEmail> _userRepository;
        public EmailController(IEmailSender emailSender, IGenericRepository<AboutEmail> genericRepository)
        {
            _emailSender = emailSender;
			_userRepository = genericRepository;
        }

		[HttpPost]
		[Route("Add")]
		public void Add(AboutEmail user)
		{
			var data = GetAll();

			bool dataAlreadyRegister = false;
			foreach(var item in data)
			{
				if(item.Email == user.Email)
				{
					dataAlreadyRegister = true;
					break;
				}
			}

			if(!dataAlreadyRegister)
				_userRepository.Add(user);
		}
		[HttpGet]
		[Route("GetAll")]
		public List<AboutEmail> GetAll()
		{
			return _userRepository.GetAll().ToList() ?? new List<AboutEmail>();
		}

		[HttpGet]
		[Route("Get")]
		public async Task<AboutEmail> Get(string id)
		{
			var data = await _userRepository.Get(ObjectId.Parse(id));

			return data ?? new AboutEmail();
		}

		[HttpDelete]
		[Route("Delete")]
		public void Delete(string id)
		{
			_userRepository.Delete(ObjectId.Parse(id));
		}

		[HttpPut]
		[Route("Update")]
		public void Update(AboutEmail user)
		{
			_userRepository.Update(user);
		}
		[HttpPost]
		[Route("SendOne")]

		public async Task SendOne(EmailData data)
		{
			await _emailSender.SendEmailAsync(data.Email, data.Subject, data.Message);
		}

		[HttpPost]
		[Route("SendMore")]
		public async Task SendMore(EmailData data)
		{
			var emails = GetAll();

			foreach(var item in emails)
			{
				await _emailSender.SendEmailAsync(item.Email, data.Subject, data.Message);
			}
		}
    }
}
