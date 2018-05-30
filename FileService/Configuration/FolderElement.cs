using System.Configuration;

namespace FileService.Configuration
{
    public class FolderElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true)]
        public string Path => (string)this["path"];
    }
}