using System;
using System.Collections.Generic;
using System.Text;

namespace Algh.Api.HttpUser
{
    static class Decode
    {
        private static string n = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN0PQRSTUVWXYZO123456789+/=";
        private static string o(string e)
        {
            int t = 0;
            int i = 0;
            string res = "";
            for (int a = 0; a < e.Length; a++)
            {
                i = n.IndexOf(e[a]);
                t = a % 4 != 0 ? 64 * t + i : i;
                int newc = 255 & t >> (-2 * (a + 1) & 6);
                if (newc != 0) res += (char)(newc);
            }
            return res;
        }

        private static int[] s(string e, int t)
        {
            int n = e.Length;
            int[] i = new int[n];
            int a = n - 1;
            for (t = Math.Abs(t); a >= 0; a--)
            {
                i[a] = (t += (a + n)) % n | 0;
            }
            return i;
        }

        private static char[] iss(string e, int t)
        {
            int n = e.Length;
            int[] i = s(e, t);
            //----------------------------------------------------------------------------------------------------------------

            char[] res = e.ToCharArray();
            for (int a = 1; a < n; a++)
            {

                int ofs = i[n - 1 - a];
                char buf = res[ofs];
                res[ofs] = res[a];
                res[a] = buf;
            }
            return res;
        }

        public static string r(string e)
        {
            try
            {
                var t = e.Split(new string[] { "?extra=" }, StringSplitOptions.None)[1].Split('#');
                var n = "" == t[1] ? "" : o(t[1]);
                int ss = Convert.ToInt32(n.Remove(0, 2));
                string sst = o(t[0]);
                return new string(iss(sst, ss));
            }
            catch
            {
                return "";
            }
        }

    }
}
