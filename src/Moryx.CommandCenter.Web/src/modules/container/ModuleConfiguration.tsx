/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiUndo } from "@mdi/js";
import Button from "@mui/material/Button";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import CircularProgress from "@mui/material/CircularProgress";
import GridLegacy from "@mui/material/GridLegacy";
import SvgIcon from "@mui/material/SvgIcon";
import * as React from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import ModuleHeader from "../../common/components/ModuleHeader";
import ModulesRestClient from "../api/ModulesRestClient";
import NavigableConfigEditor from "../components/ConfigEditor/NavigableConfigEditor";
import DropDownButton from "../components/DropDownButton";
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

    const svgIcon = (path: string) => {
        return (
            <SvgIcon>
                <path d={path} />
            </SvgIcon>
        );
    };

    return (
        <Card>
            <ModuleHeader ModuleName={props.ModuleName} selectedTab="configuration" />
            <CardContent>
                {moduleConfig.ConfigIsLoading &&
                    <CircularProgress />
                }
                <GridLegacy container={true} spacing={1}>
                    <GridLegacy item={true} md={12}>
                    <NavigableConfigEditor ParentEntry={moduleConfig.ParentEntry}
                        Entries={moduleConfig.CurrentSubEntries}
                        Root={moduleConfig.ModuleConfig.root}
                        IsReadOnly={false} />
                        </GridLegacy>
                    <GridLegacy item={true} md={12}>
                        <DropDownButton Buttons={[
                            {Label: "Save + Restart", onClick: onApply},
                            {Label: "Save", onClick: onSave},
                        ]}/>
                        <Button sx={{marginLeft: 1}} color="secondary" variant="outlined" onClick={() => onRevert()} startIcon={svgIcon(mdiUndo)}>
                            Revert
                        </Button>
                    </GridLegacy>
                </GridLegacy>
            </CardContent>
        </Card>
    );
}

export default ModuleConfiguration;
