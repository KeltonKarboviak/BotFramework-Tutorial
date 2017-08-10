using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DigiBot_Tutorial.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string name;
        private long age;

        public Task StartAsync(IDialogContext context)
        {
            // Wait until the first message is received from the conversation and call MessageReceivedAsync to process that message
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm the Basic Multi Dialog Bot. Let's get started.");

            context.Call(new NameDialog(), this.ResumeAfterNameDialog);
        }

        private async Task ResumeAfterNameDialog(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this.name = await result;
                context.Call(new AgeDialog(this.name), this.ResumeAfterAgeDialog);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");

                await this.SendWelcomeMessageAsync(context);
            }
        }

        private async Task ResumeAfterAgeDialog(IDialogContext context, IAwaitable<long> @result)
        {
            try
            {
                this.age = await result;
                await context.PostAsync($"Your name is {this.name} and your age is {this.age}.");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("I'm sorry, I'm having issues understanding you. Let's try again.");
            }
            finally
            {
                await this.SendWelcomeMessageAsync(context);
            }
        }
    }
}