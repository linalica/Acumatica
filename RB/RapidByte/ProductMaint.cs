using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace RB.RapidByte
{

    // Definition of 9 standard actions for the specified class are added to the graph, such as PXCancel<Product>, PXSave<Product>, ...
    public class ProductMaint : PXGraph<ProductMaint, Product>
    {

        public PXSelect<Product> Products;

        // Type attribute is required in the definition
        [PXDBString(15, IsUnicode = true, IsKey = true)]
        // When you don't provide the default value, the PXDefault attribute just makes the field mandatory for input
        [PXDefault]
        // Mandatory for fields displayed in the UI
        [PXUIField(DisplayName = "Product ID")]
        // Configures a selector control
        [PXSelector(
            typeof(Product.productCD),
            typeof(Product.productCD),
            typeof(Product.productName),
            typeof(Product.unitPrice),
            typeof(Product.stockUnit),
            typeof(Product.availQty))]
        // The event handler is empty, defined just to replace attributes
        protected void Product_ProductCD_CacheAttached(PXCache sender)
        {
        }

    }
}