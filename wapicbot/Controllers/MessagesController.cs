using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace wapicbot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {

            var cli = new ConnectorClient(new Uri(activity.ServiceUrl));

            if (activity.Type == ActivityTypes.Message)
            {

                var TypingReply = activity.CreateReply();
                TypingReply.Type = ActivityTypes.Typing;
                await cli.Conversations.ReplyToActivityAsync(TypingReply);
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    message.Type = ActivityTypes.Message;

                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    message.TextFormat = "markdown";
                    message.Text = "Hello!, I'm Mavis, you are welcome to Wapic. Please how can i help you ?\n you can begin by sending help";
                    Activity reply = message.CreateReply(message.Text);
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        { new CardAction() {Title="Make an Enquiry", Type= ActionTypes.ImBack, Value="enquiry"},
                            new CardAction() {Title="Buy insurance", Type= ActionTypes.ImBack, Value="i want insurance"},
                            new CardAction() {Title="Chat with an agent", Type= ActionTypes.MessageBack,}
                        }
                    };
                    connector.Conversations.ReplyToActivityAsync(reply);
                }

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}