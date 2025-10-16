using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a user defined DAX function with properties and methods for comparison purposes.
    /// </summary>
    public class Function : TabularObject
    {
        private TabularModel _parentTabularModel;
        private Microsoft.AnalysisServices.Tabular.Function _tomFunction;

        /// <summary>
        /// Initializes a new instance of the Function class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Function object belongs to.</param>
        /// <param name="function">Tabular Object Model Function object abtstracted by the Function class.</param>
        public Function(TabularModel parentTabularModel, Microsoft.AnalysisServices.Tabular.Function function) : base(function, parentTabularModel)
        {
            _parentTabularModel = parentTabularModel;
            _tomFunction = function;
        }

        /// <summary>
        /// TabularModel object that the Function object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model Function object abtstracted by the Function class.
        /// </summary>
        public Microsoft.AnalysisServices.Tabular.Function TomFunction => _tomFunction;

        public override string ToString() => this.GetType().FullName;
    }
}
