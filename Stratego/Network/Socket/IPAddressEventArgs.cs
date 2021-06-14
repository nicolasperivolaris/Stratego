using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Stratego.Network.Socket
{
    public class IPAddressEventArgs: EventArgs
    {
        public IPAddress Address;

        public IPAddressEventArgs(IPAddress address)
        {
            Address = address;
        }
    }
}
