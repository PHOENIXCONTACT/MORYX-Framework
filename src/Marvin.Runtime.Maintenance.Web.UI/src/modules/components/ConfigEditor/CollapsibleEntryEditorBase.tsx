import * as React from "react";
import { Collapse } from "reactstrap";
import Entry from "../../models/Entry";

export interface CollapsibleEntryEditorBasePropModel {
    Entry: Entry;
    IsExpanded: boolean;
    IsReadOnly: boolean;
    Root: Entry;
    navigateToEntry(entry: Entry): void;
}

export default class CollapsibleEntryEditorBase<T> extends React.Component<CollapsibleEntryEditorBasePropModel, T> {
    constructor(props: CollapsibleEntryEditorBasePropModel) {
        super(props);
    }

    public render(): React.ReactNode {
        return (
            <div>
                <Collapse isOpen={this.props.IsExpanded}>
                    {this.props.children}
                </Collapse>
            </div>
        );
    }
}
