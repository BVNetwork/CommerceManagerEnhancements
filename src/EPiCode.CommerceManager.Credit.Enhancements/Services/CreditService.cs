using System;
using System.Collections.Generic;
using System.Linq;
using CommerceManagerCreditEnhancements.DTO;
using CommerceManagerCreditEnhancements.Validators;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using Mediachase.BusinessFoundation;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Engine;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;

namespace CommerceManagerCreditEnhancements.Services
{
    [ServiceConfiguration(typeof(ICreditService))]
    public class CreditService : ICreditService
    {
        private readonly ISynchronizedObjectInstanceCache _cacheInstance;


        public CreditService(ISynchronizedObjectInstanceCache cacheInstance, IDiscountValidator validator)
        {
            _cacheInstance = cacheInstance;
        }

        public virtual ServiceResult Credit(OrderGroup order, IEnumerable<CreditItem> creditItems)
        {
            ServiceResult serviceResult = new ServiceResult();
            decimal totalDiscount = 0M;
            //Add discount
            foreach (CreditItem creditItem in creditItems)
            {
                if (creditItem.Code == "shipment")
                {
                    var shipment =
                        order.OrderForms[0].Shipments.FirstOrDefault(x => (long) x.ShipmentId == creditItem.LineItemId);

                    if (shipment != null)
                    {
                        AddShippingDiscount(shipment, creditItem);
                        totalDiscount += creditItem.ExtraDiscount;
                        shipment.ShippingDiscountAmount =
                            shipment.Discounts.OfType<ShipmentDiscount>().Sum(x => x.DiscountAmount);
                    }   
                }
                else
                {
                    var lineItem = order.OrderForms[0].LineItems.FirstOrDefault(x => (long) x.LineItemId == creditItem.LineItemId);
                    if (lineItem != null)
                    {
                        AddDiscount(lineItem, creditItem);                                                
                        totalDiscount += creditItem.ExtraDiscount;
                        lineItem.LineItemDiscountAmount =
                            lineItem.Discounts.OfType<LineItemDiscount>().Sum(x => x.DiscountAmount) * lineItem.Quantity;
                    }
                }
            }

            var result = OrderGroupWorkflowManager.RunWorkflow(order, "OrderGoodwillCreditWorkflow");

            if (result.Status == WorkflowStatus.Completed)
            {
                var warnings = OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(result).ToList();
                if (warnings.Any())
                {
                    serviceResult.Messages = warnings;
                    serviceResult.IsSuccess = false;
                }
                else
                {
                    OrderNotesManager.AddNoteToPurchaseOrder(order as PurchaseOrder, "Sum discount " + totalDiscount,
                        OrderNoteTypes.System, CustomerContext.Current.CurrentContactId);

                    CreditPayment(order, totalDiscount);

                    var saveChangesResult = OrderGroupWorkflowManager.RunWorkflow(order,
                        OrderGroupWorkflowManager.OrderSaveChangesWorkflowName);
                    if (saveChangesResult.Status == WorkflowStatus.Completed)
                    {
                        var saveChangesWarnings =
                            OrderGroupWorkflowManager.GetWarningsFromWorkflowResult(result).ToList();
                        if (saveChangesWarnings.Any())
                        {
                            serviceResult.Messages = warnings;
                            serviceResult.IsSuccess = false;
                        }
                        else
                        {
                            serviceResult.IsSuccess = true;
                        }
                    }
                }
            }

            order.AcceptChanges();       
            _cacheInstance.RemoveLocal(string.Format("EP:DOR:{0}",order.OrderGroupId));
            return serviceResult;
        }

        private static void AddShippingDiscount(Shipment shipment, CreditItem creditItem)
        {
            var discountAmount = creditItem.ExtraDiscount;

            var existingDiscount = shipment.Discounts.OfType<Discount>().FirstOrDefault();
            if (existingDiscount != null)
            {
                existingDiscount.DiscountAmount += discountAmount;
                existingDiscount.DiscountValue += discountAmount;
            }
            else
            {
                shipment.Discounts.Add(new ShipmentDiscount()
                {
                    DiscountAmount = discountAmount,
                    DiscountValue = discountAmount,
                    DiscountName = "@Extra discount",
                    DisplayMessage = "@Extra discount",
                    ShipmentId = (int) creditItem.LineItemId
                });
            }

        }

        private static void AddDiscount(LineItem lineItem, CreditItem creditItem)
        {
            var discountAmount = creditItem.ExtraDiscount / creditItem.Quantity;

            var existingDiscount = lineItem.Discounts.OfType<Discount>().ToList().FirstOrDefault(x => x.DiscountId == 0);
            if (existingDiscount != null)
            {
                existingDiscount.DiscountAmount += discountAmount;
                existingDiscount.DiscountValue += discountAmount;
            }
            else
            {
                lineItem.Discounts.Add(new LineItemDiscount()
                {
                    DiscountAmount = discountAmount,
                    DiscountValue = discountAmount,
                    DiscountName = "@Extra discount",
                    DisplayMessage = "@Extra discount",
                    LineItemId = (int) creditItem.LineItemId
                });
            }
        }

        public virtual bool IsOrderEnabledForLineItemCredit(OrderGroup order)
        {
            return order is PurchaseOrder;
        }

        public virtual ServiceResult PostCredit(OrderGroup order, IEnumerable<CreditItem> creditItems)
        {
            return new ServiceResult()
            {
                IsSuccess = true
            };
        }

        public virtual ServiceResult PreCredit(OrderGroup order, IEnumerable<CreditItem> creditItems)
        {
            return new ServiceResult()
            {
                IsSuccess = true
            };
        }

        private void CreditPayment(OrderGroup order, decimal sumCredit)
        {
            var capturePayment =
                order.OrderForms[0].Payments.FirstOrDefault(
                    x =>
                        x.TransactionType == TransactionType.Capture.ToString() &&
                        x.Status == PaymentStatus.Processed.ToString());

            CreateCreditPayment(order, capturePayment, sumCredit);
        }


        private void CreateCreditPayment(OrderGroup order, Payment capturePayment, decimal amount)
        {
            var paymentMethodDto = PaymentManager.GetPaymentMethod(capturePayment.PaymentMethodId, true);
            var className = paymentMethodDto.PaymentMethod[0].PaymentImplementationClassName;
            var paymentType = AssemblyUtil.LoadType(className);
            var payment = order.OrderForms[0].Payments.AddNew(paymentType);

            foreach (var field in capturePayment.MetaClass.MetaFields)
            {
                if (!field.Name.Equals("PaymentId", StringComparison.InvariantCultureIgnoreCase))
                    payment[field.Name] = capturePayment[field.Name];
            }

            payment.Amount = amount;
            payment.TransactionType = TransactionType.Credit.ToString();
            payment.Status = PaymentStatus.Pending.ToString();
        }

    }
}