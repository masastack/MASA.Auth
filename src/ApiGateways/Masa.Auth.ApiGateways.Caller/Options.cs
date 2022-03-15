namespace Masa.Auth.ApiGateways.Caller
{
    public class Options
    {
        public string AuthServiceBaseAdress { get; set; }

        public Options(string authServiceBaseAdress)
        {
            AuthServiceBaseAdress = authServiceBaseAdress;
        }
    }
}
