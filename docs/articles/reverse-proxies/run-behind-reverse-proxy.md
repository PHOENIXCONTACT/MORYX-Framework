# Run behind reverse proxy

Running behind a reverse proxy is a common scenario for web applications.
That's why there is proper support for this in ASP.NET Core and hence in MORYX.
There is ample documentation for it by [Microsoft](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balance).

Depending on your reverse proxy setup there are different levels of awareness needed in your app.

A common use case for reverse proxies is to allow multiple web applications to share the same server and port.
This can be done in one of two ways. Either by hostname based routing or by path based routing.

## Setup
If you want to follow along with the examples you can use this docker-compose file together with the nginx configuration examples to setup a reverse proxy and modify the StartProject.Asp project to see if everything works as expected.
The nginx configuration needs to be placed in a file named `nginx.conf` next to your `docker-compose.yml` when executing `docker compose up`.

```yaml
services:
  nginx-proxy:
    image: nginx:latest
    ports:
      - "8080:8080"
    volumes:
      # you can mount ./nginx-stripping.conf instead. To test the case where nginx strips the 
      # prefix instead of removing it in asp.net
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
    network_mode: host
This is very convenient for us, because it "just works" from our perspective as an app developer.
The disadvantage is the required infrastructure. We need DNS Entries for the subdomains that point to our server and if we server our content over HTTPs we need either a wildcard certificate for our subdomain or a certificate that explicitly lists all our applications.

## Path based routing
That's why it might be more interesting to use path based routing. In this case we only have one hostname `example.com` and use the route after it to distinguish the different apps, like example.com/app1 and example.com/app2. 
There are two different ways to handle this scenario in our reverse proxy. The reverse proxy can either strip this prefix for passing the request along to our application or it can leave it as is. In both cases we need to enable support in our app.

### Changes to the frontend code
Regardless of the specific method, we need support not only in our backend, but also in our frontend applications. This support is already available in all MORYX Modules that are in this repository, but for your own UIs we need a few simple steps.

In the razor pages we usually mostly reference our script files and style sheets. It's important to not use an absolute path, but to start with a `~` instead.
This is shorthand for the razor engine to automatically include the correct path. This is also required for your module eventstream.

```html
<!--absolute-->
<script src="/_content/<YourC#ProjectName>/main.js" type="module"></script>

<!-- base path aware -->
<script src="~/_content/<YourC#ProjectName>/main.js" type="module"></script>

<!-- Example eventstream annotation from VisualInstructions.cshtml -- >
[ModuleEventStream("~/api/moryx/<your-api>/stream")]
```

Additionally we need to make sure the API Clients in the JS/TS Applications don't try to access the wrong endpoint. 
We usally do this using the environment for angular applications and we have a helper function in the @moryx/ngx-web-framework/environments package for this purpose. This computes the correct path for the APIs from your module path.

```typescript
// environment.prod.ts

import { getPathBase } from '@moryx/ngx-web-framework/environments';

let path_base = getPathBase("/<Your Module Route>");

export const environment = {
  production: true,
  assets: path_base + "/_content/<YourC#ProjectName>/",
  rootUrl: path_base,
};

```

### Stripping proxy

If the proxy strips the prefix, it should set Headers to inform our application what the original request path was. An example configuration for nginx could look like this:

```conf
events {}
# make sure "PathBase" is either empty or not present in appsettings
http {
    server {
        listen 8080;
        server_name example.com; # Replace with your domain or IP

        location /test/ {
            proxy_pass https://127.0.0.1:5000/;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Prefix /test;
        }
    }
}
```

In this case we need to instruct Asp.Net to use these headers using the ForwardedHeaders Middleware like so.


```csharp
// Startup.cs or Program.cs before mapping static files, razor pages and so on.
app.UseForwardedHeaders(new ForwardedHeadersOptions()
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
});
```

### Route without modifying the request

If the proxy does not strip the prefix, we can use the `UsePathBase` middleware by adding before adding our routing.

```csharp
app.UsePathBase(requiredPathBase);
```

An example nginx.conf for this case:
```conf
events {}
# make sure appsettings contains "PathBase": "/test"
# this setup does only work, if docker runs on the same host as the moryx app.
# running the app in windows and the container in WSL *won't* work
http {
    server {
        listen 8080;
        server_name example.com; # Replace with your domain or IP

        location /test/ {
            proxy_pass https://127.0.0.1:5000/test;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}

```
