/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

import { mdiViewDashboard } from "@mdi/js";
import Icon from "@mdi/react";
import * as React from "react";
import { connect } from "react-redux";
import { Card, CardBody, CardHeader, Col, Container, Progress, Row } from "reactstrap";
import CommonRestClient from "../../common/api/CommonRestClient";
import ApplicationInformationResponse from "../../common/api/responses/ApplicationInformationResponse";
import ApplicationLoadResponse from "../../common/api/responses/ApplicationLoadResponse";
import HostInformationResponse from "../../common/api/responses/HostInformationResponse";
import WrapPanel from "../../common/components/Panels/WrapPanel";
import kbToString from "../../common/converter/ByteConverter";
import SystemLoadSample from "../../common/models/SystemLoadSample";
import { AppState } from "../../common/redux/AppState";
import { ActionType } from "../../common/redux/Types";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { updateModules } from "../../modules/redux/ModulesActions";
import { CPUMemoryChart } from "../components/CPUMemoryChart";
import { Module } from "../components/Module";

interface DashboardPropModel {
    ApplicationInfo: ApplicationInformationResponse;
    HostInfo: HostInformationResponse;
    ApplicationLoad: ApplicationLoadResponse;
    SystemLoad: SystemLoadSample[];
    Modules: ServerModuleModel[];
    RestClient: CommonRestClient;
}

const mapStateToProps = (state: AppState): DashboardPropModel => {
    return {
        ApplicationInfo: state.Dashboard.ApplicationInfo,
        HostInfo: state.Dashboard.HostInfo,
        ApplicationLoad: state.Dashboard.ApplicationLoad,
        SystemLoad: state.Dashboard.SystemLoad,
        Modules: state.Modules.Modules,
        RestClient: state.Common.RestClient,
    };
};

class Dashboard extends React.Component<DashboardPropModel> {
    private restCallDate: number;

    constructor(props: DashboardPropModel) {
        super(props);

        this.restCallDate = Date.now();
    }

    public calculateMemoryUsagePercentage(): number {
        return Math.round((this.props.ApplicationLoad.workingSet / this.props.ApplicationLoad.systemMemory) * 100.0);
    }

    public preRenderModules(): React.ReactNode {
        return this.props.Modules.map((module, idx) => <Module key={idx} ServerModule={module} />);
    }

    public render(): React.ReactNode {
        return (
            <Card className="component">
                <CardHeader tag="h2">
                    <Icon path={mdiViewDashboard} className="icon right-space" />
                    Dashboard
                </CardHeader>
                <CardBody>
                    <Container fluid={true}>
                        <Row>
                            <Col md={4}>
                                <h3>Application</h3>
                                <Container fluid={true}>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Name:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.ApplicationInfo.assemblyProduct}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Description:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.ApplicationInfo.assemblyDescription}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Version:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.ApplicationInfo.assemblyVersion} ({this.props.ApplicationInfo.assemblyInformationalVersion})</span></Col>
                                    </Row>
                                    <Row style={{paddingTop: "15px"}}>
                                        <Col md={4}><span className="font-bold font-small">CPU usage:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.ApplicationLoad.cpuLoad}%</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={12}><Progress value={this.props.ApplicationLoad.cpuLoad} /></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Memory usage:</span></Col>
                                        <Col md={8}><span className="font-small">{kbToString(this.props.ApplicationLoad.workingSet)} ({kbToString(this.props.ApplicationLoad.systemMemory)})</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={12}><Progress value={this.calculateMemoryUsagePercentage()} /></Col>
                                    </Row>
                                </Container>
                            </Col>
                            <Col md={3}>
                                <h3>Server</h3>
                                <Container fluid={true}>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Hostname:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.HostInfo.machineName}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">OS:</span></Col>
                                        <Col md={8}><span className="font-small">{this.props.HostInfo.osInformation}</span></Col>
                                    </Row>
                                    <Row>
                                        <Col md={4}><span className="font-bold font-small">Up since:</span></Col>
                                        <Col md={8}><span className="font-small">{new Date(this.restCallDate - this.props.HostInfo.upTime).toLocaleString()}</span></Col>
                                    </Row>
                                </Container>
                            </Col>
                            <Col md={5}>
                                <h3>CPU &amp; Memory</h3>
                                <CPUMemoryChart Samples={this.props.SystemLoad} />
                            </Col>
                        </Row>
                        <Row style={{marginTop: "10px"}}>
                            <Col md={12}>
                                <h3>Modules</h3>
                                <WrapPanel>
                                    {this.preRenderModules()}
                                </WrapPanel>
                            </Col>
                        </Row>
                    </Container>
                </CardBody>
            </Card>
        );
    }
}

export default connect<DashboardPropModel>(mapStateToProps)(Dashboard);
