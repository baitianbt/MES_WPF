using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 维护执行记录表
    /// </summary>
    [Table("MaintenanceExecution")]
    public class MaintenanceExecution
    {
        /// <summary>
        /// 执行记录唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联维护工单ID
        /// </summary>
        [Required]
        public int MaintenanceOrderId { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        [Required]
        public int ExecutorId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column(TypeName = "DATETIME2")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 工时(分钟)
        /// </summary>
        [Column(TypeName = "DECIMAL(10,2)")]
        public decimal? LaborTime { get; set; }

        /// <summary>
        /// 执行结果(1:正常,2:异常)
        /// </summary>
        public byte? ExecutionResult { get; set; }

        /// <summary>
        /// 结果描述
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string ResultDescription { get; set; }

        /// <summary>
        /// 图片URL(JSON数组)
        /// </summary>
        [StringLength(1000)]
        [Column(TypeName = "NVARCHAR")]
        public string ImageUrls { get; set; }

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