# Module - Workplans

## Description

This module provides the functiality to edit workplans graphically. It provides workplans and enables operations on workplan items. It also enables saving modified workplans with additional graphical information.

## Provided facades

`IWorkplanEditing` (see Moryx.Workplans.IWorkplanEditing in Moryx Platform)

## Provided Endpoint

This module provides a REST API for editing workplans. See [Workplans Endpoint](endpoint.md) for details on available operations and permissions.

## Dependencies

Working with wokplans is done using the following APIs:

`IWorkplanEditing` (see Moryx.Workplans.IWorkplanEditing in Moryx Platform)

`IWorkplans` (see Moryx.Workplans.IWorkplans in Moryx Platform)

### Referenced facades

Plugin API | Start Dependency | Optional | Usage
-----------|------------------|----------|------
`IProductManagement` (see Moryx.AbstractionLayer.Products.IProductManagement in AbstractionLayer)| Yes | No | The Product Management is used the get products and workplan information.

### Used DataModels

## Architecture

### Overview

Component name|Implementation|Desription
--------------|--------------|----------
