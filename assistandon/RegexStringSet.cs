using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assistandon
{
    class RegexStringSet
    {
        public static string CallMePattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*$";
        public static string QuakeCheckPattern = @"^.*(?<!「)(ゆき|ユキ|悠希|ゆっきー|ユッキー)(?!」).*((揺|ゆ)れた).*$";
    }
}
