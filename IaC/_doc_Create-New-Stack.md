# Create New Stack

## Fix arrow keys not responding

[Fix arrow keys not working in devenvironment] -> `#: tput rmkx`

## New pulumi project

1. Create project folder: `mkdir job-application`
2. Open the folder in the bash
3. Run: `pulumi new azure-classic-csharp`
4. Fill in the blanks

```
Project name (ktk):  
Project description (A minimal C# Pulumi program with the classic Azure provider):  
Created project 'ktk'

Please enter your desired stack name.
To create a stack in an organization, use the format <org-name>/<stack-name> (e.g. `acmecorp/dev`).
Stack name (dev): young-heiselberg/ktk/prod 
Created stack 'young-heiselberg/prod'

The Azure location to use (azure:location) (WestUS2): norteurope 
The Azure Subscription to deploy into (azure:subscriptionId): <your-subscription-id> 
```

Your stack is now ready
