---
uid: ContainerAndComponents
---
# Container &amp; Components

`Marvin.Runtime.Maintenance.Web.UI` follows the same approach as intended by React. Typical React applications are divided in containers and components.

## Container

A container in React means a layouting unit to arrange several components. Typically a container hosts many components and sub containers. A container is comparable to a simple view in `WPF`. Only in containers interaction between the data layer is allowed.

## Component

A component is like a control. It does not contain any container and it has no connection to the data layer. It is closed and for a special purpose.

## How to differentiate

The difference between containers and components is not given in code. Both of them usually inherit from `React.Component`. It's part of React's concept to achieve a better reusability and testability. In the further course of this article we use `component` as synonym for container and components, because technically they are the same but not conceptuelly.

## The component state

Every React component has so called `props` which are part of the constructor parameters. These props have to be seen as immutable from inside your component but not from outside. A change of a property will force a rerendering of your component. In a certain way we can speak of `dependency injection`.

Now let's talk how React treats data within a component that is a viewable part of that component. Every component has a state object that tiggers the render process. Not all data of your component has to be part of the state object just the data that influences the visual representation. If you then change the state object with e.g.

```js
// default way to manipulate the state object
this.setState({textcolor: "red"});

// alternative way
this.setState((prevState) => {textcolor: "red"});
```

then your component will be rerendered. React is doing a partial rendering of the parts that has been changed.

## A word to the render function

A `React.Component` implementation has also a `render()` function which gets called on every render call. As mentioned before a rerender is triggered when the state or an injected property was changed.

Example of a simple render function implementation. We assume that `MyValue` was added to the state.

```js
public render() {
    return (<span>{this.state.MyValue}</span>);
}
```
