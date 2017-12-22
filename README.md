# C# Events System
Event System is a tool that allows for custom events and custom listeners. By extending and or implementing some simple classes, you have a very powerful events system that can work with any C# .NET program.

## Examples

Basic example of a Event listener. These are how events notify particular parts of your codebase.
```
public class ExampleListener : IEventListener
{
	public ExampleListener()
    {
    	Event.RegisterListener(this);
    }

	@EventHandler
	public void listenForEvent(ExampleEvent event)
    {
    	//Handle your code here
    }
}
```

Basic Example of a Event. These are what can be called to allow you to notify parts of your codebase.
```
public class ExampleEvent : Event
{
	private static EventHandlerContainer _hc = new EventHandlerContainer();
}
```

Using Event Listener Priorities. These can be used so certain listeners can be notifed beforce other listeners of events occuring.
```
public class ExampleListenerWithPriorities : IEventListener
{
	public ExampleListener()
    {
    	Event.RegisterListener(this);
    }

	@EventHandler(priority = EventPriority.Low)
	public void listenerLowPriority(ExampleEvent event)
    {
    	//This will happen last
    }
    
    @EventHandler(priority = EventPriority.Normal)
	public void listenerLowPriority(ExampleEvent event)
    {
    	//This will happen second
    }
    
    @EventHandler(priority = EventPriority.High)
	public void listenerLowPriority(ExampleEvent event)
    {
    	//This will happen first
    }
}
```

Using event cancelling. Cancelling a event will stop it in it's tracks. No further listeners will be called if the event has been cancelled.
```
public class ExampleEvent : Event, ICancellable
{
	private static EventHandlerContainer _hc = new EventHandlerContainer();
    
    public bool Cancelled { get; set; }
}
```

## Motivation

A short description of the motivation behind the creation and maintenance of the project. This should explain **why** the project exists.

## Installation

Provide code examples and explanations of how to get the project.

## Tests

Describe and show how to run the tests with code examples.

## Contributors

Let people know how they can dive into the project, include important links to things like issue trackers, irc, twitter accounts if applicable.

## License

A short snippet describing the license (MIT, Apache, etc.)