Verndale.ImpersonateUsers


Installation
============


ImpersonateUsers is available for download and installation as NuGet packages.


Usage
------------------------

When logged in, pull down the global menu available at the very top then select Admin --> Impersonate Users.


Quick Note on using the Add-On with Active Directory Membership Provider
-------------------------------------------------------------------------------------------

A virtual role named ImpersonateUsers has been set for accessing the add-on. So your new virtual role in Web.config should look like this:

<add name="ImpersonateUsers" type="EPiServer.Security.MappedRole, EPiServer.Framework" roles="[AD_ROLES]" mode="Any" />

[AD_ROLES] should be replaced by a comma-separated list of the AD Roles you want to authorize for the Impersonate Users Add-On.