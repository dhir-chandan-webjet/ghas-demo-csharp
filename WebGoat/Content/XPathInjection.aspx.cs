using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;

namespace OWASP.WebGoat.NET
{
    public partial class XPathInjection : System.Web.UI.Page
    {
        // Make into actual lesson
        private string xml = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?><sales><salesperson><name>David Palmer</name><city>Portland</city><state>or</state><ssn>123-45-6789</ssn></salesperson><salesperson><name>Jimmy Jones</name><city>San Diego</city><state>ca</state><ssn>555-45-6789</ssn></salesperson><salesperson><name>Tom Anderson</name><city>New York</city><state>ny</state><ssn>444-45-6789</ssn></salesperson><salesperson><name>Billy Moses</name><city>Houston</city><state>tx</state><ssn>333-45-6789</ssn></salesperson></sales>";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["state"] != null)
            {
                FindSalesPerson(Request.QueryString["state"]);
            }
        }

        private void FindSalesPerson(string state)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);
            
            // Create an XPath expression with a variable placeholder
            string xpathExpression = "//salesperson[state=$state]";
            XPathExpression expr = xDoc.CreateNavigator().Compile(xpathExpression);
            
            // Create an XsltArgumentList and add the user input as a parameter
            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("state", "", state);
            
            // Create a custom context and set it to the XPath expression
            CustomContext context = new CustomContext(new NameTable(), args);
            expr.SetContext(context);
            
            // Select nodes using the compiled expression
            XmlNodeList list = xDoc.SelectNodes(expr);
            
            if (list.Count > 0)
            {

            }

        }
    }
    
    // Custom context class to resolve variables in XPath expression
    public class CustomContext : XsltContext
    {
        private readonly XsltArgumentList _args;
        
        public CustomContext(NameTable nt, XsltArgumentList args) : base(nt)
        {
            _args = args;
        }
        
        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            return new XsltContextVariable(_args.GetParam(name, ""));
        }
        
        public override bool Whitespace => false;
        public override bool PreserveWhitespace(XPathNavigator node) => false;
        public override int CompareDocument(string baseUri, string nextbaseUri) => 0;
        public override bool ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes, out IXsltContextFunction func) { func = null; return false; }
    }
}
