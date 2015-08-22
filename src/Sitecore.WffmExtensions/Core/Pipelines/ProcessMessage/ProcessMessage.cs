using System;
using System.Linq;
using Microsoft.Exchange.WebServices.Data;
using Sitecore.Abstractions;
using Sitecore.Form.Core.Pipelines.ProcessMessage;

namespace Sitecore.WffmExtensions.Core.Pipelines.ProcessMessage
{
    public class ProcessMessage
    {
        private const string EwsSettingsPrefix = "Sitecore.WffmExtensions.ProcessMessage.SendEwsEmail.";

        public void SendEwsEmail(ProcessMessageArgs args)
        {
            ISettings settings = new SettingsWrapper();

            var exchangeUrl = settings.GetSetting(EwsSettingsPrefix + "ExchangeUrl");
            var exchangeVersion = settings.GetSetting(EwsSettingsPrefix + "ExchangeVersion");
            var userName = settings.GetSetting(EwsSettingsPrefix + "UserName");
            var userDomain = settings.GetSetting(EwsSettingsPrefix + "UserDomain");
            var userPassword = settings.GetSetting(EwsSettingsPrefix + "UserPassword");
            var fromEmail = settings.GetSetting(EwsSettingsPrefix + "FromEmail");

            ExchangeVersion version;
            if (!Enum.TryParse(exchangeVersion, out version))
            {
                //Default to latest version
                version = ExchangeVersion.Exchange2013_SP1;
            }

            var service = new ExchangeService(version)
            {
                Url = new Uri(exchangeUrl),
                Credentials =
                    !string.IsNullOrEmpty(userDomain)
                        ? new WebCredentials(userName, userPassword, userDomain)
                        : new WebCredentials(userName, userPassword)
            };

            var mail = GetMail(args, service);

            mail.From = fromEmail;

            mail.Send();
        }

        private EmailMessage GetMail(ProcessMessageArgs args, ExchangeService service)
        {
            var mail = new EmailMessage(service);

            if (args.To.Length > 0)
                args.To.ToString().Split(';').ToList().ForEach(emailAddress => mail.ToRecipients.Add(emailAddress));
            if (args.CC.Length > 0)
                args.CC.ToString().Split(';').ToList().ForEach(emailAddress => mail.CcRecipients.Add(emailAddress));
            if (args.BCC.Length > 0)
                args.BCC.ToString().Split(';').ToList().ForEach(emailAddress => mail.BccRecipients.Add(emailAddress));

            mail.Subject = args.Subject.ToString();

            mail.Body = args.Mail.ToString();

            args.Attachments.ForEach(attachment => mail.Attachments.AddFileAttachment(attachment.Name, attachment.ContentStream));

            return mail;
        }
    }
}
