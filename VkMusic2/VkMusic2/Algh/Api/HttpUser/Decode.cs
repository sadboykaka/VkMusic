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
                t = (n * (a + 1) ^ t + a) % n;
                i[a] = t;
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

        private static string nn = n + n;
        private static char[] isr(string t, int e)
        {
            int i;
            char[] res = t.ToCharArray();
            for (int a = t.Length - 1; a >= 0; a--)
            {
                i = nn.IndexOf(res[a]);
                if (i == -1) continue;
                i -= e;
                if (i < 0) i = nn.Length + i;
                res[a] = nn[i];
            }
            return res;
        }

        private static char[] isx(string t, int e)
        {
            int ex = e.ToString()[0];
            char[] res = new char[t.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = (char)(t[i] ^ ex);
            }
            return res;
        }

        const char sep = ((char)9);

        public static string r(string e, int id)
        {
            try
            {
                if (e.IndexOf("audio_api_unavailable") == -1) return e;
                var t = e.Split(new string[] { "?extra=" }, StringSplitOptions.None)[1].Split('#');
                var n = "" == t[1] ? "" : o(t[1]);
                var spl = n.Split(sep);
                int[] codes = new int[spl.Length];
                char[] alghr = new char[spl.Length];
                for (int i = 0; i < codes.Length; i++)
                {
                    alghr[i] = spl[i][0];
                    codes[i] = Convert.ToInt32(spl[i].Remove(0, 2));
                }
                string sst = o(t[0]);
                for (int i = codes.Length - 1; i >= 0; i--)
                {
                    if (alghr[i] == 'r') sst = new string(isr(sst, codes[i]));
                    else if (alghr[i] == 's') sst = new string(iss(sst, codes[i]));
                    else if (alghr[i] == 'x') sst = new string(isx(sst, codes[i]));
                    else if (alghr[i] == 'i') sst = new string(iss(sst, codes[i] ^ id));
                    else return "";
                }

                return sst;
            }
            catch
            {
                return "";
            }
        }

    }
}
