using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace TOControl.Notification.Business.Services
{
    public interface XMLService
    {
        T Parser<T>(string xml);
    }

    public class XMLServiceImp : XMLService
    {
        public T Parser<T>(string xml)
        {
            var objXml = new XmlDocument();
            objXml.LoadXml(xml);
            var childNode = objXml.FirstChild;
            return ParseXMLToModel<T>(childNode.Attributes);
        }

        private static T ParseXMLToModel<T>(XmlAttributeCollection attibutes)
        {
            var atts = new Dictionary<string, string>();
            foreach (XmlAttribute att in attibutes)
            {
                atts.Add(att.Name.ToLowerInvariant(), att.Value);
            }

            var model = Activator.CreateInstance<T>();
            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead || !prop.CanWrite || !atts.ContainsKey(prop.Name.ToLowerInvariant()))
                {
                    continue;
                }

                object value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(atts[prop.Name.ToLowerInvariant()]);
                prop.SetValue(model, value, null);
            }

            return model;
        }
    }
}