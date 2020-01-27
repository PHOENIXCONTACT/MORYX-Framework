---
uid: BusinessLogicWithRedux
---
# Business logic with Redux

In the typical UI world you have different data to operate on and visualize. In bigger applications you need to organize this data properly to share between views. Another benefit should be a notification mechanism that triggers data changes automatically.

In our environment we use the Redux library to maintain applications data. The data is saved within so called states and is manipulated via `Reducers` which get called via `Actions`.

Let's have a look an a small example how the minimal Redux setup looks like. First we define the action part:

```js
// The update action name for the time
export const UPDATE_TIME = "UPDATE_TIME";

// Action function that creates the update action object. The update action object contains the action name and the payload data
// Note that ActionType is defined in in 'Marvin.Runtime.Maintenance.Web.UI' and not part of Redux. It simplifies our action calls in combination with
// TypeScript
export function updateTime(time: Date): ActionType<Date> {
    return { type: UPDATE_TIME, payload: time };
}
```

Then we proceed with the reducer:

```js
// Type definition of our state
export interface IApplicationState {
    Time: Date;
}

// Everything needs a nucleus. That's the data base on which our application will start until updated data is available.
export const initialApplicationState: IApplicationState = {
    Time: Date.parse(Date.now()),
};

// Last but not least we implement the reducer. The reducer manipulates the state and return it as a new state.
export function getApplicationReducer(state: IApplicationState = initialApplicationState, action: ActionType<{}>): IApplicationState {
  switch (action.type) {
    case UPDATE_TIME:
    {
        return { ...state, Time: action.payload as Date };
    }
  }
  return state;
}
```

The next lines puts the reducer into operation. Note that nested states are also possible.

```js
const store = createStore<IApplicationState>(getApplicationReducer, initialApplicationState);

ReactDOM.render(
    <Provider store={store}>
        <HashRouter>
            <App />
        </HashRouter>
    </Provider>
    ,
    document.getElementById("app"),
);
```

So, the reducer and its action is implemented, the state is registered to your application, now we want to use it. This is achieved via injection. The injection is done by the `connect` function of Redux. A common mistake here is not to use the default export of the connect function rather the implementation itself. So be careful to use the right instance.

```js
// Properties that get injected by Redux or from outside
interface IAppPropsModel {
    Time?: Date;
}

// Update functions if neccessary
interface IAppDispatchPropModel {
    onUpdateDatabaseConfigs?: (databaseConfigs: DataModel[]) => void;
}

// Map the state properties to the local property
const mapStateToProps = (state: IAppState): IDatabasesPropsModel => {
    return {
        Time: state.Time,
    };
};

// Map update functions to the corresponding dispatch function
const mapDispatchToProps = (dispatch: Dispatch<ActionType<{}>>): IAppDispatchPropModel => {
    return {
        onUpdateTime: (time: Date) => dispatch(updateTime(time)),
    };
};

class App extends React.Component<IAppPropsModel & IDatabasesDispatchPropModel> {
    constructor(props: IDatabasesDispatchPropModel) {
        super(props);
        this.state = { };
    }

    private doClockUpdate() {
        // Call Redux to update the state with a new time
        this.props.onUpdateTime(Date.parse(Date.now));
    }

    public render() {
        // show the time and update the time on a click
        return (<span onClick={this.doClockUpdate.bind(this)}>this.props.Time</span>)
    }
}

// Wire all together. Be sure that you use this implementation.
export default connect<IAppPropsModel, IDatabasesDispatchPropModel>(mapStateToProps, mapDispatchToProps)(App);
```

That's it. If you click on the `span` an updated time will be passed to the state and Redux will trigger the render process and the new time will be shown.