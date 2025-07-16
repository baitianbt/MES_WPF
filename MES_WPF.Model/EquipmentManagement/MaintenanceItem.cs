using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 设备维护项目表
    /// </summary>
    [Table("MaintenanceItem")]
    public class MaintenanceItem
    {
        /// <summary>
        /// 维护项目唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string ItemName { get; set; }

        /// <summary>
        /// 关联维护计划ID
        /// </summary>
        [Required]
        public int MaintenancePlanId { get; set; }

        /// <summary>
        /// 项目类型(1:检查,2:清洁,3:润滑,4:更换,5:调整)
        /// </summary>
        [Required]
        public byte ItemType { get; set; }

        /// <summary>
        /// 标准值
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string? StandardValue { get; set; }

        /// <summary>
        /// 上限
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string? UpperLimit { get; set; }

        /// <summary>
        /// 下限
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string? LowerLimit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(20)]
        [Column(TypeName = "VARCHAR")]
        public string? Unit { get; set; }

        /// <summary>
        /// 维护方法
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string? Method { get; set; }

        /// <summary>
        /// 所需工具
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string? Tool { get; set; }

        /// <summary>
        /// 序号
        /// </summary>
        [Required]
        public int SequenceNo { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        [Required]
        public bool IsRequired { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string? Remark { get; set; }
    }
}