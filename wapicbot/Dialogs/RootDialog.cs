using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using wapicbot.Forms;

namespace wapicbot.Dialogs
{
    [Serializable]
    [LuisModel("3887f262-808a-4aaa-ad79-b23439d53d55", "b7978e42c77e4476a26586e4e2915aff")]
    public class RootDialog : LuisDialog<object>
    {

        private const string PolicyOption = "Verify Policy";

        private const string Veichile = "veichile";

        private const string Mortgage = "mortgage";

        static string qnamaker_endpointKey = "c85e484e-b16d-4183-ab76-8d15703b2546";
        static string qnamaker_endpointDomain = "sambot1";
        static string HR_kbID = "d9cfd4e8-58b9-4fef-8892-2ded6698b03e";

        public QnAMakerService hrQnAService = new QnAMakerService("https://" + qnamaker_endpointDomain + ".azurewebsites.net", HR_kbID, qnamaker_endpointKey);

        public enum help
        {
            Verify_Policy,
            Make_Claim,
            Track_Claim,
            Request_Call,
            Custome_care
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        private async Task NoIntentReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        { 
            var qnaMakerAnswer = await hrQnAService.GetAnswer(result.Query);
            await context.PostAsync($"{qnaMakerAnswer}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("help")]
        public async Task Help(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult) {

            if (luisResult.Intents[0].Score >= 0.8)
            {

                var option = luisResult;
                var description = new string[] { "Verify Policy", "Make a claim", "Track claim", "Request call", "Customer care" };
                PromptDialog.Choice(
                 context: context,
                 resume: ChoiceReceivedAsync,
                 options: (IEnumerable<help>)Enum.GetValues(typeof(help)),
                 prompt: "Hi. Please Select from the listed options :",
                 retry: "Selected plan not avilabel . Please try again.",
                 promptStyle: PromptStyle.Auto,
                 descriptions: description
                 );
            }
            else {
                var qnaMakerAnswer = await hrQnAService.GetAnswer(luisResult.Query);
                await context.PostAsync($"{qnaMakerAnswer}");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("WhoAreYou")]
        public async Task Who(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {

            if (luisResult.Intents[0].Score >= 0.8)
            {
                await context.PostAsync("I can help you answer some questions on insurance and also track claims and verify policy\n you can type help to begin");
            }
            else {

                var qnaMakerAnswer = await hrQnAService.GetAnswer(luisResult.Query);
                await context.PostAsync($"{qnaMakerAnswer}");
                context.Wait(MessageReceived);
            }
        }
        [LuisIntent("insurance")]
        public async Task Insurance(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {
            if (luisResult.Intents[0].Score >= 0.8)
            {
                await context.PostAsync("Select the type of isurance you want to buy");
                var reply = context.MakeMessage();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = new List<Attachment>();

                List<CardImage> cardImages1 = new List<CardImage>();
                List<CardAction> cardButtons = new List<CardAction>();
                cardImages1.Add(new CardImage(url: "https://i.ibb.co/j9Y26HC/auto.jpg"));
                CardAction individual = new CardAction()
                {
                    Type = "imBack",
                    Title = "Register",
                    Value = "Auto insurance"
                };
                cardButtons.Add(individual);
                HeroCard plCard1 = new HeroCard()
                {
                    Title = "Auto insurance",
                    Subtitle = "Auto insurance",
                    Images = cardImages1,
                    Buttons = cardButtons
                };
                Attachment plAttachment1 = plCard1.ToAttachment();
                reply.Attachments.Add(plAttachment1);

                List<CardImage> cardImages2 = new List<CardImage>();
                List<CardAction> cardButtons2 = new List<CardAction>();
                cardImages2.Add(new CardImage(url: "https://i.ibb.co/WgYJngH/Property-Insurance-800x518.jpg"));
                CardAction corporate = new CardAction()
                {
                    Type = "imBack",
                    Title = "Register",
                    Value = "Property insurance"
                };
                cardButtons2.Add(corporate);
                HeroCard plCard2 = new HeroCard()
                {
                    Title = "Property",
                    Subtitle = "Property Insurance",
                    Images = cardImages2,
                    Buttons = cardButtons2
                };
                Attachment plAttachment2 = plCard2.ToAttachment();
                reply.Attachments.Add(plAttachment2);

                List<CardImage> cardImages3 = new List<CardImage>();
                List<CardAction> cardButtons3 = new List<CardAction>();
                cardImages3.Add(new CardImage(url: "https://i.ibb.co/X38m5BX/life.jpg"));
                CardAction forex = new CardAction()
                {
                    Type = "imBack",
                    Title = "Register",
                    Value = "Life insurance"
                };
                cardButtons3.Add(forex);
                HeroCard plCard3 = new HeroCard()
                {
                    Title = "Life",
                    Subtitle = "Life insurance",
                    Images = cardImages3,
                    Buttons = cardButtons3
                };
                Attachment plAttachment3 = plCard3.ToAttachment();
                reply.Attachments.Add(plAttachment3);
                reply.Text = "Pease Select insurance type";
                await context.PostAsync(reply);
            }
            else
            {
                var qnaMakerAnswer = await hrQnAService.GetAnswer(luisResult.Query);
                await context.PostAsync($"{qnaMakerAnswer}");
                context.Wait(MessageReceived);
            }
        }
        [LuisIntent("Auto")]
        public async Task AutoInsurance(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {
            if (luisResult.Intents[0].Score >= 0.8) {

                var autoForm = new FormDialog<Auto>(new Auto(), Auto.BuildAutoForm, FormOptions.PromptInStart, null);
                context.Call<Auto>(autoForm, FormCompleteCallback);
            }
        }

        [LuisIntent("chatAgent")]
        public async Task chatAgent(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {
            await context.PostAsync("All Agents are currently busy please try after some time");
        }
        [LuisIntent("GenQuote")]
        public async Task QuoteGen(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {

            if (luisResult.Intents[0].Score >= 0.8)
            {
                context.Call(new testDialog(), this.ResumeAfterQuoteDialog);
                //var QuoteForm = new FormDialog<QuoteGenerate>(new QuoteGenerate(), QuoteGenerate.BuildQuoteForm, FormOptions.PromptInStart, null);
                //context.Call<QuoteGenerate>(QuoteForm, QuoteFormCompleteCallback);

            }
            else
            {
                var qnaMakerAnswer = await hrQnAService.GetAnswer(luisResult.Query);
                await context.PostAsync($"{qnaMakerAnswer}");
                context.Wait(MessageReceived);
            }
        }

        private async Task QuoteFormCompleteCallback(IDialogContext context, IAwaitable<QuoteGenerate> result)
        {
            await context.PostAsync("Nice talking to you");
            context.Done(true);
        }


        private Task ResumeAfterQuoteDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

        [LuisIntent("TrackClaim")]
        public async Task trackClaim(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult luisResult)
        {
            await context.PostAsync("Please Select the type of claim you want to track");
            this.ShowOptions(context);
        }
        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelectedAsync, new List<string>() { Veichile, Mortgage, }, "Please Kindly Select from the following", "Not a valid option", 3);
        }

        private async Task OnOptionSelectedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {

                    case Veichile:
                        var VeichleForm = new FormDialog<Vehicle>(new Vehicle(), Vehicle.BuildVehicleForm, FormOptions.PromptInStart, null);
                        context.Call<Vehicle>(VeichleForm, VehicleFormCompleteCallback);
                        break;
                       
                    case Mortgage:

                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attempts :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceived);
            }
        }

        private async Task VehicleFormCompleteCallback(IDialogContext context, IAwaitable<Vehicle> result)
        {
            context.Done<object>("You can ask questin on insurnce or check other services");
        }

        private async Task FormCompleteCallback(IDialogContext context, IAwaitable<Auto> result)
        {
            await context.PostAsync("Nice talking to you");
            context.Done<object>("Thanks");
        }

        private async Task ChoiceReceivedAsync(IDialogContext context, IAwaitable<help> result)
        {
            await context.PostAsync("Nice talking to you");
            context.Done<object>(null);
        }
    }
}