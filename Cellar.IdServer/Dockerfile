# # Sample contents of Dockerfile
#  # Stage 1
#  FROM microsoft/aspnetcore-build:1.1 AS builder
#  WORKDIR /source

# #  #copy Nuget.config to root
# # # RUN cp ./nuget.config /root/.nuget/NuGet/nuget.config
# # COPY NuGet.Config /root/.nuget/NuGet/NuGet.Config

#  # caches restore result by copying csproj file separately
#  COPY . .
# #  COPY NuGet.Config .
#  RUN dotnet restore --configfile NuGet.Config

#  # copies the rest of your code
# #  COPY . .
#  RUN dotnet publish --output /app/ --configuration Release

 # Stage 2
 FROM microsoft/aspnetcore:1.1

ENV ASPNETCORE_ENVIRONMENT="Development"

 WORKDIR /app
 EXPOSE 80
 COPY . .
#  COPY --from=builder /app .
 ENTRYPOINT ["dotnet", "./bin/Release/netcoreapp1.1/publish/Cellar.IdServer.dll"]


# # Sample contents of Dockerfile
#  # Stage 1
#  FROM microsoft/dotnet:1.1.2-sdk  AS builder
#  WORKDIR /source

#  #copy Nuget.config to root
# # RUN cp ./nuget.config /root/.nuget/NuGet/nuget.config
# COPY . .
# RUN dotnet restore ./IdentityServer4/
# RUN dotnet build ./IdentityServer4/
# RUN dotnet pack ./IdentityServer4/

# # Stage 2
# FROM microsoft/aspnetcore:2.0.0-preview2
# WORKDIR /app
# EXPOSE 80
# COPY --from=builder /IdentityServer4/bin/Debug/IdentityServer4Cellarstone.2.0.0.nupkg .





