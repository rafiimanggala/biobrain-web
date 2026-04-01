using System;
using System.IO;
using Android.Content;
using BioBrain.Interfaces;
using Microsoft.Maui.ApplicationModel;
using AndroidX.Core.Content;

// TODO: Register via DI in MauiProgram.cs instead of DependencyService
// builder.Services.AddSingleton<IEmailSender, EmailSender>();
namespace BioBrain.Platforms.Android.PlatformImplementation
{
    public class EmailSender : IEmailSender
    {
        public void Compose(EmailMessage message, string textMessage)
        {
            var activity = Platform.CurrentActivity;
            var email = new Intent(Intent.ActionSend);
            email.PutExtra(Intent.ExtraSubject, message.Subject);
            email.PutExtra(Intent.ExtraText, textMessage);
            email.PutExtra(Intent.ExtraHtmlText, message.Body);

            // TODO: Update FileProvider authority and attachment handling for MAUI
            var providerAuthority = activity.ApplicationContext.PackageName + ".fileProvider";
            message.Attachments.ForEach(x =>
                email.PutExtra(Intent.ExtraStream,
                    AndroidX.Core.Content.FileProvider.GetUriForFile(activity.ApplicationContext, providerAuthority,
                        new Java.IO.File(Path.GetDirectoryName(x.FullPath), Path.GetFileName(x.FullPath)))));

            email.SetType("message/rfc822");
            activity.StartActivity(email);
        }
    }
}
