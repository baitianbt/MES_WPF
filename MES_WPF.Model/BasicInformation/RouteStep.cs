using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 工艺路线明细表/工艺步骤
    /// </summary>
    public class RouteStep
    {
        /// <summary>
        /// 工艺步骤唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联工艺路线ID
        /// </summary>
        [Required]
        public int RouteId { get; set; }

        /// <summary>
        /// 关联工序ID
        /// </summary>
        [Required]
        public int OperationId { get; set; }

        /// <summary>
        /// 步骤序号
        /// </summary>
        [Required]
        public int StepNo { get; set; }

        /// <summary>
        /// 工作站类型ID
        /// </summary>
        public int? WorkstationTypeId { get; set; }

        /// <summary>
        /// 准备时间(分钟)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SetupTime { get; set; }

        /// <summary>
        /// 加工时间(分钟)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ProcessTime { get; set; }

        /// <summary>
        /// 等待时间(分钟)
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal WaitTime { get; set; }

        /// <summary>
        /// 步骤描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// 是否关键工序
        /// </summary>
        [Required]
        public bool IsKeyOperation { get; set; }

        /// <summary>
        /// 是否质检点
        /// </summary>
        [Required]
        public bool IsQualityCheckPoint { get; set; }

        // 导航属性
        [ForeignKey("RouteId")]
        public virtual ProcessRoute ProcessRoute { get; set; }

        [ForeignKey("OperationId")]
        public virtual Operation Operation { get; set; }
    }
} 