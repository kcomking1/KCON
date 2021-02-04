using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KCSystem.Web.Models
{
    public class Menu
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Text { get; set; }
        public string Icon { get; set; }
       
        public string Url { get; set; }
        public string Permission { get; set; }
        public bool IsShow { get; set; } = true;
        public string TargetType { get; } = "iframe-tab";
        public bool IsHeader { get; } = false;
        public List<Menu> Children { get; set; }
    }
}
