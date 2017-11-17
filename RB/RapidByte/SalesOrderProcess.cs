using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace RB.RapidByte
{
    public class SalesOrderProcess : PXGraph<SalesOrderProcess>
    {
        public PXProcessing<SalesOrder> Orders;
        public PXCancel<SalesOrder> Cancel;

        public SalesOrderProcess()
        {
            Orders.SetProcessCaption("Approve");
            Orders.SetProcessAllCaption("Approve All");
            Orders.SetProcessDelegate<SalesOrderEntry>(
            delegate (SalesOrderEntry graph, SalesOrder order)
            {
                graph.Clear();
                graph.ApproveOrder(order, true);
            });
        }

    }
}