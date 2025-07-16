using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 维护工单表
    /// </summary>
    [Table("MaintenanceOrder")]
    public class MaintenanceOrder
    {
        /// <summary>
        /// 维护工单唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 工单编码
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string OrderCode { get; set; }

        /// <summary>
        /// 工单类型(1:计划维护,2:故障维修,3:紧急维修)
        /// </summary>
        [Required]
        public byte OrderType { get; set; }

        /// <summary>
        /// 关联设备ID
        /// </summary>
        [Required]
        public int EquipmentId { get; set; }

        /// <summary>
        /// 关联维护计划ID(计划维护时)
        /// </summary>
        public int? MaintenancePlanId { get; set; }

        /// <summary>
        /// 故障描述(故障维修时)
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string FaultDescription { get; set; }

        /// <summary>
        /// 故障代码
        /// </summary>
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string FaultCode { get; set; }

        /// <summary>
        /// 故障等级(1:轻微,2:一般,3:严重)
        /// </summary>
        public byte? FaultLevel { get; set; }

        /// <summary>
        /// 优先级(1-10)
        /// </summary>
        [Required]
        public byte Priority { get; set; }

        /// <summary>
        /// 状态(1:待处理,2:已分配,3:处理中,4:已完成,5:已取消)
        /// </summary>
        [Required]
        public byte Status { get; set; }

        /// <summary>
        /// 计划开始时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime PlanStartTime { get; set; }

        /// <summary>
        /// 计划结束时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime PlanEndTime { get; set; }

        /// <summary>
        /// 实际开始时间
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? ActualStartTime { get; set; }

        /// <summary>
        /// 实际结束时间
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? ActualEndTime { get; set; }

        /// <summary>
        /// 报修人
        /// </summary>
        [Required]
        public int ReportBy { get; set; }

        /// <summary>
        /// 分配给
        /// </summary>
        public int? AssignedTo { get; set; }

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