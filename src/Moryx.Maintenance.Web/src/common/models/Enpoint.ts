/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export const enum ServiceBindingType {
    WebHttp = "WebHttp",
    BasicHttp = "BasicHttp",
    NetTcp = "NetTcp",
}

export default class Endpoint {
    public Service: string;
    public Binding: ServiceBindingType;
    public Path: number;
    public Address: string;
    public Version: string;
    public RequiresAuthentication: Boolean;
}
