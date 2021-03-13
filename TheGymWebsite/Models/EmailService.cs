using MailKit.Net.Smtp;
using MimeKit;

namespace TheGymWebsite.Models
{
    public class EmailService : IEmailService
    {
        public void Send(Email email)
        {
            // Setting up the email message.
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(email.Name, email.Address));
            emailMessage.To.Add(new MailboxAddress(email.Name, email.Address));

            emailMessage.Subject = email.Subject;

            // The email message is set to plain, ie, no attachments, etc.
            emailMessage.Body = new TextPart("plain")
            {
                Text = email.Message
            };

            using (SmtpClient client = new SmtpClient())
            {
                // Connecting to client.
                client.Connect("smtp.gmail.com", 587, false);

                // Google blocks access from less secure devices by default. Change this setting on this link:
                // https://myaccount.google.com/lesssecureapps 
                client.Authenticate(email.Address, email.Password);
                client.Send(emailMessage);
                client.Disconnect(true);
            }
        }
    }
}
