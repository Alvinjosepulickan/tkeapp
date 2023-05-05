using SpecMemoService;
using System;
using System.Collections.Generic;
using System.Text;

namespace TKE.SC.PIPO
{
    public class SpecMemoAndOBOM_CreateAndUpdate_Request 
    {
        private BusinessDocumentMessageHeader messageHeaderField;

        private SpecMemoShipOBOMElevator[] elevatorField;

        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public BusinessDocumentMessageHeader MessageHeader
        {
            get
            {
                return this.messageHeaderField;
            }
            set
            {
                this.messageHeaderField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Elevator", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public SpecMemoShipOBOMElevator[] Elevator
        {
            get
            {
                return this.elevatorField;
            }
            set
            {
                this.elevatorField = value;
            }
        }
    }
}
