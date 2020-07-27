using System;
using System.IO;
using System.Net.Mail;

namespace Udemy_Word_To_Pdf.Consumer
{
    class Program
    {

        public static bool EmailSend(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {

                memoryStream.Position = 0;
                System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
                Attachment attachment = new Attachment(memoryStream, ct);
                attachment.ContentDisposition.FileName = $"{fileName}.pdf";
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp-mail.outlook.com";
                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("serkanyurttapan", "Nfs597194");
                mailMessage.From = new MailAddress("serkanyurttapan34@hotmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Pdf Dosyası Oluşturma";
                mailMessage.Body = "Pdf dosyası Word den oluşturuldu";
                mailMessage.IsBodyHtml = true;
                mailMessage.Attachments.Add(attachment);
                memoryStream.Close();
                memoryStream.Dispose();
                smtpClient.Send(mailMessage);
                Console.WriteLine("Emaile gönderildi");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Emaile gönderilemedi +{ex.ToString()}");
                return false;
            }
           
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
