using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 工序表
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// 工序唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 工序编码 - 需要创建唯一索引
        /// </summary>
        [Required]
        [StringLength(50)]
        public string OperationCode { get; set; }

        /// <summary>
        /// 工序名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string OperationName { get; set; }

        /// <summary>
        /// 工序类型(1:加工,2:检验,3:搬运)
        /// </summary>
        [Required]
        public byte OperationType { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [StringLength(50)]
        public string Department { get; set; }

        /// <summary>
        /// 工序描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 标准工时(分钟)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal StandardTime { get; set; }

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
        public virtual ICollection<RouteStep> RouteSteps { get; set; }
    }
}