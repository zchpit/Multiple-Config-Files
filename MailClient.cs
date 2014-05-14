using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Reflection;
using Mail.Contracts;
using System.ServiceModel;
using NLog;
//using Utils;

namespace MailClient
{
    public class MailClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationManager configuration;
        string contentPath;
        string content;
        string attachementPath;
        string sender;
        string recipient;
        string cc;
        string subject;

        public MailClient() : this(new ConfigurationManagerWrapper())
        {
        }
        public MailClient(IConfigurationManager configuration)
        {
            this.configuration = configuration;

            this.content = configuration.GetSetting("content");
            this.attachementPath = configuration.GetSetting("attachementPath");
            this.sender = configuration.GetSetting("sender");
            this.recipient = configuration.GetSetting("recipient");
            this.cc = configuration.GetSetting("cc");
            this.subject = configuration.GetSetting("subject");
        }
        public void SendMail(string subject, string content)
        {
            try
            {
                var mail = new MailModel
                {
                    Sender = sender,
                    Content = content,
                    Recipent = recipient,
                    CC = cc,
                    Subject = subject
                };

                if (!string.IsNullOrEmpty(attachementPath))
                {
                    using (var stream = new StreamReader(attachementPath))
                    using (var memoryStream = new MemoryStream())
                    {
                        CopyStream(stream.BaseStream, memoryStream);
                        mail.Attachement = memoryStream;
                        mail.AttachementName = Path.GetFileName(attachementPath);
                        SendMailByService(mail);
                    }
                }
                else
                {
                    SendMailByService(mail);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    logger.Info("Sending mail finish with failure");
                    //logger.LogFullExceptionDetails(ex);
                }
                catch(Exception exe) //chce, aby to się pojawiło w logach klienta, a to jest, gdyby klient nie miał logowania
                {

                }
            }
        }
        private void SendMailByService(MailModel mail)
        {
            var mailEndpoint = this.configuration.GetClient("IMailService");
            BasicHttpBinding myBinding = new BasicHttpBinding();
            EndpointAddress myEndpoint = new EndpointAddress(mailEndpoint.Address.AbsoluteUri);

            IMailService client = new ChannelFactory<IMailService>(myBinding, myEndpoint).CreateChannel();
            client.Send(mail);
        }
        static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}
