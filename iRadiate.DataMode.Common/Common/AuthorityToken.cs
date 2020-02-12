using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iRadiate.Common
{
    /// <summary>
    /// Represents a group of AuthorityTokens
    /// </summary>
    /// <remarks>
    /// AuthorityTokens and AuthorityTokenGroups are hard coded and instantiated
    /// at run time. The list of Authority Tokens that each user has is persistently stored.
    /// AuthorityTokens are heirarchially store in AuthorityTokenGroups
    /// </remarks>
    public class AuthorityTokenGroup
    {
        private string _name;
        private AuthorityTokenGroup _parent;
        private List<AuthorityToken> _authorityTokens;
        public string ParentName;

        public AuthorityTokenGroup(string fullName)
        {
            //See if there are dots
            if (fullName.Split('.').Length == 1)
            {
                //There are no dots
                ParentName = null;
                _name = fullName.Split('.')[0];
            }
        }

        /// <summary>
        /// Gets the name of the AuthorityTokenGrop
        /// </summary>
        public virtual string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// The parent AuthorityTokenGroup of this
        /// </summary>
        public AuthorityTokenGroup Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        /// <summary>
        /// The fullname of the group
        /// </summary>
        /// <remarks>
        /// If this group has a parent group it will prefix the full name of the parent etc.
        /// </remarks>
        public string FullName
        {
            get
            {
                if (Parent == null)
                {
                    return Name;
                }
                else
                {
                    return Parent.FullName + "." + Name;
                }
            }
        }

        /// <summary>
        /// The AuthorityTokens in this group
        /// </summary>
        public List<AuthorityToken> AuthorityTokens
        {
            get
            {
                if (_authorityTokens == null)
                {
                    _authorityTokens = new List<AuthorityToken>();
                }
                return _authorityTokens;
            }
            set
            {
                _authorityTokens = value;
            }
        }
        
    }

    /// <summary>
    /// Represents an AuhtorityToken
    /// </summary>
    /// <remarks>
    /// An AuthorityToken determines whether a User can perform some action such as opening a form
    /// or clicking a button. When the user has the token they can perform the action. Tokens themselves
    /// are not derived from DataStoreItem and are not stored in the entity framework. However the list of
    /// which tokens each user has is stored in the database.
    /// </remarks>
    public class AuthorityToken
    {
        private AuthorityTokenGroup _group;
        protected string _parentName;
        private string _name;

        /// <summary>
        /// The name of the parent group
        /// </summary>
        public virtual string ParentName 
        { 
            get
            {
                return _parentName;
            } 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        public AuthorityToken(string name, string group)
        {
            _name = name;
            //_group = group;
        }

        /// <summary>
        /// The group this token belongs to
        /// </summary>
        public AuthorityTokenGroup Group
        {
            get
            {
                return _group;
            }
            
        }

        /// <summary>
        /// The name of the token which is unique within the group
        /// </summary>
        public virtual string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// The fullname of the token which uniquely identifies it within iRadiate
        /// </summary>
        public string FullName
        {
            get
            {
                return Group.FullName + "." + Name;
            }
        }
    }
}
