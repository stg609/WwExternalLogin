using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class WwUserInfoDto
    {
        /// <summary>
        /// 成员UserID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        public long Errcode { get; set; }

        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string Errmsg { get; set; }

    }

    public class WwUserDto
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 员工名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 座机
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 员工在当前企业内的唯一标识，也称staffId。可由企业在创建时指定，并代表一定含义比如工号，创建后不可修改
        /// </summary>
        public string Userid { get; set; }

        /// <summary>
        /// 职位信息
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 激活状态: 1=已激活，2=已禁用，4=未激活，5=退出企业。已激活代表已激活企业微信或已关注微工作台（原企业号）。未激活代表既未激活企业微信又未关注微工作台（原企业号）。
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// 头像url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 成员所属部门id列表
        /// </summary>
        public List<long> Department { get; set; }

        /// <summary>
        /// 主部门
        /// </summary>
        public long Main_department { get; set; }

        /// <summary>
        /// 员工的电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 返回码
        /// </summary>
        public long Errcode { get; set; }

        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string Errmsg { get; set; }

        /// <summary>
        /// 扩展属性，第三方仅通讯录应用可获取；对于非第三方创建的成员，第三方通讯录应用也不可获取
        /// </summary>
        public ExtAttrDto Extattr { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public WxGender Gender { get; set; }
    }

    public enum WxGender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public enum Status
    {
        Active = 1,
        Disable = 2,
        Inactive = 4,
        Exit = 5
    }

    public class ExtAttrDto
    {
        public object[] Attrs { get; set; }
    }
}
