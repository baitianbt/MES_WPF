using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 设备参数记录表
    /// </summary>
    [Table("EquipmentParameterLog")]
    public class EquipmentParameterLog
    {
        /// <summary>
        /// 参数记录唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联设备ID
        /// </summary>
        [Required]
        public int EquipmentId { get; set; }

        /// <summary>
        /// 参数代码
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string ParameterCode { get; set; }

        /// <summary>
        /// 参数名称
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string ParameterValue { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [StringLength(20)]
        [Column(TypeName = "VARCHAR")]
        public string Unit { get; set; }

        /// <summary>
        /// 采集时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 是否报警
        /// </summary>
        [Required]
        public bool IsAlarm { get; set; }

        /// <summary>
        /// 报警级别(1:提示,2:警告,3:严重)
        /// </summary>
        public byte? AlarmLevel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        [Column(TypeName = "DATETIME2")]
        public DateTime CreateTime { get; set; }
    }
}