
# About


Learn: [[https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website static]]

## Custom domain name with https

This tutorial is based on cloudflare's DNS and having created a CDN profile in azure.

When pulumi has ben run the first time one need to configure the 'Azure CDN profile' manually.

__Go through the steps in the order they appear__
_The order matters!_

### CLOUDFLARE

1. Add this DNS record to cloudflare DNS:
Type: CNAME
Name: www
Content: www.young-heiselberg.com
Proxy status: DNS only
TTL: Auto (But you decide)

![CLOUDFLARE DNS](/Documentation/Screenshots/CLOUDFLARE-DNS-for-azure-custum-domain.png)

2. Add asverify DNS record
Type: CNAME
Name: asverify.www.contoso.com
Content: asverify.[FIND-THIS-UNDER-NETWORKING].windows.net

![Configure domain validation](/Documentation/Screenshots/Azure-Custom-Domain-Network-Validation.png)


2. Redirect from domain name to the fully qualified domain name (FQDN)

![CLOUDFLARE redirect to from root domain to www](/Documentation/Screenshots/CLOUDFLARE-page-rule-for-www.png)

__STEP FOR VALIDATING OWNERSHIP IS MISSING__
But azure will guide you :) (See the next step)

### Azure DNS profile

[Torurial: Configure HTTPS on an azure CDN custom domain](https://learn.microsoft.com/en-us/azure/cdn/cdn-custom-ssl?tabs=option-1-default-enable-https-with-a-cdn-managed-certificate)


1. Go to your azure DNS profile resource
![Azure DNS profile](/Documentation/Screenshots/Azure-DNS-Profile-empty.png)
2.Press [+ Endpoint]
3. Add your endpoint
![Azure add an endpoint](/Documentation/Screenshots/Azure-DNS-Profile-add-endpoint.png)
4. Select your newly created endpoint.
5. Go to the Setting __Custom domains__ and add a Custom domain
![Azure endpoint - add custom domain](/Documentation/Screenshots/Azure-DNS-Profile-Endpoint-Custom-domain.png)
An error with red ink will appear if you didn't add the DNS record in CLOUDFLARE AND the DNS record has been registered by azure. (Remember to use [www].YOUR-DOMAIN-NAME.topdomain)
6. Select your newly created custom domain and configure it to "Custom domain HTTPS (On))
![Add custom domain](/Documentation/Screenshots/Azure-DNS-Endpoint-Custom-domain-https.png)
7. Validate domain ownership

Add dns records
