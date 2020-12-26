FROM mcr.microsoft.com/dotnet/sdk:5.0
RUN apt-get update
RUN apt-get -y --no-install-recommends install unzip
RUN apt-get -y --no-install-recommends install procps
RUN rm -rf /var/lib/apt/lists/*
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
VOLUME /app
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore otilos.csproj
COPY . ./
ENTRYPOINT [ "dotnet", "watch", "run", "--no-restore", "--urls", "https://0.0.0.0:5001"]