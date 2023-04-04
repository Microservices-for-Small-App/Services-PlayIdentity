# Services-PlayIdentity

.NET 7 Identity System for a small microservices application

```dotnetcli
dotnet pack -o C:\LordKrishna\Packages\
```

## Create and publish package to GitHub using PowerShell

```powershell
$version="1.0.8"
$owner="Microservices-for-Small-App"
$username="vishipayyallore"
$repo="Services-PlayIdentity"
$libname="Identity.Contracts"
$gh_pat="ghp_Your_GitHib_Classic_PAT"

dotnet clean
dotnet build -c Release
dotnet pack --configuration Release -o C:\LordKrishna\SSP\Packages

dotnet nuget push C:\LordKrishna\SSP\Packages\$libname.$version.nupkg --source "gHmicroservices" --api-key $gh_pat
```

```powershell
$host.ui.RawUI.WindowTitle = "Identity"
```

```dotnetcli
dotnet tool install --global dotnet-aspnet-codegenerator --version 7.0.4
dotnet tool update -g dotnet-aspnet-codegenerator

dotnet tool uninstall -g dotnet-aspnet-codegenerator

dotnet aspnet-codegenerator identity --files "Account.Register"

dotnet aspnet-codegenerator identity --files "Account.Logout"
```

```xml
  <ItemGroup>
    <PackageReference Include="AspNetCore.Identity.MongoDbCore" Version="3.1.2" />
    <PackageReference Include="CommonLibrary" Version="1.0.6" />
    <PackageReference Include="Duende.IdentityServer" Version="6.2.3" />
    <PackageReference Include="Duende.IdentityServer.AspNetIdentity" Version="6.2.3" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="7.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="7.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="7.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="7.0.3">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="7.0.3" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="7.0.3" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="7.0.4" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
```

## Support for ASP.NET Core Identity was added to your project

> 1. For setup and configuration information, see <https://go.microsoft.com/fwlink/?linkid=2116645>.
