FROM microsoft/dotnet:2.1-sdk AS build
ARG build_number=0.0.5
ENV build_number=$build_number
ARG nuget_feedz_api_key=
ENV NUGET_FEEDZ_API_KEY=$nuget_feedz_api_key
RUN apt-get update && apt-get install unzip -y && apt-get install nuget -y
WORKDIR /app
COPY . /app/
RUN chmod +x ./build.sh && ./build.sh