using Algh.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algh.Api.HttpUser
{
    [Serializable]
    public class Audio : IAudio
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }

        public string ID { get; set; }
        public string Cover { get; set; }

        public string From { get; set; }

        public static Audio empty = new Audio("", "", "","","","");

        public Audio()
        {

        }

        public Audio(string url, string name, string author, string id, string cover, string from)
        {
            Url = url;
            Name = ResetTags(name);
            Author = ResetTags(author);
            ID = id;
            Cover = cover;
            From = from;
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