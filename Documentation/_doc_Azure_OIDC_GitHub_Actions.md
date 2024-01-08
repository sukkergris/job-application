
# Using Azure OIDC - Open ID Connect - for GitGhub Actions

No password required.
Works with eg. Azure resources.

1. Register GitHub as a trusted resource in `Azure Active Directory app registration with federated credential`

## Using Pulumi

## Using The Azure Portal

[Youtube - ASP.NET MONSTERS](https://www.youtube.com/watch?v=qSIs7HzgpiA&t=137s)
[Youtube - GitHub Actions - OIDC](https://www.youtube.com/watch?v=7iCtY0ztYY4)

[Link to Azure app registrations](https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade)

1. Add or create an app registration (Service Principal - SP)
2. Select the SP and go to the phane `Certificates & secets`
3. Go to the phane `Federated credentials (n)`
4. Add a credential for the scenario `GitHub Actions deployning Azure resources`
5. To the `.github/workflows/YOUR_FLOW_NAME.yml` add permission: `id-token: write`
6. In the appropiate step add with:

* `client-id`: ${{ secrets.AZURE_CLIENT_ID }} // Get the value from the SP just created
* `tenant-id`: ${{ secrets.AZURE_TENANT_ID }} // Get the value from the SP just created

## Add SP to eg. Subscription

### Add Member

Home -> YOUR_SUBSCRIPTION -> Access control (IAM) -> [+Add] -> Add role assignment -> Members

* Assign access to: (User, group, or service principal)
* Menbers -> [+Select members] (Search using SP name)

Press [Select]

### Add Role

Back on the `Add role assignment` seletc the `Role` phane:

### Finalize

Back on the `Add role assignmen` press [next]
