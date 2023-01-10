# Verndale ImpersonateUsers Module for Optimizely 12 and ASP.NET Core

## Installation

ImpersonateUsers is available for download and installation as a NuGet package.

Run the command below to install the Impersonation Addon into your Optimizely ASP.NET Core project.

```
Install-Package Verndale.ImpersonateUsers
```

## Configuration

Configure the Impersonation add-on by calling the 'AddVerndaleImpersonation' IServiceCollection extension method at startup.
This method is generic and requires a type paramater matching the user type on your site.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddVerndaleImpersonation<ApplicationUser>();
    ...
}
```

By default the plugin will be restricted to users in the 'Administrators' role.
You can also, optionally, pass a configuration action to define the Authorization Policy used to restrict UI access to the plugin.

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddVerndaleImpersonation<ApplicationUser>(policy => policy.RequireRole(Roles.Administrators, Roles.WebAdmins));
    ...
}
```

## Usage

When logged in click the 'Impersonate Users' menu option from the CMS section of the Optimizely admin UI.


## Quick Note on using the Add-On with Active Directory Membership Provider

A virtual role named ImpersonateUsers has been set for accessing the add-on. So your new virtual role in Web.config should look like this:

```
<add name="ImpersonateUsers" type="EPiServer.Security.MappedRole, EPiServer.Framework" roles="[AD_ROLES]" mode="Any" />
```

[AD_ROLES] should be replaced by a comma-separated list of the AD Roles you want to authorize for the Impersonate Users Add-On.