using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BismNormalizer.TabularCompare.MultidimensionalMetadata
{
    //[Obsolete("This class is obsolete. Left over from BISM Normalizer 2, which supported BIDS Helper actions.")]

    /// <summary>
    /// Represents a collection of Action objects.
    /// </summary>
    public class ActionCollection : List<Action>
    {
        public Action FindByName(string name)
        {
            foreach (Action action in this)
            {
                if (action.Name == name)
                {
                    return action;
                }
            }
            return null;
        }
        public bool ContainsName(string name)
        {
            foreach (Action action in this)
            {
                if (action.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public Action FindById(string id)
        {
            foreach (Action action in this)
            {
                if (action.Id == id)
                {
                    return action;
                }
            }
            return null;
        }
        public bool ContainsId(string id)
        {
            foreach (Action action in this)
            {
                if (action.Id == id)
                {
                    return true;
                }
            }
            return false;
        }

        public bool RemoveById(string id)
        {
            foreach (Action action in this)
            {
                if (action.Id == id)
                {
                    this.Remove(action);
                    return true;
                }
            }
            return false;
        }
    }
}
