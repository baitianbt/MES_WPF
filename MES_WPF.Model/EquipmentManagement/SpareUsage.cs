using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 备件使用记录表
    /// </summary>
    [Table("SpareUsage")]
    public class SpareUsage
    {
        /// <summary>
        /// 使用记录唯一标识
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
        /// 关联备件ID
        /// </summary>
        [Required]
        public int SpareId { get; set; }

        /// <summary>
        /// 使用数量
        /// </summary>
        [Required]
        [Column(TypeName = "DECIMAL(18,4)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 使用类型(1:更换,2:添加,3:消耗)
        /// </summary>
        [Required]
        public byte UsageType { get; set; }

        /// <summary>
        /// 使用时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime UsageTime { get; set; }

        /// <summary>
        /// 操作员ID
        /// </summary>
        [Required]
        public int OperatorId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200)]
        [Column(TypeName = "NVARCHAR")]
        public string Remark { get; set; }
    }
}