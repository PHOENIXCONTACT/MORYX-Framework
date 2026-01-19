/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import CircularProgress from "@mui/material/CircularProgress";
import GridLegacy from "@mui/material/GridLegacy";
import List from "@mui/material/List";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemText from "@mui/material/ListItemText";
import * as React from "react";
import { connect } from "react-redux";
import ModuleHeader from "../../common/components/ModuleHeader";
import { updateShowWaitDialog } from "../../common/redux/CommonActions";
import { ActionType } from "../../common/redux/Types";
import ModulesRestClient from "../api/ModulesRestClient";
import ConsoleMethodConfigurator from "../components/ConsoleMethodConfigurator";
import ConsoleMethodResult from "../components/ConsoleMethodResult";
import Config from "../models/Config";
import Entry from "../models/Entry";
import { EntryValueType } from "../models/EntryValueType";
import MethodEntry from "../models/MethodEntry";

interface ModuleConsolePropModel {
    RestClient?: ModulesRestClient;
    ModuleName: string;
}

interface ModuleConsoleDispatchPropsModel {
    onShowWaitDialog?(showWaitDialog: boolean): void;
}

const mapDispatchToProps = (dispatch: React.Dispatch<ActionType<{}>>): ModuleConsoleDispatchPropsModel => {
    return {
        onShowWaitDialog: (showWaitDialog: boolean) => dispatch(updateShowWaitDialog(showWaitDialog)),
    };
};

interface InvokeResult {
    MethodName: string;
    Result: Entry;
}

interface ModuleConsoleStateModel {
    IsLoading: boolean;
    Methods: MethodEntry[];
    SelectedMethod: MethodEntry;
    InvokeResults: InvokeResult[];
    Seed: number;
}

class ModuleConsole extends React.Component<ModuleConsolePropModel & ModuleConsoleDispatchPropsModel, ModuleConsoleStateModel> {
    constructor(props: ModuleConsolePropModel & ModuleConsoleDispatchPropsModel) {
        super(props);

        this.state = { IsLoading: false, SelectedMethod: null, Methods: [], InvokeResults: [], Seed: 0 };
    }

    public componentDidMount(): void {
        this.onReset();
    }

    private onReset(): void {
        this.setState({ Methods: [], SelectedMethod: null, IsLoading: true, InvokeResults: [] });
        this.props.RestClient.moduleMethods(this.props.ModuleName).then((data) => this.onModuleMethodsLoaded(data));
    }

    private onModuleMethodsLoaded(methods: MethodEntry[]): void {
        methods.forEach((element: MethodEntry) => {
            element.parameters.subEntries.forEach((parameter: Entry) => {
                Config.patchParent(parameter, element.parameters);
                Entry.generateUniqueIdentifiers(parameter);
            });
        });

        this.setState({ Methods: methods, IsLoading: false });
    }

    private onSelectMethod(methodEntry: MethodEntry): void {
        this.setState({ SelectedMethod: methodEntry, Seed: this.state.Seed + 1 });
    }

    private onInvokeMethod(methodEntry: MethodEntry): void {
        this.props.onShowWaitDialog(true);

        this.props.RestClient.invokeMethod(this.props.ModuleName, methodEntry).then((data) => {
            this.props.onShowWaitDialog(false);

            let resultEntry = data;

            switch (resultEntry.value.type) {
                case EntryValueType.Byte:
                case EntryValueType.Boolean:
                case EntryValueType.Int16:
                case EntryValueType.UInt16:
                case EntryValueType.Int32:
                case EntryValueType.UInt32:
                case EntryValueType.Int64:
                case EntryValueType.UInt64:
                case EntryValueType.Single:
                case EntryValueType.Double:
                case EntryValueType.String:
                case EntryValueType.Enum:
                    const simpleValueEntry = new Entry();
                    simpleValueEntry.subEntries.push(resultEntry);
                    resultEntry = simpleValueEntry;
                    break;
                case EntryValueType.Collection:
                case EntryValueType.Class:
                    Config.patchParent(resultEntry, null);
                    break;
                default:
                    resultEntry = new Entry();
            }

            this.onUpdateInvokeResults({ MethodName: methodEntry.name, Result: resultEntry });
        });
    }

    private onUpdateInvokeResults(invokeResult: InvokeResult): void {
        this.setState((prevState) => ({ InvokeResults: [...prevState.InvokeResults.filter((r) => r.MethodName !== invokeResult.MethodName), invokeResult] }));
    }

    private invokeResult(methodName: string): InvokeResult {
        return this.state.InvokeResults.find((r: InvokeResult) => r.MethodName === methodName);
    }

    private resetInvokeResult(methodName: string): void {
        this.setState((prevState) => ({ InvokeResults: [...prevState.InvokeResults.filter((r) => r.MethodName !== methodName)] }));
    }

    private preRenderFunctions(): React.ReactNode {
        return (
            <Card variant="outlined" style={{paddingBottom: 0}}>
                <List dense={true}>
                    <ListItemButton
                        onClick={() => this.onReset()}
                        divider={true}>
                        <ListItemText primary="Reset" sx={{color: "secondary.dark"}}/>
                    </ListItemButton>

                    {this.state.Methods.map((methodEntry, idx) => {
                        return [
                            <ListItemButton
                                key={idx}
                                onClick={() => this.onSelectMethod(methodEntry)}
                                selected={this.state.SelectedMethod === methodEntry}
                                divider={idx < this.state.Methods.length - 1}
                                >
                                <ListItemText secondary={false} primary={methodEntry.displayName}/>
                            </ListItemButton>,
                        ];
                    })
                    }
                </List>
            </Card>
        );
    }

    public render(): React.ReactNode {
        let content = (<span className="font-italic">No exported methods found.</span>);

        if (this.state.Methods.length > 0) {
            let view = <ConsoleMethodConfigurator
                key={this.state.Seed}
                Method={this.state.SelectedMethod}
                ModuleName={this.props.ModuleName}
                onInvokeMethod={this.onInvokeMethod.bind(this)} />;

            if (this.state.SelectedMethod != null) {
                const invokeResult = this.invokeResult(this.state.SelectedMethod.name);

                if (invokeResult != null) {
                    view = <ConsoleMethodResult Method={this.state.SelectedMethod}
                        InvokeResult={invokeResult.Result}
                        onResetInvokeResult={this.resetInvokeResult.bind(this)} />;
                }
            }

            content = (
                <GridLegacy container={true} spacing={2}>
                    <GridLegacy item={true} md={4}>
                        {this.preRenderFunctions()}
                    </GridLegacy>
                    <GridLegacy item={true}  md={8}>
                        {view}
                    </GridLegacy>
                </GridLegacy>
            );
        }

        return (
            <Card>
                <ModuleHeader ModuleName={this.props.ModuleName} selectedTab="console" />

                <CardContent>
                    {this.state.IsLoading ? (
                        <CircularProgress />
                    ) : (
                        content
                    )}
                </CardContent>
            </Card>
        );
    }
}

export default connect<{}, ModuleConsoleDispatchPropsModel>(null, mapDispatchToProps)(ModuleConsole);
