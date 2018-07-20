import { faSitemap } from "@fortawesome/fontawesome-free-solid";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import * as React from "react";
import { Link } from "react-router-dom";
import { Card, CardBody, CardHeader, Col, Container, Progress, Row } from "reactstrap";
import ServerModuleModel from "../../modules/models/ServerModuleModel";
import { HealthStateToCssClassConverter } from "../converter/HealthStateToCssClassConverter";
import { HealthStateBadge } from "./HealthStateBadge";

interface ModulePropsModel {
    ServerModule: ServerModuleModel;
}

export class Module extends React.Component<ModulePropsModel, {}> {
    private healthStateCssConverter: HealthStateToCssClassConverter;

    constructor(props: ModulePropsModel) {
        super(props);
        this.state = {};
    }

    public render(): React.ReactNode {
        const stateCssClass = HealthStateToCssClassConverter.Convert(this.props.ServerModule.HealthState);

        return (
            <div className="modulebox-container">
                <Card className="modulebox">
                    <CardHeader tag="h3" className={"bg-" + stateCssClass.Background}>
                        <Link to={"/modules/" + this.props.ServerModule.Name} className={stateCssClass.Foreground}>
                            <FontAwesomeIcon icon={faSitemap} className="right-space" />
                            {this.props.ServerModule.Name}
                        </Link>
                    </CardHeader>
                    <CardBody className="modulebox-body">
                        <Container fluid={true}>
                            <Row>
                                <Col md={4}><span className="font-bold font-small">State:</span></Col>
                                <Col md={8}><span className="font-small"><HealthStateBadge HealthState={this.props.ServerModule.HealthState} /></span></Col>
                            </Row>
                            <Row>
                                <Col md={4}><span className="font-bold font-small">Bundle:</span></Col>
                                <Col md={8}><span className="font-small font-italic">{this.props.ServerModule.Assembly.Bundle}</span></Col>
                            </Row>
                            <Row>
                                <Col md={4}><span className="font-bold font-small">Assembly:</span></Col>
                                <Col md={8}><span className="font-small font-italic">{this.props.ServerModule.Assembly.Name}</span></Col>
                            </Row>
                            <Row>
                                <Col md={4}><span className="font-bold font-small">Version:</span></Col>
                                <Col md={8}><span className="font-small font-italic">{this.props.ServerModule.Assembly.Version}</span></Col>
                            </Row>
                        </Container>
                    </CardBody>
                </Card>
            </div>
        );
    }
}
