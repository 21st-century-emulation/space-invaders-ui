# Space Invaders UI

This is the project containing the user facing portions of the 21st century 8080 emulator as placed into a Space Invaders console.

## How to run

The application can be run from any computer with `docker` & `docker-compose` by running:

```
docker-compose up --build
```

The UI will be available on [http://localhost:8080] and the application can be started by POSTing the rom to [http://localhost:8080/api/v1/start]. e.g. 

```
curl -X POST -F 'romForm=@~/invaders.rom' localhost:8080/api/v1/cpu/start
```

You will need to source the `invaders.rom` file yourself but it's quite easy to track down using bing.

## Architecture

![Space Invaders UI Architecture](.github/images/space-invaders-architecture.png?raw=true "Space Invaders UI - Architecture")

## Development

### Pre-requisites

- Dotnet 5 & suitable text editor/IDE
- To restore packages you require an access key to the github private package registry of the 21st-century organisation on github (stored in GITHUB_USER/GITHUB_PASSWORD environment variables)