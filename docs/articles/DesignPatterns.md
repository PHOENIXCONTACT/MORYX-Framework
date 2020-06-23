---
uid: DesignPatterns
---
# Design Patters

This should be an overview of design patterns and how they will be used in MarvinPlatform applications. The Platform will provide several base classes, helper, structures and implementations to support different patterns.

This should be no *me too* documentation. So the focus is on the implementation and usage with the MarvinPlatform.

A good starting point is a github project where a community of developers provide a detailed description with abstract samples to implement design patterns: [Design-Patterns-for-Humans](https://github.com/kamranahmedse/design-patterns-for-humans). Other sources can also be used. The world wide web is full with tons of samples and implementations. To implement the patters all on the same way that every colleagues will understand the code, please read the next pages.

## Behavioral Design Patterns

### State

*The state pattern is a behavioral software design pattern that implements a state machine in an object-oriented way. With the state pattern, a state machine is implemented by implementing each individual state as a derived class of the state pattern interface, and implementing state transitions by invoking methods defined by the pattern's superclass. The state pattern can be interpreted as a strategy pattern which is able to switch the current strategy through invocations of methods defined in the pattern's interface.* (Please read the [wikipedia article](https://en.wikipedia.org/wiki/State_pattern) carefully.)

The MARVIN Platform provides some base classes and interfaces in the [Marvin.StateMachines](xref:Marvin.StateMachines)-Namespace. The general interface of a state context is the [IStateContext](xref:Marvin.StateMachines.IStateContext). It represents the context object of the state machine and provides a `SetState` method to provide the next state. The base interface of a state is the [IState](xref:Marvin.StateMachines.IState). All states should derive from this interface. It provides actions like `OnEnter` and `OnExti` to indicate the entering and exiting of a state. For the [IState](xref:Marvin.StateMachines.IState) a base class exists. The base class will handle all the actions of the state machine.

In the past we implemented the states by hand. A base state implemented a `NextState()` method which created a new state depending on the given argument. This leaded to big switch- or if-blocks. With every state change a new object instance was created which extended the workload of the garbage collector. To reduce the amount of boilerplate code we implemented a dictionary with the state name as key and a creation delegate as value to create a state like this: `var state = Map[name]`. 
This was nice because the instanciation of states was done only when the state machine was created. The problem with this idea was that it leads to Copy/Paste and much manual work for big state machines. If some state had been changed, the map had to be changed too.

**Old Version 1**

````cs
private static readonly Dictionary<string, StateConstructor> StateConstructors = new Dictionary<string, StateConstructor>
{
    {StateDisconnected, context => new DisconnectedState(context)},
    {StateConnecting, context => new ConnectingState(context)},
    ...
};

protected const int StateDisconnected = 10;
protected const int StateConnecting = 20;
````

**Old Version 2**

````cs
public static MyStateBase Create(PhoenixPlcDriver driver)
{
    var map = new StateMap();
    map[StateDisconnected] = new DisconnectedState(driver, map);
    map[StateConnecting] = new ConnectingState(driver, map);
    return (MyStateBase) map[StateDisconnected];
}

protected const int StateDisconnected = 10;
protected const int StateConnecting = 20;

````

The last step leading to the current implementation was to try to remove the manual work (Boilerplate Code). There is no need to implement the state map with every new implementation again. In General we know all states because all states derive from base class and have a unique identifier defined by constant `integer`. The complete statemap can be instantiated by reflection. The fields can be attributed with the needed type information to reduce the reflection work to a minumum. 

The next example shows a simple state machine and a simple context. The context initializes the state machine (described later) and selects the initial state. If the state machine will change the state, the `SetState`-Method will be called. We save it into a private field to access it in the next methods. The class is holding a `_currentValue` which is simply an `int`. In the `DoSomething`-Method the state machine is called. `AtoB` moves the state machine from the A-State to B-State and calls `HandleAtoB` on the context. `HandleAtoB` sets the current value to `10`. The next call is the `BToA`. Like before, the A-State will be set and `HandleBtoA` will be called.

````cs
public class MyContext : IStateContext
{
    private MyStateBase _state;
    private int _currentValue;

    void IStateContext.SetState(IState state)
    {
        _state = (MyStateBase)state;
    }

    public void Initial()
    {
        StateMachine.Initialize<MyStateBase>(this); 
        //State is not Initial = A
        _currentValue = 0;
    }

    public void DoSomething()
    {
        // State is now B and HandleAtoB was called
        _state.AtoB();
        Console.WriteLine("_currentValue is 10");

        // State is now A and HandleBtoA was called
        _state.BtoA();
        Console.WriteLine("_currentValue is 20");

        // This will throw a InvalidOperationException
        // because the state machine is in state A
        _state.BtoA();
    }

    internal void HandleAtoB() => _currentValue = 10;
    internal void HandleBtoA() => _currentValue = 20;
}
````

The base class of the state should derive from `StateBase<TContext>` while `TContext` is a type of [IStateContext](xref:Marvin.StateMachines.IStateContext). With the given type the `abstract class` is fully typed. 

The states are defined for the base class by adding the Attributes [StateDefinitionAttribute](xref:Marvin.StateMachines.StateDefinitionAttribute) above the `protected const int` Definition. The attribute defines the type of the regarding state. Additionally it have a property to set the initial state of the machine. Only one state can be the initial.

Every transition of the state machine musst be provided as a `virtual`-Method which will be overwritten by the specialized state.

````cs
public abstract class MyStateBase : StateBase<MyContext>
{
    protected MyStateBase(MyContext context, StateMap stateMap)
        : base(context, stateMap)
    {
    }

    public virtual void AtoB()
    {
        InvalidState();
    }

    public virtual void BtoA()
    {
        InvalidState();
    }

    [StateDefinition(typeof(AState), IsInitial = true)]
    protected const int StateA = 10;

    [StateDefinition(typeof(BState))]
    protected const int StateB = 20;
}
````

Let us have a look on a state implementation:

````cs
internal sealed class AState : MyStateBase
{
    public AState(MyContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    public override void AtoB()
    {
        NextState(StateB);
        Context.HandleAtoB();
    }
}
````

````cs
internal sealed class BState : MyStateBase
{
    public BState(MyContext context, StateMap stateMap) : base(context, stateMap)
    {
    }

    public override void BtoA()
    {
        NextState(StateA);
        Context.HandleBtoA();
    }
}
````

The parameters of the constructor will be given to the base. 

The `NextState()`-Method is implemented as a `virtual` method in the base class. It will select the next state using the internal state map and executes the `IState.OnExit()` by leaving the state and `IState.OnEnter()` by entering the next state.

**Initialze the machine**

The state machine will be initialized with the static helper class [StateMachine](xref:Marvin.StateMachines.StateMachine). It will use the base class of the machine and the instance of the context.

````cs
private void InitializeMachine()
{
    StateMachine.Initialize<MyStateBase>(this); 
}
````

**Persist and reload the machine**

The state machine will be reloaded with the static helper class [StateMachine](xref:Marvin.StateMachines.StateMachine). It will use the base class of the machine and the instance of the context.
Additionally a `stateKey` can be given to the method.

````cs
private void ReloadMachine() 
{
    // Get the key of current state
    var key = StateMachine.GetKey(_stat);

    // ... save key to db or somewhere else

    // Reload the state with the saved key
    StateMachine.Reload<MyStateBase>(this, key);
} 
````

**Force a machine to a specific state**

Sometimes it is required to force a specific state machine state for debugging purposes.
It is possible to force the state machine with the static helper class [StateMachine](xref:Marvin.StateMachines.StateMachine):

````cs
private void ForceMachine()
{
    StateMachine.Force(context.State, MyStateBase.StateA);
}
````

The default overload of `Force` does not call `OnExit` of the given state nor calling the `OnEnter` of the forced state. 
A specialized overload for this can be used:

````cs
private void ForceMachine()
{
    StateMachine.Force(context.State, MyStateBase.StateA, exitCurrent: true, enterForced: false);
}
````

Please open the class documentation to see all functions of the [StateMachine](xref:Marvin.StateMachines.StateMachine) class.

**Some rules:**

- The state base class must be abstract!
- There musst be at least one constant integer attributed with the `StateDefinitionAttribute` upon the definition of the integer constant!
- State types Defintions should be defined only once!
- At least only ONE state must be flagged as `IsInitial = true`!
- Initial key while reloading must exist

**-> If One of the rules is broken the intialze method of the StateMachine helper class  will throw an exception !**
