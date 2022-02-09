# How to Deploy

(Guideline for people not knowing container.)

## First

Install Docker.

## Second

```bash
docker build -t whu-lambda/web/grpc .
docker run -p 80:80 -p 443:443 -d --name TadokoroKoji whu-lambda/web/grpc Github:ClientID={clientID} Github:ClientSecret={clientSecret} Microsoft:ClientID={clientID} Microsoft:ClientSecret={clientSecret}
```

Replace `{clientID}` and `{clientSecret}` with the correct secret.  
Or use envirenment variables(replace `:` with `__`).  
Or use Secret Manager(not sure if works in production).  
See [Secrets Management](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets).
