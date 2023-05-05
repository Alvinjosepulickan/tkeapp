using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using TKE.SC.BFF.DataAccess.Helpers;

namespace TKE.SC.BFF.DataAccess.Services
{

    public class WsSecurityMessageInspector : IClientMessageInspector
    {
        private readonly string password;
        private readonly string username;

        /// <summary>
        /// WsSecurityMessageInspector
        /// </summary>
        /// <param Name="username"></param>
        /// <param Name="password"></param>
        /// <param Name="logger"></param>
        public WsSecurityMessageInspector(string username, string password, ILogger<WsSecurityMessageInspector> logger)
        {
            this.username = username;
            this.password = password;
            Utility.SetLogger(logger);
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var methodBegin = Utility.LogBegin();
            var sessionkeepalive_header = MessageHeader.CreateHeader("SessionKeepAlive", "urn:crmondemand/ws", "true", true);
            request.Headers.Add(sessionkeepalive_header);

            var header = new Security
            {
                UsernameToken =
            {
                Password = new Password
                {
                    Value = password,
                    Type =
                        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-_username-token-profile-1.0#PasswordText"
                },
                Username = username
            }
            };
            request.Headers.Add(header);
            Utility.LogEnd(methodBegin);
            return null;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }


    public class Password
    {
        [XmlAttribute] public string Type { get; set; }

        [XmlText] public string Value { get; set; }
    }

    [XmlRoot(Namespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
    public class UsernameToken
    {
        [XmlElement] public string Username { get; set; }

        [XmlElement] public Password Password { get; set; }
    }

    public class Security : MessageHeader
    {
        public Security()
        {
            UsernameToken = new UsernameToken();
        }

        public UsernameToken UsernameToken { get; set; }

        public override string Name => GetType().Name;

        public override string Namespace =>
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        public override bool MustUnderstand => true;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            var serializer = new XmlSerializer(typeof(UsernameToken));
            serializer.Serialize(writer, UsernameToken);
        }



    }


}
