using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace assistandon
{
    [DataContract]
    class UserListConst
    {
        [DataMember]
        public List<UserData> Users;
        //private List<UserData> _userList;

        //public List<UserData> Users { get { return _userList; } set { _userList = value; } }
        public UserListConst()
        {
            this.Users = new List<UserData>();
        }
    }
}
