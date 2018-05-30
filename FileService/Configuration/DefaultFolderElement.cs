using System.Configuration;

namespace FileService.Configuration
{
    public class DefaultFolderElement : ConfigurationElement
    {
        [ConfigurationProperty("path")]
        public string Path => (string)this["path"];
    }
}