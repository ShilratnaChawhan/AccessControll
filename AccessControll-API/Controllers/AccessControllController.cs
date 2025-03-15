using AccessControll_API.Domain;
using AccessControll_API.ModelAndContext;
using AccessControll_API.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessControll_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessControllController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly EntityContext _context;

        public AccessControllController(IAuthenticationService authenticationService, EntityContext context)
        {
            _authenticationService = authenticationService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<long>> Register([FromBody] RegisterRequest request)
        {
            var response = await _authenticationService.Register(request);
            return Ok(response);
        }

        [HttpPost("confirm-registration")]
        public async Task<ActionResult<Entity_User>> ConfirmRegistration([FromBody] ConfirmRegistrationRequest request)
        {
            var response = await _authenticationService.ConfirmRegisteration(request);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var response = await _authenticationService.Login(request);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<ActionResult<bool?>> Logout([FromQuery] string authToken)
        {
            var response = await _authenticationService.Logout(authToken);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<long?>> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var response = await _authenticationService.ForgotPassword(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var response = await _authenticationService.ChangePassword(request);
            return Ok(response);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromQuery] string refreshToken)
        {
            var response = await _authenticationService.RefreshToken(refreshToken);
            return Ok(response);
        }

        [HttpPost("send-otp")]
        public async Task<ActionResult<bool>> SendOtp([FromBody] SendOtpRequest request)
        {
            var response = await _authenticationService.SendOtp(request);
            return Ok(response);
        }

        [HttpPost("verify-otp")]
        public async Task<ActionResult<bool>> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await _authenticationService.VerifyOtp(request);
            return Ok(response);
        }

        [HttpGet("get-all-users")]
        public async Task<ActionResult<object>> GetAllUsers()
        {
            var response = await _authenticationService.GetAllUsers();
            return Ok(response);
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<Entity_Role>>> GetRoles()
        {
            return Ok(await _authenticationService.GetRolesAsync());
        }

        [HttpGet("menus")]
        public async Task<ActionResult<IEnumerable<Entity_Menu>>> GetMenus()
        {
            return Ok(await _authenticationService.GetMenusAsync());
        }

        [HttpGet("permissions/{roleId}")]
        public async Task<ActionResult<RolePermissionViewModel>> GetRolePermissions(long roleId)
        {
            var result = await _authenticationService.GetRolePermissionsAsync(roleId);
            if (result == null) return NotFound(new { message = "Role not found" });

            return Ok(result);
        }

        [HttpPost("permissions")]
        public async Task<ActionResult> SaveRolePermissions(RolePermissionViewModel model)
        {
            var success = await _authenticationService.SaveRolePermissionsAsync(model);
            return success ? Ok(new { message = "Permissions saved successfully" }) :
                             StatusCode(500, new { message = "Error saving permissions" });
        }

        [HttpDelete("permissions/{rolePermissionId}")]
        public async Task<ActionResult> DeleteRolePermission(long rolePermissionId)
        {
            var success = await _authenticationService.DeleteRolePermissionAsync(rolePermissionId);
            return success ? Ok(new { message = "Permission deleted successfully" }) :
                             NotFound(new { message = "Permission not found" });
        }

        [HttpDelete("role/{roleId}")]
        public async Task<ActionResult> DeleteRole(long roleId)
        {
            var success = await _authenticationService.DeleteRoleAsync(roleId);
            return success ? Ok(new { message = "Role deleted successfully" }) :
                             NotFound(new { message = "Role not found" });
        }

        [HttpDelete("menu/{menuId}")]
        public async Task<ActionResult> DeleteMenu(long menuId)
        {
            var success = await _authenticationService.DeleteMenuAsync(menuId);
            return success ? Ok() : NotFound();
        }

        [HttpPost("role")]
        public async Task<ActionResult<Entity_Role>> CreateRole(Entity_Role role)
        {
            var createdRole = await _authenticationService.CreateRoleAsync(role);
            return CreatedAtAction(nameof(GetRoles), new { id = createdRole.Role_Id }, createdRole);
        }

        [HttpPost("menu")]
        public async Task<ActionResult<Entity_Menu>> CreateMenu(Entity_Menu menu)
        {
            var createdMenu = await _authenticationService.CreateMenuAsync(menu);
            return CreatedAtAction(nameof(GetMenus), new { id = createdMenu.Menu_Id }, createdMenu);
        }
    }
}