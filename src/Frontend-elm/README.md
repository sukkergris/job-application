
# Fixing Elm intellisence

## Error in output -> Elm (workspace)

[Warn  - 6:20:38 PM] {"shortMessage":"Command failed with exit code 1: elm make","command":"elm make","escapedCommand":"elm make","exitCode":1,"stdout":"","stderr":"-- NO INPUT --------------------------------------------------------------------\n\nWhat should I make though? I need specific files like:\n\n    elm make src/Main.elm\n    elm make src/This.elm src/That.elm\n\nI recommend reading through https://guide.elm-lang.org for guidance on what to\nactually put in those files!\n\n","failed":true,"timedOut":false,"isCanceled":false,"killed":false}
[Error - 6:20:38 PM] Error parsing files for file:///workspace/elm.json:
SyntaxError: Unexpected end of JSON input
    at JSON.parse (<anonymous>)
    at Program.loadElmJson (/home/node/.vscode-server/extensions/elmtooling.elm-ls-vscode-2.8.0/out/nodeServer.js:31573:23)
    at async Program.loadPackage (/home/node/.vscode-server/extensions/elmtooling.elm-ls-vscode-2.8.0/out/nodeServer.js:31343:27)
    at async Program.loadDependencyMap (/home/node/.vscode-server/extensions/elmtooling.elm-ls-vscode-2.8.0/out/nodeServer.js:31376:36)
    at async Program.loadRootProject (/home/node/.vscode-server/extensions/elmtooling.elm-ls-vscode-2.8.0/out/nodeServer.js:31273:29)
    at async Program.initWorkspace (/home/node/.vscode-server/extensions/elmtooling.elm-ls-vscode-2.8.0/out/nodeServer.js:31203:32)
[Info  - 6:21:35 PM] Reading elm.json from file:///workspace/elm.json
[Info  - 6:21:35 PM] Elm version 0.19.1 detected.

## Fix

Use dos2unix
```bash
`$ sudo dos2unix elm.json`
```

## Course

Don't let git change the line endings!