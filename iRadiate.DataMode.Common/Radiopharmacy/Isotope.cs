using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using iRadiate.DataModel.Common;

namespace iRadiate.DataModel.Radiopharmacy
{
    /// <summary>
    /// Represents an isotope
    /// </summary>
    public class Isotope : DataStoreItem
    {
        private int _weight;
        private Element _element;
        private Isotope _daughter;
        private bool _metastable;
        

        private double _halfLife;

        /// <summary>
        /// Gets or sets the halflife of the isotope in seconds (SI unit)
        /// </summary>
        [Queryable]
        public double HalfLife
        {
            get { return _halfLife; }
            set { _halfLife = value; }
        }

        /// <summary>
        /// Gets the decay constant for the isope in seconds^-1
        /// </summary>
        [Queryable]
        public double DecayConst
        {
            get
            {
                return Math.Log(2) / HalfLife;
            }
        }

        /// <summary>
        /// Gets or set the atomic weight of the isotope (unitless)
        /// </summary>
        [Queryable]
        public int Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
            }
        }

        /// <summary>
        /// Gets or sets the element to which the isotope belongs
        /// </summary>
        [Queryable]
        public virtual Element Element
        {
            get { return _element; }
            set { _element = value; }
        }

        /// <summary>
        /// Gets the name of the isotope e.g. Technetium-99m
        /// </summary>
        [Queryable]
        public string Name
        {
            get
            {
                if(Element == null)
                {
                    return "";
                }
                return (_element.Name + "-" + WeightString);
            }
        }

        /// <summary>
        /// Gets the abbreviation for isotope eg. Tc-99m
        /// </summary>
        [Queryable]
        public string Abbreviation
        {
            get
            {
                return Element.Symbol + "-" + WeightString;
            }
        }

        /// <summary>
        /// Gets or sets the daughter product that this decays to
        /// </summary>
        
        public virtual Isotope Daugher
        {
            get { return _daughter; }
            set { _daughter = value; }
        }

         

        /// <summary>
        /// Bool-indicating whether the isotope is mestable and will therefore take the 'm'
        /// </summary>
        [Queryable]
        public bool Metastable
        {
            get { return _metastable; }
            set { _metastable = value; }
        }

        public string WeightString
        {
            get
            {
                if (Metastable)
                {
                    return Weight.ToString() + "m";
                }
                else
                {
                    return Weight.ToString();
                }
            }
        }

        


        

        
    }

   
}
