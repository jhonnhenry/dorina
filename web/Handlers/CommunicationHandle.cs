using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using web.Models;

namespace web.Handlers
{
    public static class CommunicationHandle
    {
        public static CommunicationModel Send(string title, 
            string message, 
            string messageType = "notice",
            string messageIcon = "far fa-envelope", object model = null)
        {
            return new CommunicationModel()
            {
                Model = model,
                Message = new MessageModel()
                {
                    title = title,
                    text = message,
                    messageType = messageType,
                    messageIcon = messageIcon,
                }

            };
        }
    }
}
