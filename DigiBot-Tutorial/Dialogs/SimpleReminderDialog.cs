using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiBot_Tutorial.Dialogs
{
    [Serializable]
    public class SimpleReminderDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceived);

            return Task.CompletedTask;
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;


        }
    }
}