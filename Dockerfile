FROM microsoft/dotnet:2.1-sdk AS build
ARG CONFIGURATION=Debug
WORKDIR /src

COPY . ./
RUN dotnet restore FWTL.Auth
RUN dotnet publish FWTL.Auth -c $CONFIGURATION -o /src/out --no-restore

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
ARG ENVIRONMENT=Development
ENV ASPNETCORE_ENVIRONMENT $ENVIRONMENT

WORKDIR /app
COPY --from=build /src/out .
ENTRYPOINT ["dotnet", "FWTL.Auth.dll"]