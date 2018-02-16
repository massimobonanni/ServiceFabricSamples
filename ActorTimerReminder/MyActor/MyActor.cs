using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Health;
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

        public MyActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see http://aka.ms/servicefabricactorsstateserialization

            return Task.Delay(0);
        }


        public Task ReceiveReminderAsync(string reminderName,
                 byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            ActorEventSource.Current.ActorMessage(this, $" ReceiveReminderAsync - {DateTime.Now: HH:mm:ss} {reminderName} - dueTime {dueTime}, period {period}.");
            ReportHealthInformation(this.GetActorId().GetStringId(), DefaultReminderName,
                $" ReceiveReminderAsync - {DateTime.Now: HH:mm:ss} {reminderName} - dueTime {dueTime}, period {period}.",
                HealthState.Error, 600);
            return Task.Delay(0);
        }


        protected void ReportHealthInformation(string sourceId, string property, string description,
            HealthState state, int secondsToLive)
        {
            HealthInformation healthInformation = new HealthInformation(sourceId, property, state);
            healthInformation.Description = description;
            if (secondsToLive > 0) healthInformation.TimeToLive = TimeSpan.FromSeconds(secondsToLive);
            healthInformation.RemoveWhenExpired = true;
            try
            {
                var activationContext = FabricRuntime.GetActivationContext();
                activationContext.ReportApplicationHealth(healthInformation);
            }
            catch { }
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

        private const string DefaultReminderName = "defaultReminder";

        public Task ScheduleReminder(TimeSpan timeToRemind, CancellationToken cancellationToken = default(CancellationToken))
        {
            ActorEventSource.Current.ActorMessage(this, $"ScheduleReminder --> Expired at {timeToRemind}");
            ReportHealthInformation(this.GetActorId().GetStringId(), DefaultReminderName,
                $" ScheduleReminder - {DateTime.Now: HH:mm:ss} timeToRemind {timeToRemind}.",
                HealthState.Warning, 60);
            return this.RegisterReminderAsync(DefaultReminderName, null, timeToRemind,TimeSpan.FromMilliseconds(-1));
        }

    }
}
