using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABB.Printing.Tools.DDSMDSCheck2
{
    class Program
    {
        static void Main(string[] args)
        {
            var t2 = @"C:\Users\chmagre\Source\Repository\WebServices\DDSMDSCheck2\Testdata\Martin\MDS_Wochenblatt_ab_20100921.xml";
            var t1 = @"C:\Users\chmagre\Source\Repository\WebServices\DDSMDSCheck2\Testdata\VRM_DDS_ZEITUNG_20100730.xml";
            var schemas = @"C:\Users\chmagre\Source\Repository\WebServices\DDSMDSCheck2\Schemas";

            var myxml = new XmlFile(t2, schemas);
            myxml.Validate();
            var ddscheck = new DDSCheck(myxml);
            ddscheck.AssertStandardBundleSize();
            ddscheck.AssertDeliveryGoodNames();
            ddscheck.AssertUniqueDeliveryUnit();
            ddscheck.AssertDropPointLinks();

            var mdscheck = new MDSCheck(myxml);
            mdscheck.AssertLocationGroupCodes();
            mdscheck.CheckSalesData();
        }
    }
}
