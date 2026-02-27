# MqttTopicJson

The Json topic is an implementation for sending and receiving JSON objects as Messages.
Other valid JSON values like Lists or simple values are not supported right now.

It uses System.Text.Json to serialize and deserialize messages. Native options are

- **Format** Decides how Property names are serialized. The default value PascalCase just maintains the c# Property name which is PascalCase by convention. camelCase is currently the only other option.
- **EnumsAsString** True if enums should be serialized as strings and not as integers
- **IgnoreCondition** controls when serialization of properties should be skipped. 
    - **Never** skips only Properties explicitly marked with JsonIgnore
    - **WhenWritingDefault** skips properties containing their default value like 0 for integers or null for objects
    - **WhenWritingNull** skips only properties containing null
- **EncoderOption** controls how special characters are encoded. Selecting UnsafeRelaxedJsonEscaping makes it not esacape HTML-sensitive characters like <, >, & and changes how " are encoded

To allow more fine grained control over serialization without reimplementing everything you can inherit from MqttTopicJson and override the GetSystemTextJsonOptions method. This allows you to include custom JsonConverters, complex naming strategies and everything else you can tweak with System.Text.Json.