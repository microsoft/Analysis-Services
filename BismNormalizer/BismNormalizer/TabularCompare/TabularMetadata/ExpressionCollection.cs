using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.TabularMetadata
{
    /// <summary>
    /// Represents a collection of Expression objects.
    /// </summary>
    public class ExpressionCollection : List<Expression>
    {
        /// <summary>
        /// Find an object in the collection by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Expression object if found. Null if not found.</returns>
        public Expression FindByName(string name)
        {
            foreach (Expression expression in this)
            {
                if (expression.Name == name)
                {
                    return expression;
                }
            }
            return null;
        }

        /// <summary>
        /// A Boolean specifying whether the collection contains object by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if the object is found, or False if it's not found.</returns>
        public bool ContainsName(string name)
        {
            foreach (Expression expression in this)
            {
                if (expression.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find an object in the collection by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Expression object if found. Null if not found.</returns>
        public Expression FindById(string id)
        {
            foreach (Expression expression in this)
            {
                if (expression.InternalName == id)
                {
                    return expression;
                }
            }
            return null;
        }

        /// <summary>
        /// Removes an object from the collection by its internal name.
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns>True if the object was removed, or False if was not found.</returns>
        public bool Remove(string internalName)
        {
            foreach (Expression expression in this)
            {
                if (expression.InternalName == internalName)
                {
                    this.Remove(expression);
                    return true;
                }
            }
            return false;
        }
    }
}
