using System;
using AccessControll_API.Domain;
using AccessControll_API.ModelAndContext;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AccessControll_API.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration configuration;
        private readonly EntityContext context;
        private readonly Helper.ApiHelper apiHelper;

        public AuthenticationService(IConfiguration _configuration, EntityContext _context)
        {
            configuration = _configuration;
            context = _context;
            apiHelper = new Helper.ApiHelper(context);

        }

        public async Task<long> Register(RegisterRequest authenticationRequest)
        {
            try
            {
                var existingUser = await context.Entity_User
                    .Where(item => item.Username == authenticationRequest.UserName &&
                                   item.Email == authenticationRequest.UserEmail &&
                                   item.Is_Active == true)
                    .ToListAsync();

                if (existingUser.Any())
                    throw new Exception("User already exists.");

                var person = new Entity_Person
                {
                    First_Name = authenticationRequest.FirstName,
                    Last_Name = authenticationRequest.LastName,
                    Email_Address = authenticationRequest.UserEmail,
                    Phone_Number = authenticationRequest.PhoneNumber,
                    Is_Active = true
                };

                await context.Entity_Person.AddAsync(person);
                await context.SaveChangesAsync();

                var user = new Entity_User
                {
                    Username = authenticationRequest.UserName,
                    Password_Hash = apiHelper.HashPassword(authenticationRequest.Password, out var salt),
                    Password_Salt = salt,
                    Person_Id = person.Person_Id,
                    Email = person.Email_Address,
                    Role_Id = 1, // Default role
                    Is_Active = false
                };

                await context.Entity_User.AddAsync(user);
                await context.SaveChangesAsync();

                var OTPText = apiHelper.GenerateOtp();
                apiHelper.SaveOtp(OTPText, user.User_Id, "Register");
                apiHelper.SendEmail(authenticationRequest.UserEmail, OTPText, authenticationRequest.UserName);

                return user.User_Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration: " + ex.Message);
            }
        }

        public async Task<Entity_User> ConfirmRegisteration(ConfirmRegistrationRequest authenticationRequest)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(item =>
                    item.User_Id == authenticationRequest.UserId && item.Is_Active == false);

                if (user == null)
                    throw new Exception("User not found or already activated.");

                bool otpresponse = apiHelper.ValidateOTP(authenticationRequest.UserId, authenticationRequest.Otp);
                if (!otpresponse)
                    throw new Exception("Invalid OTP.");

                user.Is_Active = true;
                user.Is_Locked = false;
                await context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration confirmation: " + ex.Message);
            }
        }

        public async Task<string> Login(LoginRequest request)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(item =>
                    item.Username == request.UserName && item.Is_Active == true);

                if (user == null)
                    throw new Exception("Invalid username or password.");

                var refreshToken = apiHelper.GenerateRefreshToken();
                Payload payload = new Payload
                {
                    UserId = user.User_Id,
                    RoleId = user.Role_Id,
                    PersonId = user.Person_Id,
                    RefreshToken = refreshToken
                };

                string payloadEncryptKey = configuration["JWTSetting:EncryptKey"];
                string apiKey = configuration["JWTSetting:Authkey"];

                user.Auth_Token = apiHelper.GenerateJwtToken(payload, apiKey, payloadEncryptKey);
                user.Refresh_Token = refreshToken;
                user.Is_Logged_In = true;

                await context.SaveChangesAsync();

                return user.Auth_Token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during login: " + ex.Message);
            }
        }

        public async Task<bool?> Logout(string auth_Token)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(item => item.Auth_Token == auth_Token);

                if (user == null)
                    throw new Exception("User not found.");

                user.Auth_Token = null;
                user.Refresh_Token = null;
                user.Is_Logged_In = false;

                await context.SaveChangesAsync();
                return user.Is_Logged_In;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during logout: " + ex.Message);
            }
        }

        public async Task<long?> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(u => u.Email == request.UserEmail);
                if (user == null)
                    throw new Exception("User with this email not found.");

                var OTPText = apiHelper.GenerateOtp();
                apiHelper.SaveOtp(OTPText, user.User_Id, "ForgotPassword");
                apiHelper.SendEmail(request.UserEmail, OTPText, user.Username);

                return user.User_Id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during forgot password: " + ex.Message);
            }
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(u => u.User_Id == request.UserId);
                if (user == null)
                    throw new Exception("User not found.");

                bool otpvalidation = apiHelper.ValidateOTP(request.UserId, request.Otp);
                if (!otpvalidation)
                    throw new Exception("Invalid OTP.");

                var hashPassword = apiHelper.HashPassword(request.NewPassword, out var salt);
                if (user.Password_Hash.Equals(hashPassword))
                    throw new Exception("New password cannot be the same as the old password.");

                user.Password_Hash = hashPassword;
                user.Password_Salt = salt;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during password change: " + ex.Message);
            }
        }

        public async Task<string> RefreshToken(string refreshToken)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(item => item.Refresh_Token == refreshToken);
                if (user == null)
                    throw new Exception("Invalid refresh token.");

                var newRefreshToken = apiHelper.GenerateRefreshToken();
                Payload payload = new Payload
                {
                    UserId = user.User_Id,
                    RoleId = user.Role_Id.GetValueOrDefault(),
                    PersonId = user.Person_Id.GetValueOrDefault(),
                    RefreshToken = newRefreshToken
                };

                string payloadEncryptKey = configuration["JWTSetting:EncryptKey"];
                string apiKey = configuration["JWTSetting:Authkey"];

                user.Auth_Token = apiHelper.GenerateJwtToken(payload, apiKey, payloadEncryptKey);
                user.Refresh_Token = newRefreshToken;

                await context.SaveChangesAsync();
                return user.Auth_Token;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during token refresh: " + ex.Message);
            }
        }

        public async Task<bool> SendOtp(SendOtpRequest request)
        {
            try
            {
                var user = await context.Entity_User.FirstOrDefaultAsync(u => u.Email == request.UserEmail);

                if (user == null)
                    throw new Exception("User with this email not found.");

                var OTPText = apiHelper.GenerateOtp();
                apiHelper.SaveOtp(OTPText, user.User_Id, request.OtpType);
                apiHelper.SendEmail(request.UserEmail, OTPText, user.Username);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending OTP: " + ex.Message);
            }
        }

        public async Task<bool> VerifyOtp(VerifyOtpRequest request)
        {
            try
            {
                var otpLog = await context.Entity_OTP_Log.FirstOrDefaultAsync(o =>
                    o.OTP == request.Otp && o.User_Id == request.UserId && o.Is_Active == true);

                if (otpLog == null)
                    throw new Exception("Invalid OTP.");

                otpLog.Is_Verified = true;
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error during OTP verification: " + ex.Message);
            }
        }

        public async Task<object> GetAllUsers()
        {
            try
            {
                return await context.Entity_User
                                             .Where(u => u.Is_Active == true)
                                             .Select(u => new
                                             {
                                                 u.User_Id,
                                                 u.Role_Id,
                                                 u.Username,
                                                 u.Email,
                                                 u.Is_Active,
                                                 u.Person_Id,
                                             })
                                             .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error during users get: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Entity_Role>> GetRolesAsync()
        {
            return await context.Entity_Role.Where(r => r.Is_Active == true).ToListAsync();
        }

        public async Task<IEnumerable<Entity_Menu>> GetMenusAsync()
        {
            return await context.Entity_Menu.Where(m => m.Is_Active == true).ToListAsync();
        }

        public async Task<RolePermissionViewModel> GetRolePermissionsAsync(long roleId)
        {
            var role = await context.Entity_Role.FindAsync(roleId);
            if (role == null)
            {
                return null;
            }

            var permissions = await (from permission in context.Entity_Role_Menu_Permission
                                     join menu in context.Entity_Menu on permission.Menu_Id equals menu.Menu_Id into menuGroup
                                     from menu in menuGroup.DefaultIfEmpty()
                                     where permission.Role_Id == roleId
                                     select new MenuPermissionItem
                                     {
                                         MenuId = permission.Menu_Id ?? 0,
                                         MenuCode = menu != null ? menu.Menu_Code : string.Empty,
                                         CanAdd = permission.Can_Create ?? false,
                                         CanEdit = permission.Can_Edit ?? false,
                                         CanView = permission.Can_View ?? false,
                                         CanDelete = permission.Can_Delete ?? false,
                                         IsActive = permission.Is_Active ?? true,
                                         RolePermissionId = permission.Role_Menu_Permission_Id
                                     }).ToListAsync();

            var assignedMenuIds = permissions.Select(p => p.MenuId).ToList();
            var unassignedMenus = await context.Entity_Menu
                .Where(m => m.Is_Active == true && !assignedMenuIds.Contains(m.Menu_Id))
                .Select(m => new MenuPermissionItem
                {
                    MenuId = m.Menu_Id,
                    MenuCode = m.Menu_Code,
                    CanAdd = false,
                    CanEdit = false,
                    CanView = false,
                    CanDelete = false,
                    IsActive = true,
                    RolePermissionId = 0
                })
                .ToListAsync();

            permissions.AddRange(unassignedMenus);

            return new RolePermissionViewModel
            {
                RoleId = role.Role_Id,
                RoleName = role.Role_Name,
                Permissions = permissions
            };
        }

        public async Task<bool> SaveRolePermissionsAsync(RolePermissionViewModel model)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingPermissions = await context.Entity_Role_Menu_Permission
                    .Where(p => p.Role_Id == model.RoleId)
                    .GroupBy(p => p.Menu_Id)
                    .Select(g => g.First())
                    .ToDictionaryAsync(p => p.Menu_Id);

                var newPermissions = new List<Entity_Role_Menu_Permission>();

                foreach (var permission in model.Permissions)
                {
                    if (permission.RolePermissionId > 0 && existingPermissions.ContainsKey(permission.MenuId))
                    {
                        var existingPermission = existingPermissions[permission.MenuId];
                        existingPermission.Can_View = permission.CanView;
                        existingPermission.Can_Create = permission.CanAdd;
                        existingPermission.Can_Edit = permission.CanEdit;
                        existingPermission.Can_Delete = permission.CanDelete;
                        existingPermission.Is_Active = permission.IsActive;
                    }
                    else if (permission.RolePermissionId == 0 && !existingPermissions.ContainsKey(permission.MenuId))
                    {
                        newPermissions.Add(new Entity_Role_Menu_Permission
                        {
                            Role_Id = model.RoleId,
                            Menu_Id = permission.MenuId,
                            Can_Create = permission.CanAdd,
                            Can_Edit = permission.CanEdit,
                            Can_View = permission.CanView,
                            Can_Delete = permission.CanDelete,
                            Is_Active = permission.IsActive
                        });
                    }
                }

                if (newPermissions.Count > 0)
                {
                    await context.Entity_Role_Menu_Permission.AddRangeAsync(newPermissions);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> DeleteRolePermissionAsync(long rolePermissionId)
        {
            var permission = await context.Entity_Role_Menu_Permission.FindAsync(rolePermissionId);
            if (permission == null) return false;

            context.Entity_Role_Menu_Permission.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(long roleId)
        {
            var role = await context.Entity_Role.FindAsync(roleId);
            if (role == null) return false;

            var permissions = await context.Entity_Role_Menu_Permission
                .Where(p => p.Role_Id == roleId)
            .ToListAsync();

            context.Entity_Role_Menu_Permission.RemoveRange(permissions);
            context.Entity_Role.Remove(role);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMenuAsync(long menuId)
        {
            var menu = await context.Entity_Menu.FindAsync(menuId);
            if (menu == null) return false;

            var permissions = await context.Entity_Role_Menu_Permission
                .Where(p => p.Menu_Id == menuId)
            .ToListAsync();

            context.Entity_Role_Menu_Permission.RemoveRange(permissions);
            context.Entity_Menu.Remove(menu);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Entity_Role> CreateRoleAsync(Entity_Role role)
        {
            context.Entity_Role.Add(role);
            await context.SaveChangesAsync();
            return role;
        }

        public async Task<Entity_Menu> CreateMenuAsync(Entity_Menu menu)
        {
            context.Entity_Menu.Add(menu);
            await context.SaveChangesAsync();
            return menu;
        }

        public async Task<List<Entity_Menu>> GetMenuByRole(long roleId)
        {
            return await this.context.Entity_Role_Menu_Permission
                .Where(o => o.Role_Id == roleId && o.Is_Active == true && o.Can_View == true)
                .Join(this.context.Entity_Menu,
                      menu => menu.Menu_Id,
                      m => m.Menu_Id,
                      (menu, m) => new Entity_Menu
                      {
                          Menu_Id = m.Menu_Id,
                          Menu_Code = m.Menu_Code,
                          Is_Active = m.Is_Active
                      }).ToListAsync();
        }

        public async Task<Entity_User> UpdateUserRole(UpdateUserRole updateUserRole)
        {
            if (updateUserRole == null)
                throw new ArgumentNullException(nameof(updateUserRole));

            var user = await context.Entity_User.FindAsync(updateUserRole.UserId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {updateUserRole.UserId} not found.");

            var role = await context.Entity_Role.FindAsync(updateUserRole.RoleId);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {updateUserRole.RoleId} not found.");
            else
            {
                user.Role_Id = updateUserRole.RoleId;
                context.Entity_User.Update(user);
            }

            await context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteUserAsync(long userId)
        {
            var user = await context.Entity_User.FindAsync(userId);
            if (user == null) return false;

            context.Entity_User.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
