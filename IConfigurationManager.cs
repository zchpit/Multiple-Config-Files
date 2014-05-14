using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;

namespace MailClient
{
    public interface IConfigurationManager
    {
        T GetSection<T>(string sectionName);
        string GetSetting(string key);
        ChannelEndpointElement GetClient(string endpointName);
    }
}