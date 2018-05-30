using System.Configuration;
using System.Text.RegularExpressions;

namespace FileService.Configuration
{
    public class RuleElement : ConfigurationElement
    {
        [ConfigurationProperty("pattern")]
        public string Pattern => (string)this["pattern"];

        [ConfigurationProperty("destinationFolder")]
        public string DestinationFolder => (string)this["destinationFolder"];

        [ConfigurationProperty("addOrder")]
        public bool AddOrder => (bool)this["addOrder"];

        [ConfigurationProperty("addDate")]
        public bool AddDate => (bool)this["addDate"];
    }
}