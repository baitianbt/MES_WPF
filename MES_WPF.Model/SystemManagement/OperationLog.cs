using System;

namespace MES_WPF.Core.Models
{
    /// <summary>
    /// 操作日志表
    /// </summary>
    public class OperationLog
    {
        /// <summary>
        /// 日志唯一标识
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 操作类型(增删改查)
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// 操作描述
        /// </summary>
        public string OperationDesc { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string RequestMethod { get; set; }

        /// <summary>
        /// 请求URL
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestParams { get; set; }

        /// <summary>
        /// 响应结果
        /// </summary>
        public string ResponseResult { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationTime { get; set; }

        /// <summary>
        /// 操作用户ID
        /// </summary>
        public int OperationUser { get; set; }

        /// <summary>
        /// 操作IP
        /// </summary>
        public string OperationIp { get; set; }

        /// <summary>
        /// 执行时长(毫秒)
        /// </summary>
        public int? ExecutionTime { get; set; }

        /// <summary>
        /// 状态(1:成功,0:失败)
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }
} 