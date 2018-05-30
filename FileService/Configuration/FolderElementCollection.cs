using System.Configuration;

namespace FileService.Configuration
{
    [ConfigurationCollection(typeof(FolderElement), AddItemName = "folder")]
    public class FolderElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)element).Path;
        }
    }
}