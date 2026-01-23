/*
 * Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

// Function executed when page is unloaded
function saveLocation() {
    // Read current location and extract route
    const location = window.location.href.substring(
        (window.location.protocol + '//' + window.location.host).length
    );
    const route = location.split('/')[1];

    // Calculate expiration date
    const now = new Date();
    let time = now.getTime();
    time += 3600 * 1000;
    now.setTime(time);

    // Skip cookie if location is identical to route
    if (location.replaceAll('/', '') === route)
        return;

    // Set cookie for location on current route
    document.cookie =
        route + '-location=' + location +
        '; expires=' + now.toUTCString() +
        '; path=/;';
}

// Register to beforeunload
window.onbeforeunload = function () {
    saveLocation();
}



