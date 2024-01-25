# Start with a minimal Debian image
FROM debian:bullseye AS base

# Install essential tools and dependencies
RUN apt-get update -y && \
    apt-get install -y \
        apt-transport-https \
        ca-certificates \
        curl \
        software-properties-common \
        git \
        zsh \
        jq \
        unzip \
        make \
        tree \
        build-essential \
        wget \
        # Other tools as needed \
    && rm -rf /var/lib/apt/lists/*

# Install gh
RUN curl -sSL https://cli.github.com/packages/githubcli-archive-keyring.gpg \
  -o /usr/share/keyrings/githubcli-archive-keyring.gpg

RUN chmod 644 /usr/share/keyrings/githubcli-archive-keyring.gpg

RUN echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null

RUN apt-get update && apt-get install gh -y

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Install Pulumi
ARG PULUMI_VERSION
RUN curl -fsSL https://get.pulumi.com/ | bash -s -- --version $PULUMI_VERSION && \
    mv ~/.pulumi/bin/* /usr/bin

ENV PATH="$PATH:/root/.pulumi/bin"

# Install .NET SDK 6.0
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-6.0

# Install .NET SDK 8.0
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0

# Install Nuke Global Tool
RUN dotnet tool install --global Nuke.GlobalTool
RUN dotnet tool install --global dotnet-ef
RUN dotnet tool install --global dotnet-format

ENV PATH="$PATH:~/.dotnet/tools"


# Set the default shell to zsh
SHELL ["/bin/zsh"]
