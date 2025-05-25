---
uid: FileSystem
---
# FileSystem

## Inspiration

The MORYX file system was inspired by the GIT tree and obj structure. 

## Architecture

````mermaid
classDiagram
    MoryxFile <|-- Blob
    MoryxFile <|-- Tree
    Tree --> MoryxFile    
    OwnerFile --> Tree
    class MoryxFileSystem{
        -string Directory
        +WriteBlob()
        +WriteTree()
        +ReadBlob()
        +ReadTree()
    }
    class MoryxFile {
        +String Hash
    }
````