using DataAccessLayer.ViewModels;

namespace BusinessAcessLayer.Interface;

public interface IDashboardService
{
    ApiResponse<DashboardViewModel> GetDashboardData(int businessId);
    ApiResponse<List<string>> Getyears(int businessId);
}
