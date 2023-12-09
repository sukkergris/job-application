
# About

The purpose of this project is to give the foundation to create, deploy and maintain a super simple contact form site - as cheap and easy as possible. While still applying good programming and ci-cd practices.

## Building blocks

![System overview](/Documentation/System%20overview.svg "mindmap").

# Getting started

In order to use this solution as it was intended you'll need some accounts and your own domain name. Plus you will need to be able to add som dns records on the way.

## Acquire your domain name (1 hour)

First buy your domain name. Eg. at [GoDaddy](https://dk.godaddy.com/domains) *NO RECOMMENDATIONS*

#todo: Descripe how to handle dns - maybe using azure?

## Create accounts

1. [Azure](https://portal.azure.com/)
2. [Pulumi](https://pulumi.com)
3. [mailchimp](https://mailchimp.com/)
4. #todo: Select and setup CAPTCHA  

## Setting up Azure (1 hour)

### Add Subscription

If you have an existing subscription it's possible to use that one. **Not recomended!**

After creating your azure account go to the [`Subscriptions` (Portal)](https://portal.azure.com/#view/Microsoft_Azure_Billing/SubscriptionsBladeV2) and add a new subscription.

Copy the subscription ID and send it to your developer. #todo: Move to c# no need to send this info.

### Add User to Subscription

Now go to [Microsoft extra - (Active Directory)](https://entra.microsoft.com) and add your developers user to your new subscription. The dev needs to be `owner` in order to provission the infrastructure

## Setting up Pulumi (1 hour)

You will need the paied version in order to use organizations. It's cheep though.

1. Create an account
2. Add an organization
3. Add your devs to your organization
4. Your devs will do the rest #todo: needs testing that the dev can add new stacks to the org.
5. Make this stack your own. [Add This Code To Your Stacks](/IaC/_doc_Add-This-Code-To-Your-Stack.md)

## Setting up mailchimp

The free version will let you send 1.000 mails from your subscription.
For most minor contact forms this sould be enough.

1. Create an account

#todo: Setup domanin, manage integrations

## Setting up CAPTCHA
