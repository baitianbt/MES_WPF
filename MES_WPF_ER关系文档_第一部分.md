 # **MES系统数据库ER关系设计文档**
**版本：** V1.0
**日期：** 2023年10月29日
**适用对象：** 开发团队、数据库管理员、系统架构师

---

## **一、文档概述**

### **1. 目的**

本文档详细描述MES系统的实体关系(ER)模型设计，为系统开发提供数据库结构参考，确保数据模型能够满足需求文档中的所有功能性和非功能性需求。

### **2. 范围**

- 覆盖MES系统所有核心业务模块的数据实体设计
- 定义实体间的关系和约束
- 规范数据字段命名和类型
- 提供索引优化建议

### **3. 参考文档**

- MES系统需求规格说明书 V1.0
- MES系统架构设计文档 V1.0
- 数据库设计规范 V2.0

---

## **二、数据库设计原则**

### **1. 命名规范**

- **表命名**：使用Pascal命名法，如`ProductionOrder`
- **字段命名**：使用Camel命名法，如`orderCode`
- **主键命名**：统一使用`id`作为主键名
- **外键命名**：使用`{关联表名}Id`格式，如`productId`

### **2. 数据类型规范**

- **标识符**：使用GUID或自增长整数
- **编码类型**：使用VARCHAR(50)
- **名称类型**：使用NVARCHAR(100)
- **描述类型**：使用NVARCHAR(500)
- **日期时间**：使用DATETIME2
- **状态标识**：使用TINYINT或枚举类型
- **金额类型**：使用DECIMAL(18,2)
- **数量类型**：使用DECIMAL(18,4)

### **3. 索引设计原则**

- 所有外键字段创建索引
- 常用查询条件字段创建索引
- 避免过度索引，控制在每表5-8个索引以内
- 对大表使用分区索引策略

### **4. 约束设计**

- 所有表必须有主键
- 关键业务字段添加NOT NULL约束
- 使用外键约束保证数据完整性
- 使用CHECK约束保证数据有效性

---

## **三、核心实体设计**

### **1. 基础信息模块**

#### **1.1 产品信息表(Product)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 产品唯一标识 |
| productCode | VARCHAR(50) | NOT NULL, UQ | 产品编码 |
| productName | NVARCHAR(100) | NOT NULL | 产品名称 |
| productType | TINYINT | NOT NULL | 产品类型(1:成品,2:半成品,3:原材料) |
| specification | NVARCHAR(200) | NULL | 规格型号 |
| unit | VARCHAR(20) | NOT NULL | 计量单位 |
| description | NVARCHAR(500) | NULL | 产品描述 |
| isActive | BIT | NOT NULL | 是否有效 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_Product_ProductCode (productCode)
- IX_Product_ProductName (productName)
- IX_Product_ProductType (productType)

#### **1.2 物料清单表(BOM)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | BOM唯一标识 |
| bomCode | VARCHAR(50) | NOT NULL, UQ | BOM编码 |
| bomName | NVARCHAR(100) | NOT NULL | BOM名称 |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| version | VARCHAR(20) | NOT NULL | BOM版本号 |
| status | TINYINT | NOT NULL | 状态(1:草稿,2:审核中,3:已发布,4:已作废) |
| effectiveDate | DATETIME2 | NOT NULL | 生效日期 |
| expiryDate | DATETIME2 | NULL | 失效日期 |
| isDefault | BIT | NOT NULL | 是否默认版本 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_BOM_ProductId (productId)
- IX_BOM_BomCode (bomCode)
- IX_BOM_Status (status)

#### **1.3 物料清单明细表(BOMItem)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | BOM明细唯一标识 |
| bomId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联BOM ID |
| materialId | UNIQUEIDENTIFIER | FK, NOT NULL | 物料ID |
| quantity | DECIMAL(18,4) | NOT NULL | 用量 |
| unitId | VARCHAR(20) | NOT NULL | 单位 |
| position | NVARCHAR(100) | NULL | 位置信息 |
| isKey | BIT | NOT NULL | 是否关键物料 |
| lossRate | DECIMAL(5,2) | NOT NULL | 损耗率(%) |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_BOMItem_BomId (bomId)
- IX_BOMItem_MaterialId (materialId)

#### **1.4 工艺路线表(ProcessRoute)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工艺路线唯一标识 |
| routeCode | VARCHAR(50) | NOT NULL, UQ | 工艺路线编码 |
| routeName | NVARCHAR(100) | NOT NULL | 工艺路线名称 |
| productId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联产品ID |
| version | VARCHAR(20) | NOT NULL | 版本号 |
| status | TINYINT | NOT NULL | 状态(1:草稿,2:审核中,3:已发布,4:已作废) |
| isDefault | BIT | NOT NULL | 是否默认版本 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_ProcessRoute_ProductId (productId)
- IX_ProcessRoute_RouteCode (routeCode)
- IX_ProcessRoute_Status (status)

#### **1.5 工序表(Operation)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工序唯一标识 |
| operationCode | VARCHAR(50) | NOT NULL, UQ | 工序编码 |
| operationName | NVARCHAR(100) | NOT NULL | 工序名称 |
| operationType | TINYINT | NOT NULL | 工序类型(1:加工,2:检验,3:搬运) |
| department | NVARCHAR(50) | NULL | 所属部门 |
| description | NVARCHAR(500) | NULL | 工序描述 |
| standardTime | DECIMAL(10,2) | NOT NULL | 标准工时(分钟) |
| isActive | BIT | NOT NULL | 是否有效 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_Operation_OperationCode (operationCode)
- IX_Operation_OperationType (operationType)

#### **1.6 工艺路线明细表(RouteStep)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 工艺步骤唯一标识 |
| routeId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工艺路线ID |
| operationId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联工序ID |
| stepNo | INT | NOT NULL | 步骤序号 |
| workstationTypeId | UNIQUEIDENTIFIER | FK, NULL | 工作站类型ID |
| setupTime | DECIMAL(10,2) | NOT NULL | 准备时间(分钟) |
| processTime | DECIMAL(10,2) | NOT NULL | 加工时间(分钟) |
| waitTime | DECIMAL(10,2) | NOT NULL | 等待时间(分钟) |
| description | NVARCHAR(500) | NULL | 步骤描述 |
| isKeyOperation | BIT | NOT NULL | 是否关键工序 |
| isQualityCheckPoint | BIT | NOT NULL | 是否质检点 |

**索引：**
- IX_RouteStep_RouteId (routeId)
- IX_RouteStep_OperationId (operationId)
- IX_RouteStep_WorkstationTypeId (workstationTypeId)

#### **1.7 资源信息表(Resource)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 资源唯一标识 |
| resourceCode | VARCHAR(50) | NOT NULL, UQ | 资源编码 |
| resourceName | NVARCHAR(100) | NOT NULL | 资源名称 |
| resourceType | TINYINT | NOT NULL | 资源类型(1:设备,2:人员,3:工装) |
| departmentId | UNIQUEIDENTIFIER | FK, NULL | 所属部门ID |
| status | TINYINT | NOT NULL | 状态(1:可用,2:占用,3:故障,4:维修中) |
| description | NVARCHAR(500) | NULL | 资源描述 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |

**索引：**
- IX_Resource_ResourceCode (resourceCode)
- IX_Resource_ResourceType (resourceType)
- IX_Resource_DepartmentId (departmentId)
- IX_Resource_Status (status)

#### **1.8 设备信息表(Equipment)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 设备唯一标识 |
| resourceId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联资源ID |
| equipmentModel | NVARCHAR(100) | NULL | 设备型号 |
| manufacturer | NVARCHAR(100) | NULL | 制造商 |
| serialNumber | VARCHAR(50) | NULL | 序列号 |
| purchaseDate | DATETIME2 | NULL | 购买日期 |
| warrantyPeriod | INT | NULL | 保修期(月) |
| maintenanceCycle | INT | NULL | 保养周期(天) |
| lastMaintenanceDate | DATETIME2 | NULL | 上次保养日期 |
| nextMaintenanceDate | DATETIME2 | NULL | 下次保养日期 |
| ipAddress | VARCHAR(50) | NULL | IP地址 |
| opcUaEndpoint | VARCHAR(200) | NULL | OPC UA端点 |

**索引：**
- IX_Equipment_ResourceId (resourceId)
- IX_Equipment_SerialNumber (serialNumber)
- IX_Equipment_NextMaintenanceDate (nextMaintenanceDate)