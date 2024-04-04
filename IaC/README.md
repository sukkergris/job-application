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
3.) var website = new Pulumi.AzureNative.Storage.StorageAccountStaticWebsite("job-application -> "YOUR-DESCRIPTIVE-NAME"
## Run the Project

Open the 'IaC' root folder in vs-code and open the devcontainer.

 Make this stack your own. [Add This Code To Your Stacks](_doc_Add-This-Code-To-Your-Stack.md)

 ### First run

Prepare for running the project for the first time locally

 ```terminal
node -> /workpace/IaC/job-application (main) $
~$ az login
~$ az account list
~$ az account subscription list
~$ az account set --subscription=YOUR-SUBSCRIPTION-ID_OR_YOUR_SUBSCRIPTION_DISPLAY_NAME

 ```
Run: `$ pulumi up`

reviewing update (young-heiselberg/dev)

View in Browser (Ctrl+O): https://app.pulumi.com/young-heiselberg/job-application/dev/previews/614c7e74-936e-4b96-8fbf-e0863c3274a3

     Type                                                 Name                             Plan        Info
     pulumi:pulumi:Stack                                  job-application-dev                          34 messages
 +   ├─ azure-native:storage:StorageAccount               heisconform-dev                  create
 +   ├─ azure-native:web/v20230101:AppServicePlan         heisconform-dev-                 create
 +   ├─ azure-native:storage:StorageAccountStaticWebsite  heisconform                      create
 +   ├─ azure-native:web/v20230101:WebApp                 heisconform-dev-                 create
 +-  ├─ azure-native:cdn:Profile                          heisconform-                     replace     [diff: ~resourceGroupName]
 -   ├─ azure-native:storage:StorageAccountStaticWebsite  job-application                  delete
 -   ├─ azure:appservice:ServicePlan                      heisconform-dev-                 delete
 -   ├─ azure:storage:Account                             sa-heisconform-dev               delete
 -   └─ azure:core:ResourceGroup                          rg-heisconform-dev-northeurope-  delete


Outputs:
  + LinuxFunctionAppId           : output<string>
  + LinuxFunctionAppName         : "heisconform-dev-7f202d5d"
  - StorageAccountDefaultEndpoint: "https://saheisconformdevadc76bfe.z16.web.core.windows.net/"
  - StorageAccountKey            : [secret]

Resources:
    + 4 to create
    - 4 to delete
    +-1 to replace
    9 changes. 2 unchanged

Do you want to perform this update?  [Use arrows to move, type to filter]
  yes
> no
  details



  Diagnostics:
  pulumi:pulumi:Stack (job-application-dev):
    error: update failed

  azure-native:storage:StorageAccount (heisconform-dev):
    error: Code="AccountNameInvalid" Message="heisconform-dev253c9004 is not a valid storage account name. Storage account name must be between 3 and 24 characters in length and use numbers and lower-case letters only."

  azure-native:cdn:Profile (heisconform-):
    error: autorest/azure: Service returned an error. Status=<nil> Code="Conflict" Message="Cannot delete custom domain \"www.young-heiselberg.com\" because it is still directly or indirectly (using \"cdnverify\" prefix) CNAMEd to CDN endpoint \"young-heiselberg.azureedge.net\". Please remove the DNS CNAME record and try again."