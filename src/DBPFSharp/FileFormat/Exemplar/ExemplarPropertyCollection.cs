// Copyright (c) 2023, 2025 Nicholas Hayes
// SPDX-License-Identifier: MIT

using DBPFSharp.FileFormat.Exemplar.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DBPFSharp.FileFormat.Exemplar
{
    /// <summary>
    /// Represents a collection of exemplar properties.
    /// </summary>
    public sealed class ExemplarPropertyCollection : IEnumerable<ExemplarProperty>
    {
        private readonly SortedList<uint, ExemplarProperty> properties;

        internal ExemplarPropertyCollection()
        {
            this.properties = [];
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ExemplarPropertyCollection"/>.
        /// </summary>
        /// <value>
        /// The number of elements contained in the <see cref="ExemplarPropertyCollection"/>.
        /// </value>
        public int Count => this.properties.Count;

        /// <summary>
        /// Gets or sets the number of elements that the <see cref="ExemplarPropertyCollection"/> can contain.
        /// </summary>
        /// <value>
        /// The number of elements that the <see cref="ExemplarPropertyCollection"/> can contain.
        /// </value>
        public int Capacity
        { 
            get => this.properties.Capacity;
            set => this.properties.Capacity = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="ExemplarProperty"/> with the specified property identifier.
        /// </summary>
        /// <value>
        /// The <see cref="ExemplarProperty"/>.
        /// </value>
        /// <param name="propertyID">The property identifier.</param>
        /// <returns>The <see cref="ExemplarProperty"/>.</returns>
        public ExemplarProperty this[uint propertyID]
        { 
            get => this.properties[propertyID];
            set => this.properties[propertyID] = value;
        }

        /// <summary>
        /// Adds or updates the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
        public void AddOrUpdate(ExemplarProperty property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            this.properties[property.Id] = property;
        }

        /// <summary>
        /// Determines whether this instance contains the specified property.
        /// </summary>
        /// <param name="propertyID">The property identifier.</param>
        /// <returns>
        /// <see langword="true"/> if this instance contains the specified property; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Contains(uint propertyID) => this.properties.ContainsKey(propertyID);

        /// <inheritdoc />
        public IEnumerator<ExemplarProperty> GetEnumerator() => new Enumerator(this.properties);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Removes the element with the specified property identifier from the <see cref="ExemplarPropertyCollection"/>.
        /// </summary>
        /// <param name="propertyID">The property identifier.</param>
        /// <returns>
        /// <see langword="true"/> if an element was removed; otherwise, <see langword="false"/>.
        /// </returns>
        public bool Remove(uint propertyID) => this.properties.Remove(propertyID);

        /// <summary>
        /// Tries the add the specified value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <see langword="true"/> if the property was successfully added; otherwise, <see langword="false"/>. 
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="property"/> is null.</exception>
        public bool TryAdd(ExemplarProperty property)
        {
            ArgumentNullException.ThrowIfNull(property, nameof(property));

            return this.properties.TryAdd(property.Id, property);
        }

        /// <summary>
        /// Gets the value associated with the specified property identifier.
        /// </summary>
        /// <param name="propertyID">The property identifier.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="ExemplarPropertyCollection"/> contains an
        /// element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue(uint propertyID, [NotNullWhen(true)] out ExemplarProperty? property)
        {
            return this.properties.TryGetValue(propertyID, out property);
        }

        /// <summary>
        /// Attempts to get the value for the specified property identifier.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyID">The property identifier.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <see langword="true"/> if the <see cref="ExemplarPropertyCollection"/> contains an
        /// element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue<TProperty>(uint propertyID, [NotNullWhen(true)] out TProperty? property) where TProperty : ExemplarProperty
        {
            if (this.properties.TryGetValue(propertyID, out ExemplarProperty? exemplarProperty))
            {
                property = exemplarProperty as TProperty;
                return property != null;
            }

            property = null;
            return false;
        }

        private readonly struct Enumerator : IEnumerator<ExemplarProperty>
        {
            private readonly IEnumerator<KeyValuePair<uint, ExemplarProperty>> enumerator;
           
            internal Enumerator(SortedList<uint, ExemplarProperty> properties)
            {
                this.enumerator = properties.GetEnumerator();
            }

            /// <inheritdoc />
            public ExemplarProperty Current => this.enumerator.Current.Value;

            object IEnumerator.Current => this.Current;

            /// <inheritdoc />
            public void Dispose() => this.enumerator.Dispose();

            /// <inheritdoc />
            public bool MoveNext() => this.enumerator.MoveNext();

            /// <inheritdoc />
            public void Reset() => this.enumerator.Reset();
        }
    }
}
