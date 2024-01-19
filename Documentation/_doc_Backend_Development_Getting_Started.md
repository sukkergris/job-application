
# Getting started with the backend

The entire project needs secrets!

This document will walk you through what is needed in order to get you up and running locally.

## Acquire the secrets

#TODO: Describe how eo acquire secrets.

## The project won't build

A development specific file named `integrationtest.json` must be added.

Now an empty file `/src/Testing/Build.Test/integrationtest.json`

## How do I run the `CloudAutomationSuite`?

This project needs some secrets as well.

### Running on the OS

Add a file  `/build/Properties/launchSettings.json`

Use `/build/Properties/launchSettings-template.json` as template

Add secrets accordingly.

### Running in devcontainer

Add a file `/.devcontainer/secrets.env`

Use `/.devcontainer/secrets-template.env` as template

Add secrets accordingly.