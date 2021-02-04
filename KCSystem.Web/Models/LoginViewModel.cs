using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace KCSystem.Web.Models
{
    public class LoginViewModel
    {
        [DisplayName("用户名")]
        [Required(ErrorMessage = "必须填写用户名")]
        [MinLength(3, ErrorMessage = "用户名最少包含 3 个字符")]
        [MaxLength(20, ErrorMessage = "用户名最多包含 20 个字符")]
        public string UserName { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage = "必须填写密码")]
        [MinLength(6, ErrorMessage = "密码最少包含 6 个字符")]
        [MaxLength(20, ErrorMessage = "密码最多包含 20 个字符")]
        [PasswordRules]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [DisplayName("当前密码")]
        [Required(ErrorMessage = "必须填写密码")]
        [MinLength(6, ErrorMessage = "密码最少包含 6 个字符")]
        [MaxLength(20, ErrorMessage = "密码最多包含 20 个字符")]
        [PasswordRules]
        public string CurrentPassword { get; set; }

        [DisplayName("新密码")]
        [Required(ErrorMessage = "必须填写密码")]
        [MinLength(6, ErrorMessage = "密码最少包含 6 个字符")]
        [MaxLength(20, ErrorMessage = "密码最多包含 20 个字符")]
        [PasswordRules]
        public string NewPassword { get; set; }
        public int UserId { get; set; }
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 密码规则：必须包含以下各类中的两种：大写字母、小写字母，数字，键盘特殊字符
    /// </summary>
    class PasswordRulesAttribute : ValidationAttribute
    {

        
        static readonly Regex _numbers = new Regex("[0-9]+", RegexOptions.Compiled);
        static readonly Regex _lowerCase = new Regex("[a-z]+", RegexOptions.Compiled);
        static readonly Regex _upperCase = new Regex("[A-Z]+", RegexOptions.Compiled);

        public PasswordRulesAttribute()
        {
            ErrorMessage = "密码必须包含以下字符中的两种以上：大写字母、小写字母，数字";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                // 此处不检查空值，空值留给 Required 检查
                return true;
            }

            var password = Convert.ToString(value);
            var contains = 0;
          

            if (_numbers.IsMatch(password))
            {
                contains++;
            }

            if (_lowerCase.IsMatch(password))
            {
                contains++;
            }

            if (_upperCase.IsMatch(password))
            {
                contains++;
            }

            return contains >= 2;
        }
    }

}