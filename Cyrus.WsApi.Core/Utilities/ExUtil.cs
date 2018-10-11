using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cyrus.WsApi.Core.Utilities
{
    public static class ExUtil
    {
        public static string RegexReplace(this string source, string regex, string reAs, RegexOptions opt = RegexOptions.None)
        {
            if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(regex))
            {
                Regex r = new Regex(regex, opt);
                if (r.IsMatch(source))
                    source = r.Replace(source, reAs);
            }
            return source;
        }
    }
}
