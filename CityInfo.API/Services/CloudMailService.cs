using System;
namespace CityInfo.API.Services
{
	public class CloudMailService : IMailService
    {
        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailFrom = configuration["mailSettings:mailFromAddress"];
            _mailTo = configuration["mailSettings:mailToAddress"];
        }

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}," +
                $"with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Subject: {message}");
        }
    }

}

