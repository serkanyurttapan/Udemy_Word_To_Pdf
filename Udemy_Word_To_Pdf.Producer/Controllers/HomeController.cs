using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Udemy_Word_To_Pdf.Producer.Models;

namespace Udemy_Word_To_Pdf.Producer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult WordToPdfPage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult WordToPdfPage(WordToPdf wordToPdf)
        {
            
            var factory = new ConnectionFactory();
            //factory.Uri = new Uri(_configuration["ConnectionString:RabbitMQCloudString"]);
            factory.HostName = "localhost";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("convert-exchange", ExchangeType.Direct, true, false, null);
                    channel.QueueDeclare(queue: "File", true, false, autoDelete: false, arguments: null);
                    channel.QueueBind("File", "convert-exchange","WordToPdf");
                    MessageWordToPdf messageWordToPdf = new MessageWordToPdf();
                    using (MemoryStream memoryStream =new MemoryStream())
                    {
                        wordToPdf.WordFile.CopyTo(memoryStream);
                        messageWordToPdf.WordToByte = memoryStream.ToArray();
                    }
                    messageWordToPdf.Email = wordToPdf.Email;
                    messageWordToPdf.FileName = Path.GetFileNameWithoutExtension(wordToPdf.WordFile.FileName);
                    string serializeMessage = JsonConvert.SerializeObject(messageWordToPdf);
                    byte[] byteMessage = Encoding.UTF8.GetBytes(serializeMessage);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish("convert-exchange", routingKey: "WordToPdf", basicProperties: properties, body: byteMessage);
                    ViewBag.result = "Word Pdf e Dönüştürülüp email atılacak";
                    return View();

                }
            }
        }

        public IActionResult Privacy()
        {
            return null;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
