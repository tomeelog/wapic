using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace wapicbot.Dialogs
{
    [Serializable]
    public class testDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            await context.PostAsync($"{activity.Text}");

            activity.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
    {
        new CardAction(){ Title = "Blue", Type=ActionTypes.ImBack, Value="Blue" },
        new CardAction(){ Title = "Red", Type=ActionTypes.ImBack, Value="Red" },
        new CardAction(){ Title = "Green", Type=ActionTypes.ImBack, Value="Green" }
    }
            };

            await context.PostAsync(activity);

            //try
            //{
            //    var smtpClient = new SmtpClient
            //    {
            //        Host = "smtp.office365.com", // set your SMTP server name here
            //        Port = 587, // Port 
            //        EnableSsl = false,
            //        Credentials = new NetworkCredential("tomeelog@hotmail.com", "window123")
            //    };


            //    using (var message = new MailMessage("tomeelog@hotmail.com", "tomeelog@gmail.com")
            //    {
            //        Subject = "Hello",
            //        Body = " Dear "

            //    })
            //       await smtpClient.SendMailAsync(message);
            //}
            //catch (Exception e)
            //{
            //    await context.PostAsync($"{e.Message}");
            //}

            //await context.PostAsync("generating your quote");

            context.Wait(MessageReceivedAsync);
        }
    }
}