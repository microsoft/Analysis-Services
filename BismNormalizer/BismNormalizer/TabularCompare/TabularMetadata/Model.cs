using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model [model] with properties and methods for comparison purposes.
    /// </summary>
    public class Model : TabularObject
    {
        #region Private Members

        private TabularModel _parentTabularModel;
        private Tom.Model _tomModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Model class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Model object belongs to.</param>
        /// <param name="tomModel">Tabular Object Model Model object abtstracted by the Model class.</param>
        public Model(TabularModel parentTabularModel, Tom.Model tomModel) : base(tomModel)
        {
            _parentTabularModel = parentTabularModel;
            _tomModel = tomModel;

            PopulateProperties();
        }

        #endregion

        #region Properties

        /// <summary>
        /// TabularModel object that the Model object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model Model object abtstracted by the Model class.
        /// </summary>
        public Tom.Model TomModel => _tomModel;

        public override string ToString() => this.GetType().FullName;

        #endregion

        private void PopulateProperties()
        {
            string customObjectDefinition = "{ ";
            if (!string.IsNullOrEmpty(_tomModel.Description))
            {
                customObjectDefinition += $"\"description\": \"{_tomModel.Description}\", ";
            }
            customObjectDefinition += $"\"defaultMode\": \"{_tomModel.DefaultMode.ToString().ToLower()}\", ";
            customObjectDefinition += $"\"discourageImplicitMeasures\": {_tomModel.DiscourageImplicitMeasures.ToString().ToLower()} }}";
            base.SetCustomObjectDefinition(customObjectDefinition);
        }


    }
}
