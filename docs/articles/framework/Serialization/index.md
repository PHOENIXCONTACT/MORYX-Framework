---
uid: Serialization
---
Serialization
=============

The platform uses two different ways of serializing objects - JSON and our custom entry format. We use the EntryFormat whenever we need to view and edit custom service side objects on the client, without knowing the type. The Entry Format exceeds other formats like JSON or XML in its possibilities to create generic editors and even fill collections with objects of derived types. Whenever we just want to store objects in a database or file, we use JSON.

The three sections below explain the entry format, how it is created from objects or classes on the server and how to use it on the client.

* [Entry Format](EntryFormat.md)
* [Entry Convert](EntryConvert.md)
