
# Create the local environment for devcontaier

## Create the build environment

Hopefully it will be able be used in the actual pipeline

## Host vs code extensions recommendations

* Vim
* .NET Install Tool
* Code Spell Checker
* Docker
* .markdownlint

## Host vs code extension prerequisites

* Dev Containers

## Create the docker file (DinD)

_This is ran directly on the host environment_ MacOS, Linux or Windows

1. Open the `Environment` folder in the terminal
2. Run: `docker build -t <username>/<image-name>:<tag> .`
3. Run: `docker build -t isuperman/job-application:1.0 .`

Deploy to dockerhub:

1. Require user is logged in
    a. Login on docker desktop
    b. `> docker login` => `Authenticating with existing credentials...`

2. Run: `docker push isuperman/job-application:1.0`

The image is now pushed to dockerhub.

# Prepare the environment

Before starting the `Dev Container`: [Prepare the environment](www.dr.dk)
