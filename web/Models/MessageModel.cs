namespace web.Models
{
    public class MessageModel
    {
        public MessageModel(string title, string text, MessageType messageType, MessageIcon messageIcon)
        {
            this.title = title;
            this.text = text;
            this.messageType = messageType == null ? MessageType.notice.Value : messageType.Value;
            this.messageIcon = $"fas fa-{(messageIcon == null ? MessageIcon.envelope.Value : messageIcon.Value)}";
        }
        public string title { get; set; }
        public string text { get; set; }
        public string messageType { get; set; }
        public string messageIcon { get; set; }
    }

    public class MessageType
    {
        private MessageType(string value) { Value = value; }
        public string Value { get; private set; }

        public static MessageType notice { get { return new MessageType("notice"); } }
        public static MessageType info { get { return new MessageType("info"); } }
        public static MessageType success { get { return new MessageType("success"); } }
        public static MessageType error { get { return new MessageType("error"); } }
    }

    public class MessageIcon
    {
        private MessageIcon(string value) { Value = value; }
        public string Value { get; private set; }

        public static MessageIcon envelope { get { return new MessageIcon("envelope"); } }
        public static MessageIcon error { get { return new MessageIcon("exclamation-triangle"); } }
    }
}
