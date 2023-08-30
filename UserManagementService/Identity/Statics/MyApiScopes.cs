namespace UserManagementService.Identity.Statics
{
    public static class MyApiScopes
    {
        public static class DeliveryManagement
        {
            public const string CheckOrderCancelation = "delivery-isCancelationAllowed";
        }
        public static class UserManagement
        {
            public const string GetCouriers = "user-getCouriersDetails";
        }
    }
}
