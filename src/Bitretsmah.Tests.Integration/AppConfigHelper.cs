using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Bitretsmah.Tests.Integration
{
    internal static class AppConfigHelper
    {
        private static Dictionary<string, string> _secretSettings = new Dictionary<string, string>();

        static AppConfigHelper()
        {
            const string secretSettingsPath = @"D:\Projects\Settings\bitretsmah.config";

            if (File.Exists(secretSettingsPath))
            {
                var json = File.ReadAllText(secretSettingsPath);
                _secretSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        public static string GetSetting(string key)
        {
            if (_secretSettings.ContainsKey(key))
            {
                return _secretSettings[key];
            }

            if (ConfigurationManager.AppSettings.AllKeys.Any(x => x == key))
            {
                return ConfigurationManager.AppSettings[key];
            }

            throw new InvalidOperationException($"Unknown setting: '{key}'.");
        }
    }
}