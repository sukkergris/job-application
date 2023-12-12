
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

If you have an existing subscription it's possible to use that one. **Not recomended!**

After creating your azure account go to the [`Subscriptions` (Portal)](https://portal.azure.com/#view/Microsoft_Azure_Billing/SubscriptionsBladeV2) and add a new subscription.

Copy the subscription ID and send it to your developer.

### Add User to Subscription

Now go to [Microsoft extra - (Active Directory)](https://entra.microsoft.com) and add your developers user to your new subscription. The dev needs to be `owner` in order to provission the infrastructure

## Setting up Pulumi (1 hour)

You will need the payed version in order to use organizations. It's cheep though.

1. Create an account
2. Add an organization
3. Add your devs to your organization
4. Your devs will do the rest `#todo: needs testing that the dev can add new stacks to the org`
5. Make this stack your own. [Add This Code To Your Stacks](/IaC/_doc_Add-This-Code-To-Your-Stack.md)
6. Acquire your [Personal access token](https://app.pulumi.com/sukkergris/settings/tokens)
7. COPY AND STORE THIS TOKEN SECURELY

## Setting up mailchimp

The free version will let you send 1.000 mails from your subscription.
For most minor contact forms this should be enough.

1. Create an account

`#todo: Setup domain, manage integrations`

## Setting up CLOUDFLARE

 1. Add CAPTCHA

## Setting up GitHub

In order to login to azure when running an github action. A service principle is created.

1.

```plantuml



```

# Running Nuke Build

This template/project is meant to run in a devcontainer.

## Now find

* Pulumi access toke
* Azure subscription id

1. In `.devcontainer` add a file named `secret.env`
2. Add these lines
    a. PULUMI_ACCESS_TOKEN={'YOUR_PULUMI_TOKEN_HERE'}
    b. AZURE_SUBSCRIPTION_ID={'YOUR_SELECTED_AZURE_SUBSCRIPTION_HERE'}
