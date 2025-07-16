using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 设备维护计划表
    /// </summary>
    [Table("EquipmentMaintenancePlan")]
    public class EquipmentMaintenancePlan
    {
        /// <summary>
        /// 维护计划唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 计划编码
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string PlanCode { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string PlanName { get; set; }

        /// <summary>
        /// 关联设备ID
        /// </summary>
        [Required]
        public int EquipmentId { get; set; }

        /// <summary>
        /// 维护类型(1:日常保养,2:定期维护,3:预防性维护)
        /// </summary>
        [Required]
        public byte MaintenanceType { get; set; }

        /// <summary>
        /// 周期类型(1:天,2:周,3:月,4:季度,5:年)
        /// </summary>
        [Required]
        public byte CycleType { get; set; }

        /// <summary>
        /// 周期值
        /// </summary>
        [Required]
        public int CycleValue { get; set; }

        /// <summary>
        /// 标准工时(分钟)
        /// </summary>
        [Required]
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal StandardTime { get; set; }

        /// <summary>
        /// 上次执行日期
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? LastExecuteDate { get; set; }

        /// <summary>
        /// 下次执行日期
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? NextExecuteDate { get; set; }

        /// <summary>
        /// 状态(1:启用,2:禁用)
        /// </summary>
        [Required]
        public byte Status { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Required]
        public int CreateBy { get; set; }

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
        public string Remark { get; set; }
    }
} 