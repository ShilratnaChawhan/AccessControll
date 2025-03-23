using AccessControll_API.ModelAndContext;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using AccessControll_API.Domain;

namespace AccessControll_API.Helper
{
  public class ApiHelper
    {

        EntityContext context;
        public ApiHelper(EntityContext entityContext)
        {
            context = entityContext;
        }
        internal string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        internal string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            salt = Convert.ToBase64String(saltBytes);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            return Convert.ToBase64String(hash);
        }

        internal bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            string newHash = Convert.ToBase64String(hash);
            return storedHash == newHash;
        }
        internal string GenerateOtp()
        {
            Random random = new Random();
            string randomno = random.Next(0, 1000000).ToString("D4");
            return randomno;
        }

        internal void SaveOtp(string OTPText, long userId, string? otpType)
        {
            var OTP = new Entity_OTP_Log()
            {
                OTP = OTPText,
                User_Id = userId,
                OTP_Type = otpType,
                Expires_At = DateTime.Now.AddMinutes(5),
                Created_At = DateTime.Now,
                Is_Active = true,
                Is_Verified = false
            };
            this.context.Entity_OTP_Log.Add(OTP);
            this.context.SaveChanges();
        }

        internal void SendEmail(string useremail, string OtpText, string Name)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "notification.priority.mail@gmail.com";
                string senderPassword = "yyyoxehnoncorasa";
                string recipientEmail = useremail;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

                    string emailbody = $"Hi {Name} ";
                    if (!OtpText.All(char.IsDigit))
                        emailbody += $"Your New Password is : {OtpText}";

                    emailbody += $"Your OTP is : {OtpText}";
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail),
                        Subject = "Notification",
                        Body = emailbody,
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(recipientEmail);
                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        internal bool ValidateOTP(long userId, string OTPText)
        {
            bool response = false;

            var _data = this.context.Entity_OTP_Log.FirstOrDefault(item => item.User_Id == userId && item.OTP == OTPText && item.Expires_At > DateTime.Now && item.Is_Active == true);
            if (_data != null)
            {
                response = true;

                _data.Is_Verified = true;
                _data.Is_Active = false;
                this.context.SaveChanges();
            }
            return response;
        }

        internal string GenerateJwtToken(Payload payload, string apiSecret, string payloadEncpKey)
        {
            var header = new JwtHeader(
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret)), SecurityAlgorithms.HmacSha256));

            string payloadJson = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            byte[] plainBytes = Encoding.UTF8.GetBytes(payloadJson);
            byte[] key = Encoding.UTF8.GetBytes(payloadEncpKey);

            byte[] cipherBytes = Encrypt(plainBytes, key);
            string cipherText = Convert.ToBase64String(cipherBytes);

            JwtPayload jwtPayload = new JwtPayload
            {
                  { "payload", cipherText}
            };

            var jwtToken = new JwtSecurityToken(header, jwtPayload);

            byte[] decryptedBytes = Decrypt(cipherBytes, key);
            string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        internal byte[] Encrypt(byte[] plainBytes, byte[] key)
        {
            byte[] encryptedBytes = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                }
            }
            return encryptedBytes;
        }

        internal byte[] Decrypt(byte[] cipherBytes, byte[] key)
        {
            byte[] decryptedBytes = null;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                }
            }

            return decryptedBytes;
        }

    }
}
