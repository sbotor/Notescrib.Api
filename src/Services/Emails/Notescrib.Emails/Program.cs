using System.Net;
using System.Net.Mail;

using var client = new SmtpClient("smtp.gmail.com")
{
    Port = 587, Credentials = new NetworkCredential("shymonbot@gmail.com", ""), EnableSsl = true
};

client.Send("shymonbot@gmail.com", "sotorr451@gmail.com", "Test", "Hello world");
