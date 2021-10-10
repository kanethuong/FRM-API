using System.Threading.Tasks;
using kroniiapi.DTO.Email;

namespace kroniiapi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailContent emailContent);
    }
}