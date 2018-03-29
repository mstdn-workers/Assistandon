using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assistandon
{
    class UserHasDataList
    {
        public List<UserHasData> list = new List<UserHasData>();

        int IndexWithUserId(long userId)
        {
            foreach (var x in this.list.Select((item, index) => new { item, index }))
                if (x.item.UserId == userId)
                    return x.index;
            return -1;
        }

        UserHasData GetUserHasData(long userId)
        {
            var data = new UserHasData();
            var index = this.IndexWithUserId(userId);
            if (index == -1)
                data.UserId = userId;
            else
                data = this.list[index];
            return data;
        }

        void SetUserHasData(UserHasData data)
        {
            var index = this.IndexWithUserId(data.UserId);
            if (index == -1)
                this.list.Add(data);
            else
                this.list[index] = data;
        }
    }
}
