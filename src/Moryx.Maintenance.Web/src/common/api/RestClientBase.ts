/*
 * Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
 * Licensed under the Apache License, Version 2.0
*/

type ReplacerFunction = (key: string, value: any) => any;

export default class RestClientBase {
    private url: string;

    constructor(baseUrl: string = "") {
        this.url = baseUrl
            ? baseUrl
            : RestClientBase.baseUrl();
    }

    public static baseUrl(): string {
        const port = window.location.port;
        const portPortion = port ? `:${port}` : "";
        return `${window.location.protocol}//${window.location.hostname}${portPortion}`;
    }

    public get<T>(path: string, errorInstance: T): Promise<T> {
        return fetch(this.url + path)
            .then((results) => results.json())
            .catch((e) => {
                console.log(e);
                return errorInstance;
            });
    }

    public put<R, T>(path: string, request: R, errorInstance: T, replacer: ReplacerFunction = null): Promise<T> {
        return fetch(this.url + path, {
            body: JSON.stringify(request, replacer),
            headers: {
                "content-type": "application/json",
              },
            cache: "no-cache",
            method: "PUT",
          })
          .then((results) => results.json())
          .catch((e) => {
                  console.log(e);
                  return errorInstance;
              });
    }

    public post<R, T>(path: string, request: R, errorInstance: T, replacer: ReplacerFunction = null): Promise<T> {
        const body = JSON.stringify(request, replacer);
        return fetch(this.url + path, {
            body,
            headers: {
                "content-type": "application/json",
              },
            cache: "no-cache",
            method: "POST",
          })
          .then((results) => results.json())
          .catch((e) => {
                  console.log(e);
                  return errorInstance;
              });
    }

    public postNoBody<T>(path: string, errorInstance: T): Promise<T> {
        return fetch(this.url + path, {
            cache: "no-cache",
            method: "POST",
          })
          .then((results) => results.json())
          .catch((e) => {
                  console.log(e);
                  return errorInstance;
              });
    }

    public delete<R, T>(path: string, request: R, errorInstance: T, replacer: ReplacerFunction = null): Promise<T> {
        return fetch(this.url + path, {
            body: JSON.stringify(request, replacer),
            headers: {
                "content-type": "application/json",
              },
            cache: "no-cache",
            method: "DELETE",
          })
          .then((results) => results.json())
          .catch((e) => {
                  console.log(e);
                  return errorInstance;
              });
    }

    public deleteNoBody<T>(path: string, errorInstance: T): Promise<T> {
        return fetch(this.url + path, {
            headers: {
                "content-type": "application/json",
              },
            cache: "no-cache",
            method: "DELETE",
          })
          .then((results) => results.json())
          .catch((e) => {
                  console.log(e);
                  return errorInstance;
              });
    }
}
