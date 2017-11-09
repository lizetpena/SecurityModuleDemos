namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdB2COptions
    {
        public const string PolicyAuthenticationProperty = "Policy";

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Instance { get; set; }

        public string Domain { get; set; }

        public string EditProfilePolicyId { get; set; }

        public string SignUpSignInPolicyId { get; set; }

        public string ResetPasswordPolicyId { get; set; }

        public string CallbackPath { get; set; }

        public string DefaultPolicy => SignUpSignInPolicyId;

        public string ApiUrl { get; set; }
        public string ApiScopes { get; set; }

        public string Authority => $"{Instance}/{Domain}/{DefaultPolicy}/v2.0";
    }
}
