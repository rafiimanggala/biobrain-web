using System;
using System.Collections.Generic;
using BioBrain.Interfaces;
using Common;
using Common.Interfaces;
using CustomControls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace BioBrain.Helpers
{
    public static class EmailHelper
    {
        public static void SendResultsEmail(List<IStatEntryViewModel> resultsToSend, IErrorLog logger)
        {
            var header = $"{StringResource.My} {StringResource.AppName} {StringResource.ResultsString}";
            var image = DrawHelper.DrawResults(header, resultsToSend);
            var textToSend = $"<html><img width=\"100%\" style=\"object-fit: contain;\" src=\"data:image/png;base64, {image.Base64Image}\"></html>";

            var message = new EmailMessage
            {
                Body = textToSend,
                BodyFormat = EmailBodyFormat.Html,
                Subject = StringResource.ResultEmailSubject,
            };

            if (Device.RuntimePlatform == Device.Android)
            {
                message.Attachments = new List<EmailAttachment>() {new EmailAttachment(image.ImagePath)};
                var sender = DependencyService.Get<IEmailSender>();
                sender.Compose(message, "The Biobrain Results can be found in the attached.\n\nFor more information on Biobrain, the leading Learning App for Physics, Biology & Chemistry, please visit www.biobrain.com.au");
            }
            else
                try
                {
                    Email.ComposeAsync(message);
                }
                catch
                {
                    try
                    {
                        message.Attachments = new List<EmailAttachment>() { new EmailAttachment(image.ImagePath) };
                        message.Body =
                            "The Biobrain Results can be found in the attached.\n\nFor more information on Biobrain, the leading Learning App for Physics, Biology & Chemistry, please visit www.biobrain.com.au";
                        message.BodyFormat = EmailBodyFormat.PlainText;
                        Email.ComposeAsync(message);
                    }
                    catch(Exception ex) { logger.Log(ex.ToString()); return; }
                }
        }
    }
}
