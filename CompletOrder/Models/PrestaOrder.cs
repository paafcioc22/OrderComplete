using System;
using System.Collections.Generic;
using System.Text;

namespace CompletOrder.Models
{
    namespace QuickType
    {
        using System;
        using System.Collections.Generic;
        using System.Runtime.Serialization;

        [DataContract]
        public partial class PrestaOrder
        {
            [DataMember(Name ="order")]
            public Order Order { get; set; }
        }
        [DataContract]
        public partial class Order
        {
            [DataMember(Name ="id")]
            public long Id { get; set; }

            [DataMember(Name ="id_address_delivery")]
            
            public long IdAddressDelivery { get; set; }

            [DataMember(Name ="id_address_invoice")] 
            public long IdAddressInvoice { get; set; }

            [DataMember(Name ="id_cart")] 
            public long IdCart { get; set; }

            [DataMember(Name ="id_currency")] 
            public long IdCurrency { get; set; }

            [DataMember(Name ="id_lang")] 
            public long IdLang { get; set; }

            [DataMember(Name ="id_customer")] 
            public long IdCustomer { get; set; }

            [DataMember(Name ="id_carrier")] 
            public long IdCarrier { get; set; }

            [DataMember(Name ="current_state")]
           
            public long CurrentState { get; set; }

            [DataMember(Name ="module")]
            public string Module { get; set; }

            [DataMember(Name ="invoice_number")]
           
            public long InvoiceNumber { get; set; }

            [DataMember(Name ="invoice_date")]
            public DateTimeOffset InvoiceDate { get; set; }

            [DataMember(Name ="delivery_number")]
           
            public long DeliveryNumber { get; set; }

            [DataMember(Name ="delivery_date")]
            public DateTimeOffset DeliveryDate { get; set; }

            [DataMember(Name ="valid")]
           
            public long Valid { get; set; }

            [DataMember(Name ="date_add")]
            public DateTimeOffset DateAdd { get; set; }

            [DataMember(Name ="date_upd")]
            public DateTimeOffset DateUpd { get; set; }

            [DataMember(Name ="shipping_number")]
            public string ShippingNumber { get; set; }

            [DataMember(Name ="id_shop_group")]
           
            public long IdShopGroup { get; set; }

            [DataMember(Name ="id_shop")]
           
            public long IdShop { get; set; }

            [DataMember(Name ="secure_key")]
            public string SecureKey { get; set; }

            [DataMember(Name ="payment")]
            public string Payment { get; set; }

            [DataMember(Name ="recyclable")]
           
            public long Recyclable { get; set; }

            [DataMember(Name ="gift")]
           
            public long Gift { get; set; }

            [DataMember(Name ="gift_message")]
            public string GiftMessage { get; set; }

            [DataMember(Name ="mobile_theme")]
           
            public long MobileTheme { get; set; }

            [DataMember(Name ="total_discounts")]
            public string TotalDiscounts { get; set; }

            [DataMember(Name ="total_discounts_tax_incl")]
            public string TotalDiscountsTaxIncl { get; set; }

            [DataMember(Name ="total_discounts_tax_excl")]
            public string TotalDiscountsTaxExcl { get; set; }

            [DataMember(Name ="total_paid")]
            public string TotalPaid { get; set; }

            [DataMember(Name ="total_paid_tax_incl")]
            public string TotalPaidTaxIncl { get; set; }

            [DataMember(Name ="total_paid_tax_excl")]
            public string TotalPaidTaxExcl { get; set; }

            [DataMember(Name ="total_paid_real")]
            public string TotalPaidReal { get; set; }

            [DataMember(Name ="total_products")]
            public string TotalProducts { get; set; }

            [DataMember(Name ="total_products_wt")]
            public string TotalProductsWt { get; set; }

            [DataMember(Name ="total_shipping")]
            public string TotalShipping { get; set; }

            [DataMember(Name ="total_shipping_tax_incl")]
            public string TotalShippingTaxIncl { get; set; }

            [DataMember(Name ="total_shipping_tax_excl")]
            public string TotalShippingTaxExcl { get; set; }

            [DataMember(Name ="carrier_tax_rate")]
            public string CarrierTaxRate { get; set; }

            [DataMember(Name ="total_wrapping")]
            public string TotalWrapping { get; set; }

            [DataMember(Name ="total_wrapping_tax_incl")]
            public string TotalWrappingTaxIncl { get; set; }

            [DataMember(Name ="total_wrapping_tax_excl")]
            public string TotalWrappingTaxExcl { get; set; }

            [DataMember(Name ="round_mode")]
           
            public long RoundMode { get; set; }

            [DataMember(Name ="round_type")]
           
            public long RoundType { get; set; }

            [DataMember(Name ="conversion_rate")]
            public string ConversionRate { get; set; }

            [DataMember(Name ="reference")]
            public string Reference { get; set; }

            [DataMember(Name ="associations")]
            public Associations Associations { get; set; }
        }
        [DataContract]
        public partial class Associations
        {
            [DataMember(Name ="order_rows")]
            public List<OrderRow> OrderRows { get; set; }
        }
        [DataContract]
        public partial class OrderRow
        {
            [DataMember(Name ="id")]
           
            public long Id { get; set; }

            [DataMember(Name ="product_id")]
           
            public long ProductId { get; set; }

            [DataMember(Name ="product_attribute_id")]
           
            public long ProductAttributeId { get; set; }

            [DataMember(Name ="product_quantity")]
           
            public long ProductQuantity { get; set; }

            [DataMember(Name ="product_name")]
            public string ProductName { get; set; }

            [DataMember(Name ="product_reference")]
            public string ProductReference { get; set; }

            [DataMember(Name ="product_ean13")]
            public string ProductEan13 { get; set; }

            [DataMember(Name ="product_isbn")]
            public string ProductIsbn { get; set; }

            [DataMember(Name ="product_upc")]
            public string ProductUpc { get; set; }

            [DataMember(Name ="product_price")]
            public string ProductPrice { get; set; }

            [DataMember(Name ="id_customization")]
           
            public long IdCustomization { get; set; }

            [DataMember(Name ="unit_price_tax_incl")]
            public string UnitPriceTaxIncl { get; set; }

            [DataMember(Name ="unit_price_tax_excl")]
            public string UnitPriceTaxExcl { get; set; }
        }
    }

}
