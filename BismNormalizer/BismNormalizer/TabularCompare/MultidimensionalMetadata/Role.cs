using System;
using System.Collections.Generic;
using Microsoft.AnalysisServices;
using Amo=Microsoft.AnalysisServices;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    /// <summary>
    /// Abstraction of a tabular model role with properties and methods for comparison purposes.
    /// </summary>
    public class Role : ITabularObject
    {
        private TabularModel _parentTabularModel;
        private Amo.Role _amoRole;
        private string _objectDefinition;
        private string _substituteId;

        /// <summary>
        /// Initializes a new instance of the Role class using multiple parameters.
        /// </summary>
        /// <param name="parentTabularModel">TabularModel object that the Role object belongs to.</param>
        /// <param name="role">Analysis Management Objects Role object abtstracted by the Role class.</param>
        public Role(TabularModel parentTabularModel, Amo.Role role)
        {
            _parentTabularModel = parentTabularModel;
            _amoRole = role;

            _objectDefinition = "Permissions:\n";
            foreach (DatabasePermission dbPermission in _parentTabularModel.AmoDatabase.DatabasePermissions)
            {
                if (dbPermission.RoleID == _amoRole.ID)
                {
                    if (dbPermission.Administer)
                    {
                        _objectDefinition += "Administrator\n";
                    }
                    else if (dbPermission.Read == ReadAccess.Allowed && dbPermission.Process)
                    {
                        _objectDefinition += "Read and Process\n";
                    }
                    else if (dbPermission.Read == ReadAccess.Allowed)
                    {
                        _objectDefinition += "Read\n";
                    }
                    else if (dbPermission.Process)
                    {
                        _objectDefinition += "Process\n";
                    }
                    else
                    {
                        _objectDefinition += "None\n";
                    }
                }
            }

            _objectDefinition += "\nRow Filters:\n";
            List<string> rowFilters = new List<string>(); //need to be added in alphabetical order to allow comparison if obj definitions
            foreach (Dimension dimension in _parentTabularModel.AmoDatabase.Dimensions)
            {
                foreach (DimensionPermission dimPermission in dimension.DimensionPermissions)
                {
                    if (dimPermission.RoleID == _amoRole.ID)
                    {
                        //_objectDefinition += "[" + dimension.Name + "]=" + dimPermission.AllowedRowsExpression + "\n";
                        rowFilters.Add("[" + dimension.Name + "]=" + dimPermission.AllowedRowsExpression + "\n");
                    }
                }
            }
            rowFilters.Sort();
            foreach (string rowFilter in rowFilters)
            {
                _objectDefinition += rowFilter;
            }

            _objectDefinition += "\nMembers:\n";
            List<string> roleMembers = new List<string>(); //need to be added in alphabetical order to allow comparison if obj definitions
            foreach (RoleMember roleMember in _amoRole.Members)
            {
                //_objectDefinition += roleMember.Name + "\n";
                roleMembers.Add(roleMember.Name + "\n");
            }
            roleMembers.Sort();
            foreach (string roleMember in roleMembers)
            {
                _objectDefinition += roleMember;
            }
        }

        /// <summary>
        /// TabularModel object that the Role object belongs to.
        /// </summary>
        public TabularModel ParentTabularModel => _parentTabularModel;

        /// <summary>
        /// Analysis Management Objects Role object abtstracted by the Role class.
        /// </summary>
        public Amo.Role AmoRole => _amoRole;

        /// <summary>
        /// Name of the Role object.
        /// </summary>
        public string Name => _amoRole.Name;

        /// <summary>
        /// Long name of the Role object.
        /// </summary>
        public string LongName => _amoRole.Name;

        /// <summary>
        /// Id of the Role object.
        /// </summary>
        public string Id => _amoRole.ID;

        /// <summary>
        /// Object definition of the Role object. This is a simplified list of relevant attribute values for comparison; not the XMLA definition of the abstracted AMO object.
        /// </summary>
        public string ObjectDefinition => _objectDefinition;

        /// <summary>
        /// Substitute Id of the Role object.
        /// </summary>
        public string SubstituteId
        {
            get
            {
                if (string.IsNullOrEmpty(_substituteId))
                {
                    return _amoRole.ID;
                }
                else
                {
                    return _substituteId;
                }
            }
            set
            {
                _substituteId = value;
            }
        }

        public override string ToString() => this.GetType().FullName;
    }
}
