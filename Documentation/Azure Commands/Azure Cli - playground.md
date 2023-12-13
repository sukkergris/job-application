# AZURE RBAC Role-Based-Access-Control

## Scope

Used to define boundaries of permissions for a Role.
It specidfies which access is granted to resources and resource groups.

### Scope levels in Azure

* Management group scope: This is the highest level of scope and applies to all resources and subscriptions within a management group.
* Subscription scope: This scope applies to all resources and resource groups within a subscription.
* Resource group scope: This scope applies to all resources within a resource group.
* Resource scope: This is the lowest level of scope and applies to a specific resource.

## Role definitions

Role definitions define what permissions can and can not be applied, as well as what Scopes the role cna actually be used in.

az role definition list -o table --query [].roleName

az role definition list -n Owner
az role deifnition list -n Contributor
az role definition list -n Reader

az role definition list -n 'Virtual Machine Contributor'

## Resource providers

Resource providers define the operartions that facilitate what actions and dataActions are exposed by individual resource types.

az provider list -o table --query "[?registrationState=='Registered']"

az provider show -n Microsoft.Portal

## Create role assignment

You take the role definition assign it to a security principle and apply it to a scope

## List Service Principals in The Portal

[SP - Azure Portal](https://entra.microsoft.com/?culture=en-us&country=us#view/Microsoft_AAD_IAM/StartboardApplicationsMenuBlade/~/AppAppsPreview)

## Who is logged in

az ad signed-in-user show
