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

        public PXSelect<ShipmentLine, Where<ShipmentLine.shipmentNbr,
                                    Equal<Current<Shipment.shipmentNbr>>>,
                                        OrderBy<Desc<ShipmentLine.gift>>> ShipmentLines;

        public PXSelect<Product, Where<Product.productCD, Equal<ShipmentLine.giftCard>>> GiftCard;


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

            // If a shipment is not canceled or delivered
            if (row.Status != Shipment.ShipmentStatus.Cancelled && row.Status != Shipment.ShipmentStatus.Delivered)
            {
                // Preventing deletion of Shipment data records unless the status is Shipping and ShippedQty is 0
                Shipments.Cache.AllowDelete = row.Status != Shipment.ShipmentStatus.Shipping && row.ShippedQty == 0;
                PXUIFieldAttribute.SetEnabled(sender, row, true);
                PXUIFieldAttribute.SetEnabled<Shipment.deliveryDate>(sender, row, row.ShipmentType == Shipment.ShipmentTypes.Single);
                PXUIFieldAttribute.SetEnabled<Shipment.shipmentDate>(sender, row, row.Status == Shipment.ShipmentStatus.Shipping);
                // Changing the list of possible values in the Status drop-down control
                PXStringListAttribute.SetList<Shipment.status>(sender, row,
                    new string[]
                    {
                        Shipment.ShipmentStatus.OnHold,
                        Shipment.ShipmentStatus.Shipping,
                    },
                    new string[]
                    {
                        "On Hold",
                        "Shipping",
                    });
                // Limiting insertion, update, and deletion of ShipmentLines data records
                ShipmentLines.Cache.AllowInsert = row.Status != Shipment.ShipmentStatus.Shipping;
                ShipmentLines.Cache.AllowUpdate = true;
                ShipmentLines.Cache.AllowDelete = row.Status != Shipment.ShipmentStatus.Shipping;

                // Enabling or disabling the actions in the UI
                CancelShipment.SetEnabled(row.Status == Shipment.ShipmentStatus.Shipping && Shipments.Cache.GetStatus(row) != PXEntryStatus.Inserted);
                DeliverShipment.SetEnabled(row.Status == Shipment.ShipmentStatus.Shipping && Shipments.Cache.GetStatus(row) != PXEntryStatus.Inserted);
            }
            else
            {
                Shipments.Cache.AllowDelete = false;
                // Disabling all fields
                PXUIFieldAttribute.SetEnabled(sender, row, false);
                // Enabling the key field
                PXUIFieldAttribute.SetEnabled<Shipment.shipmentNbr>(sender, row, true);

                // Changing the list of possible values in the Status drop-down control
                PXStringListAttribute.SetList<Shipment.status>(sender, row,
                    new string[]
                    {
                        Shipment.ShipmentStatus.Cancelled,
                        Shipment.ShipmentStatus.Delivered
                    },
                    new string[]
                    {
                        "Canceled",
                        "Delivered"
                    });
                // Blocking insertion, update, and deletion of  ShipmentLine data records
                ShipmentLines.Cache.AllowInsert = false;
                ShipmentLines.Cache.AllowUpdate = false;
                ShipmentLines.Cache.AllowDelete = false;
                // Disabling the actions in the UI
                CancelShipment.SetEnabled(false);
                DeliverShipment.SetEnabled(false);
            }
        }
        protected virtual void Shipment_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            // The modified data record (not in the cache yet)
            Shipment row = (Shipment)e.NewRow;
            // The data record as stored in the cache
            Shipment originalRow = (Shipment)e.Row;
            if (row.ShipmentType == Shipment.ShipmentTypes.Single && !sender.ObjectsEqual<Shipment.deliveryMaxDate>(row, originalRow))
            {
                if (row.DeliveryDate != null && row.DeliveryMaxDate != null && row.DeliveryMaxDate < row.DeliveryDate)
                {
                    // Correcting the value
                    row.DeliveryMaxDate = row.DeliveryDate;
                    // Adding a warning message to the field
                    sender.RaiseExceptionHandling<Shipment.deliveryMaxDate>(
                        row, row.DeliveryMaxDate,
                        new PXSetPropertyException("Specified value was too early.",
                        PXErrorLevel.Warning));
                }
            }
            else if (row.ShipmentType == Shipment.ShipmentTypes.Single && !sender.ObjectsEqual<Shipment.deliveryDate>(row, originalRow))
            {
                if (row.DeliveryDate != null && row.DeliveryMaxDate != null && row.DeliveryDate > row.DeliveryMaxDate)
                {
                    // Adding an error message to the field
                    sender.RaiseExceptionHandling<Shipment.deliveryDate>(
                            row, row.DeliveryDate,
                            new PXSetPropertyException("Delivery time expired."));
                    // Canceling the update of the data record
                    e.Cancel = true;
                }
            }
        }
        protected virtual void Shipment_DeliveryDate_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            Shipment row = (Shipment)e.Row;
            if (e.NewValue == null)
            {
                return;
            }
            if (row.ShipmentDate != null && row.ShipmentDate > (DateTime)e.NewValue)
            {
                // Correcting the value
                e.NewValue = row.ShipmentDate;
                // Throwing an exception to break the update process
                throw new PXSetPropertyException<Shipment.shipmentDate>("Shipment Date cannot be later than Delivery Date");
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
        protected virtual void Shipment_ShipmentType_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            Shipment row = (Shipment)e.Row;
            if (e.NewValue == null)
            {
                return;
            }
            if ((string)e.NewValue == Shipment.ShipmentTypes.Single && row.ShippedQty > 0)
            {
                throw new PXSetPropertyException("Delivery Type can not be changed when a shipment is partially delivered.");
            }
        }
        protected virtual void Shipment_TotalQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Shipment row = (Shipment)e.Row;
            row.PendingQty = row.TotalQty - row.ShippedQty;
        }
        protected virtual void Shipment_ShippedQty_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            Shipment row = (Shipment)e.Row;
            row.PendingQty -= row.ShippedQty;
            row.PendingQty += (decimal)e.OldValue;
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
            // Enabling or disabling the Cancelled field
            if (row.ShipmentType != Shipment.ShipmentTypes.Single)
            {
                PXUIFieldAttribute.SetEnabled<ShipmentLine.cancelled>(sender, line, line.Gift != true && row.Status != Shipment.ShipmentStatus.Shipping && line.ShipmentDate == null);
            }
            else
            {
                PXUIFieldAttribute.SetEnabled<ShipmentLine.cancelled>(sender, line, line.Gift != true && row.Status != Shipment.ShipmentStatus.Shipping);
            }
            // Enabling or disabling other fields
            PXUIFieldAttribute.SetEnabled<ShipmentLine.shipmentDate>(sender, line, row.Status == Shipment.ShipmentStatus.Shipping && line.Cancelled != true);
            PXUIFieldAttribute.SetEnabled<ShipmentLine.shipmentTime>(sender, line, row.Status == Shipment.ShipmentStatus.Shipping && line.Cancelled != true);
            PXUIFieldAttribute.SetEnabled<ShipmentLine.shipmentMinTime>(sender, line, row.Status != Shipment.ShipmentStatus.Shipping && line.Cancelled != true);
            PXUIFieldAttribute.SetEnabled<ShipmentLine.shipmentMaxTime>(sender, line, row.Status != Shipment.ShipmentStatus.Shipping && line.Cancelled != true);

        }
        //protected virtual void ShipmentLine_RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        //{
        //    ShipmentLine line = (ShipmentLine)e.Row;
        //    Shipment row = Shipments.Current;
        //    row.TotalQty += line.LineQty;
        //    Shipments.Update(row);
        //}
        protected virtual void ShipmentLine_RowUpdating(PXCache sender, PXRowUpdatingEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.NewRow;
            ShipmentLine originalLine = (ShipmentLine)e.Row;
            Shipment row = Shipments.Current;
            // Checking whether the ShipmentTime, ShipmentMinTime,  or ShipmentMaxTime value has changed
            if (row.ShipmentType != Shipment.ShipmentTypes.Single && !sender.ObjectsEqual<ShipmentLine.shipmentTime,
                    ShipmentLine.shipmentMinTime, ShipmentLine.shipmentMaxTime>(line, originalLine))
            {
                // Checking that the delivery time is not smaller than  the minimum delivery time
                if (line.ShipmentTime != null && line.ShipmentMinTime != null && line.ShipmentTime < line.ShipmentMinTime)
                {
                    sender.RaiseExceptionHandling<ShipmentLine.shipmentTime>(line, line.ShipmentTime,
                            new PXSetPropertyException("Delivery Time is too early."));
                    e.Cancel = true;
                }
                // Checking that the delivery time is not greater than the maximum delivery time
                if (line.ShipmentTime != null && line.ShipmentMaxTime != null && line.ShipmentTime > line.ShipmentMaxTime)
                {
                    line.ShipmentTime = line.ShipmentMaxTime;
                    sender.RaiseExceptionHandling<ShipmentLine.shipmentTime>(line, line.ShipmentTime,
                            new PXSetPropertyException("Specified Delivery Time was too late.", PXErrorLevel.Warning));
                }
            }
        }
        //protected virtual void ShipmentLine_RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        //{
        //    ShipmentLine newLine = (ShipmentLine)e.Row;
        //    ShipmentLine oldLine = (ShipmentLine)e.OldRow;
        //    Shipment row = Shipments.Current;
        //    bool rowUpdated = false;
        //    // If the LineQty value has changed, adjust the TotalQty value accordingly
        //    if (!sender.ObjectsEqual<ShipmentLine.lineQty>(newLine, oldLine) && newLine.Cancelled != true)
        //    {
        //        row.TotalQty -= oldLine.LineQty;
        //        row.TotalQty += newLine.LineQty;
        //        rowUpdated = true;
        //    }
        //    if (!sender.ObjectsEqual<ShipmentLine.cancelled>(newLine, oldLine))
        //    {
        //        // If the shipment line has been canceled, substract the value
        //        if (newLine.Cancelled == true)
        //        {
        //            row.TotalQty -= oldLine.LineQty;
        //        }
        //        // If the canceled shipment line has been restored, add its value back to the total value
        //        else if (oldLine.Cancelled == true)
        //        {
        //            row.TotalQty += newLine.LineQty;
        //        }
        //        rowUpdated = true;
        //    }

        //    // Calculating ShippedQty for shipments of Multiple delivery type
        //    if (row.ShipmentType != Shipment.ShipmentTypes.Single)
        //    {
        //        // Making the calculation if ShipmentDate or ShipmentTime has changed
        //        if (!sender.ObjectsEqual<ShipmentLine.shipmentDate, ShipmentLine.shipmentTime>(newLine, oldLine))
        //        {
        //            // Checking that both fields in the new data record have values
        //            if (newLine.ShipmentDate != null && newLine.ShipmentTime != null)
        //            {
        //                row.ShippedQty += newLine.LineQty;
        //                rowUpdated = true;
        //            }
        //            // Checking that both fields in the old data records have values
        //            if (oldLine.ShipmentDate != null && oldLine.ShipmentTime != null)
        //            {
        //                row.ShippedQty -= oldLine.LineQty;
        //                rowUpdated = true;
        //            }
        //        }

        //        // Updating the shipment in the cache if it was modifiedED
        //        if (rowUpdated == true)
        //        {
        //            Shipments.Update(row);
        //        }
        //    }
        //}
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
        //protected virtual void ShipmentLine_RowDeleted(PXCache sender, PXRowDeletedEventArgs e)
        //{
        //    ShipmentLine line = (ShipmentLine)e.Row;
        //    Shipment row = Shipments.Current;
        //    if (line.Cancelled != true &&
        //        Shipments.Cache.GetStatus(row) != PXEntryStatus.InsertedDeleted &&
        //        Shipments.Cache.GetStatus(row) != PXEntryStatus.Deleted)
        //    {
        //        row.TotalQty -= line.LineQty;
        //        Shipments.Update(row);
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
        protected virtual void Shipment_ShipmentType_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            foreach (ShipmentLine line in ShipmentLines.Select())
            {
                line.ShipmentDate = null;
                ShipmentLines.Cache.SetDefaultExt<ShipmentLine.shipmentMinTime>(line);
                ShipmentLines.Cache.SetDefaultExt<ShipmentLine.shipmentMaxTime>(line);
                // Updating the line in the cache
                ShipmentLines.Update(line);
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
        protected virtual void ShipmentLine_ShipmentMinTime_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            Shipment row = Shipments.Current;
            if (row != null && row.ShipmentType != Shipment.ShipmentTypes.Single)
            {
                e.NewValue = "9:00 AM";
            }
            else
            {
                e.NewValue = null;
            }
        }
        protected virtual void ShipmentLine_ShipmentMaxTime_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            Shipment row = Shipments.Current;
            if (row != null && row.ShipmentType != Shipment.ShipmentTypes.Single)
            {
                e.NewValue = "7:00 PM";
            }
            else
            {
                e.NewValue = null;
            }
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
        protected virtual void ShipmentLine_ShipmentDate_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            ShipmentLine line = (ShipmentLine)e.Row;
            line.ShipmentTime = null;
        }


        #region CancelShipment Action
        public PXAction<Shipment> CancelShipment;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Cancel Shipment")]
        protected virtual void cancelShipment()
        {
            Shipment row = Shipments.Current;
            row.Status = Shipment.ShipmentStatus.Cancelled;
            // Update the data record in the cache of Shipment data records
            Shipments.Update(row);
            // Triggering the Save action to save changes in the database
            Actions.PressSave();
        }
        #endregion

        #region DeliverShipment Action
        public PXAction<Shipment> DeliverShipment;
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = "Deliver Shipment")]
        protected virtual void deliverShipment()
        {
            Shipment row = Shipments.Current;
            PXCache cache = Shipments.Cache;
            bool errorOccured = false;

            //// Validating that DeliveryDate and DeliveryMaxDate are not null
            //if (row.DeliveryDate == null)
            //{
            //    cache.RaiseExceptionHandling<Shipment.deliveryDate>(row, row.DeliveryDate,
            //            new PXSetPropertyException("Delivery date may not be empty."));
            //    errorOccured = true;
            //}
            //if (row.DeliveryMaxDate == null)
            //{
            //    cache.RaiseExceptionHandling<Shipment.deliveryMaxDate>(row, row.DeliveryMaxDate,
            //            new PXSetPropertyException("Delivery Before date may not be empty."));
            //    errorOccured = true;
            //}
            //if (errorOccured)
            //{
            //    throw new PXException("Shipment '{0}' can not be delivered.", row.ShipmentNbr);
            //}
            //// Calculating total shipped quantity
            //foreach (ShipmentLine line in ShipmentLines.Select())
            //{
            //    if (line.Cancelled != true)
            //    {
            //        row.ShippedQty += line.LineQty;
            //    }
            //}

            if (row.ShipmentType != Shipment.ShipmentTypes.Single)
            {
                // Preventing delivery when not all products have been delivered
                if (row.PendingQty > 0)
                {
                    cache.RaiseExceptionHandling<Shipment.pendingQty>(
                        row, row.PendingQty,
                        new PXSetPropertyException(
                        "Products have not been completely delivered yet."));
                    errorOccured = true;
                }
            }
            else
            {
                // Ensuring that the DeliveryDate value is specified
                if (row.DeliveryDate == null)
                {
                    cache.RaiseExceptionHandling<Shipment.deliveryDate>(
                        row, row.DeliveryDate,
                        new PXSetPropertyException(
                        "Delivery date may not be empty."));
                    errorOccured = true;
                }
                // Ensuring that the DeliveryMaxDate value is specified
                if (row.DeliveryMaxDate == null)
                {
                    cache.RaiseExceptionHandling<Shipment.deliveryMaxDate>(
                        row, row.DeliveryMaxDate,
                        new PXSetPropertyException(
                        "Delivery Before date may not be empty."));
                    errorOccured = true;
                }
            }
            // Throwing an exception if validation hasn't passed
            if (errorOccured)
            {
                throw new PXException("Shipment '{0}' can not be delivered.", row.ShipmentNbr);
            }
            if (row.ShipmentType != Shipment.ShipmentTypes.Single)
            {
                row.DeliveryDate = Accessinfo.BusinessDate;
            }
            else
            {
                // Calculating the ShippedQty for the Single delivery type
                foreach (ShipmentLine line in ShipmentLines.Select())
                {
                    if (line.Cancelled != true)
                    {
                        row.ShippedQty += line.LineQty;
                    }
                }
            }


            // Changing the status
            row.Status = Shipment.ShipmentStatus.Delivered;
            // Updating the data record in the cache
            Shipments.Update(row);
            // Saving changes in the database
            Actions.PressSave();
        }
        #endregion
    }
}