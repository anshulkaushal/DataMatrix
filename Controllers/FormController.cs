using DataMatrix.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;
using Umbraco.Extensions;
namespace DataMatrix.Controllers
{
    public class FormController : SurfaceController
    {
        private readonly IPublishedContentQuery _contentQuery;
        private readonly ISqliteFormService _formService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FormController> _logger;
        public FormController(
            ISqliteFormService formService,
            IConfiguration configuration,
            IPublishedContentQuery contentQuery,
              ILogger<FormController> logger,
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext serviceContext,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider)
            : base(umbracoContextAccessor, databaseFactory, serviceContext, appCaches, profilingLogger, publishedUrlProvider)
        {
            _formService = formService;
            _contentQuery = contentQuery;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SubmitForm(string fullName, string email, string subject, string message)
        {

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(subject) ||
                string.IsNullOrWhiteSpace(message))
            {
                return Json(new
                {
                    success = false,
                    message = "Fill out all required fields."
                });
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);

            if (!emailRegex.IsMatch(email))
            {
                return Json(new
                {
                    success = false,
                    message = "Please enter a valid email address."
                });
            }

            var settings = GetEmailSettings();
            if (settings == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Email settings are not configured."
                });
            }

            string formInfo = string.Empty;
            string url = string.Empty;

             var thankYouLink = settings.Value<IEnumerable<Link>>("thankYouPageUrl");
             url = thankYouLink?.FirstOrDefault()?.Url;
            

            _formService.SaveFormData(fullName, email, subject, message);

            var templates = _configuration["Umbraco:CMS:EmailTemplates:ThankYouMail"];

            templates = templates.Replace("{{FullName}}", fullName);

            _formService.SendThankYouEmail(email, fullName, templates);

            var template = _configuration["Umbraco:CMS:EmailTemplates:ContactInquiry"];
            if (!string.IsNullOrWhiteSpace(template))
            {
                template = template
                    .Replace("{{FullName}}", fullName)
                    .Replace("{{Email}}", email)
                    .Replace("{{Subject}}", subject)
                    .Replace("{{Message}}", message.Replace("\n", "<br/>"));
            }
            // Send email
            _formService.SendEmail(template);


            return Json(new
            {
                success = true,
                message = "Thank you for contacting us. Our team will get back to you shortly."
            });


        }


        private IPublishedContent? GetEmailSettings()
        {
            if (!UmbracoContextAccessor.TryGetUmbracoContext(out var context))
                return null;

            return _contentQuery
          .ContentAtRoot()
          .FirstOrDefault(x => x.ContentType.Alias == "smtpEmailCredentials");
        }

    }
}

