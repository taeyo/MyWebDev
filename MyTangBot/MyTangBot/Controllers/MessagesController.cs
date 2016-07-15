using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Services.Description;

namespace MyBot
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
            if (activity.Type == ActivityTypes.Message)
            {
                //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                //// calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                //// return our reply to the user
                //Activity reply = activity.CreateReply($"You sent '{activity.Text}' which was {length} characters");
                //await connector.Conversations.ReplyToActivityAsync(reply);

               await Conversation.SendAsync(activity, () => new HelpDialog());
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


    [Serializable]
    public class HelpDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(HelpMessageReceived);
        }
        public async Task HelpMessageReceived(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text == "help")
            {
                PromptDialog.Confirm(
                    context,
                    HelpCommandResponseFunction,
                    "도움말을 보시겠어요?",
                    "Didn't get that!");
            }
            else
            {
                int length = (message.Text ?? string.Empty).Length;
                await context.PostAsync($"입력한 문장의 총 글자수는 {length} 개 입니다");
                context.Wait(HelpMessageReceived);
            }
        }
        public async Task HelpCommandResponseFunction(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync("-- 도움말 --\n 이 봇은 당신이 쓴 글의 글자 수를 세어주는 단순한 녀석입니다.");
            }
            context.Wait(HelpMessageReceived);
        }
    }
}