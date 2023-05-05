using SpecMemoService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace TKE.SC.PIPO
{
    public class XmlHelper
    {
        public SpecMemoShipOBOM DeserializeXml(string xmlFilePath)
        {
            string[] readText = File.ReadAllLines(xmlFilePath);

            return Translate(Deserialize(string.Join("\n", readText)));
        }

        public SpecMemoShipOBOM DeserializeString(string xmlContent)
        {
            return Translate(Deserialize(xmlContent));
        }

        private SpecMemoAndOBOM_CreateAndUpdate_Request Deserialize(string xmlContent)
        {
            var serializer = new XmlSerializer(typeof(SpecMemoAndOBOM_CreateAndUpdate_Request));

            using TextReader reader = new StringReader(xmlContent);

            return (SpecMemoAndOBOM_CreateAndUpdate_Request)serializer.Deserialize(reader);
        }

        private SpecMemoShipOBOM Translate(SpecMemoAndOBOM_CreateAndUpdate_Request request)
        {
            var specMemo = new SpecMemoShipOBOM();
            specMemo.MessageHeader = request.MessageHeader;
            specMemo.Elevator = request.Elevator;
            return specMemo;
        }

    }
}
