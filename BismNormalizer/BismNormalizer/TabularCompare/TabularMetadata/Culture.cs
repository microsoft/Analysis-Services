using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model culture with properties and methods for comparison purposes.
    /// </summary>
    public class Culture : TabularObject
    {
        #region Private Members

        private TabularModel _parentTabularModel;
        private Tom.Culture _tomCulture;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Culture class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Culture object belongs to.</param>
        /// <param name="tomCulture">Tabular Object Model Culture object abtstracted by the Culture class.</param>
        public Culture(TabularModel parentTabularModel, Tom.Culture tomCulture) : base(tomCulture)
        {
            _parentTabularModel = parentTabularModel;
            _tomCulture = tomCulture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// TabularModel object that the Culture object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model Culture object abtstracted by the Culture class.
        /// </summary>
        public Tom.Culture TomCulture => _tomCulture;

        public override string ToString() => this.GetType().FullName;

        #endregion

        #region Public Methods

        /// <summary>
        /// Verifies whether another Culture object's translations are included in this one.
        /// </summary>
        /// <param name="otherCulture">The other culture for comparison.</param>
        /// <returns>Returns true if all translations contained.</returns>
        public bool ContainsOtherCultureTranslations(Culture otherCulture)
        {
            foreach (ObjectTranslation otherTranslation in otherCulture.TomCulture.ObjectTranslations)
            {
                bool foundMatch = false;
                foreach (ObjectTranslation translation in _tomCulture.ObjectTranslations)
                {
                    if (translation.Object is NamedMetadataObject &&
                        otherTranslation.Object is NamedMetadataObject &&
                        (
                                ((NamedMetadataObject)translation.Object).Name == ((NamedMetadataObject)otherTranslation.Object).Name            //Name of the object matches
                            ||  (translation.Object.ObjectType == ObjectType.Model && otherTranslation.Object.ObjectType == ObjectType.Model)    //Model name can legitimately have different names - and there can only be 1 model, so we are OK.
                        ) &&
                        translation.Object.ObjectType == otherTranslation.Object.ObjectType &&  //ObjectType like Measure, Table, ...
                        translation.Property == otherTranslation.Property &&                    //Property like Caption, DisplayFolder, Description
                        translation.Value == otherTranslation.Value)                            //Value is the translated value
                    {
                        foundMatch = true;
                        break;
                    }
                }
                if (!foundMatch)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
