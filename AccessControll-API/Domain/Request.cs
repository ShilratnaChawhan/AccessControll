namespace AccessControll_API.Domain
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }


    public class ConfirmRegistrationRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string UserEmail { get; set; }
    }

    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetPasswordRequest
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; }
    }

    public class SendOtpRequest
    {
        public string UserEmail { get; set; }
        public string OtpType { get; set; }
    }

    public class VerifyOtpRequest
    {
        public int UserId { get; set; }
        public string Otp { get; set; }
    }

    public class RolePermissionViewModel
    {
        public long? RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuPermissionItem>? Permissions { get; set; }
    }

    public class MenuPermissionItem
    {
        public long? MenuId { get; set; }
        public string MenuCode { get; set; }
        public bool? CanAdd { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanView { get; set; }
        public bool? CanDelete { get; set; }
        public bool? IsActive { get; set; }
        public long RolePermissionId { get; set; }
    }

    public class UpdateUserRole
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
