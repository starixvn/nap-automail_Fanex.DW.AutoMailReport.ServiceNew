using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using NUnit.Framework;

namespace TOControl.CustomerMovement.UnitTests.Services
{
    public class XMLServiceUnitTests
    {
        [Test]
        public void Parser()
        {
            var xml = "<GroupHeadline custid=\"15527459,15527460\" oldcustid=\"123456789\" GroupID=\"553\" IP=\"10.21.10.20\" ErrCode=\"0\" ErrMsg=\"Testing\" />";
            var objXml = new XmlDocument();
            objXml.LoadXml(xml);
            var childNode = objXml.FirstChild;
            var groupHeadline = ParseXMLToModel<TestModel>(childNode.Attributes);
        }

        private T ParseXMLToModel<T>(XmlAttributeCollection attibutes)
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

    public class TestModel
    {
        public string Custid { get; set; }

        public int OldCustid { get; set; }

        public bool IsMaster { get; set; }

        public string ErrMsg { get; set; }

        public string OldIP { get; set; }

        public string IP { get; set; }
    }
}