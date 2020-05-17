using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel.Request
{
    public class UserLoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(50, ErrorMessage = "用户名长度设置过长")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>

        [Required(ErrorMessage = "用户密码不能为空")]
        [StringLength(maximumLength: 50, MinimumLength = 6, ErrorMessage = "密码长度至少是6位")]
        public string UserPwd { get; set; }
    }
}
