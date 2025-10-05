# Analytics
## Configure Grafana
In the conf folder there is a file called **defaults.ini**, which contains the configuration of grafana. <br/>
First enable the anonymous autentification. Then enable **allow_embedding** to allow the browser to render Grafana in iframe.
``` html
[auth.anonymous]
# enable anonymous access
enabled = true

...
# set to true if you want to allow browsers to render Grafana in a <frame>, <iframe>, <embed> or <object>. default is false.
allow_embedding = true
```
After that restart Grafana
