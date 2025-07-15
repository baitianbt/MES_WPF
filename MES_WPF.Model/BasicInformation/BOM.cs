using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 物料清单表
    /// </summary>
    public class BOM
    {
        /// <summary>
        /// BOM唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// BOM编码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string BomCode { get; set; }

        /// <summary>
        /// BOM名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string BomName { get; set; }

        /// <summary>
        /// 关联产品ID
        /// </summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// BOM版本号
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Version { get; set; }

        /// <summary>
        /// 状态(1:草稿,2:审核中,3:已发布,4:已作废)
        /// </summary>
        [Required]
        public byte Status { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [Required]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// 是否默认版本
        /// </summary>
        [Required]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        // 导航属性
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        
        public virtual ICollection<BOMItem> BOMItems { get; set; }
    }
} 