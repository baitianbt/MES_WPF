using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 物料清单明细表
    /// </summary>
    public class BOMItem
    {
        /// <summary>
        /// BOM明细唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联BOM ID
        /// </summary>
        [Required]
        public int BomId { get; set; }

        /// <summary>
        /// 物料ID (关联的产品ID)
        /// </summary>
        [Required]
        public int MaterialId { get; set; }

        /// <summary>
        /// 用量
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [Required]
        [StringLength(20)]
        public string UnitId { get; set; }

        /// <summary>
        /// 位置信息
        /// </summary>
        [StringLength(100)]
        public string Position { get; set; }

        /// <summary>
        /// 是否关键物料
        /// </summary>
        [Required]
        public bool IsKey { get; set; }

        /// <summary>
        /// 损耗率(%)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal LossRate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(200)]
        public string Remark { get; set; }

        // 导航属性
        [ForeignKey("BomId")]
        public virtual BOM BOM { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Product Material { get; set; }
    }
}