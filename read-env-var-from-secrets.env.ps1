# Set the path to the secrets.env file
$secretsFilePath = ".\.devcontainer\secrets.env"

# Check if the file exists
if (Test-Path $secretsFilePath) {
    # Read the content of the file
    $secretsContent = Get-Content $secretsFilePath

    # Iterate through each line and set environment variables
    foreach ($line in $secretsContent) {
        # Split the line into key and value
        $key, $value = $line -split '=', 2

        # Set environment variable
        [System.Environment]::SetEnvironmentVariable($key, $value, [System.EnvironmentVariableTarget]::Process)
    }

    Write-Host "Environment variables set from $secretsFilePath"
} else {
    Write-Host "File not found: $secretsFilePath"
}

# I have spoken!
