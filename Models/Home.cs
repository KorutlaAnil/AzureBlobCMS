using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureBlobCMS.Models
{
    public class Home
    {
        public English eng { get; set; }
        public Chinese chn { get; set; }
    }

    public class English
    {
        public string Name { get; set; }
        public string Html { get; set; }
    }
    public class Chinese
    {
        public string Name { get; set; }
        public string Html { get; set; }
    }
}
