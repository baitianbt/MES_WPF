using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.EquipmentManagement
{
    /// <summary>
    /// 备件表
    /// </summary>
    [Table("Spare")]
    public class Spare
    {
        /// <summary>
        /// 备件唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 备件编码
        /// </summary>
        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string SpareCode { get; set; }

        /// <summary>
        /// 备件名称
        /// </summary>
        [Required]
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string SpareName { get; set; }

        /// <summary>
        /// 备件类型(1:易耗品,2:维修件,3:备用件)
        /// </summary>
        [Required]
        public byte SpareType { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(200)]
        [Column(TypeName = "NVARCHAR")]
        public string Specification { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [Required]
        [StringLength(20)]
        [Column(TypeName = "VARCHAR")]
        public string Unit { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        [Required]
        [Column(TypeName = "DECIMAL(18,4)")]
        public decimal StockQuantity { get; set; }

        /// <summary>
        /// 最低库存
        /// </summary>
        [Required]
        [Column(TypeName = "DECIMAL(18,4)")]
        public decimal MinimumStock { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        [Column(TypeName = "DECIMAL(18,2)")]
        public decimal? Price { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string Supplier { get; set; }

        /// <summary>
        /// 采购周期(天)
        /// </summary>
        public int? LeadTime { get; set; }

        /// <summary>
        /// 存放位置
        /// </summary>
        [StringLength(100)]
        [Column(TypeName = "NVARCHAR")]
        public string Location { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

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