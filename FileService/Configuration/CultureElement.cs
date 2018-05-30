using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Configuration
{
    public class CultureElement : ConfigurationElement
    {
        [ConfigurationProperty("type")]
        public CultureInfo Culture => (CultureInfo)this["type"];
    }
}
