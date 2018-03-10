# Fire and Forget #

This demo shows how you can implement a simple "Fire and Forget" scenarious.
You must use this kind of scenario when you want to be sure that a long time operation doesn't block any other actor.
The ActorDemo actor implements the interface `IFireAndForget`:

    public interface IFireAndForgetActor : IActor
    {
    	Task DoOperationWithCallbackAsync(ActorReference caller, string operationPayload, CancellationToken cancellationToken = default(CancellationToken));
    }

The ActorReference is used by the ActorDemo clients to communicate to the ActorDemo their own reference in terms of ServiceUri and Id.

The ActorDemo clients can call the `DoOperationWithCallBackAsync` to execute a long time operation but they have to implement the interface `ICallbackActor`

    public interface ICallbackActor : IActor
    {
    	Task CallbackAsync(ActorReference caller, string callbackPayload, CancellationToken cancellationToken = default(CancellationToken));
    }

and the ActorDemo, when it finishs the long time operation, will call back the client actor using the `ActorReference` and the `ICallbackActor`.
Of course, the payloads used in both `IFireAndForgetActor` and `ICallbackActor` may be whatever you need, in this simple scenarious, I use a string.

To run the demo, you can run the TestConsole with this command line:

    TestConsole.exe callbackoperation <servuceUri> <actorId> <operationPayload>

Sample: Call the actor with service uri `fabric:/ActorModelDemo/ClientActor` and id "test" and use "Operation payload" as payload for the long time operation.
	
    TestConsole.exe callbackoperation fabric:/ActorModelDemo/ClientActor test "Operation payload"
 

 