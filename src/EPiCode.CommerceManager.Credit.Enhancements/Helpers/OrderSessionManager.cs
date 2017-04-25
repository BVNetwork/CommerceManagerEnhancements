using System;
using System.Globalization;
using System.Web;
using Mediachase.Commerce.Orders;

namespace CommerceManagerCreditEnhancements.Helpers
{
	/// <summary>
	/// Represents session storage for temporary order objects
	/// </summary>
	internal static class OrderSessionManager 
	{
		private const string ORDER_GROUP_DTO_SESSION_KEY = "ECF.OrderGroup.Edit";
		private const string NEW_ORDER_GROUP_SESSION_KEY = "ECF.OrderGroup.New";
		public const string ReturnOrderFormSessionKey = "ECF.ReturnOrderForm.Edit";
		public const string CouponCodeSessionKey = "ECF.CouponCode";
	

		#region OrderGroup
		/// <summary>
		/// Gets the order group object.
		/// </summary>
		/// <param name="orderGroupId">The order group id.</param>
		/// <returns></returns>
		internal static OrderGroup GetOrderGroupObject(int orderGroupId)
		{
			return HttpContext.Current.Session[GetSessionKey(orderGroupId)] as OrderGroup;
		}

		/// <summary>
		/// Sets the order group object.
		/// </summary>
		/// <param name="orderGroup">The order group.</param>
		internal static void SetOrderGroupObject(OrderGroup orderGroup)
		{
			HttpContext.Current.Session[GetSessionKey(orderGroup.OrderGroupId)] = orderGroup;
		}

		/// <summary>
		/// Clears the order group object.
		/// </summary>
		/// <param name="orderGroupId">The order group id.</param>
		internal static void ClearOrderGroupObject(int orderGroupId)
		{
			if (orderGroupId <= 0)
				ClearNewOrderGroupObject(0, 0);
			else
				HttpContext.Current.Session.Remove(GetSessionKey(orderGroupId));
		}

		internal static string GetSessionKey(int orderGroupId)
		{
			return ORDER_GROUP_DTO_SESSION_KEY + ":" + orderGroupId;
		}
		#endregion

		#region New Order
		/// <summary>
		/// Gets the order group object.
		/// </summary>
		/// <returns></returns>
		internal static OrderGroup GetNewOrderGroupObject(int parentOrderGroupId, int parentOrderFormId)
		{
			return HttpContext.Current.Session[GetNewOrderSessionKey(parentOrderGroupId, parentOrderFormId)] as OrderGroup;
		}

	    /// <summary>
	    /// Sets the order group object.
	    /// </summary>
	    /// <param name="orderGroup">The order group.</param>
	    /// <param name="parentOrderGroupId"></param>
	    /// <param name="parentOrderFormId"></param>
	    internal static void SetNewOrderGroupObject(OrderGroup orderGroup, int parentOrderGroupId, int parentOrderFormId)
		{
			HttpContext.Current.Session[GetNewOrderSessionKey(parentOrderGroupId, parentOrderFormId)] = orderGroup;
		}

		/// <summary>
		/// Clears the order group object.
		/// </summary>
		internal static void ClearNewOrderGroupObject(int parentOrderGroupId, int parentOrderFormId)
		{
			HttpContext.Current.Session.Remove(GetNewOrderSessionKey(parentOrderGroupId, parentOrderFormId));
		}

		internal static string GetNewOrderSessionKey(int parentOrderGroupId, int parentOrderFormId)
		{
			return String.Join(":", new[] { NEW_ORDER_GROUP_SESSION_KEY, parentOrderGroupId.ToString(CultureInfo.InvariantCulture), parentOrderFormId.ToString(CultureInfo.InvariantCulture) });
		}
		#endregion

		#region Coupon Code
		/// <summary>
		/// Gets the coupon code.
		/// </summary>
		/// <returns></returns>
		internal static string GetCouponCode()
		{
			return HttpContext.Current.Session[CouponCodeSessionKey] as string;
		}

		/// <summary>
		/// Sets the coupon code.
		/// </summary>
		/// <param name="couponCode">The coupon code.</param>
		internal static void SetCouponCode(string couponCode)
		{
			HttpContext.Current.Session[CouponCodeSessionKey] = couponCode;
		}

		/// <summary>
		/// Clears the coupon code.
		/// </summary>
		internal static void ClearCouponCode()
		{
			HttpContext.Current.Session.Remove(CouponCodeSessionKey);
		}

		#endregion

	}
}
