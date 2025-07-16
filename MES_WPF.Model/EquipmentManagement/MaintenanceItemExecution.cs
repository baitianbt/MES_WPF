using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 维护项目执行表
    /// </summary>
    [Table("MaintenanceItemExecution")]
    public class MaintenanceItemExecution
    {
        /// <summary>
        /// 项目执行唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联维护执行ID
        /// </summary>
        [Required]
        public int MaintenanceExecutionId { get; set; }

        /// <summary>
        /// 关联维护项目ID
        /// </summary>
        public int? MaintenanceItemId { get; set; }

        /// <summary>
        /// 项目名称(可能是临时项目)
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string ItemName { get; set; }

        /// <summary>
        /// 实际值
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string ActualValue { get; set; }

        /// <summary>
        /// 是否合格
        /// </summary>
        [Required]
        public bool IsQualified { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// 执行人ID
        /// </summary>
        [Required]
        public int ExecutorId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime CreateTime { get; set; }
    }
}