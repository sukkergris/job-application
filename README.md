
# About

The purpose of this project is to give the foundation to create, deploy and maintain a super simple contact form site - as cheap and easy as possible. While still applying good programming and ci-cd practices.

## Building blocks

![System overview](/Documentation/System%20overview.svg "mindmap").

# Getting started

In order to use this solution as it was intended you'll need some accounts and your own domain name. Plus you will need to be able to add som dns records on the way.

## Acquire your domain name (1 hour)

First buy your domain name. Eg. at [GoDaddy](https://dk.godaddy.com/domains) or [Cloudflare](https://dash.cloudflare.com/) *NO RECOMMENDATIONS*

## Create accounts

1. [Azure](https://portal.azure.com/)
2. [Pulumi](https://pulumi.com)
3. [mailchimp](https://mailchimp.com/)
4. [Cloudflare](https://dash.cloudflare.com/)
5. [GitHub](https://github.com/)

## Setting up Azure (1 hour)

### Add Subscription

If you have an existing subscription it's possible to use that one. **This is NOT recomended!**

After creating your azure account go to the [`Subscriptions` (Portal)](https://portal.azure.com/#view/Microsoft_Azure_Billing/SubscriptionsBladeV2) and add a new subscription.

Copy the subscription ID and send it to your developer.

### Add User to Subscription

Now go to [Microsoft extra - (Active Directory)](https://entra.microsoft.com) and add your developers user to your new subscription. The dev needs to be `owner` in order to provision the infrastructure

### Setup using custom domain name

1. storage-account => Networking => Custom domain => www.YOUR_DOMAIN_NAME.TLD // Eg. www.young-heiselberg.com
2. Front Door and CDN => Host name (YOUR_DOMAIN_NAME.azureedge.net) => + Endpoint

Endpoint:

* Custom domain HTTPS (On)
* Certificate management type (CDN managed)
* Minimum TLS version (TLS 1.2)

## Setting up Pulumi (1 hour)

You will need the payed version in order to use organizations. It's cheep though.

1. Create an account
2. Add an organization
3. Add your devs to your organization
4. Your devs will do the rest `#todo: needs testing that the dev can add new stacks to the org`
5. Make this stack your own. [Add This Code To Your Stacks](/IaC/_doc_Add-This-Code-To-Your-Stack.md)
6. Acquire your [Personal access token](https://app.pulumi.com/sukkergris/settings/tokens)
7. COPY AND STORE THIS TOKEN SECURELY

## Setting up email sending smtp (1 hour)

This solution is hardcoded to use ssl.

I used zoho mail for this takes but both gmail and outlook should suffice.

1. Add your mail's user name to github action secrets. Environment variable: MAIL_USER_NAME
2. Add your mail's password to github action secrets. Environment variable: MAIL_PW
3. Add your mail's hostname to github action secrets. Environment variable: MAIL_HOST
4. Add your mail's port to github action secrets. Environment variable: MAIL_HOST_PORT

## Setting up CLOUDFLARE (1 hour)

 1. Validate domain ownership for mail supplier
 2. Validate domain ownership for azure (Custom domain)
 3. Add dns records
 4. Add page redirect rule

* Add CAPTCHA

## Setting up GitHub (1 hour)

In order to login to azure when running an github action. A service principle is created.

**In the console use the following commands:**

## Create SP using OIDC (Recommended)

[Link](/Documentation/_doc_Azure_OIDC_GitHub_Actions.md)

## Create SP and Secrets using hardcoded values (or see next step for using env)

(_This is more suitable for local development_)

Learn: [Azure RBAC](https://www.youtube.com/watch?v=1OBi93apLdo)

1. Run: `az login`
2. Set default subscription: Run `az account set --subscription $Env:AZURE_SUBSCRIPTION_I`
3. Run: `az ad sp create-for-rbac --name "YOUR_NEW_SERVICE_PRINCIPAL_NAME" --role contributor --scope /subscriptions/YOUR_SUBSCRIPTION_ID_HERE --sdk-auth`
4. Copy the json output

## Create SP using Env Variables

1. Read `.devcontainer/secrets.env` into the session
2. Run: `read-env-var-from.secrets.env.ps1`
3. Run: `az login`
4. Run: `az ad sp create-for-rbac --name "YOUR_NEW_SERVICE_PRINCIPAL_NAME" --role contributor --scope /subscriptions/$Env:AZURE_SUBSCRIPTION_ID --sdk-auth`
5. Copy the json output

## Apply SP (Service principal)

1. Copy the json output.
2. Go to [Actions secrets and variables](https://github.com/sukkergris/job-application/settings/secrets/actions)
3. Paste the entire json into a `Repository Secret` named 'AZURE_CREDENTIALS'

# Running Nuke Build (Hour estimate)

This template/project is meant to run in a devcontainer.

## Now find

* Pulumi access toke
* Azure subscription id

1. In `.devcontainer` add a file named `secrets.env`
2. Add these lines

* PULUMI_ACCESS_TOKEN=YOUR_PULUMI_TOKEN_HERE
* AZURE_SUBSCRIPTION_ID=YOUR_SELECTED_AZURE_SUBSCRIPTION_HERE
