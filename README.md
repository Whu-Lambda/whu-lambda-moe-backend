# How to Deploy

## First

Install Docker(or any container supporting OCI).

## Second

```bash
docker build -t whu-lambda/web/grpc .
docker run -p 7262:14514 -d --name TadokoroKoji whu-lambda/web/grpc
```

Change 14514 to the real listening port.  
Change the container name if you prefer.  
Change the image name if you prefer.
