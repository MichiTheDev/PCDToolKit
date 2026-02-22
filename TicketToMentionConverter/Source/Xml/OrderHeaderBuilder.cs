using System.Xml.Linq;

namespace TicketToMentionConverter;

public static class OrderHeaderBuilder
{
    public static XElement Build(TicketHeaderData ticket, MentionSettings mention)
    {
        return new XElement(Namespaces.OpenTrans + "ORDER_HEADER",
            new XElement(Namespaces.OpenTrans + "ORDER_INFO",

                new XElement(Namespaces.OpenTrans + "ORDER_ID", ticket.OrderId),

                new XElement(Namespaces.OpenTrans + "ORDER_DATE", ticket.OrderDate.ToString("yyyy-MM-dd")),

                new XElement(Namespaces.Bmecat + "LANGUAGE", mention.Language),

                new XElement(Namespaces.OpenTrans + "PARTIES",

                    new XElement(Namespaces.OpenTrans + "PARTY",
                        new XElement(Namespaces.Bmecat + "PARTY_ID",
                            new XAttribute("type", "supplier_specific"), ticket.BuyerId),
                        new XElement(Namespaces.OpenTrans + "PARTY_ROLE", "buyer")
                    ),

                    new XElement(Namespaces.OpenTrans + "PARTY",
                        new XElement(Namespaces.Bmecat + "PARTY_ID",
                            new XAttribute("type", mention.Supplier.IdType), mention.Supplier.Id),
                        new XElement(Namespaces.OpenTrans + "PARTY_ROLE", "supplier")
                    )
                ),

                new XElement(Namespaces.OpenTrans + "ORDER_PARTIES_REFERENCE",
                    new XElement(Namespaces.Bmecat + "BUYER_IDREF",
                        new XAttribute("type", "supplier_specific"), ticket.BuyerId),

                    new XElement(Namespaces.Bmecat + "SUPPLIER_IDREF",
                        new XAttribute("type", mention.Supplier.IdType), mention.Supplier.Id)
                ),

                new XElement(Namespaces.OpenTrans + "HEADER_UDX",
                    new XElement("UDX.MENTION.BSWAEHRUNG", mention.Currency)
                )
            )
        );
    }
}