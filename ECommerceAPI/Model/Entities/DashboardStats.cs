namespace ECommerceAPI.Model.Entities
{
    public class DashboardStats
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveUsers { get; set; }
        public decimal RevenueGrowth { get; set; }
        public int OrdersGrowth { get; set; }
        public int ProductsGrowth { get; set; }
        public int UsersGrowth { get; set; }
    }
}
