using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace wapicbot.Forms
{
    [Serializable]
    public class Vehicle
    {
        [Prompt("Kindly Provide your name?")]
        public string Name { get; set; }

        [Prompt("Provide Your Pin")]
        public string PIN { get; set; }

        public static IForm<Vehicle> BuildVehicleForm()
        {

            return new FormBuilder<Vehicle>().Message("Kindly Provide your details!").Field(nameof(Name)).Field(nameof(PIN))
                .AddRemainingFields().OnCompletion(async (context, profileForm) =>
                {

                    
                    string message = "Verifying details....:";

                    string message2 = $"{profileForm.Name} We can see you have N50,000 left in your account for compensation coverage";

                    string message3 = $"Your Vehicle claim compensation has been fully paid";
                    await context.PostAsync(message);
                    await Task.Delay(1234);
                    await context.PostAsync(message3);
                    await Task.Delay(1234);
                    await context.PostAsync(message2);
                }).Build();
        }
    }

}