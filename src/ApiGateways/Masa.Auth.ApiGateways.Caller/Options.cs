namespace Masa.Auth.ApiGateways.Caller
{
    public class Options
    {
        public string AuthServiceBaseAddress { get; set; }

        public Options(string authServiceBaseAddress)
        {
            AuthServiceBaseAddress = authServiceBaseAddress;
        }
    }
}
