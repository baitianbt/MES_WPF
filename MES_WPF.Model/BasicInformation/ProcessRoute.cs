using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 工艺路线表
    /// </summary>
    public class ProcessRoute
    {
        /// <summary>
        /// 工艺路线唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 工艺路线编码 - 需要创建唯一索引
        /// </summary>
        [Required]
        [StringLength(50)]
        public string RouteCode { get; set; }

        /// <summary>
        /// 工艺路线名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string RouteName { get; set; }

        /// <summary>
        /// 关联产品ID
        /// </summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// 版本号
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
        
        public virtual ICollection<RouteStep> RouteSteps { get; set; }
    }
}