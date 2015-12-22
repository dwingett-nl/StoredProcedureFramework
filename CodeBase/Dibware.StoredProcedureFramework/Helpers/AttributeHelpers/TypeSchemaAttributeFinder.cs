﻿using Dibware.StoredProcedureFramework.StoredProcedureAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dibware.StoredProcedureFramework.Helpers.AttributeHelpers
{
    public class TypeSchemaAttributeFinder
    {
        #region Fields

        private readonly Type _type;
        private SchemaAttribute _attributeFound;

        #endregion

        #region Construtors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSchemaAttributeFinder"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public TypeSchemaAttributeFinder(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            _type = type;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Looks for the attribute.
        /// </summary>
        /// <returns>The current instance for fluid API</returns>
        public TypeSchemaAttributeFinder DetectAttribute()
        {
            SetAttributeIfExists();
            return this;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has found an attribute.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has found an attribute; otherwise, <c>false</c>.
        /// </value>
        public bool HasFoundAttribute
        {
            get { return _attributeFound != null; }
        }

        /// <summary>
        /// Gets the AttributeFound that was found.
        /// </summary>
        /// <value>
        /// The AttributeFound.
        /// </value>
        public SchemaAttribute AttributeFound
        {
            get
            {
                if (!HasFoundAttribute)
                {
                    throw new InvalidOperationException("No attribute type was found so cannot be returned. Hint: Use HasFoundAttribute first.");
                }
                return _attributeFound;
            }
        }

        #endregion

        #region Private Members

        private void SetAttributeIfExists()
        {
            IEnumerable<SchemaAttribute> attributes = _type.GetCustomAttributes<SchemaAttribute>();
            _attributeFound = attributes.FirstOrDefault();
        }
        #endregion
    }
}