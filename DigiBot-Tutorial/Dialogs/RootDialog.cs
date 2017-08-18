using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;

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

        public static readonly IDialog<string> dialog = Chain.PostToChain()
            .Select(msg => msg.Text)
            .Switch(
                new Case<string, IDialog<string>>(text =>
                {
                    var regex = new Regex("^reset");
                    return regex.Match(text).Success;
                }, (cxt, txt) =>
                {
                    return Chain.From(() => new PromptDialog.PromptConfirm("Are you sure you want to reset the count?", "Didn't get that!", 3, PromptStyle.Keyboard))
                        .ContinueWith<bool, string>(async (ctx, res) =>
                        {
                            string reply;
                            if (await res)
                            {
                                ctx.UserData.SetValue("count", 0);
                                reply = "Reset count.";
                            }
                            else
                            {
                                reply = "Did not reset count.";
                            }
                            return Chain.Return(reply);
                        });
                }),
                new RegexCase<IDialog<string>>(new Regex("^help", RegexOptions.IgnoreCase), (ctx, txt) =>
                {
                    return Chain.Return("I am a simple echo dialog with a counter! Reset my counter by typing \"reset\"!");
                }),
                new DefaultCase<string, IDialog<string>>((ctx, txt) =>
                {
                    int count;
                    ctx.UserData.TryGetValue("count", out count);
                    ctx.UserData.SetValue("count", ++count);
                    string reply = $"{count}: You said {txt}";
                    return Chain.Return(reply);
                }))
            .Unwrap()
            .PostToUser();

        public static readonly IDialog<string> new_dialog = (from x in new PromptDialog.PromptString("p1", "p1", 1)
                                                             from y in new PromptDialog.PromptString("p2", "p2", 1)
                                                             select string.Join(" ", x, y))
            .PostToUser();

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            var message = await activity;

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm the Basic Multi Dialog Bot. Let's get started.");

            //context.Call(new NameDialog(), this.ResumeAfterNameDialog);

            var query = from x in new NameDialog()
                        from y in new AgeDialog(x)
                        select $"Your name is {x} and your age is {y}.";

            //await Conversation.SendAsync((IMessageActivity)context.Activity, () => dialog);
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