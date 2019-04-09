using Chronic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using wapicbot.Services;

namespace wapicbot.Forms
{


    [Serializable]
    public class MyAwaitableImage : AwaitableAttachment
    {
        // Mandatory: you should have this ctor as it is used by the recognizer
        public MyAwaitableImage(Microsoft.Bot.Connector.Attachment source) : base(source) { }

        // Mandatory: you should have this serialization ctor as well & call base
        protected MyAwaitableImage(SerializationInfo info, StreamingContext context) : base(info, context) { }

        // Optional: here you can check for content-type for ex 'image/png' or other..
        public override async Task<ValidateResult> ValidateAsync<T>(IField<T> field, T state)
        {
            var result = await base.ValidateAsync(field, state);

            if (result.IsValid)
            {
                var isValidForMe = this.Attachment.ContentType.ToLowerInvariant().Contains("image/png");

                if (!isValidForMe)
                {
                    result.IsValid = false;
                    result.Feedback = $"Hey, dude! Provide a proper 'image/png' attachment, not any file on your computer like '{this.Attachment.Name}'!";
                }
                else
                {
                    var url = this.Attachment.ContentUrl;

                    HttpClient httpClient = new HttpClient();
                    Stream filestrem = await httpClient.GetStreamAsync(url);
                    httpClient.Dispose();

                    byte[] ImageAsByteArray = null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            byte[] buf = new byte[1024];
                            count = filestrem.Read(buf, 0, 1024);
                            ms.Write(buf, 0, count);
                        } while (filestrem.CanRead && count > 0);
                        ImageAsByteArray = ms.ToArray();
                    }

                    HttpClient client = new HttpClient();

                    // Request headers.
                    client.DefaultRequestHeaders.Add(
                        "Ocp-Apim-Subscription-Key", "fcb7a03c796c40a3954a249c9b87d79c");

                    // Request parameters. A third optional parameter is "details".
                    string requestParameters =
                    "visualFeatures=Categories,Description,Color";

                    // Assemble the URI for the REST API Call.
                    string uri = "https://southcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze?" + requestParameters;

                    HttpResponseMessage response;


                    using (ByteArrayContent content = new ByteArrayContent(ImageAsByteArray))
                    {
                        // This example uses content type "application/octet-stream".
                        // The other content types you can use are "application/json"
                        // and "multipart/form-data".
                        content.Headers.ContentType =
                            new MediaTypeHeaderValue("application/octet-stream");

                        // Make the REST API call.
                        response = await client.PostAsync(uri, content);
                    }

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(contentString);
                    var rs = Newtonsoft.Json.Linq.JToken.Parse(contentString);

                    if (rs.HasValues)
                    {
                        string val = ((Newtonsoft.Json.Linq.JValue)((Newtonsoft.Json.Linq.JProperty)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)rs).First).First).First).First).Value).Value.ToString();

                        Result rsult = JsonConvert.DeserializeObject<Result>(contentString);

                        var f = rsult.description;
                        string r = f.captions[0].text.ToString();
                        Console.WriteLine(r);
                        if (!val.Contains("car"))
                        {

                            result.IsValid = false;
                            result.Feedback = $"I can see {r}. please upload your lovely image ";
                        }
                    }
                }
            }
            return result;
        }
        // Optional: here you can provide additional or override custom help text completely..
        public override string ProvideHelp<T>(IField<T> field)
        {
            var help = base.ProvideHelp(field);

            help += $"{Environment.NewLine}- Only 'image/png' can be attached to this field.";

            return help;
        }

        // Optional: here you can define your custom logic to get the attachment data or add custom logic to check it, etc..
        protected override async Task<Stream> ResolveFromSourceAsync(Microsoft.Bot.Connector.Attachment source)
        {
            var result = await base.ResolveFromSourceAsync(source);

            return result;
        }
    }

    [Serializable]
    public class Auto
    {
        public enum ReportType
        {
            Yes = 1,
            No
        }

        public bool AskToChooseReport = true;

        [Prompt("Are You a new Customer? {||}")]
        public ReportType? Report { get; set; }
    
        [Prompt("What is starting date (MM-DD-YYYY)")]
        public string StartDate { get; set; }

        [Prompt("What is your Full name")]
        public string FullName { get; set; }

        [Prompt("What is your PIN")]
        public  int Pin { get; set; }

        [Prompt("Upload your veichle image")]
        public MyAwaitableImage Image;

        public static IForm<Auto> BuildAutoForm()
        {
            var parser = new Parser();
            return new FormBuilder<Auto>()
                .Message("I can get you some nice Auto policy!!")
                .Field(nameof(Report)).Message("Great!Let me retrieve your information").Field(nameof(FullName)).Message("Hello {FullName}").Field(nameof(Pin)).Confirm("Are you sure you entered the correct Pin number?")
                .Message("One Moment please").Message("I've found your existing policy\n if you insure your new car with us you'll get 10% multi policy discount")
                .Field(nameof(Image)).Confirm("Are these information correct?\n{*}").OnCompletion(async (context, profileForm) =>
                {
                    string message = "That's all the information we need:";

                    string message2 = "Let me genetate a quote for you";

                    string message3 = $"{profileForm.FullName} We can insure your new car fro just N30,000 pet month with 10% discount";


                    string message4 = "Thanks for doing business with us";

                    await context.PostAsync(message);
                    await Task.Delay(1234);
                    await context.PostAsync(message2);
                    await Task.Delay(1234);
                    await context.PostAsync(message3);
                    await Task.Delay(1234);
                    await context.PostAsync(message4);
                })
               .Build();

        }

    }
}