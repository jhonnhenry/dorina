using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using web.Models;

namespace web.Handlers
{
    public static class CommunicationHandle
    {
        public static CommunicationModel Send(
            string title, 
            string message,
            MessageType messageType = null,
            MessageIcon messageIcon = null,
            object model = null) {
            var communicationModel = new MessageModel(title, message, messageType, MessageIcon.envelope);
            return new CommunicationModel()
            {
                Model = model,
                Message = communicationModel
            };
        }
    }
}
