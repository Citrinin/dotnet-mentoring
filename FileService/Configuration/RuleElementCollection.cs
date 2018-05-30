using System.Configuration;

namespace FileService.Configuration
{
    [ConfigurationCollection(typeof(RuleElement), AddItemName = "rule")]
    public class RuleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuleElement)element).Pattern;
        }
    }
}