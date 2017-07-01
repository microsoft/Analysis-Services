using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model expression with properties and methods for comparison purposes.
    /// </summary>
    public class Expression : TabularObject
    {
        private TabularModel _parentTabularModel;
        private NamedExpression _tomExpression;

        /// <summary>
        /// Initializes a new instance of the Expression class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Expression object belongs to.</param>
        /// <param name="expression">Tabular Object Model Expression object abtstracted by the Expression class.</param>
        public Expression(TabularModel parentTabularModel, NamedExpression expression) : base(expression)
        {
            _parentTabularModel = parentTabularModel;
            _tomExpression = expression;
        }

        /// <summary>
        /// TabularModel object that the Expression object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model NamedExpression object abtstracted by the Expression class.
        /// </summary>
        public NamedExpression TomExpression => _tomExpression;

        public override string ToString() => this.GetType().FullName;
    }
}
