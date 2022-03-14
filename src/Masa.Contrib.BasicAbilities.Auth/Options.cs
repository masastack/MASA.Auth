namespace Masa.Contrib.BasicAbilities.Auth
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
