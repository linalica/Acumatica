using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;


namespace RB.RapidByte
{
    public class ShipmentEntry : PXGraph<ShipmentEntry, Shipment>
    {

        public PXSelect<Shipment> Shipments;
        public PXSelect<ShipmentLine, Where<ShipmentLine.shipmentNbr, Equal<Current<Shipment.shipmentNbr>>>,
                            OrderBy<Desc<ShipmentLine.gift>>> ShipmentLines;
        public PXSelect<Product, Where<Product.productCD, Equal<ShipmentLine.giftCard>>> GiftCard;


        protected virtual void Shipment_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            Shipment row = (Shipment)e.Row;
            if (row == null)
            {
                return;
            }

            // Configuring fields of the master form
            PXUIFieldAttribute.SetEnabled<Shipment.deliveryDate>(sender, row, row.ShipmentType == Shipment.ShipmentTypes.Single);
            PXUIFieldAttribute.SetEnabled<Shipment.shipmentDate>(sender, row, row.Status == Shipment.ShipmentStatus.Shipping);
            PXUIFieldAttribute.SetVisible<Shipment.deliveryMaxDate>(sender, row, row.ShipmentType == Shipment.ShipmentTypes.Single);
            PXUIFieldAttribute.SetVisible<Shipment.pendingQty>(sender, row, row.ShipmentType != Shipment.ShipmentTypes.Single);

            // Preventing insertion and deletion of shipment lines for shipments whose shipping has started
            ShipmentLines.Cache.AllowInsert = row.Status != Shipment.ShipmentStatus.Shipping;
            ShipmentLines.Cache.AllowDelete = row.Status != Shipment.ShipmentStatus.Shipping;
            // Configuring columns of the details grid
            PXUIFieldAttribute.SetVisible<ShipmentLine.shipmentDate>(ShipmentLines.Cache, null, row.ShipmentType != Shipment.ShipmentTypes.Single);
            PXUIFieldAttribute.SetVisible<ShipmentLine.shipmentTime>(ShipmentLines.Cache, null, row.ShipmentType != Shipment.ShipmentTypes.Single);
            PXUIFieldAttribute.SetVisible<ShipmentLine.shipmentMinTime>(ShipmentLines.Cache, null, row.ShipmentType != Shipment.ShipmentTypes.Single);
            PXUIFieldAttribute.SetVisible<ShipmentLine.shipmentMaxTime>(ShipmentLines.Cache, null, row.ShipmentType != Shipment.ShipmentTypes.Single);
        }
        protected virtual void Shipment_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            if (ShipmentLines.Select().Count == 0)
            {
                // Retrieving the gift card
                bool oldDirty = ShipmentLines.Cache.IsDirty;
                Product card = GiftCard.Select();
                if (card != null)
                {
                    // Initializing a new ShipmentLine data record
                    ShipmentLine line = new ShipmentLine();
                    line.ProductID = card.ProductID;
                    line.LineQty = card.MinAvailQty;

                    // Inserting the data record into the cache
                    ShipmentLines.Insert(line);

                    // Clearing the flag that indicates in the UI whether the cache
                    // contains changes
                    ShipmentLines.Cache.IsDirty = oldDirty;
                }
            }
        }

        //protected virtual void ShipmentLine_RowInserting(PXCache sender, PXRowInsertingEventArgs e)
        //{
        //    ShipmentLine line = (ShipmentLine)e.Row;
        //    if (ShipmentLines.Select().Count == 0)
        //    {
        //        // Retrieve the gift card
        //        Product card = GiftCard.Select();
        //        if (card != null)
        //        {
        //            line.ProductID = card.ProductID;
        //            line.Description = card.ProductName;
        //            line.LineQty = card.MinAvailQty;
        //            line.Gift = true;
        //        }
        //    }

        //}

        protected virtual void ShipmentLine_ProductID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            // Obtain the new data record that contains the updated values of all data fields
            ShipmentLine line = (ShipmentLine)e.Row;
            line.Description = string.Empty;
            if (line.ProductID != null)
            {
                Product product = PXSelectorAttribute.Select<ShipmentLine.productID>(sender, line) as Product;
                if (product != null)
                {
                    // Copy the product name to the description of the shipment line
                    line.Description = product.ProductName;
                }
            }
        }
        protected virtual void ShipmentLine_Gift_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            if (line == null)
            {
                return;
            }
            Product card = GiftCard.Select();
            if (card != null && line.ProductID == card.ProductID)
            {
                // Setting the default value
                e.NewValue = true;
                // Setting a flag to prevent execution of FieldDefaulting event
                // handlers that are defined in attributes
                e.Cancel = true;
            }
        }
        protected virtual void ShipmentLine_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            Shipment row = Shipments.Current;
            if (line == null || row == null)
            {
                return;
            }
            PXUIFieldAttribute.SetEnabled(sender, line, line.Gift != true);
            PXUIFieldAttribute.SetEnabled<ShipmentLine.lineQty>(sender, line, line.Gift != true && row.Status != Shipment.ShipmentStatus.Shipping);
            PXUIFieldAttribute.SetEnabled<ShipmentLine.cancelled>(sender, line, line.Gift != true && row.Status != Shipment.ShipmentStatus.Shipping);
        }

        protected virtual void ShipmentLine_LineQty_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            if (e.NewValue == null)
            {
                return;
            }
            if ((decimal)e.NewValue < 0)
            {
                // Throwing an exception to cancel assignment of the new
                // value to the field
                throw new PXSetPropertyException("Item Qty. cannot be negative.");
            }
            // Retrieving the product related to the shipment line
            Product product = PXSelect<Product, Where<Product.productID,
                                Equal<Required<Product.productID>>>>.Select(this, line.ProductID);
            if (product != null && (decimal)e.NewValue < product.MinAvailQty)
            {
                // Correcting the LineQty value
                e.NewValue = product.MinAvailQty;

                // Raising the ExceptionHandling event for LineQty to attach the exception object to the field
                sender.RaiseExceptionHandling<ShipmentLine.lineQty>(line, e.NewValue,
                                                new PXSetPropertyException(
                                                "Item Qty. was too small for the selected product.",
                                                PXErrorLevel.Warning));
            }
        }

        protected virtual void ShipmentLine_RowDeleting(PXCache sender, PXRowDeletingEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            // Checking whether the deletion has been initiated from the UI
            if (!e.ExternalCall)
            {
                return;
            }
            if (line.Gift == true)
            {
                // Preventing deletion of the gift card
                throw new PXException("Product {0} cannot be deleted", ShipmentLine.GiftCard);
            }
            else if (sender.GetStatus(line) != PXEntryStatus.InsertedDeleted)
            {
                // Asking for confirmation on an attempt to delete a shipment line other than the gift card line
                if (ShipmentLines.Ask("Confirm Delete", "Are you sure?", MessageButtons.YesNo) != WebDialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
        protected virtual void ShipmentLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            Shipment row = Shipments.Current;
            row.TotalQty += line.LineQty;
            Shipments.Update(row);
        }
        protected virtual void ShipmentLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            ShipmentLine newLine = (ShipmentLine)e.Row;
            ShipmentLine oldLine = (ShipmentLine)e.OldRow;
            Shipment row = Shipments.Current;
            bool rowUpdated = false;
            // If the LineQty value has changed, adjust the TotalQty value accordingly
            if (!sender.ObjectsEqual<ShipmentLine.lineQty>(newLine, oldLine) && newLine.Cancelled != true)
            {
                row.TotalQty -= oldLine.LineQty;
                row.TotalQty += newLine.LineQty;
                rowUpdated = true;
            }
            if (!sender.ObjectsEqual<ShipmentLine.cancelled>(newLine, oldLine))
            {
                // If the shipment line has been canceled, substract the value
                if (newLine.Cancelled == true)
                {
                    row.TotalQty -= oldLine.LineQty;
                }
                // If the canceled shipment line has been restored, add its value back to the total value
                else if (oldLine.Cancelled == true)
                {
                    row.TotalQty += newLine.LineQty;
                }
                rowUpdated = true;
            }
            // Updating the shipment in the cache
            if (rowUpdated == true)
            {
                Shipments.Update(row);
            }
        }
        protected virtual void ShipmentLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            Shipment row = Shipments.Current;
            if (line.Cancelled != true &&
                Shipments.Cache.GetStatus(row) != PXEntryStatus.InsertedDeleted &&
                Shipments.Cache.GetStatus(row) != PXEntryStatus.Deleted)
            {
                row.TotalQty -= line.LineQty;
                Shipments.Update(row);
            }
        }

        protected virtual void Shipment_Status_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            bool errorOccured = false;
            if ((string)e.NewValue == Shipment.ShipmentStatus.Shipping)
            {
                // Validating LineQty in all related ShipmentLine data records
                foreach (ShipmentLine line in ShipmentLines.Select())
                {
                    if (line.LineQty == 0)
                    {
                        // Adding an error message to a grid cell
                        ShipmentLines.Cache.RaiseExceptionHandling<
                        ShipmentLine.lineQty>(
                                        line, line.LineQty,
                                        new PXSetPropertyException("Item Qty. is not specified."));
                        errorOccured = true;
                    }
                }
            }
            if (errorOccured)
            {
                // Adding an error message to a UI control
                e.NewValue = sender.GetValue<Shipment.status>(e.Row);
                throw new PXSetPropertyException("Product quantities have not been specified.");
            }
        }
    }
}