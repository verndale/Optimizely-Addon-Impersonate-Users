namespace Verndale.ImpersonateUsers
{
    public static class Constants
    {
        public const string AuthorizationPolicyName = "ImpersonateUsers";
        public const string ModuleName = "Verndale.ImpersonateUsers";

        public static class ImpersonationClaims
        {
            public const string UserImpersonation = "UserImpersonation";
            public const string OriginalUsername = "OriginalUsername";
            public const string ImpersonatedUsername = "ImpersonatedUsername";
        }
    }
}
