#!/bin/bash
if [ -z "$1" ]
then
	echo "Need a commit message!"
	exit 1
fi
git add -A .
git commit -m "$1"
git push
dotnet publish -c Release
docker build -t jamie-bickers-personal-website ./PersonalWebsite/bin/release/netcoreapp2.0/publish
docker tag jamie-bickers-personal-website registry.heroku.com/jamie-bickers-personal-website/web
heroku container:login
docker push registry.heroku.com/jamie-bickers-personal-website/web