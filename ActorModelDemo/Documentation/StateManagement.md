# State Management #

This demo shows you whats happened when you store objects in the actor state manager in the wrong manner.

The ActorDemo actor implements the interface `IStateActor`:

    public interface IStateActor : IActor
    {
    	Task InitializeActorAsync(StateType stateType, int numberOfContacts, CancellationToken cancellationToken = default(CancellationToken));
    
    	Task UpdateContactAsync(int contactIndex, Contact contact, CancellationToken cancellationToken = default(CancellationToken));
    }

When you call the `InitializeActorAsync` method with the `stateType == StateType.Wrong`, the actor stores a colletion of `numberOfContact` Contact object in a single dictionary state item, while when you call the method with `stateType == StateType.Right` the actor stores one Contact object in one dictionary state item.

The `UpdateContactAsync` method updates the contact with index equals to `contactIndex` in `stateType == StateType.Wrong`, the actor retrieves the whole collection, changes contact and saves all the list, in `stateType == StateType.Right` the Actor changes only one dictionary state item.

To run the demo, you can run the TestConsole with this command line:
    
    TestConsole.exe statemanager <servuceUri> <actorId> <stateType> <numberOfContact>

Sample: Call the actor with service uri `fabric:/ActorModelDemo/ClientActor`, id "test", with  `stateType == StateType.Wrong` and using 10000 contacts.

    TestConsole.exe statemanagement fabric:/ActorModelDemo/ActorDemo test Right 10000
 

 