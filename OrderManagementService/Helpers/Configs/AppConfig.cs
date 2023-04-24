namespace OrderManagementService.Helpers.Configs
{
    public class AppConfig
    {
        public string UserManagementApiKey { get; set; }
        public string UserManagementBaseUrl { get; set; }
        public string GetCouiriersEndpoint { get; set; }
        public string GetCourierEndpoint { get; set; }
        public string DeliveryManagementBaseUrl { get; set; }
        public string IsCancellationAllowedEndpoint { get; set; }
    }
}
