
# How to use

## Get an overview of the plan

```bash
nuke --plan // HTML plan file is crated here: `.nuke/temp/execution-plan.html`
nuke --target // List possible targets (Steps in the plan)
```
## Debugging in Visual Studio 202x

If you want to debug your nuke build using Visual Studio 202x then:

1. In the _build project add a folder named 'Properties'
2. Add a file named launchSettings.json
3. Add your settings:
{
  "profiles": {
    "Development": {
      "commandName": "Project",
      "environmentVariables": {
        "PULUMI_ACCESS_TOKEN": "YOUR_PULUMI_ACCESS_TOKEN"
      }
    }
  }
}
