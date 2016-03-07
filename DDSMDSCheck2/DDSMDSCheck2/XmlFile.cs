using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Text.RegularExpressions;

namespace ABB.Printing.Tools.DDSMDSCheck2
{
    /// <summary>
    /// Does some basic checking of the xml files
    /// </summary>
    public class XmlFile
    {

        static public string[] DefaultSchemaFiles = {
        "dds_imf_R201.xsd",
        "mds_R102.xsd",
        "ads_imf_ex.xsd",
        "ibi_imf_R101.xsd",
        @"IfraTrack-4.1-ControlledVocabularies-AS.xsd",
        @"IfraTrack-4.1-MessageFormat-AS.xsd",
        @"IfraTrack-4.1-SharedDefinitions-AS.xsd",
        @"imf-base.xsd",
        @"imf.xsd"};

        public string[] DefaultSchemaPaths(string path)
        {
            string[] schemas = new string[DefaultSchemaFiles.Length];
            for (int i = 0; i < DefaultSchemaFiles.Length; i++)
            {
                schemas[i] = Path.GetFullPath(path) + "\\" + DefaultSchemaFiles[i];
            }
            return schemas;
        }

        static void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("\nWARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("\nERROR: ");

            Console.WriteLine(args.Message);
        }

        private string filename;
        private string schemapath;
        private string[] schemas;
        private List<Tuple<string, string>> namespaces = new List<Tuple<string, string>>();
        private XmlReaderSettings settings;
        private XmlDocument document;
        private XmlNamespaceManager nsManager;
        public XmlNamespaceManager NamespaceManager { get { return nsManager; } }
        public XmlDocument Doc { get { return document; } }


        public XmlFile(string filename, string schemapath)
        {
            this.filename = filename;
            this.schemapath = schemapath;
            schemas = DefaultSchemaPaths(this.schemapath);

            settings = new XmlReaderSettings();
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);
            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            document = new XmlDocument();
            nsManager = new XmlNamespaceManager(document.NameTable);

            foreach (var s in schemas)
            {
                if (!File.Exists(s))
                    Console.WriteLine("Cannot find schema " + s);
                // now extract the target namespace
                string target = GetTargetNamespace(filename);
                settings.Schemas.Add(target, s);
            }
        }

        public void Validate()
        {
            XmlReader reader = XmlReader.Create(filename, settings);
            document.Load(reader);

            var elems = document.GetElementsByTagName("imf:Imf");
            if (elems.Count == 0)
                elems = document.GetElementsByTagName("Imf");

            // get attributes if imf:Imf or Imf to set namespaces
                var node = elems.Item(0);
                Console.WriteLine(node.Value);
               foreach (XmlAttribute attr in node.Attributes)
                {
                    Console.WriteLine(attr.Name + " " + attr.Value);
                    if (attr.Name != "xmlns")
                        nsManager.AddNamespace(attr.Name.Replace("xmlns:", ""), attr.Value);
                }

            // go to the first object to find what this really is
            var objs = document.GetElementsByTagName("isd:Object");
            node = objs.Item(0);
            var typ = node.Attributes["xs:type"].Value;
            typ = typ.Substring(0, 3);
            
        }

        static Regex TargetNamespace = new Regex(@"targetNamespace\s*=\s*""([^""]+)""");

        static string GetTargetNamespace(string filename)
        {

            using (StreamReader stream = File.OpenText(filename))
            {
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    var match = TargetNamespace.Match(line);
                    if (match.Length > 0)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            return null;
        }
    }

}
