import createHashHistory from "history/createHashHistory";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { Provider } from "react-redux";
import { HashRouter, Router } from "react-router-dom";
import { ConnectedRouter, routerMiddleware, routerReducer } from "react-router-redux";
import { applyMiddleware, combineReducers, compose, createStore } from "redux";
import App from "./common/container/App";
import { AppState, getAppReducer, initialAppState } from "./common/redux/AppState";

export const history = createHashHistory();
const store = createStore<AppState>(getAppReducer, initialAppState);

ReactDOM.render(
    <Provider store={store}>
        <HashRouter>
            <App />
        </HashRouter>
    </Provider>
    ,
    document.getElementById("app"),
);
