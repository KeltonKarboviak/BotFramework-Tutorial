using DigiBot_Tutorial.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DigiBot_Tutorial.Dialogs
{
    [LuisModel("344f13be-b8b2-4747-b8cc-923169a006c6", "4e5dba9280d34fdfb0da6a48e5b66f68", Staging = true, TimezoneOffset = -360, Verbose = true)]
    [Serializable]
    public class SimpleReminderLuisDialog : LuisDialog<object>
    {
        private readonly Dictionary<string, Reminder> reminderByWhat = new Dictionary<string, Reminder>();

        public const string DefaultReminderWhat = "default";

        public const string Entity_ReminderText = "Reminder.Text";
        public const string Entity_BuiltInDate = "builtin.datetimeV2.date";
        public const string Entity_BuiltInDateTime = "builtin.datetimeV2.datetime";
        public const string Entity_BuiltInTime = "builtin.datetimeV2.time";
        public const string Entity_BuiltInReminderWhen = "";

        public const string Entity_BuiltInAlarmTitle = "builtin.alarm.title";
        public const string Entity_BuiltInAlarmStartTime = "builtin.alarm.start_time";
        public const string Entity_BuiltInAlarmStartDate = "builtin.alarm.start_date";

        public SimpleReminderLuisDialog()
        {
        }

        public SimpleReminderLuisDialog(ILuisService luis)
            : base(luis)
        {
        }

        private bool TryFindReminder(LuisResult result, out Reminder reminder)
        {
            reminder = null;

            EntityRecommendation titleER;
            string what = result.TryFindEntity(Entity_ReminderText, out titleER)
                ? titleER.Entity
                : DefaultReminderWhat;

            return this.reminderByWhat.TryGetValue(what, out reminder);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand: {result.Query}";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Reminder.Create")]
        public async Task SetReminder(IDialogContext context, LuisResult result)
        {
            EntityRecommendation titleER;
            if (!result.TryFindEntity(Entity_ReminderText, out titleER))
            {
                titleER = new EntityRecommendation(type: Entity_ReminderText) { Entity = DefaultReminderWhat };
            }

            EntityRecommendation datetimeER;
            if (!result.TryFindEntity(Entity_BuiltInTime, out datetimeER))
            {
                datetimeER = new EntityRecommendation(type: Entity_BuiltInReminderWhen) { Entity = string.Empty };
            }

            var parser = new Chronic.Parser();
            var span = parser.Parse(datetimeER.Entity);

            if (span != null)
            {
                var when = span.Start ?? span.End;
                var reminder = new Reminder() { What = titleER.Entity, When = when.Value };
                this.reminderByWhat[reminder.What] = reminder;

                string reply = $"reminder {reminder} created";
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("could not find time for the reminder");
            }

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Reminder.Find")]
        public async Task FindReminder(IDialogContext context, LuisResult result)
        {
            Reminder reminder;
            if (TryFindReminder(result, out reminder))
            {
                await context.PostAsync($"found reminder: {reminder}");
            }
            else
            {
                await context.PostAsync("did not find reminder");
            }

            context.Wait(this.MessageReceived);
        }
    }
}