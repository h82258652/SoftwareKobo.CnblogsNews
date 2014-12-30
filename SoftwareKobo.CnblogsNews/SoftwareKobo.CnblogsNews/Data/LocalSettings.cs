using System;
using System.Net;
using Windows.Storage;

namespace SoftwareKobo.CnblogsNews.Data
{
    public static class LocalSettings
    {
        private static readonly ApplicationDataContainer LocalSettingsInstance = ApplicationData.Current.LocalSettings;

        public static RenderingEngine RenderingEngine
        {
            get
            {
                RenderingEngine renderingEngine;
                if (LocalSettingsInstance.Values.ContainsKey("RenderingEngine") == false || Enum.TryParse(LocalSettingsInstance.Values["RenderingEngine"] as string,
                    out renderingEngine) == false)
                {
                    LocalSettingsInstance.Values["RenderingEngine"] = RenderingEngine.Inter.ToString();
                }
                return (RenderingEngine)Enum.Parse(typeof(RenderingEngine),
                    LocalSettingsInstance.Values["RenderingEngine"].ToString());
            }
            set
            {
                LocalSettingsInstance.Values["RenderingEngine"] = value.ToString();
            }
        }

        public static Cookie LoginCookie
        {
            get
            {
                if (LocalSettingsInstance.Containers.ContainsKey("LoginCookie") == false)
                {
                    return null;
                }
                else
                {
                    var name = (string)LocalSettingsInstance.Containers["LoginCookie"].Values["Name"];
                    var value = (string)LocalSettingsInstance.Containers["LoginCookie"].Values["Value"];
                    var path = (string)LocalSettingsInstance.Containers["LoginCookie"].Values["Path"];
                    var domain = (string)LocalSettingsInstance.Containers["LoginCookie"].Values["Domain"];
                    return new Cookie(name, value, path, domain);
                }
            }
            set
            {
                if (value == null)
                {
                    if (LocalSettingsInstance.Containers.ContainsKey("LoginCookie"))
                    {
                        LocalSettingsInstance.DeleteContainer("LoginCookie");
                    }
                }
                else
                {
                    if (LocalSettingsInstance.Containers.ContainsKey("LoginCookie") == false)
                    {
                        LocalSettingsInstance.CreateContainer("LoginCookie",
                            ApplicationDataCreateDisposition.Always);
                    }
                    LocalSettingsInstance.Containers["LoginCookie"].Values["Name"] = value.Name;
                    LocalSettingsInstance.Containers["LoginCookie"].Values["Value"] = value.Value;
                    LocalSettingsInstance.Containers["LoginCookie"].Values["Path"] = value.Path;
                    LocalSettingsInstance.Containers["LoginCookie"].Values["Domain"] = value.Domain;
                }
            }
        }
    }
}