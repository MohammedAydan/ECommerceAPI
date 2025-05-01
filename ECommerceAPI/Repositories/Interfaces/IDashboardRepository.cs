using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardStats> GetDashboardStatsAsync();
    }
}
