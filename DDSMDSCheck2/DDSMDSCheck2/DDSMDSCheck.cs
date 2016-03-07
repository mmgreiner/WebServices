using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ABB.Printing.Tools.DDSMDSCheck2
{
    /// <summary>
    /// Checks the following:
    /// DeliveryGood Nmaes must be unique
    /// DropPointLinks must refer to a given DropPoint
    /// LocationGroupCode and TargetGroupCode must be defined in the MDS data
    /// ObjectUID must be unique
    /// </summary>
    public class DDSCheck
    {
        // object uid and location where first found
        Dictionary<string, string> deliveryUnits = new Dictionary<string, string>();
        Dictionary<string, string> deliveryGoodNames = new Dictionary<string, string>();
        Dictionary<string, string> targetGroupCodes = new Dictionary<string, string>();
        Dictionary<string, string> locationGroupCodes = new Dictionary<string, string>();
        Dictionary<string, string> editionShortNames = new Dictionary<string, string>();
        Dictionary<string, string> dropPoints = new Dictionary<string, string>();
        Dictionary<string, string> productionOrderShortNames = new Dictionary<string, string>();

        XmlFile xmlfile;

        public DDSCheck(XmlFile xmlfile)
        {
            this.xmlfile = xmlfile;
        }

        public void AssertStandardBundleSize()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("dds:StandardBundleSize");
            foreach (XmlNode n in nodes)
            {
                if (n.Value == "0")
                {
                    Console.WriteLine("Bundlesize cannot be 0 at ");
                }
            }

        }

        public void AssertDeliveryGoodNames()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("dds:DeliveryGood");
            foreach (XmlNode n in nodes)
            {
                // move to child 
                foreach (XmlNode c in n.ChildNodes)
                {
                    if (c.Name == "dds:Name")
                    {
                        var name = c.InnerText;
                        if (deliveryGoodNames.ContainsKey(name))
                            Console.WriteLine("deliveryGoodName " + name + " used twice");
                        else
                            deliveryGoodNames.Add(name, "");
                        break;
                    }
                }
            }
        }

        public void AssertUniqueDeliveryUnit()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("dds:DeliveryUnit");
            foreach (XmlNode n in nodes)
            {
                var name = n.InnerText;
                if (deliveryUnits.ContainsKey(name))
                    Console.WriteLine("DeliveryUnit " + name + " not unique");
                else
                    deliveryUnits.Add(name, "");
            }
        }

        public void AssertDropPointLinks()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("dds:DropPoint");
            foreach (XmlNode n in nodes)
            {
                // move to child 
                XmlNode c = n.FirstChild;
                if (c.Name == "isd:ObjectUID")
                {
                    var name = c.InnerText;
                    if (dropPoints.ContainsKey(name))
                        Console.WriteLine("DropPOint " + name + " not unique");
                    else
                        dropPoints.Add(name, "");
                }

            }

            // now find all links
            nodes = xmlfile.Doc.GetElementsByTagName("dds:DropPointLink");
            foreach (XmlNode n in nodes)
            {
                XmlNode c = n.FirstChild;
                var name = c.InnerText;
                if (!dropPoints.ContainsKey(name))
                    Console.WriteLine("DropPointLink " + name + "is wrong");
            }

        }
    }

    public class MDSCheck
    {
        // object uid and location where first found
        Dictionary<string, string> locationGroupCodes = new Dictionary<string, string>();
        Dictionary<string, string> targetGroupCodes = new Dictionary<string, string>();
        Dictionary<string, string> editionShortNames = new Dictionary<string, string>();
        Dictionary<string, string> circulationData = new Dictionary<string, string>();

        XmlFile xmlfile;

        public MDSCheck(XmlFile xmlfile)
        {
            this.xmlfile = xmlfile;
        }

        public void AssertLocationGroupCodes()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("mds:Code");
            foreach (XmlNode n in nodes)
            {
                string code = n.InnerText;
                if (locationGroupCodes.ContainsKey(code))
                    Console.WriteLine("duplicate location group code " + code);
                else
                    locationGroupCodes.Add(code, "");

            }
        }

        public void CheckSalesData()
        {
            var nodes = xmlfile.Doc.GetElementsByTagName("mds:SalesData");
            foreach (XmlNode n in nodes)
            {
                string code = n.Attributes["code"].Value;
                targetGroupCodes.Add(code, "");

                string circulation = n.InnerText;
                string weekdays = n.Attributes["weekdays"].Value;

                
            }
        }
    }
}
