using System.Configuration;

namespace FileService.Configuration
{
    public class ServiceConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("culture")]
        public CultureElement Culture => (CultureElement)this["culture"];

        [ConfigurationProperty("defaultFolder")]
        public DefaultFolderElement DefaultFolder => (DefaultFolderElement)this["defaultFolder"];

        [ConfigurationProperty("folders")]
        public FolderElementCollection Folders => (FolderElementCollection)this["folders"];

        [ConfigurationProperty("rules")]
        public RuleElementCollection Rules => (RuleElementCollection)this["rules"];
    }
}
