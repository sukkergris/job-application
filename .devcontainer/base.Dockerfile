# Start with a minimal Debian image
FROM debian:bullseye

# Install essential tools and dependencies
RUN apt-get update && \
    apt-get install -y \
        apt-transport-https \
        ca-certificates \
        curl \
        software-properties-common \
        git \
        vim \
        lynx \
        sed \
        wget \
        tmux \
        zsh \
        jq \
        locales \
        zip \
        unzip \
        make \
        tree \
        net-tools

# Install Docker
RUN curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian bullseye stable" > /etc/apt/sources.list.d/docker.list && \
    apt-get update && \
    apt-get install -y docker-ce

# Install Docker Compose
RUN curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose && \
    chmod +x /usr/local/bin/docker-compose

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

# Install Pulumi
RUN curl -fsSL https://get.pulumi.com | sh

# Install .NET SDK
# (adjust version as needed)
RUN curl -SL --output dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Sdk/6.0.100/dotnet-sdk-6.0.100-linux-x64.tar.gz && \
    mkdir -p /usr/share/dotnet && \
    tar -ozxf dotnet.tar.gz -C /usr/share/dotnet && \
    rm dotnet.tar.gz && \
    ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

# Install .NET tools
RUN dotnet tool install --global Nuke.GlobalTool

# Set the default shell to zsh
SHELL ["/bin/zsh"]