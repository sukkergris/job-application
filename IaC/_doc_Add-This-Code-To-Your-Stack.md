
# Add this code to your own stack

1. Add a file named `pulumi.env` to the `IaC\.devcontainer` folder
2. Add your pulumi key like this: PULUMI_ACCESS_TOKEN=YOUR_KEY_HERE
3. Open the IaC folder in dev-container
4. Open a terminal and run:

``` bash
pulumi stack init <not sure if you need to write a name for it here>
pulumi up
```

## Working with an organization

To set an organization as your default organization:
`pulumi org set-default young-heiselberg`
