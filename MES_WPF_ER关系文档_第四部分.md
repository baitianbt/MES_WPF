 # **MES系统数据库ER关系设计文档（第四部分）**
**版本：** V1.0
**日期：** 2023年10月29日
**适用对象：** 开发团队、数据库管理员、系统架构师

---

## **八、设备管理模块**

### **1. 设备维护计划表(EquipmentMaintenancePlan)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 维护计划唯一标识 |
| planCode | VARCHAR(50) | NOT NULL, UQ | 计划编码 |
| planName | NVARCHAR(100) | NOT NULL | 计划名称 |
| equipmentId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联设备ID |
| maintenanceType | TINYINT | NOT NULL | 维护类型(1:日常保养,2:定期维护,3:预防性维护) |
| cycleType | TINYINT | NOT NULL | 周期类型(1:天,2:周,3:月,4:季度,5:年) |
| cycleValue | INT | NOT NULL | 周期值 |
| standardTime | DECIMAL(10,2) | NOT NULL | 标准工时(分钟) |
| lastExecuteDate | DATETIME2 | NULL | 上次执行日期 |
| nextExecuteDate | DATETIME2 | NULL | 下次执行日期 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_EquipmentMaintenancePlan_PlanCode (planCode)
- IX_EquipmentMaintenancePlan_EquipmentId (equipmentId)
- IX_EquipmentMaintenancePlan_MaintenanceType (maintenanceType)
- IX_EquipmentMaintenancePlan_NextExecuteDate (nextExecuteDate)
- IX_EquipmentMaintenancePlan_Status (status)

### **2. 设备维护项目表(MaintenanceItem)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 维护项目唯一标识 |
| itemCode | VARCHAR(50) | NOT NULL, UQ | 项目编码 |
| itemName | NVARCHAR(100) | NOT NULL | 项目名称 |
| maintenancePlanId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联维护计划ID |
| itemType | TINYINT | NOT NULL | 项目类型(1:检查,2:清洁,3:润滑,4:更换,5:调整) |
| standardValue | NVARCHAR(100) | NULL | 标准值 |
| upperLimit | NVARCHAR(100) | NULL | 上限 |
| lowerLimit | NVARCHAR(100) | NULL | 下限 |
| unit | VARCHAR(20) | NULL | 单位 |
| method | NVARCHAR(500) | NULL | 维护方法 |
| tool | NVARCHAR(100) | NULL | 所需工具 |
| sequenceNo | INT | NOT NULL | 序号 |
| isRequired | BIT | NOT NULL | 是否必填 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_MaintenanceItem_ItemCode (itemCode)
- IX_MaintenanceItem_MaintenancePlanId (maintenancePlanId)
- IX_MaintenanceItem_ItemType (itemType)
- IX_MaintenanceItem_SequenceNo (sequenceNo)

### **3. 维护工单表(MaintenanceOrder)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 维护工单唯一标识 |
| orderCode | VARCHAR(50) | NOT NULL, UQ | 工单编码 |
| orderType | TINYINT | NOT NULL | 工单类型(1:计划维护,2:故障维修,3:紧急维修) |
| equipmentId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联设备ID |
| maintenancePlanId | UNIQUEIDENTIFIER | FK, NULL | 关联维护计划ID(计划维护时) |
| faultDescription | NVARCHAR(500) | NULL | 故障描述(故障维修时) |
| faultCode | VARCHAR(50) | NULL | 故障代码 |
| faultLevel | TINYINT | NULL | 故障等级(1:轻微,2:一般,3:严重) |
| priority | TINYINT | NOT NULL | 优先级(1-10) |
| status | TINYINT | NOT NULL | 状态(1:待处理,2:已分配,3:处理中,4:已完成,5:已取消) |
| planStartTime | DATETIME2 | NOT NULL | 计划开始时间 |
| planEndTime | DATETIME2 | NOT NULL | 计划结束时间 |
| actualStartTime | DATETIME2 | NULL | 实际开始时间 |
| actualEndTime | DATETIME2 | NULL | 实际结束时间 |
| reportBy | UNIQUEIDENTIFIER | NOT NULL | 报修人 |
| assignedTo | UNIQUEIDENTIFIER | NULL | 分配给 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_MaintenanceOrder_OrderCode (orderCode)
- IX_MaintenanceOrder_OrderType (orderType)
- IX_MaintenanceOrder_EquipmentId (equipmentId)
- IX_MaintenanceOrder_MaintenancePlanId (maintenancePlanId)
- IX_MaintenanceOrder_Status (status)
- IX_MaintenanceOrder_Priority (priority)
- IX_MaintenanceOrder_ReportBy (reportBy)
- IX_MaintenanceOrder_AssignedTo (assignedTo)

### **4. 维护执行记录表(MaintenanceExecution)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 执行记录唯一标识 |
| maintenanceOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联维护工单ID |
| executorId | UNIQUEIDENTIFIER | FK, NOT NULL | 执行人ID |
| startTime | DATETIME2 | NOT NULL | 开始时间 |
| endTime | DATETIME2 | NULL | 结束时间 |
| laborTime | DECIMAL(10,2) | NULL | 工时(分钟) |
| executionResult | TINYINT | NULL | 执行结果(1:正常,2:异常) |
| resultDescription | NVARCHAR(500) | NULL | 结果描述 |
| imageUrls | NVARCHAR(1000) | NULL | 图片URL(JSON数组) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_MaintenanceExecution_MaintenanceOrderId (maintenanceOrderId)
- IX_MaintenanceExecution_ExecutorId (executorId)
- IX_MaintenanceExecution_StartTime (startTime)
- IX_MaintenanceExecution_EndTime (endTime)
- IX_MaintenanceExecution_ExecutionResult (executionResult)

### **5. 维护项目执行表(MaintenanceItemExecution)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 项目执行唯一标识 |
| maintenanceExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联维护执行ID |
| maintenanceItemId | UNIQUEIDENTIFIER | FK, NULL | 关联维护项目ID |
| itemName | NVARCHAR(100) | NOT NULL | 项目名称(可能是临时项目) |
| actualValue | NVARCHAR(100) | NULL | 实际值 |
| isQualified | BIT | NOT NULL | 是否合格 |
| executionTime | DATETIME2 | NOT NULL | 执行时间 |
| executorId | UNIQUEIDENTIFIER | FK, NOT NULL | 执行人ID |
| remark | NVARCHAR(500) | NULL | 备注 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |

**索引：**
- IX_MaintenanceItemExecution_MaintenanceExecutionId (maintenanceExecutionId)
- IX_MaintenanceItemExecution_MaintenanceItemId (maintenanceItemId)
- IX_MaintenanceItemExecution_IsQualified (isQualified)
- IX_MaintenanceItemExecution_ExecutorId (executorId)

### **6. 备件表(Spare)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 备件唯一标识 |
| spareCode | VARCHAR(50) | NOT NULL, UQ | 备件编码 |
| spareName | NVARCHAR(100) | NOT NULL | 备件名称 |
| spareType | TINYINT | NOT NULL | 备件类型(1:易耗品,2:维修件,3:备用件) |
| specification | NVARCHAR(200) | NULL | 规格型号 |
| unit | VARCHAR(20) | NOT NULL | 单位 |
| stockQuantity | DECIMAL(18,4) | NOT NULL | 库存数量 |
| minimumStock | DECIMAL(18,4) | NOT NULL | 最低库存 |
| price | DECIMAL(18,2) | NULL | 单价 |
| supplier | NVARCHAR(100) | NULL | 供应商 |
| leadTime | INT | NULL | 采购周期(天) |
| location | NVARCHAR(100) | NULL | 存放位置 |
| isActive | BIT | NOT NULL | 是否有效 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_Spare_SpareCode (spareCode)
- IX_Spare_SpareType (spareType)
- IX_Spare_StockQuantity (stockQuantity)
- IX_Spare_IsActive (isActive)

### **7. 备件使用记录表(SpareUsage)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 使用记录唯一标识 |
| maintenanceExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联维护执行ID |
| spareId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联备件ID |
| quantity | DECIMAL(18,4) | NOT NULL | 使用数量 |
| usageType | TINYINT | NOT NULL | 使用类型(1:更换,2:添加,3:消耗) |
| usageTime | DATETIME2 | NOT NULL | 使用时间 |
| operatorId | UNIQUEIDENTIFIER | FK, NOT NULL | 操作员ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_SpareUsage_MaintenanceExecutionId (maintenanceExecutionId)
- IX_SpareUsage_SpareId (spareId)
- IX_SpareUsage_UsageType (usageType)
- IX_SpareUsage_UsageTime (usageTime)
- IX_SpareUsage_OperatorId (operatorId)

### **8. 设备参数记录表(EquipmentParameterLog)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 参数记录唯一标识 |
| equipmentId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联设备ID |
| parameterCode | VARCHAR(50) | NOT NULL | 参数代码 |
| parameterName | NVARCHAR(100) | NOT NULL | 参数名称 |
| parameterValue | NVARCHAR(100) | NOT NULL | 参数值 |
| unit | VARCHAR(20) | NULL | 单位 |
| collectTime | DATETIME2 | NOT NULL | 采集时间 |
| isAlarm | BIT | NOT NULL | 是否报警 |
| alarmLevel | TINYINT | NULL | 报警级别(1:提示,2:警告,3:严重) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |

**索引：**
- IX_EquipmentParameterLog_EquipmentId (equipmentId)
- IX_EquipmentParameterLog_ParameterCode (parameterCode)
- IX_EquipmentParameterLog_CollectTime (collectTime)
- IX_EquipmentParameterLog_IsAlarm (isAlarm)

---

## **九、绩效分析模块**

### **1. KPI指标表(KPI)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | KPI唯一标识 |
| kpiCode | VARCHAR(50) | NOT NULL, UQ | KPI编码 |
| kpiName | NVARCHAR(100) | NOT NULL | KPI名称 |
| kpiCategory | TINYINT | NOT NULL | 指标类别(1:生产,2:质量,3:设备,4:人员) |
| calculationMethod | NVARCHAR(500) | NOT NULL | 计算方法 |
| unit | VARCHAR(20) | NULL | 单位 |
| targetValue | DECIMAL(18,4) | NULL | 目标值 |
| warningValue | DECIMAL(18,4) | NULL | 预警值 |
| dataSource | NVARCHAR(200) | NOT NULL | 数据来源 |
| frequency | TINYINT | NOT NULL | 统计频率(1:实时,2:每日,3:每周,4:每月) |
| isActive | BIT | NOT NULL | 是否有效 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_KPI_KpiCode (kpiCode)
- IX_KPI_KpiCategory (kpiCategory)
- IX_KPI_Frequency (frequency)
- IX_KPI_IsActive (isActive)

### **2. KPI数据表(KPIData)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | KPI数据唯一标识 |
| kpiId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联KPI ID |
| objectType | TINYINT | NOT NULL | 对象类型(1:全厂,2:车间,3:产线,4:工位,5:设备) |
| objectId | UNIQUEIDENTIFIER | NULL | 对象ID |
| statisticalDate | DATE | NOT NULL | 统计日期 |
| statisticalHour | INT | NULL | 统计小时(0-23) |
| actualValue | DECIMAL(18,4) | NOT NULL | 实际值 |
| targetValue | DECIMAL(18,4) | NULL | 目标值 |
| achievement | DECIMAL(10,2) | NULL | 达成率(%) |
| trend | TINYINT | NULL | 趋势(1:上升,2:持平,3:下降) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_KPIData_KpiId (kpiId)
- IX_KPIData_ObjectType_ObjectId (objectType, objectId)
- IX_KPIData_StatisticalDate (statisticalDate)
- IX_KPIData_StatisticalHour (statisticalHour)

### **3. 生产日报表(DailyProductionReport)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 日报唯一标识 |
| reportDate | DATE | NOT NULL | 报表日期 |
| workshopId | UNIQUEIDENTIFIER | FK, NULL | 车间ID |
| productionLineId | UNIQUEIDENTIFIER | FK, NULL | 产线ID |
| planQuantity | DECIMAL(18,4) | NOT NULL | 计划产量 |
| actualQuantity | DECIMAL(18,4) | NOT NULL | 实际产量 |
| qualifiedQuantity | DECIMAL(18,4) | NOT NULL | 合格产量 |
| defectQuantity | DECIMAL(18,4) | NOT NULL | 不良品数量 |
| firstPassYield | DECIMAL(10,2) | NULL | 一次合格率(%) |
| planWorkingTime | DECIMAL(10,2) | NOT NULL | 计划工作时间(分钟) |
| actualWorkingTime | DECIMAL(10,2) | NOT NULL | 实际工作时间(分钟) |
| downtime | DECIMAL(10,2) | NOT NULL | 停机时间(分钟) |
| oee | DECIMAL(10,2) | NULL | 设备综合效率(%) |
| availability | DECIMAL(10,2) | NULL | 可用率(%) |
| performance | DECIMAL(10,2) | NULL | 性能率(%) |
| quality | DECIMAL(10,2) | NULL | 质量率(%) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_DailyProductionReport_ReportDate (reportDate)
- IX_DailyProductionReport_WorkshopId (workshopId)
- IX_DailyProductionReport_ProductionLineId (productionLineId)

### **4. 停机原因分析表(DowntimeAnalysis)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 停机分析唯一标识 |
| reportDate | DATE | NOT NULL | 报表日期 |
| equipmentId | UNIQUEIDENTIFIER | FK, NOT NULL | 设备ID |
| reasonCode | VARCHAR(50) | NOT NULL | 原因代码 |
| reasonCategory | TINYINT | NOT NULL | 原因类别(1:设备故障,2:换型,3:缺料,4:缺人,5:计划停机) |
| reasonDescription | NVARCHAR(200) | NOT NULL | 原因描述 |
| duration | DECIMAL(10,2) | NOT NULL | 持续时间(分钟) |
| frequency | INT | NOT NULL | 发生频次 |
| responsibleDept | NVARCHAR(50) | NULL | 责任部门 |
| improvementMeasures | NVARCHAR(500) | NULL | 改进措施 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_DowntimeAnalysis_ReportDate (reportDate)
- IX_DowntimeAnalysis_EquipmentId (equipmentId)
- IX_DowntimeAnalysis_ReasonCode (reasonCode)
- IX_DowntimeAnalysis_ReasonCategory (reasonCategory)
- IX_DowntimeAnalysis_Duration (duration)

### **5. 质量分析表(QualityAnalysis)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 质量分析唯一标识 |
| reportDate | DATE | NOT NULL | 报表日期 |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 产品ID |
| workshopId | UNIQUEIDENTIFIER | FK, NULL | 车间ID |
| productionLineId | UNIQUEIDENTIFIER | FK, NULL | 产线ID |
| defectCode | VARCHAR(50) | NOT NULL | 缺陷代码 |
| defectCategory | TINYINT | NOT NULL | 缺陷类别(1:外观,2:功能,3:尺寸,4:其他) |
| defectDescription | NVARCHAR(200) | NOT NULL | 缺陷描述 |
| quantity | INT | NOT NULL | 数量 |
| percentage | DECIMAL(10,2) | NOT NULL | 占比(%) |
| rootCause | NVARCHAR(500) | NULL | 根本原因 |
| correctiveAction | NVARCHAR(500) | NULL | 纠正措施 |
| responsiblePerson | UNIQUEIDENTIFIER | NULL | 责任人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_QualityAnalysis_ReportDate (reportDate)
- IX_QualityAnalysis_ProductId (productId)
- IX_QualityAnalysis_WorkshopId (workshopId)
- IX_QualityAnalysis_ProductionLineId (productionLineId)
- IX_QualityAnalysis_DefectCode (defectCode)
- IX_QualityAnalysis_DefectCategory (defectCategory)
- IX_QualityAnalysis_Quantity (quantity)

### **6. 报表模板表(ReportTemplate)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 模板唯一标识 |
| templateCode | VARCHAR(50) | NOT NULL, UQ | 模板编码 |
| templateName | NVARCHAR(100) | NOT NULL | 模板名称 |
| reportType | TINYINT | NOT NULL | 报表类型(1:日报,2:周报,3:月报,4:自定义) |
| templateContent | NVARCHAR(MAX) | NOT NULL | 模板内容(JSON格式) |
| isDefault | BIT | NOT NULL | 是否默认 |
| isActive | BIT | NOT NULL | 是否有效 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_ReportTemplate_TemplateCode (templateCode)
- IX_ReportTemplate_ReportType (reportType)
- IX_ReportTemplate_IsDefault (isDefault)
- IX_ReportTemplate_IsActive (isActive)

---

## **十、ER关系图**

本文档中的实体关系可以使用以下ER图表示（简化版）：

```
┌───────────┐      ┌───────────┐      ┌───────────┐
│  产品     │◀─────┤  BOM      │─────▶│  物料     │
└───────────┘      └───────────┘      └───────────┘
      ▲                                     ▲
      │                                     │
      │                                     │
┌───────────┐      ┌───────────┐      ┌───────────┐
│ 生产订单  │─────▶│  工单     │─────▶│ 物料批次  │
└───────────┘      └───────────┘      └───────────┘
                         │                  │
                         ▼                  │
┌───────────┐      ┌───────────┐            │
│ 工艺路线  │─────▶│ 工序计划  │◀───────────┘
└───────────┘      └───────────┘
      │                  │
      ▼                  ▼
┌───────────┐      ┌───────────┐      ┌───────────┐
│  工序     │◀─────┤ 工序执行  │─────▶│  设备     │
└───────────┘      └───────────┘      └───────────┘
                         │                  │
                         ▼                  ▼
┌───────────┐      ┌───────────┐      ┌───────────┐
│ 检验计划  │─────▶│ 检验任务  │      │ 维护工单  │
└───────────┘      └───────────┘      └───────────┘
      │                  │                  │
      ▼                  ▼                  ▼
┌───────────┐      ┌───────────┐      ┌───────────┐
│ 检验项目  │─────▶│ 检验结果  │      │ 维护执行  │
└───────────┘      └───────────┘      └───────────┘
                                            │
                                            ▼
                                      ┌───────────┐
                                      │  备件     │
                                      └───────────┘
```

---

## **十一、数据库优化建议**

1. **分区策略**
   - 对历史数据量大的表（如工序执行、设备状态记录、参数记录等）采用按时间范围分区
   - 建议分区粒度：月度或季度，根据数据增长速度调整

2. **索引优化**
   - 定期分析查询性能，优化索引结构
   - 对于大表的复合索引，考虑列顺序的选择性
   - 定期维护索引碎片

3. **数据归档**
   - 设计数据归档策略，将历史数据迁移到归档表
   - 建议归档周期：生产数据保留1-2年，质量数据保留3-5年

4. **查询优化**
   - 复杂报表查询考虑使用物化视图
   - 针对常用统计分析，建立汇总表或OLAP多维数据集

5. **高可用性**
   - 采用SQL Server Always On或Oracle RAC等高可用方案
   - 实施定期备份和灾难恢复测试