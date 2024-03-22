# How to assimilate this Pulumi stack

## Rename folder and filenames

1.) Copy/clone the folder to your solution
2.) Cook up a descriptive name for your stack. Eg. `coca-cola`
3.) Rename the 'job-application' folder to your descriptive name
4.) Rename the 'job-application/job-application.csproj to eg `coca-cola/coca-cola.csproj`

## Update files in .vscode

1.) Update the .vscode/tasks.json width your descriptive name
2.) Update the .vscode/launch.json width your descriptive name

## Update the Pulumi yaml files (Project configurations)

In the `Pulumi.yaml` update the `name` attribute to your stack name/descriptive name

## Update the Pulumi script with correct configurations

Your script is located in the `coca-cola/Program.cs` file

1.) var application = "heisconform" -> "YOUR-DESCRIPTIVE-NAME"
2.) ProfileName = $"cdn-heis-sw" -> Use a short name here!
3.)   var website = new Pulumi.AzureNative.Storage.StorageAccountStaticWebsite("job-application -> "YOUR-DESCRIPTIVE-NAME"
## Run the Project

Open the 'IaC' root folder in vs-code and open the devcontainer.

 Make this stack your own. [Add This Code To Your Stacks](_doc_Add-This-Code-To-Your-Stack.md)

 ##