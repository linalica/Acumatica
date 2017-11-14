using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace RB.RapidByte
{
    public class SalesOrderEntry : PXGraph<SalesOrderEntry, SalesOrder>
    {

        public PXSelect<SalesOrder> Orders;

        public PXSelect<OrderLine,
            Where<OrderLine.orderNbr,
                Equal<Current<SalesOrder.orderNbr>>>> OrderDetails;

    }
}