using System.Xml.Linq;

namespace TicketToMentionConverter;

public static class OrderRootBuilder
{
    public static XDocument Create(XElement header)
    {
        XElement order = new XElement(Namespaces.OpenTrans + "ORDER",

            new XAttribute("version", "2.1"),
            new XAttribute("type", "standard"),

            new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.Xsi),
            new XAttribute(XNamespace.Xmlns + "bmecat", Namespaces.Bmecat),
            new XAttribute(XNamespace.Xmlns + "xmime", Namespaces.Xmime),

            new XAttribute(Namespaces.Xsi + "schemaLocation", "http://www.opentrans.org/XMLSchema/2.1 opentrans_2_1.xsd"),

            header,

            new XElement(Namespaces.OpenTrans + "ORDER_ITEM_LIST")
        );

        return new XDocument(new XDeclaration("1.0", "UTF-8", null), order);
    }
}