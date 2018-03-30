using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace assistandon
{
    [DataContract]
    class UserData
    {
        [DataMember]
        public string UserName;

        [DataMember]
        public long UserId;

        [DataMember]
        public string NickName;

        public UserData()
        {
            UserName = "";
            UserId = -1;
            NickName = "";
        }
    }
}
