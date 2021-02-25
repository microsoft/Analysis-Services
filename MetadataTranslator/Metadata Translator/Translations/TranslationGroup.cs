using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public class TranslationGroup
    {
        public string Name { get; set; }
        public string Tag { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is TranslationGroup comparewith)
            {
                return Name == comparewith.Name && Tag == comparewith.Tag;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return new { Tag, Name }.GetHashCode();
        }
    }
}
