using Microsoft.AspNetCore.Http;

namespace Services.Notifications.Application.BusinessLogics.Emails;
public class SendEmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile>? Attachments { get; set; }
    public SendEmailRequest()
    {
        
    }
    public SendEmailRequest(string toMail, string subject, string body)
    {
        ToEmail = toMail;
        Subject = subject;
        Body = body;
    }
}
