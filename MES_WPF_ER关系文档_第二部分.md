 # **MES系统数据库ER关系设计文档（第二部分）**
**版本：** V1.0
**日期：** 2023年10月29日
**适用对象：** 开发团队、数据库管理员、系统架构师

---

## **四、生产计划与调度模块**

### **1. 生产订单表(ProductionOrder)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 生产订单唯一标识 |
| orderCode | VARCHAR(50) | NOT NULL, UQ | 订单编码 |
| orderType | TINYINT | NOT NULL | 订单类型(1:正常,2:返修,3:试制) |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| planQuantity | DECIMAL(18,4) | NOT NULL | 计划数量 |
| actualQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 实际数量 |
| unitId | VARCHAR(20) | NOT NULL | 单位 |
| priority | TINYINT | NOT NULL DEFAULT 5 | 优先级(1-10) |
| status | TINYINT | NOT NULL | 状态(1:未下达,2:已下达,3:生产中,4:已完成,5:已关闭) |
| planStartDate | DATETIME2 | NOT NULL | 计划开始日期 |
| planEndDate | DATETIME2 | NOT NULL | 计划结束日期 |
| actualStartDate | DATETIME2 | NULL | 实际开始日期 |
| actualEndDate | DATETIME2 | NULL | 实际结束日期 |
| sourceType | TINYINT | NULL | 来源类型(1:ERP,2:手工创建) |
| sourceId | VARCHAR(50) | NULL | 来源单据ID |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_ProductionOrder_OrderCode (orderCode)
- IX_ProductionOrder_ProductId (productId)
- IX_ProductionOrder_Status (status)
- IX_ProductionOrder_PlanStartDate (planStartDate)
- IX_ProductionOrder_PlanEndDate (planEndDate)
- IX_ProductionOrder_Priority (priority)

### **2. 工单表(WorkOrder)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工单唯一标识 |
| workOrderCode | VARCHAR(50) | NOT NULL, UQ | 工单编码 |
| productionOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联生产订单ID |
| routeId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工艺路线ID |
| planQuantity | DECIMAL(18,4) | NOT NULL | 计划数量 |
| actualQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 实际数量 |
| status | TINYINT | NOT NULL | 状态(1:未下达,2:已下达,3:生产中,4:已完成,5:已关闭) |
| planStartDate | DATETIME2 | NOT NULL | 计划开始日期 |
| planEndDate | DATETIME2 | NOT NULL | 计划结束日期 |
| actualStartDate | DATETIME2 | NULL | 实际开始日期 |
| actualEndDate | DATETIME2 | NULL | 实际结束日期 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_WorkOrder_WorkOrderCode (workOrderCode)
- IX_WorkOrder_ProductionOrderId (productionOrderId)
- IX_WorkOrder_RouteId (routeId)
- IX_WorkOrder_Status (status)
- IX_WorkOrder_PlanStartDate (planStartDate)
- IX_WorkOrder_PlanEndDate (planEndDate)

### **3. 工序计划表(OperationPlan)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工序计划唯一标识 |
| workOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工单ID |
| routeStepId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工艺步骤ID |
| operationId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序ID |
| sequenceNo | INT | NOT NULL | 序号 |
| workstationId | UNIQUEIDENTIFIER | FK, NULL | 关联工作站ID |
| planQuantity | DECIMAL(18,4) | NOT NULL | 计划数量 |
| completedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 已完成数量 |
| scrappedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 报废数量 |
| status | TINYINT | NOT NULL | 状态(1:未开始,2:进行中,3:已完成,4:已跳过) |
| planStartTime | DATETIME2 | NOT NULL | 计划开始时间 |
| planEndTime | DATETIME2 | NOT NULL | 计划结束时间 |
| actualStartTime | DATETIME2 | NULL | 实际开始时间 |
| actualEndTime | DATETIME2 | NULL | 实际结束时间 |
| planSetupTime | DECIMAL(10,2) | NOT NULL | 计划准备时间(分钟) |
| planProcessTime | DECIMAL(10,2) | NOT NULL | 计划加工时间(分钟) |
| actualSetupTime | DECIMAL(10,2) | NULL | 实际准备时间(分钟) |
| actualProcessTime | DECIMAL(10,2) | NULL | 实际加工时间(分钟) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_OperationPlan_WorkOrderId (workOrderId)
- IX_OperationPlan_RouteStepId (routeStepId)
- IX_OperationPlan_OperationId (operationId)
- IX_OperationPlan_WorkstationId (workstationId)
- IX_OperationPlan_Status (status)
- IX_OperationPlan_PlanStartTime (planStartTime)
- IX_OperationPlan_PlanEndTime (planEndTime)

### **4. 资源排程表(ResourceSchedule)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 排程唯一标识 |
| resourceId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联资源ID |
| operationPlanId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序计划ID |
| startTime | DATETIME2 | NOT NULL | 开始时间 |
| endTime | DATETIME2 | NOT NULL | 结束时间 |
| status | TINYINT | NOT NULL | 状态(1:计划,2:已确认,3:已完成,4:已取消) |
| scheduleType | TINYINT | NOT NULL | 排程类型(1:自动排程,2:手动排程,3:拖拽调整) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_ResourceSchedule_ResourceId (resourceId)
- IX_ResourceSchedule_OperationPlanId (operationPlanId)
- IX_ResourceSchedule_StartTime (startTime)
- IX_ResourceSchedule_EndTime (endTime)
- IX_ResourceSchedule_Status (status)

### **5. 班次表(Shift)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 班次唯一标识 |
| shiftCode | VARCHAR(50) | NOT NULL, UQ | 班次编码 |
| shiftName | NVARCHAR(100) | NOT NULL | 班次名称 |
| startTime | TIME | NOT NULL | 开始时间 |
| endTime | TIME | NOT NULL | 结束时间 |
| isNextDay | BIT | NOT NULL | 是否跨天 |
| isActive | BIT | NOT NULL | 是否有效 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_Shift_ShiftCode (shiftCode)

### **6. 工作日历表(WorkCalendar)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工作日历唯一标识 |
| calendarDate | DATE | NOT NULL | 日期 |
| isWorkDay | BIT | NOT NULL | 是否工作日 |
| dayType | TINYINT | NOT NULL | 日类型(1:工作日,2:周末,3:节假日) |
| remark | NVARCHAR(200) | NULL | 备注 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_WorkCalendar_CalendarDate (calendarDate)
- IX_WorkCalendar_IsWorkDay (isWorkDay)

### **7. 资源日历表(ResourceCalendar)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 资源日历唯一标识 |
| resourceId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联资源ID |
| calendarDate | DATE | NOT NULL | 日期 |
| shiftId | UNIQUEIDENTIFIER | FK, NULL | 关联班次ID |
| isAvailable | BIT | NOT NULL | 是否可用 |
| unavailableReason | TINYINT | NULL | 不可用原因(1:维修,2:保养,3:休息,4:其他) |
| remark | NVARCHAR(200) | NULL | 备注 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_ResourceCalendar_ResourceId_CalendarDate (resourceId, calendarDate)
- IX_ResourceCalendar_ShiftId (shiftId)
- IX_ResourceCalendar_IsAvailable (isAvailable)

---

## **五、生产执行模块**

### **1. 工单执行表(WorkOrderExecution)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工单执行唯一标识 |
| workOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工单ID |
| executionStatus | TINYINT | NOT NULL | 执行状态(1:未开始,2:暂停,3:执行中,4:已完成) |
| startTime | DATETIME2 | NULL | 开始时间 |
| endTime | DATETIME2 | NULL | 结束时间 |
| executedBy | UNIQUEIDENTIFIER | NULL | 执行人 |
| supervisorId | UNIQUEIDENTIFIER | NULL | 主管ID |
| completedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 完成数量 |
| scrappedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 报废数量 |
| reworkQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 返工数量 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_WorkOrderExecution_WorkOrderId (workOrderId)
- IX_WorkOrderExecution_ExecutionStatus (executionStatus)
- IX_WorkOrderExecution_StartTime (startTime)
- IX_WorkOrderExecution_EndTime (endTime)
- IX_WorkOrderExecution_ExecutedBy (executedBy)

### **2. 工序执行表(OperationExecution)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工序执行唯一标识 |
| operationPlanId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序计划ID |
| workstationId | UNIQUEIDENTIFIER | FK, NOT NULL | 实际工作站ID |
| operatorId | UNIQUEIDENTIFIER | FK, NULL | 操作员ID |
| startTime | DATETIME2 | NULL | 开始时间 |
| endTime | DATETIME2 | NULL | 结束时间 |
| setupStartTime | DATETIME2 | NULL | 准备开始时间 |
| setupEndTime | DATETIME2 | NULL | 准备结束时间 |
| status | TINYINT | NOT NULL | 状态(1:未开始,2:准备中,3:加工中,4:已完成,5:已中断) |
| inputQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 投入数量 |
| outputQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 产出数量 |
| scrappedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 报废数量 |
| reworkQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 返工数量 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_OperationExecution_OperationPlanId (operationPlanId)
- IX_OperationExecution_WorkstationId (workstationId)
- IX_OperationExecution_OperatorId (operatorId)
- IX_OperationExecution_Status (status)
- IX_OperationExecution_StartTime (startTime)
- IX_OperationExecution_EndTime (endTime)

### **3. 生产报工表(ProductionReport)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 报工唯一标识 |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序执行ID |
| reportType | TINYINT | NOT NULL | 报工类型(1:正常报工,2:异常报工,3:返修报工) |
| reportTime | DATETIME2 | NOT NULL | 报工时间 |
| reportBy | UNIQUEIDENTIFIER | NOT NULL | 报工人 |
| goodQuantity | DECIMAL(18,4) | NOT NULL | 良品数量 |
| defectQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 不良品数量 |
| reworkQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 返工数量 |
| scrappedQuantity | DECIMAL(18,4) | NOT NULL DEFAULT 0 | 报废数量 |
| startTime | DATETIME2 | NOT NULL | 开始时间 |
| endTime | DATETIME2 | NOT NULL | 结束时间 |
| laborTime | DECIMAL(10,2) | NOT NULL | 工时(分钟) |
| isConfirmed | BIT | NOT NULL DEFAULT 0 | 是否已确认 |
| confirmedBy | UNIQUEIDENTIFIER | NULL | 确认人 |
| confirmedTime | DATETIME2 | NULL | 确认时间 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_ProductionReport_OperationExecutionId (operationExecutionId)
- IX_ProductionReport_ReportType (reportType)
- IX_ProductionReport_ReportTime (reportTime)
- IX_ProductionReport_ReportBy (reportBy)
- IX_ProductionReport_IsConfirmed (isConfirmed)

### **4. 不良品记录表(DefectRecord)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 不良品记录唯一标识 |
| productionReportId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联报工ID |
| defectCodeId | UNIQUEIDENTIFIER | FK, NOT NULL | 不良代码ID |
| defectQuantity | DECIMAL(18,4) | NOT NULL | 不良数量 |
| defectLevel | TINYINT | NOT NULL | 不良等级(1:轻微,2:一般,3:严重) |
| causeAnalysis | NVARCHAR(500) | NULL | 原因分析 |
| isReworkable | BIT | NOT NULL | 是否可返工 |
| imageUrls | NVARCHAR(1000) | NULL | 图片URL(JSON数组) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_DefectRecord_ProductionReportId (productionReportId)
- IX_DefectRecord_DefectCodeId (defectCodeId)
- IX_DefectRecord_DefectLevel (defectLevel)
- IX_DefectRecord_IsReworkable (isReworkable)

### **5. 不良代码表(DefectCode)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 不良代码唯一标识 |
| defectCode | VARCHAR(50) | NOT NULL, UQ | 不良代码 |
| defectName | NVARCHAR(100) | NOT NULL | 不良名称 |
| defectType | TINYINT | NOT NULL | 不良类型(1:外观,2:功能,3:尺寸,4:其他) |
| defectCategory | TINYINT | NOT NULL | 不良类别(1:原材料,2:加工,3:装配,4:其他) |
| defaultLevel | TINYINT | NOT NULL | 默认等级(1:轻微,2:一般,3:严重) |
| description | NVARCHAR(500) | NULL | 描述 |
| possibleCauses | NVARCHAR(500) | NULL | 可能原因 |
| suggestedActions | NVARCHAR(500) | NULL | 建议措施 |
| isActive | BIT | NOT NULL | 是否有效 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_DefectCode_DefectCode (defectCode)
- IX_DefectCode_DefectType (defectType)
- IX_DefectCode_DefectCategory (defectCategory)

### **6. 设备状态记录表(EquipmentStatusLog)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 设备状态记录唯一标识 |
| equipmentId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联设备ID |
| statusCode | TINYINT | NOT NULL | 状态代码(1:运行,2:待机,3:故障,4:维修,5:关机) |
| startTime | DATETIME2 | NOT NULL | 开始时间 |
| endTime | DATETIME2 | NULL | 结束时间 |
| duration | DECIMAL(18,2) | NULL | 持续时间(分钟) |
| operatorId | UNIQUEIDENTIFIER | NULL | 操作员ID |
| reasonCode | VARCHAR(50) | NULL | 原因代码 |
| reasonDescription | NVARCHAR(500) | NULL | 原因描述 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_EquipmentStatusLog_EquipmentId (equipmentId)
- IX_EquipmentStatusLog_StatusCode (statusCode)
- IX_EquipmentStatusLog_StartTime (startTime)
- IX_EquipmentStatusLog_EndTime (endTime)
- IX_EquipmentStatusLog_ReasonCode (reasonCode)

### **7. 工艺参数记录表(ProcessParameterLog)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工艺参数记录唯一标识 |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序执行ID |
| parameterId | UNIQUEIDENTIFIER | FK, NOT NULL | 参数ID |
| parameterName | NVARCHAR(100) | NOT NULL | 参数名称 |
| parameterValue | NVARCHAR(100) | NOT NULL | 参数值 |
| standardValue | NVARCHAR(100) | NULL | 标准值 |
| lowerLimit | NVARCHAR(100) | NULL | 下限 |
| upperLimit | NVARCHAR(100) | NULL | 上限 |
| unit | VARCHAR(20) | NULL | 单位 |
| isQualified | BIT | NOT NULL | 是否合格 |
| collectTime | DATETIME2 | NOT NULL | 采集时间 |
| collectMethod | TINYINT | NOT NULL | 采集方式(1:自动,2:手动) |
| operatorId | UNIQUEIDENTIFIER | NULL | 操作员ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |

**索引：**
- IX_ProcessParameterLog_OperationExecutionId (operationExecutionId)
- IX_ProcessParameterLog_ParameterId (parameterId)
- IX_ProcessParameterLog_CollectTime (collectTime)
- IX_ProcessParameterLog_IsQualified (isQualified)