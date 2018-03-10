# Demo commands #





## Fire and Forget ##
This command executes the "Fire and Forget" scenarious:

    TestConsole.exe callbackoperation <servuceUri> <actorId> <operationPayload>

The command executes the following steps:

1. Creates a proxy to an actor that exposes the `IClientActor` interface;
2. Calls the `ExecuteOperationAsync` metod of the actor;
3. Makes an infinite loop and calls the `GetStatusAsync` method of the actor.

You can see that the actor is always free to answer and when the longtime operation callback arrives, the actor status changes.  

Sample: Call the actor with service uri fabric:/ActorModelDemo/ClientActor and id "test" and use "Operation payload" as payload for the long time operation.

    TestConsole.exe callbackoperation fabric:/ActorModelDemo/ClientActor test "Operation payload"


----------


## Blocked operation ##
This command shows that an actor who executes a long time operation without return the control to the caller, is blocked:

    TestConsole.exe blockoperation <servuceUri> <actorId> <operationPayload>

The command executes the following steps:

1. Creates a proxy to an actor that exposes the `IClientActor` interface;
2. Creates one task that calls (in an infinite loop, every 10 seconds) the `ExecuteBlockedOperationAsync` method of the actor;
3. Creates 4 tasks that call (in an infinite loop, every second) the `GetStatusAsync` method of the actor;
4. Executes the tasks in parallel.

You can see that, during the `ExecuteBlockedOperationAsync`call, all the other tasks are blocked and wait for the GetStatusAsync response. 

Sample: Call the actor with service uri fabric:/ActorModelDemo/ClientActor and id "test" and use "Operation payload" as payload for the long time operation.

    TestConsole.exe blockoperation fabric:/ActorModelDemo/ClientActor test "Operation payload"
     


----------
   

## State Management ##
This command shows the behaviour of an actor state management:

    TestConsole.exe statemanager <servuceUri> <actorId> <stateType> <numberOfContact>

The command executes the following steps:

1. Creates a proxy to an actor that exposes the `IStateActor` interface;
2. Initializes the actor with the steteType and numberoOfContacts;
3. Makes an infinite loop and calls the `UpdateContactAsync` method of the actor with random contact index.

You can see that, in average, the elapsed time to update a contact for `StateType.Wrong` is more than the elapsed time for `StateType.Right`. 

Sample: Call the actor with service uri `fabric:/ActorModelDemo/ClientActor`, id "test", with  `stateType == StateType.Wrong` and using 10000 contacts.

    TestConsole.exe statemanagement fabric:/ActorModelDemo/ActorDemo test Right 10000
