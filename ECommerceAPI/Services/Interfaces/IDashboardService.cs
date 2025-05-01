using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStats> GetDashboardStatsAsync();
    }
}
