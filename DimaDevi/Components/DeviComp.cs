using DimaDevi.Libs.Extensions;
using System;

namespace DimaDevi.Components
{
    public sealed class DeviComp : IDeviComponent
    {
        private string Result = string.Empty;
        public Func<string, string> Replacement { get; set; }
        public string BaseHardware { set;get; } = null;

        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A function that returns the component value.
        /// </summary>
        private readonly Func<string> _valueFactory;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="value">The component value.</param>
        public DeviComp(string name, string value) : this(name, () => value) { }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="name">The name of the component.</param>
        /// <param name="valueFactory">A function that returns the component value.</param>
        public DeviComp(string name, Func<string> valueFactory)
        {
            Name = name;
            _valueFactory = valueFactory;
        }

        /// <summary>
        /// Gets the component value.
        /// </summary>
        /// <returns>The component value.</returns>
        public string GetValue()
        {
            if (Replacement != null)
                return Replacement(_valueFactory.Invoke());
            return _valueFactory.Invoke();
        }
        public bool Equals(IDeviComponent other)
        {
            return this.EqualsObject(other);
        }
    }
}
