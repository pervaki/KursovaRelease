using MimeKit;
using MimeKit.Text;
using RestSharp;
using System.Net;
using System.Net.Mail;

namespace Furni.Api.Email
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmailAsync(string email, string subject, string message)
		{
			var mail = "pervak222@gmail.com";
			var pw = "74007E1CDED615AFFE9BED476C4CA3B222BD";

			var client = new SmtpClient("smtp.elasticemail.com", 2525)
			{
				EnableSsl = true,
				Credentials = new NetworkCredential(mail, pw),
			};

			await client.SendMailAsync(
				new MailMessage(from: mail,
								to: email,
								subject,
								message));
		}
	}	
}

