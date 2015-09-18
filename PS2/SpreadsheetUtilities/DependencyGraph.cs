// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// s1 depends on t1 --> t1 must be evaluated before s1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// (Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.)
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {

        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int size;
          
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return dependees.ContainsKey(s) ? dependees[s].Count : 0; }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s)) return dependents[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (dependees.ContainsKey(s)) return dependees[s].Count > 0;
            return false;
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            return dependents.ContainsKey(s) ? new HashSet<string>(dependents[s]) : new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            return dependees.ContainsKey(s) ? new HashSet<string>(dependees[s]) : new HashSet<string>();
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   s depends on t
        ///
        /// </summary>
        /// <param name="s"> s cannot be evaluated until t is</param>
        /// <param name="t"> t must be evaluated first.  S depends on T</param>
        public void AddDependency(string s, string t)
        {
            //Make sure size actually changed before we increment size
            bool sizeChanged = false;

            //If there's already a list of dependents, then we can add another one
            if (dependents.ContainsKey(s))
            {
                if (dependents[s].Add(t)) sizeChanged = true;
            }
            //Add a dependent to an existing list of dependents for this key
            else
            {
                HashSet<string> set = new HashSet<string>();
                set.Add(t);
                dependents.Add(s, set);
                sizeChanged = true;
            }

            //If there's already a list of dependees, then we can add another one
            if (dependees.ContainsKey(t))
            {
                if (dependees[t].Add(s)) sizeChanged = true;
            }
            else
            {
                HashSet<string> set = new HashSet<string>();
                set.Add(s);
                dependees.Add(t, set);
                sizeChanged = true;
            }

            //If anything was added, increment size
            if(sizeChanged)
            {
                size++;
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            int initialSize = size;
            bool anythingRemoved = false;

            //Only try to remove the dependents if they exist
            if (dependents.ContainsKey(s))
            {
                if(dependents[s].Remove(t)) anythingRemoved = true;
            }
            if (dependees.ContainsKey(t))
            {
                if(dependees[t].Remove(s)) anythingRemoved = true;
            }

            //If anything was actually removed, decrement size.
            if (anythingRemoved)
            {
                size--;
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (dependents.ContainsKey(s))
            {
                //Remove all the existing dependent and dependee records for this dependent first
                HashSet<string> oldDependents = new HashSet<string>(dependents[s]);
                foreach(string oldDependent in oldDependents)
                {
                    RemoveDependency(s, oldDependent);
                }
            }

            foreach(string newDependent in newDependents)
            {
                AddDependency(s, newDependent);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (dependees.ContainsKey(s))
            {
                HashSet<string> oldDependees = new HashSet<string>(dependees[s]);
                foreach(string oldDependee in oldDependees)
                {
                    RemoveDependency(oldDependee, s);
                }
            }

            foreach(string newDependee in newDependees)
            {
                AddDependency(newDependee, s);
            }
        }
        
    }

    
}