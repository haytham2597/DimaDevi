using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DimaDevi.Formatters
{
    /// <summary>
    /// An implementation of <see cref="IDeviFormatter"/> that combines the components into an XML string.
    /// </summary>
    public class XmlForm : IDeviFormatter
    {
        /// <summary>
        /// TODO: Create combination Cryptography on this values elements components All result is XML but every value of elements can be encrypt with AES, RSA, etc.
        /// </summary>
        private readonly string NameElement = "DimaDevi";
        public XmlForm() { }
        public XmlForm(string nameElement)
        {
            if (string.IsNullOrEmpty(nameElement))
                return;
            this.NameElement = nameElement;
        }
        /// <summary>
        /// Returns the device identifier string created by combining the specified <see cref="IDeviComponent"/> instances.
        /// </summary>
        /// <param name="components">A sequence containing the <see cref="IDeviComponent"/> instances to combine into the device identifier string.</param>
        /// <returns>The device identifier string.</returns>
        public string GetDevi(IEnumerable<IDeviComponent> components)
        {
            if (components == null)
                throw new ArgumentNullException(nameof(components));

            var document = new XDocument(GetElement(components));
            return document.ToString(SaveOptions.DisableFormatting);
        }

        public string GetDevi(string componentsResult, string separator)
        {
            var spl = componentsResult.Split(new string[] { separator }, StringSplitOptions.None);
            List<XElement> elements = new List<XElement>();
            for (int i = 0; i < spl.Length; i++)
            {
                var splElem = spl[i].Split('=');
                if (splElem.Length != 2)
                    continue;
                elements.Add(
                    new XElement("Components", 
                        new XAttribute("Name", splElem[0]),
                        new XAttribute("Value", splElem[1])
                    )
                );
            }
            XElement main = new XElement(NameElement, elements);
            var document = new XDocument(main);
            return document.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// Returns an <see cref="XElement"/> representing the specified collection of <see cref="IDeviComponent"/> instances.
        /// </summary>
        /// <param name="components">The sequence of <see cref="IDeviComponent"/> instances to represent.</param>
        /// <returns>An <see cref="XElement"/> representing the specified collection of <see cref="IDeviComponent"/> instances</returns>
        private XElement GetElement(IEnumerable<IDeviComponent> components)
        {
            var elements = components
                .OrderBy(x => x.Name)
                .Select(GetElement);

            return new XElement(NameElement, elements);
        }

        /// <summary>
        /// Returns an <see cref="XElement"/> representing the specified <see cref="IDeviComponent"/> instance.
        /// </summary>
        /// <param name="component">The <see cref="IDeviComponent"/> to represent.</param>
        /// <returns>An <see cref="XElement"/> representing the specified <see cref="IDeviComponent"/> instance.</returns>
        private XElement GetElement(IDeviComponent component)
        {
            return new XElement("Component",
                new XAttribute("Name", component.Name),
                new XAttribute("Value", component.GetValue()));
        }

        public void Dispose()
        {
            
        }
    }
}
