using Microsoft.AspNetCore.Mvc;
using PersonalSite02.Models;
using System.Diagnostics;
using MailKit.Net.Smtp;
using MimeKit;
using PersonalSite02.Models;
namespace PersonalSite02.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;


        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Portfolio()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }

        public IActionResult Links()
        {
            return View();
        }

        //GET: Contact
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost] //This means the Action below will handle the POST requests
        public IActionResult Contact(ContactViewModel cvm)
        {
            //When a class has validation attributes, that validation should be checked
            //BEFORE attempting to process any of the data provided. 

            if (!ModelState.IsValid)
            {
                //Send the user back to the form. We can also pass the cvm object
                //to the View so the form will contain the original information they provided.

                return View(cvm);
            }

            //Create the format for the message content we will recieve from the contact form
            string message = $"You have recieved a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />" +
                $"Message: {cvm.Message}";

            //Create a MimeMessage Object to assist with storing/transporting the email information
            //from the contact form
            var mm = new MimeMessage();

            //Even though the user is the one attempting to send a message to us, the ACTUAL sender
            //of the email is the email user we set up with our hosting provider

            //We can access the credentials for this email user from our appsettings.json file as shown below:

            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            //The recipient of this email will be our personal email address, also stored in appsetting.json

            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //The subject will be the one provided by the user, which we stored in our cvm object

            mm.Subject = cvm.Subject;

            //the body of the message will be formatted with the string we created above

            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message as "urgent" so it will be flagged in our email client

            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses.
            //This lets us reply directly to the person who sent the message instead of our email user.
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create an SmtpClient object used to sent the email.
            //Once all of the code inside the using directive's scope has been executed,
            //it will close any open connections and dispose of the object automatically.
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using the credentials in our appsettings.json
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                //Login to the mail server using the credentials for our email user
                client.Authenticate(

                    //Username
                    _config.GetValue<string>("Credentials:Email:User"),

                    //Password
                    _config.GetValue<string>("Credentials:Email:Password")

                    );

                //It's possible the mail server may be down when the user attempts to contact us
                //so we can encapsulate our code to send the message in a try/catch.

                try
                {
                    //Try to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue we can store an error message in a ViewBag variable
                    //to be displayed in the View
                    ViewBag.ErrorMessage = $"There was an error processing your request. Please" +
                        $"try again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with their form info intact
                    return View(cvm);

                }

            }

            //If all goes well, return a View that displays a confirmation to the user
            //that their email was sent.

            return View("EmailConfirmation", cvm);
        }
    }
}