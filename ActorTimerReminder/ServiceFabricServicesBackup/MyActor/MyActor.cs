using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using MyActor.Interfaces;
using PosteItaliane.Sin.SF.Actors;

namespace MyActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class MyActor : Actor, IMyActor, IRemindable
    {

        public static string QueueName = "queue";

        private IActorTimer Timer;
        private IActorReminder Reminder;
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            //Timer = RegisterTimer(a => Increment(), null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));

            //await this.StateManager.EnqueueAsync<Queue<Guid>>(QueueName, new Queue<Guid>());

            // decido di farlo partire alle 19:15

            //Reminder = await RegisterReminderAtTimeAsync("MyReminder", new byte[] { 0, 1, 2, 3, 4 },
            //    TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));

            //Reminder = await RegisterReminderAtTimeAsync("MyReminder", new byte[] { 0, 1, 2, 3, 4 },
            //    TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));

            await this.StateManager.TryAddStateAsync("count", 0);
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        async Task<int> IMyActor.GetCountAsync()
        {
            Reminder = await RegisterReminderAsync("MyReminder", new byte[] { 0, 1, 2, 3, 4 },
                TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(20));

            Reminder = await RegisterReminderAsync("MyReminder", new byte[] { 0, 1, 2, 3, 4 },
                TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(20));

            return await this.StateManager.GetStateAsync<int>("count");
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        Task IMyActor.SetCountAsync(int count)
        {
            // Requests are not guaranteed to be processed in order nor at most once.
            // The update function here verifies that the incoming count is greater than the current count to preserve order.
            return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value);
        }


        private async Task Increment()
        {
            ActorEventSource.Current.ActorMessage(this, $"{DateTime.Now: HH:mm:ss} Increment.");
            await this.StateManager.AddOrUpdateStateAsync("count", 0, (key, value) => value++);
            var count = await (this as IMyActor).GetCountAsync();
            ActorEventSource.Current.ActorMessage(this, $"Incremented {count}.");
        }

        public Task ReceiveReminderAsync(string reminderName,
            byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            ActorEventSource.Current.ActorMessage(this, $" ReceiveReminderAsync - {DateTime.Now: HH:mm:ss} {reminderName} - dueTime {dueTime}, period {period}.");

            var queue = this.StateManager.DequeueAsync<Queue<Guid>>(QueueName);



            return Task.Delay(0);
        }


        protected Task<IActorReminder> RegisterReminderAtTimeAsync(string reminderName,
            byte[] state, TimeSpan startTime, TimeSpan period)
        {
            var now = DateTime.Now;

            var startDateTime = now.TimeOfDay > startTime ?
                now.Date.AddDays(1).Add(startTime) : now.Date.Add(startTime);

            var dueTime = startDateTime.Subtract(now);

            return RegisterReminderAsync(reminderName, state, dueTime, period);
        }


    }
}
