FROM mcr.microsoft.com/dotnet/core/sdk:2.1-bionic AS build
ARG build_number=0.0.5
ENV build_number=$build_number
ARG nuget_feedz_api_key=
ENV NUGET_FEEDZ_API_KEY=$nuget_feedz_api_key
ENV DEBIAN_FRONTEND=noninteractive
RUN apt install gnupg ca-certificates \
      && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
      && echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official-stable.list \
      && apt update \
      && ln -fs /usr/share/zoneinfo/America/New_York /etc/localtime \
      && apt-get update \
      && apt-get install unzip -y \
      && apt-get install mono-devel -y \
      && apt-get install nuget -y
WORKDIR /app
COPY . /app/
RUN chmod +x ./build.sh && ./build.sh
