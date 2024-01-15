
# Cloudflare

## Start

Begin by selecting your website:
`Websites -> young-heiselberg.com`

## Ensure HTTPS

 SSL/TLS -> Edge Certificates -> Always Use HTTPS (v)

## DNS

Websites -> (young-heiselberg) ->

| Type | Name | Content | Proxy status | TTL |
| :----: | :----: | :----: | :----: | :----: |
| CNAME | young-heiselberg.com | www.young-heiselberg.com | Proxied (v) | Auto |
| CNAME | www | young-heiselberg.azureedge.net | DNS only | 1 min |
| CNAME | asverify.www.contoso.com | asverify.YOUR_CODE.blob.core.windows.net | DNS only | 1 min |

## Redirect

Website -> (young-heiselber) ->

Rules -> Page Rules -> [Create Page Rule]

`URL (required)`: https://young-heiselberg.com/*

`Pick a Setting (Required)`: Forwarding URL `Select status code (required)`: 301 - Permanent Redirect

`Enter destination URL (required)`: https://www.young-heiselberg.com/$1