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
    public class NameDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context,
                this.ResumeAfterPromptDialog,
                "What is your name?",
                "Sorry, I don't understand your reply.",
                3);
        }

        private async Task ResumeAfterPromptDialog(IDialogContext context, IAwaitable<string> result)
        {
            var name = await result;

            context.Done(name);
        }
    }
}