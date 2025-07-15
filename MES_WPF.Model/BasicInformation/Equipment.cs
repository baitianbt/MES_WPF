using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES_WPF.Model.BasicInformation
{
    /// <summary>
    /// 设备信息表
    /// </summary>
    public class Equipment
    {
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 关联资源ID
        /// </summary>
        [Required]
        public int ResourceId { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        [StringLength(100)]
        public string EquipmentModel { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        [StringLength(100)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        [StringLength(50)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 购买日期
        /// </summary>
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        /// 保修期(月)
        /// </summary>
        public int? WarrantyPeriod { get; set; }

        /// <summary>
        /// 保养周期(天)
        /// </summary>
        public int? MaintenanceCycle { get; set; }

        /// <summary>
        /// 上次保养日期
        /// </summary>
        public DateTime? LastMaintenanceDate { get; set; }

        /// <summary>
        /// 下次保养日期
        /// </summary>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [StringLength(50)]
        public string IpAddress { get; set; }

        /// <summary>
        /// OPC UA端点
        /// </summary>
        [StringLength(200)]
        public string? OpcUaEndpoint { get; set; }

        // 导航属性
        [ForeignKey("ResourceId")]
        public virtual Resource Resource { get; set; }
    }
}