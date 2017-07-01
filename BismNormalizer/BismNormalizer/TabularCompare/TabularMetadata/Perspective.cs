using System;
using Microsoft.AnalysisServices.Tabular;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Abstraction of a tabular model perspective with properties and methods for comparison purposes.
    /// </summary>
    public class Perspective : TabularObject
    {
        private TabularModel _parentTabularModel;
        private Tom.Perspective _tomPerspective;

        /// <summary>
        /// Initializes a new instance of the Perspective class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the perspective belongs to.</param>
        /// <param name="tomPerspective">Tabular Object Model Perspective object abtstracted by the Perspective class.</param>
        public Perspective(TabularModel parentTabularModel, Tom.Perspective tomPerspective): base(tomPerspective)
        {
            _parentTabularModel = parentTabularModel;
            _tomPerspective = tomPerspective;
        }

        /// <summary>
        /// TabularModel object that the Perspective object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Tabular Object Model Perspective object abtstracted by the Perspective class.
        /// </summary>
        public Tom.Perspective TomPerspective => _tomPerspective;

        public override string ToString() => this.GetType().FullName;

        /// <summary>
        /// Verifies whether another Perspective object's selections are included in this Perspective object.
        /// </summary>
        /// <param name="otherPerspective">The other Perspective object to be verified.</param>
        /// <returns>True if otherPerspective matches. False if does not match.</returns>
        public bool ContainsOtherPerspectiveSelections(Perspective otherPerspective)
        {
            bool everythingMatches = true;

            //Tables
            foreach (PerspectiveTable otherTable in otherPerspective.TomPerspective.PerspectiveTables)
            {
                bool foundTableMatch = false;
                foreach (PerspectiveTable perspectiveTable in _tomPerspective.PerspectiveTables)
                {
                    if (perspectiveTable.Name == otherTable.Name)
                    {
                        foundTableMatch = true;

                        #region Columns
                        foreach (PerspectiveColumn otherColumn in otherTable.PerspectiveColumns)
                        {
                            bool foundColumnMatch = false;
                            foreach (PerspectiveColumn perspectiveColumn in perspectiveTable.PerspectiveColumns)
                            {
                                if (perspectiveColumn.Name == otherColumn.Name)
                                {
                                    foundColumnMatch = true;
                                    break;
                                }
                            }
                            if (!foundColumnMatch)
                            {
                                everythingMatches = false;
                                break;
                            }
                        }
                        #endregion

                        #region Hierarchies
                        foreach (PerspectiveHierarchy otherHierarchy in otherTable.PerspectiveHierarchies)
                        {
                            bool foundHierarchyMatch = false;
                            foreach (PerspectiveHierarchy perspectiveHierarchy in perspectiveTable.PerspectiveHierarchies)
                            {
                                if (perspectiveHierarchy.Hierarchy.Name == otherHierarchy.Hierarchy.Name)
                                {
                                    foundHierarchyMatch = true;
                                    break;
                                }
                            }
                            if (!foundHierarchyMatch)
                            {
                                everythingMatches = false;
                                break;
                            }
                        }
                        #endregion

                        #region Measures
                        foreach (PerspectiveMeasure otherMeasure in otherTable.PerspectiveMeasures)
                        {
                            bool foundMeasureMatch = false;
                            foreach (PerspectiveMeasure perspectiveMeasure in perspectiveTable.PerspectiveMeasures)
                            {
                                if (perspectiveMeasure.Name == otherMeasure.Name)
                                {
                                    foundMeasureMatch = true;
                                    break;
                                }
                            }
                            if (!foundMeasureMatch)
                            {
                                everythingMatches = false;
                                break;
                            }
                        }
                        #endregion

                    }
                    if (!everythingMatches) break;
                }

                if (!foundTableMatch)
                {
                    everythingMatches = false;
                    break;
                }
            }

            return everythingMatches;
        }
    }
}
