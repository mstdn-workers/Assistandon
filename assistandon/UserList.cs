using System;
using System.Collections.Generic;
using System.Linq;
using Mastonet.Entities;
using System.Runtime.Serialization;
using System.Xml;

namespace assistandon
{
    class UserList
    {
        public List<UserData> list = new List<UserData>();
        public UserListConst li = new UserListConst();

        public string fileName = @".\UserList.xml";

        public bool SearchUserDataWithUserId(long userId)
        {
            foreach (var data in li.Users)
                if (data.UserId == userId)
                    return true;
            return false;
        }

        public UserData GetUserDataWithUserId(long userId)
        {
            var userData = new UserData();
            foreach (var data in li.Users)
                if (data.UserId == userId)
                    userData = data;
            return userData;
        }

        public UserData GetUserDataWithUserName(string userName)
        {
            var userData = new UserData();
            foreach (var data in li.Users)
                if (data.UserName.Equals(userName))
                    userData = data;
            return userData;
        }

        public UserData GetUserDataWithNickName(string nickName)
        {
            var userData = new UserData();
            foreach (var data in li.Users)
                if (data.NickName.Equals(nickName))
                    userData = data;
            return userData;
        }

        public void SetUserDataWithUserId(UserData data)
        {
            foreach (var x in li.Users.Select((item, index) => new {item,  index }))
            {
                if(x.item.UserId == data.UserId)
                {
                    li.Users[x.index] = data;
                    this.SaveUserList();
                    return;
                }
            }
            li.Users.Add(data);
        }

        public void SetUserDataWithUserName(UserData data)
        {
            foreach (var x in li.Users.Select((item, index) => new { item, index }))
            {
                if (x.item.UserName.Equals(data.UserName))
                {
                    li.Users[x.index] = data;
                    this.SaveUserList();
                    return;
                }
            }
            li.Users.Add(data);
        }

        public void SetUserDataWithNickName(UserData data)
        {
            foreach (var x in li.Users.Select((item, index) => new { item, index }))
            {
                if (x.item.NickName.Equals(data.NickName))
                {
                    li.Users[x.index] = data;
                    this.SaveUserList();
                    return;
                }
            }
            li.Users.Add(data);
        }

        public void AddUser(Account account)
        {
            if (!this.SearchUserDataWithUserId(account.Id))
            {
                var data = new UserData();
                data.UserId = account.Id;
                data.UserName = account.UserName;
                data.NickName = account.DisplayName;
                li.Users.Add(data);
            }
            this.SaveUserList();
        }


        public void SaveUserList()
        {
            
            var serializer = new DataContractSerializer(typeof(UserListConst));
            using (XmlWriter xw = XmlWriter.Create(this.fileName))
            {
                serializer.WriteObject(xw, this.li);
            }
        }

        public void LoadUserLists()
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(UserListConst));
                using (XmlReader xr = XmlReader.Create(this.fileName))
                {
                    this.li = (UserListConst)serializer.ReadObject(xr);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}  
