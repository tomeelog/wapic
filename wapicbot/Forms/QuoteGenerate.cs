using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Bot.Connector;
using AdaptiveCards;
using Image = AdaptiveCards.Image;
using Choice = AdaptiveCards.Choice;
using Microsoft.Bot.Builder.Scorables;
using System.Net.Mail;
using Attachment = Microsoft.Bot.Connector.Attachment;
using System.Text;
using Microsoft.Bot.Builder.Dialogs;
using System.Net;

namespace wapicbot.Forms
{
    [Serializable]
    public class QuoteGenerate
    {

        public enum FrequencyType
        {
            Monthly = 1,
            Yearly
        }

        [Prompt("Provide your first Name")]
        public string FirstName { get; set; }

        [Prompt("What is your Last Name")]
        public string LastName { get; set; }

        [Prompt("What your age")]
        public int Age { get; set; }

        [Prompt("What is your Email")]
        public string Email { get; set; }

        [Prompt("What is your Phone number")]
        public string Phone { get; set; }

        [Prompt("Enter Amount")]
        public int Amount{ get; set; }

        [Prompt("Enter frequency {||}")]
        public FrequencyType? frequency { get; set; }

        [Prompt("Enter duration in years")]
        public int Duration { get; set; }






        public static IForm<QuoteGenerate> BuildQuoteForm()
        {

            return new FormBuilder<QuoteGenerate>().Message("I can help you generate quotes Kindly Provide your details!").Field(nameof(FirstName)).Field(nameof(LastName)).Field(nameof(frequency))
                .AddRemainingFields().OnCompletion(async (context, profileForm) =>
                {
                    try
                    {
                        var smtpClient = new SmtpClient
                        {
                            Host = "smtp-mail.outlook.com", // set your SMTP server name here
                            Port = 25, // Port 
                            EnableSsl = true,
                            Credentials = new NetworkCredential("babalola@lotusbetaanalytics.com", "ImpressiveDude@1")
                        };
                       

                        using (var message = new MailMessage("babalola@lotusbetaanalytics.com", "babalola@lotusbetaanalytics.com")
                        {
                            Subject = "Hello",
                            Body = " Dear "

                        })
                            await smtpClient.SendMailAsync(message);
                    }
                    catch (Exception e) {
                        await context.PostAsync($"{e.Message}"); 
                    }

                    await context.PostAsync("generating your quote");

                    Activity replyToConversation = (Activity)context.MakeMessage();
                    
                    replyToConversation.Attachments = new List<Attachment>();
                    AdaptiveCard card = new AdaptiveCard()
                    {
                        Body = new List<CardElement>()
                        {
                                    new Container()
                                    {
                                         Speak = "<s>Hello!</s>",
                                          Items = new List<CardElement>()
                                          {
                                              new ColumnSet()
                                              {
                                                  Columns = new List<Column>()
                                                  {
                                                       new Column()
                                                       {
                                                           Size = ColumnSize.Auto,
                                                           Items = new List<CardElement>()
                                                           {
                                                               //new Image()
                                                               //{
                                                               //     Url = "https://image.ibb.co/chDkEy/1gov101.png",
                                                               //     Size = ImageSize.Large,
                                                               //     Style = ImageStyle.Normal
                                                               //}
                                                           }
                                                       },
                                                       new Column()
                                                       {
                                                           Size = "300",
                                                           Items = new List<CardElement>()
                                                           {
                                                               new TextBlock()
                                                               {
                                                                   Text =  $"Quote for {profileForm.FirstName}",
                                                                   Weight = TextWeight.Bolder,
                                                                   //IsSubtle = true
                                                               },
                                                               new TextBlock()
                                                               {
                                                                   Text =  "Maximum Sum Assured 10,000",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },

                                                               new TextBlock()
                                                               {
                                                                   Text =  "Age at inception | 29",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },

                                                               new TextBlock()
                                                               {
                                                                   Text =  "Treasury Bill |10,000",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },

                                                               new TextBlock()
                                                               {
                                                                   Text =  "Anual Contribution |120,000",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },
                                                                new TextBlock()
                                                               {
                                                                   Text =  $"Frequency | {profileForm.frequency}",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },
                                                                 new TextBlock()
                                                               {
                                                                   Text =  $"Duration | {profileForm.Duration}",
                                                                   Weight = TextWeight.Bolder,
                                                                   IsSubtle = true
                                                               },
                                                           }
                                                       }
                                                  }
                                              },
                                              new ColumnSet()
                                              {
                                                  Columns = new List<Column>()
                                                  {
                                                      new Column()
                                                      {
                                                           Size = ColumnSize.Auto,
                                                           Separation =SeparationStyle.Strong,
                                                           Items = new List<CardElement>()
                                                           {
                                                               new TextBlock()
                                                               {
                                                                   Text = "A copy of this quotes has been sen to your mail.",
                                                                   Wrap = true
                                                               }
                                                           }
                                                      }
                                                  }

                                              }
                                          }
        
                                    }
                        },
                        Actions = new List<ActionBase>() {
                            new SubmitAction()
                            {
                                 Title = "Buy Now",
                                 Speak = "<s>Buy</s>",
                            }
                        }
                    };
                    Microsoft.Bot.Connector.Attachment attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card
                    };
                    replyToConversation.Attachments.Add(attachment);

                    await context.PostAsync(replyToConversation);

                    

                }).Build();
        }
        }
    }
