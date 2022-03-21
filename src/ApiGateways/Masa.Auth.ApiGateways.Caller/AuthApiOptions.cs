namespace Masa.Auth.ApiGateways.Caller
{
    public class AuthApiOptions
    {
        public string AuthServiceBaseAddress { get; set; }

        public AuthApiOptions(string authServiceBaseAddress)
        {
            AuthServiceBaseAddress = authServiceBaseAddress;
        }
    }
}
