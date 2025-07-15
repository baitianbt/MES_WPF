using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 资源信息表
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// 资源唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 资源编码 - 需要创建唯一索引
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ResourceCode { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ResourceName { get; set; }

        /// <summary>
        /// 资源类型(1:设备,2:人员,3:工装)
        /// </summary>
        [Required]
        public byte ResourceType { get; set; }

        /// <summary>
        /// 所属部门ID
        /// </summary>
        public int? DepartmentId { get; set; }

        /// <summary>
        /// 状态(1:可用,2:占用,3:故障,4:维修中)
        /// </summary>
        [Required]
        public byte Status { get; set; }

        /// <summary>
        /// 资源描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        // 导航属性 - 暂时注释掉Department引用，需要添加合适的引用
         [ForeignKey("DepartmentId")]
         public virtual MES_WPF.Core.Models.Department Department { get; set; }

        // 设备信息导航属性
        public virtual Equipment Equipment { get; set; }
    }
} 