
# Add this code to your own stack

## Logging in to azure cli

`terminal $` `az login`
`terminal $` `az account list`
`terminal $` `az account subscription list`
`terminal $` `az set --subscription=YOUR-SUBSCRIPTION-ID`

Or make the usage of a specific subscription permanent for this project.
You need to create the stack first (See 'Assimilating the stack')

`terminal $` `pulumi config set azure-native:subscriptionId YOUR-SUBSCRIPTION-ID`

Or add it to the `pulumi.env` file
PULUMI_ACCESS_TOKEN=YOUR-PULUMI-ACCESS-TOKEN
ARM_SUBSCRIPTION_ID=YOUR-SUBSCRIPTION-ID

### Working with the config

pulumi config get azure-native:subscriptionId

pulumi stack rename young-heiselberg/coca-cola/prod

pulumi stack ls

pulumi stack ls --all #Every stack you have access to

## Assimilating the stack

Once you have set up the usage of azure cli assimilating the stack can begin.

1. Add a file named `pulumi.env` to the `IaC\.devcontainer` folder
2. Add your pulumi key like this: PULUMI_ACCESS_TOKEN=YOUR_KEY_HERE
3. Open the IaC folder in dev-container
4. Open a terminal and run:

``` bash
pulumi stack init young-heiselberg/job-application/prod
pulumi up
```

## Working with an organization

To set an organization as your default organization:
`pulumi org set-default young-heiselberg`

