---
uid: FileSystem
---
# FileSystem

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