using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.ServiceModel.Configuration;

namespace MailClient
{
    public class ConfigurationManagerWrapper : IConfigurationManager
    {
        private Configuration cfg;

        public ConfigurationManagerWrapper()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = string.Concat(path, "\\mail.config")
            };
            cfg = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }
        public string GetSetting(string key)
        {
            AppSettingsSection section = (cfg.GetSection("appSettings") as AppSettingsSection);
            var value = section.Settings[key].Value;
            return value;
        }
        public ChannelEndpointElement GetClient(string key)
        {
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(cfg);
            var endpoints = serviceModel.Client.Endpoints;

            if (endpoints.Count > 0)
                return endpoints[0];
            return null;
        }
        public T GetSection<T>(string sectionName)
        {
            return (T)ConfigurationManager.GetSection(sectionName);
        }
    }
}