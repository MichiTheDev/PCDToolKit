using System.Xml.Linq;

namespace TicketToMentionConverter;

public class OrderItemBuilder
{
    public static XElement Create(int lineId, OrderItemData item)
    {
        return new XElement(Namespaces.OpenTrans + "ORDER_ITEM",

            new XElement(Namespaces.OpenTrans + "LINE_ITEM_ID", lineId),

            new XElement(Namespaces.OpenTrans + "PRODUCT_ID",
                new XElement(Namespaces.Bmecat + "SUPPLIER_PID",
                    new XAttribute("type","supplier_specific")),

                new XElement(Namespaces.Bmecat + "BUYER_PID",
                    new XAttribute("type","buyer_specific"), item.MentionArticleId)
            ),

            new XElement(Namespaces.OpenTrans + "QUANTITY", item.Quantity),

            new XElement(Namespaces.Bmecat + "ORDER_UNIT", "C62"),

            new XElement(Namespaces.OpenTrans + "PRODUCT_PRICE_FIX",
                new XElement(Namespaces.Bmecat + "PRICE_AMOUNT", item.Price)
            ),

            new XElement(Namespaces.OpenTrans + "PRICE_LINE_AMOUNT", item.Price * item.Quantity),
            
            new XElement(Namespaces.OpenTrans + "ITEM_UDX",
                new XElement("UDX.MENTION.BPPTEXT", new XCData(UdxTextBuilder.Build(item)))
            )
        );
    }
}