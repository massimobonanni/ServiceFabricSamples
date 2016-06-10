using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class ServiceContextExtensions
    {
        public static string GetConfigurationValue(this ServiceContext context, 
            string sectionName, string valueName)
        {
            if (context == null) throw new NullReferenceException();

            var configSection = 
                context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            var configValue = 
                configSection.Settings.Sections[sectionName].Parameters[valueName].Value;

            return configValue;
        }
    }
}
