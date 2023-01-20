/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default class DatabaseConfigModel {
    public server: string;
    public port: number;
    public database: string;
    public user: string;
    public password: string;
    public connectionString: string;
    public configuratorTypename: string;
}
