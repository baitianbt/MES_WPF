using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 产品信息表
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 产品唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        /// <summary>
        /// 产品类型(1:成品,2:半成品,3:原材料)
        /// </summary>
        [Required]
        public byte ProductType { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        [StringLength(200)]
        public string Specification { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Unit { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

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
        public virtual ICollection<BOM> BOMs { get; set; }
        public virtual ICollection<ProcessRoute> ProcessRoutes { get; set; }
    }
}