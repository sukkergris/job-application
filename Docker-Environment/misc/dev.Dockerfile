FROM mcr.microsoft.com/devcontainers/javascript-node:latest
# Base image
# Create the node user
RUN groupadd --gid 1000 node \
    && useradd --uid 1000 --gid node --shell /bin/bash --create-home node
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

# Configuring Elm version
ARG ELM_VERSION=latest-0.19.1
ARG ELM_TEST_VERSION=latest-0.19.1
ARG ELM_FORMAT_VERSION=latest-0.19.1

# This Dockerfile adds a non-root user with sudo access. Update the “remoteUser” property in
# devcontainer.json to use it. More info: https://aka.ms/vscode-remote/containers/non-root-user.
ARG USERNAME=node
ARG USER_UID=1000
ARG USER_GID=$USER_UID

# Install elm, elm-test and elm-format
RUN export DEBIAN_FRONTEND=noninteractive \
    && sudo -u ${USERNAME} npm install --global \
    elm@${ELM_VERSION} \
    elm-test@${ELM_TEST_VERSION} \
    elm-format@${ELM_FORMAT_VERSION} \
    #
    # [Optional] Update UID/GID if needed
    && if [ "$USER_GID" != "1000" ] || [ "$USER_UID" != "1000" ]; then \
    groupmod --gid $USER_GID $USERNAME \
    && usermod --uid $USER_UID --gid $USER_GID $USERNAME \
    && chown -R $USER_UID:$USER_GID /home/$USERNAME; \
    fi \
    # Create the elm cache directory where we can mount a volume. If we don't create it like this
    # it is auto created by docker on volume creation but with root as owner which makes it unusable.
    && mkdir /home/$USERNAME/.elm \
    && chown $USERNAME:$USERNAME /home/$USERNAME/.elm

RUN npm install -g run elm-test elm-format elm-watch