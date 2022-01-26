# How to Deploy

(Guideline for people not knowing container.)

## First

Install Docker.

## Second

```bash
docker build -t whu-lambda/web/grpc .
docker run -p 80:80 -p 443:443 -d --name TadokoroKoji whu-lambda/web/grpc
```
