using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 员工表
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// 员工唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 员工编码
        /// </summary>
        public string EmployeeCode { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; }

        /// <summary>
        /// 性别(1:男,2:女,0:未知)
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string? IdCard { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 所属部门ID
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// 离职日期
        /// </summary>
        public DateTime? LeaveDate { get; set; }

        /// <summary>
        /// 状态(1:在职,2:离职,3:休假)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
} 