# Module - Workplans

## Description

This module provides the functiality to edit workplans graphically. It provides workplans and enables operations on workplan items. It also enables saving modified workplans with additional graphical information.

## Provided facades

[`IWorkplanEditing`](/src/Moryx.Workplans/IWorkplanEditing.cs)

## Provided Endpoint

This module provides a REST API for editing workplans. See [Workplans Endpoint](endpoint.md) for details on available operations and permissions.

## Dependencies

Working with workplans is done using the following APIs:

- [`IWorkplanEditing`](/src/Moryx.Workplans/IWorkplanEditing.cs)
- [`IWorkplans`](/src/Moryx/Workplans/API/IWorkplans.cs)

### Referenced facades

Plugin API | Start Dependency | Optional | Usage
-----------|------------------|----------|------
[`IProductManagement`](/src/Moryx.AbstractionLayer/Products/IProductManagement.cs) | Yes | No | The Product Management is used the get products and workplan information.

### Used DataModels

## Architecture

### Overview

Component name|Implementation|Desription
--------------|--------------|----------
