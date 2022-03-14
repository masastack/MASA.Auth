using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Auth.Sdk
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
