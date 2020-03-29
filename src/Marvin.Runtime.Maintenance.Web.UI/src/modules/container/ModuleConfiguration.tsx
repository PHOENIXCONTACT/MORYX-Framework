/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiCogs, mdiContentSave, mdiSync, mdiUndo} from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import NotificationSystem = require("react-notification-system");
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Button, ButtonGroup, Card, CardBody, CardHeader, Col, Container, Row } from "reactstrap";
import ModulesRestClient from "../api/ModulesRestClient";
import NavigableConfigEditor from "../components/ConfigEditor/NavigableConfigEditor";
import Config from "../models/Config";
import { ConfigUpdateMode } from "../models/ConfigUpdateMode";
import Entry from "../models/Entry";

interface ModuleConfigurationPropModel {
    RestClient?: ModulesRestClient;
    ModuleName: string;
    NotificationSystem: NotificationSystem.System;
}

interface ModuleConfigurationStateModel {
    ModuleConfig: Config;
    ConfigIsLoading: boolean;
    ParentEntry: Entry;
    CurrentSubEntries: Entry[];
}

class ModuleConfiguration extends React.Component<ModuleConfigurationPropModel & RouteComponentProps<{}>, ModuleConfigurationStateModel> {
    constructor(props: ModuleConfigurationPropModel & RouteComponentProps<{}>) {
        super(props);

        const config = new Config();
        config.Module = this.props.ModuleName;
        config.Root = new Entry();

        this.state = {
            ModuleConfig: config,
            ConfigIsLoading: true,
            ParentEntry: null,
            CurrentSubEntries: [],
        };
    }

    public componentDidMount(): void {
        this.loadConfig();
    }

    public loadConfig(): Promise<void> {
        return this.props.RestClient.moduleConfig(this.props.ModuleName)
                             .then((data) => {
                                 Config.patchConfig(data);
                                 this.setState(
                                    {
                                        ModuleConfig: data,
                                        ParentEntry: data.Root,
                                        CurrentSubEntries: data.Root.SubEntries,
                                        ConfigIsLoading: false,
                                    });
                                });
    }

    public onApply(): void {
        this.props.RestClient.saveModuleConfig(this.props.ModuleName, { Config: this.state.ModuleConfig, UpdateMode: ConfigUpdateMode.SaveAndReincarnate })
                             .then((result) => this.props.NotificationSystem.addNotification({ title: "Saved", message: "Configuration was saved successfully. Module is restarting...", level: "success", autoDismiss: 5 }));
    }

    public onSave(): void {
        this.props.RestClient.saveModuleConfig(this.props.ModuleName, { Config: this.state.ModuleConfig, UpdateMode: ConfigUpdateMode.OnlySave })
                             .then((result) => this.props.NotificationSystem.addNotification({ title: "Saved", message: "Configuration was saved successfully", level: "success", autoDismiss: 5 }));
    }

    public onRevert(): void {
        this.props.history.push("?");
        this.setState({ ConfigIsLoading: true });
        this.loadConfig()
            .then((result) => this.props.NotificationSystem.addNotification({ title: "Reverted", message: "Configuration was reverted", level: "success", autoDismiss: 3 }));
    }

    public render(): React.ReactNode {
        return (
            <Card>
                <CardHeader tag="h2">
                    <Icon path={mdiCogs} className="icon right-space" />
                    {this.props.ModuleName} - Configuration
                </CardHeader>
                <CardBody>
                    {this.state.ConfigIsLoading &&
                        <span className="font-bold font-small">Loading config ...</span>
                    }
                    <NavigableConfigEditor ParentEntry={this.state.ParentEntry}
                                           Entries={this.state.CurrentSubEntries}
                                           Root={this.state.ModuleConfig.Root}
                                           IsReadOnly={false}
                                           History={this.props.history}
                                           Location={this.props.location} />

                    <ButtonGroup className="up-space-lg">
                        <Button color="primary" onClick={() => this.onApply()}>
                            <Icon path={mdiSync} className="icon-white right-space" />
                            Save &amp; Restart
                        </Button>
                        <Button color="primary" onClick={() => this.onSave()}>
                            <Icon path={mdiContentSave} className="icon-white right-space" />
                            Save only
                        </Button>
                        <Button color="dark" onClick={() => this.onRevert()}>
                            <Icon path={mdiUndo} className="icon-white right-space" />
                            Revert
                        </Button>
                    </ButtonGroup>
                </CardBody>
            </Card>
        );
    }
}

export default withRouter<ModuleConfigurationPropModel & RouteComponentProps<{}>>(ModuleConfiguration);
