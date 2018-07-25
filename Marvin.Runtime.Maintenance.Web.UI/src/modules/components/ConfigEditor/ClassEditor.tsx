import * as React from "react";
import { Button, ButtonGroup, Col, Collapse, Container, DropdownItem, DropdownMenu, DropdownToggle, Row, UncontrolledDropdown } from "reactstrap";
import CollapsibleEntryEditorBase, { CollapsibleEntryEditorBasePropModel } from "./CollapsibleEntryEditorBase";
import ConfigEditor from "./ConfigEditor";

interface ClassEditorStateModel {
}

export default class ClassEditor extends CollapsibleEntryEditorBase<ClassEditorStateModel> {
    constructor(props: CollapsibleEntryEditorBasePropModel) {
        super(props);
        this.state = { };
    }

    public preRenderConfigEditor(): React.ReactNode {
        return <ConfigEditor ParentEntry={this.props.Entry}
                             Entries={this.props.Entry.SubEntries}
                             RootEntries={this.props.RootEntries}
                             navigateToEntry={this.props.navigateToEntry}
                             IsReadOnly={this.props.IsReadOnly} /> as React.ReactNode;
    }

    public render(): React.ReactNode {
        return (
            <div>
                <Collapse isOpen={this.props.IsExpanded}>
                    <Container fluid={true} className="no-padding">
                        {this.preRenderConfigEditor()}
                    </Container>
                </Collapse>
            </div>
        );
    }
}
