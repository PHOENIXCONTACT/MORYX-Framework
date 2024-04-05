/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiContentSave, mdiHexagon, mdiSync, mdiUndo } from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import { Button, ButtonGroup, Card, CardBody, CardHeader, ListGroup, ListGroupItem } from "reactstrap";
import ModuleHeader from "../../common/components/ModuleHeader";
import ModulesRestClient from "../api/ModulesRestClient";
import NavigableConfigEditor from "../components/ConfigEditor/NavigableConfigEditor";
import Config from "../models/Config";
import { ConfigUpdateMode } from "../models/ConfigUpdateMode";
import Entry from "../models/Entry";

interface ModuleConfigurationPropModel {
    RestClient?: ModulesRestClient;
    ModuleName: string;
}

interface ModuleConfigurationStateModel {
    ModuleConfig: Config;
    ConfigIsLoading: boolean;
    ParentEntry: Entry;
    CurrentSubEntries: Entry[];
}

function ModuleConfiguration(props: ModuleConfigurationPropModel) {
    const navigate = useNavigate();

    const config = new Config();
    config.module = props.ModuleName;
    config.root = new Entry();

    const [moduleConfig, setModuleConfig] = React.useState<ModuleConfigurationStateModel>({
        ModuleConfig: config,
        ConfigIsLoading: true,
        ParentEntry: null,
        CurrentSubEntries: [],
    });

    React.useEffect(() => {
        loadConfig();
    }, []);

    const loadConfig = (): Promise<void> => {
        return props.RestClient.moduleConfig(props.ModuleName)
            .then((data) => {
                Config.patchConfig(data);
                setModuleConfig({
                    ModuleConfig: data,
                    ParentEntry: data.root,
                    CurrentSubEntries: data.root.subEntries,
                    ConfigIsLoading: false,
                });
            });
    };

    const onApply = (): void => {
        props.RestClient.saveModuleConfig(props.ModuleName, { Config: moduleConfig.ModuleConfig, UpdateMode: ConfigUpdateMode.SaveAndReincarnate })
            .then(() => toast.success("Configuration was saved successfully. Module is restarting...", { autoClose: 5000 }));
    };

    const onSave = (): void => {
        props.RestClient.saveModuleConfig(props.ModuleName, { Config: moduleConfig.ModuleConfig, UpdateMode: ConfigUpdateMode.OnlySave })
            .then(() => toast.success("Configuration was saved successfully", { autoClose: 5000 }));
    };

    const onRevert = (): void => {
        navigate("?");
        setModuleConfig({ ...moduleConfig, ConfigIsLoading: true });
        loadConfig()
            .then(() => toast.success("Configuration was reverted", { autoClose: 3000 }));
    };

    return (
        <Card>
            <CardHeader tag="h2">
                <Icon path={mdiHexagon} className="icon right-space" />
                {props.ModuleName}
            </CardHeader>
            <ListGroup>
                <ListGroupItem className="nav-listgroup-item">
                    <ModuleHeader ModuleName={props.ModuleName} />
                </ListGroupItem>
            </ListGroup>
            <CardBody>
                {moduleConfig.ConfigIsLoading &&
                    <span className="font-bold font-small">Loading config ...</span>
                }
                <NavigableConfigEditor ParentEntry={moduleConfig.ParentEntry}
                    Entries={moduleConfig.CurrentSubEntries}
                    Root={moduleConfig.ModuleConfig.root}
                    IsReadOnly={false} />

                <ButtonGroup className="up-space-lg">
                    <Button color="primary" onClick={() => onApply()}>
                        <Icon path={mdiSync} className="icon-white right-space" />
                        Save &amp; Restart
                    </Button>
                    <Button color="primary" onClick={() => onSave()}>
                        <Icon path={mdiContentSave} className="icon-white right-space" />
                        Save only
                    </Button>
                    <Button color="dark" onClick={() => onRevert()}>
                        <Icon path={mdiUndo} className="icon-white right-space" />
                        Revert
                    </Button>
                </ButtonGroup>
            </CardBody>
        </Card>
    );
}

export default ModuleConfiguration;
