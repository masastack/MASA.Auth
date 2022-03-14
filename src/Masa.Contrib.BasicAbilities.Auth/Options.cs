using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
