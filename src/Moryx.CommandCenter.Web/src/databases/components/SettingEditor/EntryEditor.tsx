/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
 */

import { Action, History, Location, UnregisterCallback } from "history";
import * as qs from "query-string";
import * as React from "react";
import { Button, ButtonGroup, Col, Container, Row } from "reactstrap";
import Input from "reactstrap/lib/Input";
import BooleanEditor from "../../../modules/components/ConfigEditor/BooleanEditor";
import EnumEditor from "../../../modules/components/ConfigEditor/EnumEditor";
import NumberEditor from "../../../modules/components/ConfigEditor/NumberEditor";
import StringEditor from "../../../modules/components/ConfigEditor/StringEditor";
import Entry from "../../../modules/models/Entry";
import { EntryValueType } from "../../../modules/models/EntryValueType";
import NavigableEntry from "./NavigableEntry";

interface EntryEditorPropModel {
  Entry: Entry;
  IsReadOnly: boolean;
}

interface EntryEditorStateModel {
  Entry: Entry;
}

export default class EntryEditor extends React.Component<
  EntryEditorPropModel,
  EntryEditorStateModel
> {
  public unregisterListener: UnregisterCallback;
  private possibleListItemTypes: string[] | undefined | null;
  private selectedListItemType: string | undefined;
  private prototypes: Entry[] | undefined | null;

  constructor(props: EntryEditorPropModel) {
    super(props);
    if (this.props.Entry.value.type === "Collection") {
      this.possibleListItemTypes = this.props.Entry.value.possible;
      this.prototypes = this.state.Entry.prototypes;

      if (
        this.props.Entry.value.possible !== undefined &&
        this.props.Entry.value.possible !== null &&
        this.props.Entry.value.default !== null
      ) {
        this.selectedListItemType = this.props.Entry.value.default;
      }
    }
  }

  public componentWillUnmount(): void {
    if (this.unregisterListener !== undefined) {
      this.unregisterListener();
    }
  }

  public onDeleteListItem(entry: Entry) {
    if (
      this.props.Entry.subEntries !== undefined &&
      this.props.Entry.subEntries !== null
    ) {
      var index = this.props.Entry.subEntries.findIndex(
        (c) => c.identifier === entry.identifier
      );
      if (index > -1) {
        this.props.Entry.subEntries.splice(index, 1);
      }
    }
  }

  private createInputBasedOnType(entry: Entry): React.ReactNode {
    return entry.subEntries.map((subentry, idx) => {
      if (subentry.value.type === EntryValueType.Boolean) {
        return (
          <Row className="up-space">
            <Col md={12}>
              <BooleanEditor
                Entry={subentry}
                IsReadOnly={subentry.value.isReadOnly}
              />
            </Col>
          </Row>
        );
      } else if (
        (subentry.value?.possible === null ||
          subentry.value?.possible === undefined) &&
        EntryValueType.String === subentry.value?.type
      ) {
        return (
          <Row className="up-space">
            <Col md={12}>
              <StringEditor
                Entry={subentry}
                IsReadOnly={subentry.value.isReadOnly}
              />
            </Col>
          </Row>
        );
      } else if (
        (subentry.value?.possible === null ||
          subentry.value?.possible === undefined) &&
        (EntryValueType.Byte === subentry.value?.type ||
          EntryValueType.Int16 === subentry.value?.type ||
          EntryValueType.UInt16 === subentry.value?.type ||
          EntryValueType.Int32 === subentry.value?.type ||
          EntryValueType.UInt32 === subentry.value?.type ||
          EntryValueType.Int64 === subentry.value?.type ||
          EntryValueType.UInt64 === subentry.value?.type ||
          EntryValueType.Single === subentry.value?.type ||
          EntryValueType.Double === subentry.value?.type)
      ) {
        return (
          <Row className="up-space">
            <Col md={12}>
              <NumberEditor
                Entry={subentry}
                IsReadOnly={subentry.value.isReadOnly}
              />
            </Col>
          </Row>
        );
      } else if (
        EntryValueType.Enum === subentry.value?.type ||
        subentry.value?.possible?.includes(
          subentry.value?.default ?? subentry.value?.current ?? ""
        )
      ) {
        return (
          <Row className="up-space">
            <Col md={12}>
              <EnumEditor
                Entry={subentry}
                IsReadOnly={subentry.value.isReadOnly}
              />
            </Col>
          </Row>
        );
      } else
      if (EntryValueType.Class === subentry.value?.type) {
        return subentry.subEntries.map((subentry_subentries, idx) => {
          return this.createInputBasedOnType(subentry_subentries);
        });
      }
    });
  }

  public render(): React.ReactNode {
    return (
      <div>
        <Container fluid={true} className="up-space-lg no-padding">
          {this.createInputBasedOnType(this.props.Entry)}
        </Container>
      </div>
    );
  }
}
