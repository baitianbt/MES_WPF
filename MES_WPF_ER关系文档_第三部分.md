 # **MES系统数据库ER关系设计文档（第三部分）**
**版本：** V1.0
**日期：** 2023年10月29日
**适用对象：** 开发团队、数据库管理员、系统架构师

---

## **六、物料与追溯模块**

### **1. 物料批次表(MaterialBatch)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 批次唯一标识 |
| batchCode | VARCHAR(50) | NOT NULL, UQ | 批次编码 |
| materialId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联物料ID |
| quantity | DECIMAL(18,4) | NOT NULL | 数量 |
| unit | VARCHAR(20) | NOT NULL | 单位 |
| status | TINYINT | NOT NULL | 状态(1:待检,2:合格,3:不合格,4:待处理,5:已用完) |
| manufactureDate | DATETIME2 | NULL | 生产日期 |
| expiryDate | DATETIME2 | NULL | 有效期 |
| supplierBatchNo | VARCHAR(50) | NULL | 供应商批次号 |
| supplierId | UNIQUEIDENTIFIER | FK, NULL | 供应商ID |
| receiveDate | DATETIME2 | NULL | 接收日期 |
| warehouseId | UNIQUEIDENTIFIER | FK, NULL | 仓库ID |
| locationId | UNIQUEIDENTIFIER | FK, NULL | 库位ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_MaterialBatch_BatchCode (batchCode)
- IX_MaterialBatch_MaterialId (materialId)
- IX_MaterialBatch_Status (status)
- IX_MaterialBatch_ManufactureDate (manufactureDate)
- IX_MaterialBatch_ExpiryDate (expiryDate)
- IX_MaterialBatch_SupplierId (supplierId)
- IX_MaterialBatch_WarehouseId_LocationId (warehouseId, locationId)

### **2. 物料消耗记录表(MaterialConsumption)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 消耗记录唯一标识 |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序执行ID |
| materialId | UNIQUEIDENTIFIER | FK, NOT NULL | 物料ID |
| batchId | UNIQUEIDENTIFIER | FK, NOT NULL | 批次ID |
| quantity | DECIMAL(18,4) | NOT NULL | 消耗数量 |
| unit | VARCHAR(20) | NOT NULL | 单位 |
| standardQuantity | DECIMAL(18,4) | NULL | 标准消耗量 |
| consumptionTime | DATETIME2 | NOT NULL | 消耗时间 |
| operatorId | UNIQUEIDENTIFIER | FK, NULL | 操作员ID |
| consumptionType | TINYINT | NOT NULL | 消耗类型(1:正常消耗,2:额外消耗,3:报废) |
| isConfirmed | BIT | NOT NULL | 是否确认 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_MaterialConsumption_OperationExecutionId (operationExecutionId)
- IX_MaterialConsumption_MaterialId (materialId)
- IX_MaterialConsumption_BatchId (batchId)
- IX_MaterialConsumption_ConsumptionTime (consumptionTime)
- IX_MaterialConsumption_OperatorId (operatorId)
- IX_MaterialConsumption_ConsumptionType (consumptionType)

### **3. 产品批次表(ProductBatch)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 产品批次唯一标识 |
| batchCode | VARCHAR(50) | NOT NULL, UQ | 批次编码 |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| workOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工单ID |
| quantity | DECIMAL(18,4) | NOT NULL | 数量 |
| unit | VARCHAR(20) | NOT NULL | 单位 |
| status | TINYINT | NOT NULL | 状态(1:生产中,2:待检,3:合格,4:不合格,5:已发货) |
| manufactureDate | DATETIME2 | NOT NULL | 生产日期 |
| warehouseId | UNIQUEIDENTIFIER | FK, NULL | 仓库ID |
| locationId | UNIQUEIDENTIFIER | FK, NULL | 库位ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_ProductBatch_BatchCode (batchCode)
- IX_ProductBatch_ProductId (productId)
- IX_ProductBatch_WorkOrderId (workOrderId)
- IX_ProductBatch_Status (status)
- IX_ProductBatch_ManufactureDate (manufactureDate)
- IX_ProductBatch_WarehouseId_LocationId (warehouseId, locationId)

### **4. 产品序列号表(SerialNumber)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 序列号唯一标识 |
| serialNumber | VARCHAR(50) | NOT NULL, UQ | 序列号 |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| productBatchId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品批次ID |
| workOrderId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工单ID |
| status | TINYINT | NOT NULL | 状态(1:生产中,2:待检,3:合格,4:不合格,5:已发货) |
| manufactureDate | DATETIME2 | NOT NULL | 生产日期 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_SerialNumber_SerialNumber (serialNumber)
- IX_SerialNumber_ProductId (productId)
- IX_SerialNumber_ProductBatchId (productBatchId)
- IX_SerialNumber_WorkOrderId (workOrderId)
- IX_SerialNumber_Status (status)
- IX_SerialNumber_ManufactureDate (manufactureDate)

### **5. 批次关系表(BatchRelationship)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 批次关系唯一标识 |
| parentBatchId | UNIQUEIDENTIFIER | FK, NOT NULL | 父批次ID |
| childBatchId | UNIQUEIDENTIFIER | FK, NOT NULL | 子批次ID |
| relationshipType | TINYINT | NOT NULL | 关系类型(1:原料-半成品,2:半成品-成品,3:成品-包装) |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序执行ID |
| quantity | DECIMAL(18,4) | NOT NULL | 使用数量 |
| unit | VARCHAR(20) | NOT NULL | 单位 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_BatchRelationship_ParentBatchId (parentBatchId)
- IX_BatchRelationship_ChildBatchId (childBatchId)
- IX_BatchRelationship_RelationshipType (relationshipType)
- IX_BatchRelationship_OperationExecutionId (operationExecutionId)

### **6. 序列号关系表(SerialRelationship)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 序列号关系唯一标识 |
| parentSerialId | UNIQUEIDENTIFIER | FK, NOT NULL | 父序列号ID |
| childSerialId | UNIQUEIDENTIFIER | FK, NOT NULL | 子序列号ID |
| relationshipType | TINYINT | NOT NULL | 关系类型(1:组装,2:包装) |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序执行ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_SerialRelationship_ParentSerialId (parentSerialId)
- IX_SerialRelationship_ChildSerialId (childSerialId)
- IX_SerialRelationship_RelationshipType (relationshipType)
- IX_SerialRelationship_OperationExecutionId (operationExecutionId)

### **7. 仓库表(Warehouse)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 仓库唯一标识 |
| warehouseCode | VARCHAR(50) | NOT NULL, UQ | 仓库编码 |
| warehouseName | NVARCHAR(100) | NOT NULL | 仓库名称 |
| warehouseType | TINYINT | NOT NULL | 仓库类型(1:原料仓,2:半成品仓,3:成品仓,4:工装仓) |
| location | NVARCHAR(200) | NULL | 位置 |
| manager | NVARCHAR(50) | NULL | 负责人 |
| status | TINYINT | NOT NULL | 状态(1:正常,2:盘点中,3:关闭) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_Warehouse_WarehouseCode (warehouseCode)
- IX_Warehouse_WarehouseType (warehouseType)
- IX_Warehouse_Status (status)

### **8. 库位表(Location)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 库位唯一标识 |
| locationCode | VARCHAR(50) | NOT NULL, UQ | 库位编码 |
| locationName | NVARCHAR(100) | NULL | 库位名称 |
| warehouseId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联仓库ID |
| locationType | TINYINT | NOT NULL | 库位类型(1:存储,2:拣货,3:暂存,4:不良品) |
| status | TINYINT | NOT NULL | 状态(1:空闲,2:占用,3:锁定,4:禁用) |
| capacity | DECIMAL(10,2) | NULL | 容量 |
| capacityUnit | VARCHAR(20) | NULL | 容量单位 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Location_LocationCode (locationCode)
- IX_Location_WarehouseId (warehouseId)
- IX_Location_LocationType (locationType)
- IX_Location_Status (status)

---

## **七、质量管理模块**

### **1. 检验计划表(InspectionPlan)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 检验计划唯一标识 |
| planCode | VARCHAR(50) | NOT NULL, UQ | 计划编码 |
| planName | NVARCHAR(100) | NOT NULL | 计划名称 |
| inspectionType | TINYINT | NOT NULL | 检验类型(1:首检,2:巡检,3:末检,4:全检) |
| objectType | TINYINT | NOT NULL | 检验对象类型(1:原材料,2:半成品,3:成品) |
| objectId | UNIQUEIDENTIFIER | FK, NOT NULL | 检验对象ID(产品/物料) |
| triggerType | TINYINT | NOT NULL | 触发类型(1:时间,2:数量,3:工序,4:手动) |
| triggerValue | NVARCHAR(100) | NULL | 触发值 |
| samplingType | TINYINT | NOT NULL | 抽样方式(1:固定数量,2:百分比,3:全检) |
| samplingValue | NVARCHAR(50) | NULL | 抽样值 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_InspectionPlan_PlanCode (planCode)
- IX_InspectionPlan_InspectionType (inspectionType)
- IX_InspectionPlan_ObjectType_ObjectId (objectType, objectId)
- IX_InspectionPlan_TriggerType (triggerType)
- IX_InspectionPlan_Status (status)

### **2. 检验项目表(InspectionItem)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 检验项目唯一标识 |
| itemCode | VARCHAR(50) | NOT NULL, UQ | 项目编码 |
| itemName | NVARCHAR(100) | NOT NULL | 项目名称 |
| inspectionPlanId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联检验计划ID |
| inspectionMethod | NVARCHAR(500) | NULL | 检验方法 |
| inspectionTool | NVARCHAR(100) | NULL | 检验工具 |
| valueType | TINYINT | NOT NULL | 值类型(1:数值,2:文本,3:是否,4:枚举) |
| unit | VARCHAR(20) | NULL | 单位 |
| standardValue | NVARCHAR(100) | NULL | 标准值 |
| upperLimit | NVARCHAR(100) | NULL | 上限 |
| lowerLimit | NVARCHAR(100) | NULL | 下限 |
| precision | INT | NULL | 精度(小数位数) |
| isRequired | BIT | NOT NULL | 是否必填 |
| sequenceNo | INT | NOT NULL | 序号 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_InspectionItem_ItemCode (itemCode)
- IX_InspectionItem_InspectionPlanId (inspectionPlanId)
- IX_InspectionItem_ValueType (valueType)
- IX_InspectionItem_SequenceNo (sequenceNo)

### **3. 检验任务表(InspectionTask)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 检验任务唯一标识 |
| taskCode | VARCHAR(50) | NOT NULL, UQ | 任务编码 |
| inspectionPlanId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联检验计划ID |
| objectType | TINYINT | NOT NULL | 检验对象类型(1:原材料批次,2:半成品批次,3:成品批次,4:工序) |
| objectId | UNIQUEIDENTIFIER | FK, NOT NULL | 检验对象ID |
| workOrderId | UNIQUEIDENTIFIER | FK, NULL | 关联工单ID |
| operationExecutionId | UNIQUEIDENTIFIER | FK, NULL | 关联工序执行ID |
| status | TINYINT | NOT NULL | 状态(1:待检验,2:检验中,3:已完成,4:已取消) |
| priority | TINYINT | NOT NULL DEFAULT 5 | 优先级(1-10) |
| planStartTime | DATETIME2 | NOT NULL | 计划开始时间 |
| planEndTime | DATETIME2 | NOT NULL | 计划结束时间 |
| actualStartTime | DATETIME2 | NULL | 实际开始时间 |
| actualEndTime | DATETIME2 | NULL | 实际结束时间 |
| inspectorId | UNIQUEIDENTIFIER | FK, NULL | 检验员ID |
| result | TINYINT | NULL | 检验结果(1:合格,2:不合格,3:让步接收) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_InspectionTask_TaskCode (taskCode)
- IX_InspectionTask_InspectionPlanId (inspectionPlanId)
- IX_InspectionTask_ObjectType_ObjectId (objectType, objectId)
- IX_InspectionTask_WorkOrderId (workOrderId)
- IX_InspectionTask_OperationExecutionId (operationExecutionId)
- IX_InspectionTask_Status (status)
- IX_InspectionTask_InspectorId (inspectorId)
- IX_InspectionTask_Result (result)

### **4. 检验结果表(InspectionResult)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 检验结果唯一标识 |
| inspectionTaskId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联检验任务ID |
| inspectionItemId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联检验项目ID |
| actualValue | NVARCHAR(100) | NOT NULL | 实际值 |
| isQualified | BIT | NOT NULL | 是否合格 |
| inspectorId | UNIQUEIDENTIFIER | FK, NOT NULL | 检验员ID |
| inspectionTime | DATETIME2 | NOT NULL | 检验时间 |
| imageUrls | NVARCHAR(1000) | NULL | 图片URL(JSON数组) |
| remark | NVARCHAR(500) | NULL | 备注 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_InspectionResult_InspectionTaskId (inspectionTaskId)
- IX_InspectionResult_InspectionItemId (inspectionItemId)
- IX_InspectionResult_IsQualified (isQualified)
- IX_InspectionResult_InspectorId (inspectorId)
- IX_InspectionResult_InspectionTime (inspectionTime)

### **5. SPC控制图表(SPCControlChart)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 控制图唯一标识 |
| chartCode | VARCHAR(50) | NOT NULL, UQ | 控制图编码 |
| chartName | NVARCHAR(100) | NOT NULL | 控制图名称 |
| chartType | TINYINT | NOT NULL | 图表类型(1:X-R图,2:X-S图,3:p图,4:np图,5:c图,6:u图) |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| inspectionItemId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联检验项目ID |
| sampleSize | INT | NOT NULL | 样本大小 |
| sampleFrequency | NVARCHAR(100) | NOT NULL | 抽样频率 |
| ucl | DECIMAL(18,6) | NULL | 上控制限 |
| lcl | DECIMAL(18,6) | NULL | 下控制限 |
| cl | DECIMAL(18,6) | NULL | 中心线 |
| usl | DECIMAL(18,6) | NULL | 上规格限 |
| lsl | DECIMAL(18,6) | NULL | 下规格限 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(500) | NULL | 备注 |

**索引：**
- IX_SPCControlChart_ChartCode (chartCode)
- IX_SPCControlChart_ChartType (chartType)
- IX_SPCControlChart_ProductId (productId)
- IX_SPCControlChart_InspectionItemId (inspectionItemId)
- IX_SPCControlChart_Status (status)

### **6. SPC数据点表(SPCDataPoint)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 数据点唯一标识 |
| chartId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联控制图ID |
| subgroupNo | INT | NOT NULL | 子组号 |
| sampleTime | DATETIME2 | NOT NULL | 采样时间 |
| sampleValue | DECIMAL(18,6) | NOT NULL | 样本值 |
| sampleIndex | INT | NOT NULL | 样本索引 |
| operatorId | UNIQUEIDENTIFIER | FK, NULL | 操作员ID |
| isOutOfControl | BIT | NOT NULL | 是否失控 |
| outOfControlReason | NVARCHAR(200) | NULL | 失控原因 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_SPCDataPoint_ChartId (chartId)
- IX_SPCDataPoint_SubgroupNo (subgroupNo)
- IX_SPCDataPoint_SampleTime (sampleTime)
- IX_SPCDataPoint_IsOutOfControl (isOutOfControl)