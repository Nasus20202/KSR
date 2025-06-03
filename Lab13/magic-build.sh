#!/bin/bash
cleanup() {
    ps -ef | grep -E 'Library.WebApi|Library.Web' | grep -v grep | awk '{print $2}' | xargs kill -9
    docker rm -f library-magic-notification-service
    docker rm -f library-magic-rabbitmq
    docker rmi library-magic-notification-service:latest
    docker network rm library-magic-network
}
cleanup

docker run --rm -td -p 9092:80 --name library-magic-notification-service mcr.microsoft.com/dotnet/sdk:8.0-alpine sh -c 'touch /var/log/notification-service.log && tail -f /var/log/notification-service.log'
docker run --rm -d -p 5672:5672 --name library-magic-rabbitmq rabbitmq:4-management-alpine
docker logs -f library-magic-rabbitmq &
docker logs -f library-magic-notification-service &

sleep 3

docker network create library-magic-network
docker network connect library-magic-network library-magic-notification-service
docker network connect library-magic-network library-magic-rabbitmq

docker cp Library.NotificationService2 library-magic-notification-service:/app
docker exec -it library-magic-notification-service /bin/sh -c "cd /app && dotnet restore && dotnet build -c Release && dotnet publish -c Release -o out"
docker exec -d library-magic-notification-service /bin/sh -c "cd /app/out && RabbitMq__ServerAddress=rabbitmq://library-magic-rabbitmq dotnet Library.NotificationService2.dll > /var/log/notification-service.log"

docker commit library-magic-notification-service library-magic-notification-service:latest
docker image ls | grep library-magic-notification-service

(
    cd Library.WebApi
    RabbitMq__ServerAddress=rabbitmq://localhost Kestrel__EndPoints__Http__Url=http://+:9091 dotnet run &
)

(
    cd Library.Web
    LibraryWebApiServiceHost=http://localhost:9091 Kestrel__EndPoints__Http__Url=http://+:9090 dotnet run &
)

echo "Press any key to stop the services..." >&2
read -n 1 -s
cleanup
