using System;

namespace DimaDevi.Components
{
    public sealed class DeviComp : IDeviComponent
    {
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
            return _valueFactory.Invoke();
        }
    }
}
