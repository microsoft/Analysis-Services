namespace AlmToolkit.Model
{
    using BismNormalizer.TabularCompare.Core;

    class AngularComposite
    {
        public ComparisonNode ngComparison;
        public ComparisonObject dotNetComparison;
        
        /// <summary>
        /// Constructor for composite node with Angular and .Net
        /// </summary>
        /// <param name="node">Set value for angular node</param>
        /// <param name="comparisonObject">set value for .Net node</param>
        public AngularComposite(ComparisonNode node, ComparisonObject comparisonObject)
        {
            ngComparison = node;
            dotNetComparison = comparisonObject;
        }
    }
}
