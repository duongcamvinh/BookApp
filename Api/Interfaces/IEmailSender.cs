using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string fromAddress,string toAddress,string subject,string messge);
    }
}
