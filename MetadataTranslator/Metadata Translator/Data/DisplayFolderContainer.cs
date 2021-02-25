using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public class DisplayFolderContainer : MetadataObjectContainer
    {
        public override NamedMetadataObject TabularObject { get => TabularObjects.FirstOrDefault(); protected set { } }
        public List<NamedMetadataObject> TabularObjects { get; private set; }

        public DisplayFolderContainer(NamedMetadataObject metadataObject, TranslatedProperty translatedProperty) : base(metadataObject, translatedProperty)
        {
            TabularObjects = new List<NamedMetadataObject>();
            TabularObjects.Add(metadataObject);
        }

        public override string ToString()
        {
            return (TabularObjects.Count > 1)?
                $"DisplayFolder - {TabularObjects.Count} Objects" :
                $"DisplayFolder - 1 {TabularObject.ObjectType}";
        }
    }
}
