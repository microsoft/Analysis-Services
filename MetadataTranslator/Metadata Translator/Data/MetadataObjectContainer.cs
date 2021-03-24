using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metadata_Translator
{
    public class MetadataObjectContainer
    {
        public virtual NamedMetadataObject TabularObject { get; protected set; }
        public TranslatedProperty TranslatedProperty { get; protected set; }

        public Guid TemporaryObjectId { get; protected set; }
        public MetadataObjectContainer(NamedMetadataObject metadataObject, TranslatedProperty translatedProperty)
        {
            TabularObject = metadataObject;
            TranslatedProperty = translatedProperty;
            TemporaryObjectId = Guid.NewGuid();
        }

        public override string ToString()
        {
            switch(TranslatedProperty)
            {
                case TranslatedProperty.Caption:
                    return $"{TabularObject.ObjectType} - Caption";
                case TranslatedProperty.Description:
                    return $"{TabularObject.ObjectType} - Description";
                case TranslatedProperty.DisplayFolder:
                    return $"{TabularObject.ObjectType} - DisplayFolder";
                default:
                    return TabularObject.ObjectType.ToString();
            }
        }
    }
}
