using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace assistandon
{
    class UserList
    {
        public List<UserData> list = new List<UserData>();
        
        UserData GetUserDataWithUserId(long userId)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.UserId == userId)
                    userData = data;
            return userData;
        }

        UserData GetUserDataWithUserName(string userName)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.UserName.Equals(userName))
                    userData = data;
            return userData;
        }

        UserData GetUserDataWithNickName(string nickName)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.NickName.Equals(nickName))
                    userData = data;
            return userData;
        }

        void SetUserDataWithUserId(long userId, UserData data)
        {
            foreach (var x in list.Select((item, index) => new {item,  index }))
            {
                if(x.item.UserId == userId)
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

        void SetUserDataWithUserName(string userName, UserData data)
        {
            foreach (var x in list.Select((item, index) => new { item, index }))
            {
                if (x.item.UserName.Equals(userName))
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

        void SetUserDataWithNickName(string nickName, UserData data)
        {
            foreach (var x in list.Select((item, index) => new { item, index }))
            {
                if (x.item.NickName.Equals(nickName))
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

    }
}  
