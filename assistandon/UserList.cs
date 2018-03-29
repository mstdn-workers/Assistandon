using System.Collections.Generic;
using System.Linq;
using Mastonet.Entities;
using System.Runtime.Serialization;

namespace assistandon
{
    [DataContract]
    public class UserList
    {
        public List<UserData> list = new List<UserData>();

        public bool SearchUserDataWithUserId(long userId)
        {
            foreach (var data in list)
                if (data.UserId == userId)
                    return true;
            return false;
        }

        public UserData GetUserDataWithUserId(long userId)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.UserId == userId)
                    userData = data;
            return userData;
        }

        public UserData GetUserDataWithUserName(string userName)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.UserName.Equals(userName))
                    userData = data;
            return userData;
        }

        public UserData GetUserDataWithNickName(string nickName)
        {
            var userData = new UserData();
            foreach (var data in list)
                if (data.NickName.Equals(nickName))
                    userData = data;
            return userData;
        }

        public void SetUserDataWithUserId(UserData data)
        {
            foreach (var x in list.Select((item, index) => new {item,  index }))
            {
                if(x.item.UserId == data.UserId)
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

        public void SetUserDataWithUserName(UserData data)
        {
            foreach (var x in list.Select((item, index) => new { item, index }))
            {
                if (x.item.UserName.Equals(data.UserName))
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

        public void SetUserDataWithNickName(UserData data)
        {
            foreach (var x in list.Select((item, index) => new { item, index }))
            {
                if (x.item.NickName.Equals(data.NickName))
                {
                    list[x.index] = data;
                    return;
                }
            }
            list.Add(data);
        }

        public void AddUser(Account account)
        {
            if (!this.SearchUserDataWithUserId(account.Id))
            {
                var data = new UserData();
                data.UserId = account.Id;
                data.UserName = account.UserName;
                data.NickName = account.DisplayName;
                list.Add(data);
            }
        }

    }
}  
