using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace wapicbot.Dialogs
{
    [Serializable]
    public class GenerateQuote : IDialog<object>
    {
        private const string FlightsOption = "Flights";

        private const string HotelsOption = "Hotels";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            await ShowOptionsAsync(context);

        }

            private async Task ShowOptionsAsync(IDialogContext context)
        {
           

            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new Container()
                    {
                        Speak = "<s>Hello!</s><s>Are you looking for a flight or a hotel?</s>",
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
                                            new Image()
                                            {
                                                Url = "https://placeholdit.imgix.net/~text?txtsize=65&txt=Adaptive+Cards&w=300&h=300",
                                                Size = ImageSize.Medium,
                                                Style = ImageStyle.Person
                                            }
                                        }
                                    },
                                    new Column()
                                    {
                                        Size = ColumnSize.Stretch,
                                        Items = new List<CardElement>()
                                        {
                                            new TextBlock()
                                            {
                                                Text =  "Hello!",
                                                Weight = TextWeight.Bolder,
                                                IsSubtle = true
                                            },
                                            new TextBlock()
                                            {
                                                Text = "Are you looking for a flight or a hotel?",
                                                Wrap = true
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                // Buttons
                Actions = new List<ActionBase>() {
                    new ShowCardAction()
                    {
                        Title = "Hotels",
                        Speak = "<s>Hotels</s>",
                        Card = GetHotelSearchCard()
                    },
                    new ShowCardAction()
                    {
                        Title = "Flights",
                        Speak = "<s>Flights</s>",
                        Card = new AdaptiveCard()
                        {
                            Body = new List<CardElement>()
                            {
                                new TextBlock()
                                {
                                    Text = "Flights is not implemented =(",
                                    Speak = "<s>Flights is not implemented</s>",
                                    Weight = TextWeight.Bolder
                                }
                            }
                        }
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);

            await context.PostAsync(reply, CancellationToken.None);

            context.Wait(MessageReceivedAsync);
        }
      
        

       

        private static AdaptiveCard GetHotelSearchCard()
        {
            return new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                        // Hotels Search form
                        new TextBlock()
                        {
                            Text = "Welcome to the Hotels finder!",
                            Speak = "<s>Welcome to the Hotels finder!</s>",
                            Weight = TextWeight.Bolder,
                            Size = TextSize.Large
                        },
                        new TextBlock() { Text = "Please enter your destination:" },
                        new TextInput()
                        {
                            Id = "Destination",
                            Speak = "<s>Please enter your destination</s>",
                            Placeholder = "Miami, Florida",
                            Style = TextInputStyle.Text
                        },
                        new TextBlock() { Text = "When do you want to check in?" },
                        new DateInput()
                        {
                            Id = "Checkin",
                            Speak = "<s>When do you want to check in?</s>"
                        },
                        new TextBlock() { Text = "How many nights do you want to stay?" },
                        new NumberInput()
                        {
                            Id = "Nights",
                            Min = 1,
                            Max = 60,
                            Speak = "<s>How many nights do you want to stay?</s>"
                        }
                },
                Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Search",
                        Speak = "<s>Search</s>",
                        DataJson = "{ \"Type\": \"HotelSearch\" }"
                    }
                }
            };
        }

        //private static async Task SendHotelSelectionAsync(IDialogContext context, Hotel hotel)
        //{
        //    var description = $"{hotel.Rating} start with {hotel.NumberOfReviews}. From ${hotel.PriceStarting} per night.";
        //    var card = new AdaptiveCard()
        //    {
        //        Body = new List<CardElement>()
        //        {
        //            new Container()
        //            {
        //                Items = new List<CardElement>()
        //                {
        //                    new TextBlock()
        //                    {
        //                        Text = $"{hotel.Name} in {hotel.Location}",
        //                        Weight = TextWeight.Bolder,
        //                        Speak = $"<s>{hotel.Name}</s>"
        //                    },
        //                    new TextBlock()
        //                    {
        //                        Text = description,
        //                        Speak = $"<s>{description}</s>"
        //                    },
        //                    new Image()
        //                    {
        //                        Size = ImageSize.Large,
        //                        Url = hotel.Image
        //                    },
        //                    new ImageSet()
        //                    {
        //                        ImageSize = ImageSize.Medium,
        //                        Separation = SeparationStyle.Strong,
        //                        Images = hotel.MoreImages.Select(img => new Image()
        //                        {
        //                            Url = img
        //                        }).ToList()
        //                    }
        //                },
        //                SelectAction = new OpenUrlAction()
        //                {
        //                     Url = "https://dev.botframework.com/"
        //                }
        //            }
        //        }
        //    };

        //    Attachment attachment = new Attachment()
        //    {
        //        ContentType = AdaptiveCard.ContentType,
        //        Content = card
        //    };

        //    var reply = context.MakeMessage();
        //    reply.Attachments.Add(attachment);

        //    await context.PostAsync(reply, CancellationToken.None);
        //}
    }
}