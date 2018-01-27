#!/bin/bash
dotnet publish -c Release
docker build -t jamie-bickers-personal-website ./PersonalWebsite/bin/release/netcoreapp2.0/publish
docker tag jamie-bickers-personal-website registry.heroku.com/jamie-bickers-personal-website/web
heroku container:login
docker push registry.heroku.com/jamie-bickers-personal-website/web