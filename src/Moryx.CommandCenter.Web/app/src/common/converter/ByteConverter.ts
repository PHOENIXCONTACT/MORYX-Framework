/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

export default function kbToString(bytes: number): string {
    const sizes = ["bytes", "KB", "MB", "GB", "TB"];

    if (bytes === 0) { return "0 bytes"; }
    const i = Math.floor(Math.log(bytes) / Math.log(1024));
    return Math.round(bytes / Math.pow(1024, i)) + " " + sizes[i];
}
