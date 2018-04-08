using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;


namespace Algh.Api.HttpUser
{

    static class Cookies
    {
        public static string appdir { get; } = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        
        public const string BaseName = "Acc.data";

        public static void DeleteCookies(string file)
        {
            if(File.Exists(appdir + "/" + file)) File.Delete(appdir + "/" + file);
        }

        public static void WriteCookiesToDisk(string file, CookieContainer cookieJar)
        {
            using (Stream stream = File.Create(appdir + "/" + file))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookieJar);
            }
        }

        public static CookieContainer ReadCookiesFromDisk(string file)
        {

            try
            {
                using (Stream stream = File.Open(appdir + "/" + file, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch 
            {
                return null;
            }
        }
    }
}
