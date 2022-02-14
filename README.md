# How to Deploy

(Guideline for people not knowing container.)

## First

Install Docker.

## Second

```bash
$ docker build -t whu-lambda/web/grpc .

$ docker run -p {real port}:80 -d --name TadokoroKoji whu-lambda/web/grpc Github:ClientSecret={clientSecret} Microsoft:ClientSecret={clientSecret}
```

Options in `[]` are optional.

In container, port `80` is for http. They are bound to `{real port}` of the host.

For now, `appsettings.Production.json` under `./Whu.Lambda.Moe.Backend` is configured for development. Rewrite it in production.

Replace `{clientSecret}` with the correct secret.

`{clientSecret}` can also be provided by envirenment variables(replace `:` with `__`) | Secret Manager(not sure if works in production) | etc.
(See [Secrets Management](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets))

Only HTTP/2 requests && requests from localhost are accepted.
