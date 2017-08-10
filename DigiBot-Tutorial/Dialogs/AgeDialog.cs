using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiBot_Tutorial.Dialogs
{
    [Serializable]
    public class AgeDialog : IDialog<long>
    {
        private string name;

        public AgeDialog(string name)
        {
            this.name = name;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Nice to meet you, {this.name}!");

            PromptDialog.Number(
                context,
                this.ResumeAfterPromptDialog,
                "What is your age?",
                "I'm sorry, I didn't understand your reply.",
                3,
                min: 0);
        }

        private async Task ResumeAfterPromptDialog(IDialogContext context, IAwaitable<long> result)
        {
            var age = await result;

            context.Done(age);
        }
    }
}