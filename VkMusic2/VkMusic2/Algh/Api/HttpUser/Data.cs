using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using Algh.interfaces;

namespace Algh.Api.HttpUser
{
    public class Data : IData
    {
        public IMusic Music { get; private set; }
        public Data(Client client)
        {
            Music = new Music(client);
        }
    }
}
