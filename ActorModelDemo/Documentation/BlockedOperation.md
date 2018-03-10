# Blocked operation #

This demo shows you whats happened when an actor method implements a long time operation without return control to the caller.

The ActorDemo actor implements the interface IFireAndForget:

    public interface IBlockActor : IActor
    {
    	Task<string> DoLongTimeOperationAsync(string operationPayload, CancellationToken cancellationToken = default(CancellationToken));
    }


The implementation of this interface simply wait for 30 seconds and then return a result to the caller.

To run the demo, you can run the TestConsole with this command line:
    
    TestConsole.exe blockoperation <servuceUri> <actorId> <operationPayload>

Sample: Call the actor with service uri fabric:/ActorModelDemo/ClientActor and id "test" and use "Operation payload" as payload for the long time operation.

    TestConsole.exe blockoperation fabric:/ActorModelDemo/ClientActor test "Operation payload"
 

 