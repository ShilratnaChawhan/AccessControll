using AccessControll_API.Domain;
using AccessControll_API.ModelAndContext;

namespace AccessControll_API.Service
{
    public interface IAuthenticationService
    {
        Task<long> Register(RegisterRequest request);
        Task<Entity_User> ConfirmRegisteration(ConfirmRegistrationRequest request);
        Task<string> Login(LoginRequest request);
        Task<bool?> Logout(string authToken);
        Task<long?> ForgotPassword(ForgotPasswordRequest request);
        Task<bool> ChangePassword(ChangePasswordRequest request);
        Task<string> RefreshToken(string refreshToken);
        Task<bool> SendOtp(SendOtpRequest request);
        Task<bool> VerifyOtp(VerifyOtpRequest request);
        Task<object> GetAllUsers();
        Task<IEnumerable<Entity_Role>> GetRolesAsync();
        Task<IEnumerable<Entity_Menu>> GetMenusAsync();
        Task<RolePermissionViewModel> GetRolePermissionsAsync(long roleId);
        Task<bool> SaveRolePermissionsAsync(RolePermissionViewModel model);
        Task<bool> DeleteRolePermissionAsync(long rolePermissionId);
        Task<bool> DeleteMenuAsync(long menuId);
        Task<bool> DeleteRoleAsync(long roleId);
        Task<Entity_Role> CreateRoleAsync(Entity_Role role);
        Task<Entity_Menu> CreateMenuAsync(Entity_Menu menu);
        Task<List<Entity_Menu>> GetMenuByRole(long roleId);
        Task<Entity_User> UpdateUserRole(UpdateUserRole updateUserRole);
        Task<bool> DeleteUserAsync(long userId);
    }
}
