using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Enums
{
    public enum NotificationType
    {
        NewPost,           
        NewComment,       
        System,            
        FriendRequest,     
        Like,              
        Mention,        
        Other         
    }

}
