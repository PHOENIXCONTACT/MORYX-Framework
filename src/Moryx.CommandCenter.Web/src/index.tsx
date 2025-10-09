/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { createTheme, ThemeProvider } from "@mui/material/styles";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { createRoot } from "react-dom/client";
import { Provider } from "react-redux";
import { HashRouter } from "react-router-dom";
import { createStore } from "redux";
import App from "./common/container/App";
import { AppState, getAppReducer } from "./common/redux/AppState";
import { ActionType } from "./common/redux/Types";

const theme = createTheme({
    palette: {
        primary: {
          main: "#24959e",
          light: "#c2e1e4",
          contrastText: "#fff",
        },
        secondary: {
          main: "#249e75",
          light: "#e1f3ee"
        },
      },
  });

const store = createStore<AppState, ActionType<{}>, any, any>(getAppReducer);

const container = document.getElementById("app");
const root = createRoot(container);
root.render(
    <Provider store={store}>
        <HashRouter>
            <ThemeProvider theme={theme}>
                <App />
            </ThemeProvider>
        </HashRouter>
    </Provider>
);
