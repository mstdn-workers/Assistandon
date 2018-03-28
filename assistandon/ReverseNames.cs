using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace assistandon
{
    public class ReverseNames
    {
        public Dictionary<string, string> _namePairs = new Dictionary<string, string>();

        public Dictionary<string, string> SetReverseName(string content)
        {
            var nickNameSet = new Dictionary<string, string>();
            nickNameSet["userName"] = string.Empty;
            nickNameSet["nickName"] = string.Empty;

            try
            {
                var setNickNameReg = new Regex(RegexStringSet.SetNickName, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var m = setNickNameReg.Match(content);

                nickNameSet["userName"] = m.Groups[2].Value;
                nickNameSet["nickName"] = m.Groups[4].Value;

                _namePairs[nickNameSet["nickName"]] = nickNameSet["userName"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return nickNameSet;
        }

        public string GetNickName(string nickName)
        {
            string userName = nickName;
            if (_namePairs.TryGetValue(nickName, out userName)) ;
            else _namePairs[nickName] = userName;
            return nickName;
        }



    }
}
