using Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model role with properties and methods for comparison purposes.
    /// </summary>
    public class Role : TabularObject
    {
        private TabularModel _parentTabularModel;
        private ModelRole _tomRole;

        /// <summary>
        /// Initializes a new instance of the Role class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Role object belongs to.</param>
        /// <param name="role">Tabular Object Model Role object abtstracted by the Role class.</param>
        public Role(TabularModel parentTabularModel, ModelRole role) : base(role)
        {
            _parentTabularModel = parentTabularModel;
            _tomRole = role;
        }

        /// <summary>
        /// TabularModel object that the Role object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model ModelRole object abtstracted by the Role class.
        /// </summary>
        public ModelRole TomRole => _tomRole;

        public override string ToString() => this.GetType().FullName;
    }
}
