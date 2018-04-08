using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algh.Api.HttpUser
{
    [Serializable]
    public class Audio : Algh.interfaces.IAudio
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }

        public static Audio empty = new Audio("", "", "");

        public Audio()
        {

        }

        public Audio(string url, string name, string author)
        {
            Url = url;
            Name = ResetTags(name);
            Author = ResetTags(author);
        }

        static string ResetTags(string name)
        {
            StringBuilder res = new StringBuilder(name);
            res.Replace("<em class=\"found\">","");
            res.Replace("</em>", "");
            return res.ToString();
        }

    }
}