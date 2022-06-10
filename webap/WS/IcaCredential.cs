using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Ica.SalesOrders.Web.WS
{
    public class IcaCredentials : ClientCredentials
    {
        public IcaCredentials()
        { }

        protected IcaCredentials(IcaCredentials cc)
            : base(cc)
        { }

        public override System.IdentityModel.Selectors.SecurityTokenManager CreateSecurityTokenManager()
        {
            return new CustomSecurityTokenManager(this);
        }

        protected override ClientCredentials CloneCore()
        {
            return new IcaCredentials(this);
        }
    }

    public class CustomSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        public CustomSecurityTokenManager(IcaCredentials cred)
            : base(cred)
        { }

        public override System.IdentityModel.Selectors.SecurityTokenSerializer CreateSecurityTokenSerializer(System.IdentityModel.Selectors.SecurityTokenVersion version)
        {
            return new CustomTokenSerializer(System.ServiceModel.Security.SecurityVersion.WSSecurity11);
        }
    }

    public class CustomTokenSerializer : WSSecurityTokenSerializer
    {
        public CustomTokenSerializer(SecurityVersion sv)
            : base(sv)
        { }

        protected override void WriteTokenCore(System.Xml.XmlWriter writer,
                                                System.IdentityModel.Tokens.SecurityToken token)
        {
            UserNameSecurityToken userToken = token as UserNameSecurityToken;

            string tokennamespace = "o";

            DateTime created = DateTime.Now;
            string createdStr = created.ToString("yyyy-MM-ddThh:mm:ss.fffZ");

            // unique Nonce value - encode with SHA-1 for 'randomness'
            // in theory the nonce could just be the GUID by itself
            string phrase = Guid.NewGuid().ToString();
            var nonce = GetSHA1String(phrase);

            // in this case password is plain text
            // for digest mode password needs to be encoded as:
            // PasswordAsDigest = Base64(SHA-1(Nonce + Created + Password))
            // and profile needs to change to
            //string password = GetSHA1String(nonce + createdStr + userToken.Password);

            string password = userToken.Password;

            writer.WriteRaw(string.Format(
            "<{0}:UsernameToken u:Id=\"" + token.Id +
            "\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
            "<{0}:Username>" + userToken.UserName + "</{0}:Username>" +
            "<{0}:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">" +
            password + "</{0}:Password>" +
            "</{0}:UsernameToken>", tokennamespace));
        }

        protected string GetSHA1String(string phrase)
        {
            SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();
            byte[] hashedDataBytes = sha1Hasher.ComputeHash(Encoding.UTF8.GetBytes(phrase));
            return Convert.ToBase64String(hashedDataBytes);
        }
    }

    public class MyInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);
            reply.WriteMessage(writer);
            writer.Flush();
            ms.Position = 0;
            doc.Load(ms);
            foreach (XmlNode node in doc.SelectNodes("//rateExchangeOverride"))
            {
                if (Regex.Matches(node.InnerText, @"[a-zA-Z]").Count > 0)
                {
                    node.InnerText = "0";
                }
            }
            //ChangeMessage(doc, false);
            ms.SetLength(0);
            writer = XmlWriter.Create(ms);
            doc.WriteTo(writer);
            writer.Flush();
            ms.Position = 0;
            XmlReader reader = XmlReader.Create(ms);
            reply = Message.CreateMessage(reader, int.MaxValue, reply.Version);
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            XmlDocument doc = new XmlDocument();
            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms);
            request.WriteMessage(writer);
            writer.Flush();
            ms.Position = 0;
            doc.Load(ms);

            foreach (XmlNode node in doc.SelectNodes("//header"))
            {
                XmlNode newNode = doc.CreateNode(XmlNodeType.Element, "statusCodeNext", null);
                newNode.InnerText = "999";
                node.AppendChild(newNode);
            }
            //ChangeMessage(doc, false);
            ms.SetLength(0);
            writer = XmlWriter.Create(ms);
            doc.WriteTo(writer);
            writer.Flush();
            ms.Position = 0;
            XmlReader reader = XmlReader.Create(ms);
            request = Message.CreateMessage(reader, int.MaxValue, request.Version);
            //Message request2 = Message.CreateMessage(MessageVersion.Default, "prova", Encoding.UTF8.GetString(ms.ToArray()));
            return request;
        }

        private void ChangeMessage(XmlDocument doc, bool flag)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("s", "http://schemas.xmlsoap.org/soap/envelope/");
            nsManager.AddNamespace("tempuri", "http://tempuri.org/");
            XmlNode node = doc.SelectSingleNode("//s:Body", nsManager);
            if (node != null)
            {
                if (flag)
                    node.InnerXml = node.InnerXml.Replace("GetData", "GetData1");
                else
                    node.InnerXml = node.InnerXml.Replace("GetData1Response", "GetDataResponse").Replace("GetData1Result", "GetDataResult");
            }
        }
    }

    public class MyBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            //endpoint.Behaviors.Add(new MyBehavior());
        }

        public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            MyInspector inspector = new MyInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
        {
        }

        protected override object CreateBehavior()
        {
            return new MyBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                Type t = Type.GetType("Ica.SalesOrders.Web.WS.MyBehavior");
                return t;
            }
        }
    }
}