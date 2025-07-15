 # **MES系统数据库ER关系设计文档（第五部分）**
**版本：** V1.0
**日期：** 2023年10月29日
**适用对象：** 开发团队、数据库管理员、系统架构师

---

## **十、系统管理模块**

### **1. 用户表(User)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 用户唯一标识 |
| username | VARCHAR(50) | NOT NULL, UQ | 用户名 |
| password | VARCHAR(100) | NOT NULL | 密码(加密存储) |
| employeeId | UNIQUEIDENTIFIER | FK, NULL | 关联员工ID |
| realName | NVARCHAR(50) | NOT NULL | 真实姓名 |
| email | VARCHAR(100) | NULL | 电子邮箱 |
| mobile | VARCHAR(20) | NULL | 手机号码 |
| avatar | VARCHAR(200) | NULL | 头像URL |
| status | TINYINT | NOT NULL | 状态(1:正常,2:锁定,3:禁用) |
| lastLoginTime | DATETIME2 | NULL | 最后登录时间 |
| lastLoginIp | VARCHAR(50) | NULL | 最后登录IP |
| passwordUpdateTime | DATETIME2 | NULL | 密码更新时间 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_User_Username (username)
- IX_User_EmployeeId (employeeId)
- IX_User_Email (email)
- IX_User_Mobile (mobile)
- IX_User_Status (status)

### **2. 角色表(Role)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 角色唯一标识 |
| roleCode | VARCHAR(50) | NOT NULL, UQ | 角色编码 |
| roleName | NVARCHAR(100) | NOT NULL | 角色名称 |
| roleType | TINYINT | NOT NULL | 角色类型(1:系统角色,2:业务角色) |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| sortOrder | INT | NOT NULL | 排序号 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Role_RoleCode (roleCode)
- IX_Role_RoleType (roleType)
- IX_Role_Status (status)

### **3. 用户角色关联表(UserRole)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 关联唯一标识 |
| userId | UNIQUEIDENTIFIER | FK, NOT NULL | 用户ID |
| roleId | UNIQUEIDENTIFIER | FK, NOT NULL | 角色ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |

**索引：**
- IX_UserRole_UserId (userId)
- IX_UserRole_RoleId (roleId)
- UQ_UserRole_UserId_RoleId (userId, roleId) - 唯一约束

### **4. 权限表(Permission)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 权限唯一标识 |
| permissionCode | VARCHAR(100) | NOT NULL, UQ | 权限编码 |
| permissionName | NVARCHAR(100) | NOT NULL | 权限名称 |
| permissionType | TINYINT | NOT NULL | 权限类型(1:菜单,2:按钮,3:数据) |
| parentId | UNIQUEIDENTIFIER | FK, NULL | 父权限ID |
| path | VARCHAR(200) | NULL | 路径(菜单类型) |
| component | VARCHAR(200) | NULL | 组件(菜单类型) |
| icon | VARCHAR(100) | NULL | 图标(菜单类型) |
| sortOrder | INT | NOT NULL | 排序号 |
| isVisible | BIT | NOT NULL | 是否可见 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Permission_PermissionCode (permissionCode)
- IX_Permission_PermissionType (permissionType)
- IX_Permission_ParentId (parentId)
- IX_Permission_Status (status)

### **5. 角色权限关联表(RolePermission)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 关联唯一标识 |
| roleId | UNIQUEIDENTIFIER | FK, NOT NULL | 角色ID |
| permissionId | UNIQUEIDENTIFIER | FK, NOT NULL | 权限ID |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |

**索引：**
- IX_RolePermission_RoleId (roleId)
- IX_RolePermission_PermissionId (permissionId)
- UQ_RolePermission_RoleId_PermissionId (roleId, permissionId) - 唯一约束

### **6. 数据字典表(Dictionary)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 字典唯一标识 |
| dictType | VARCHAR(100) | NOT NULL, UQ | 字典类型 |
| dictName | NVARCHAR(100) | NOT NULL | 字典名称 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Dictionary_DictType (dictType)
- IX_Dictionary_Status (status)

### **7. 字典项表(DictionaryItem)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 字典项唯一标识 |
| dictId | UNIQUEIDENTIFIER | FK, NOT NULL | 关联字典ID |
| itemValue | VARCHAR(100) | NOT NULL | 字典项值 |
| itemText | NVARCHAR(100) | NOT NULL | 字典项文本 |
| itemDesc | NVARCHAR(200) | NULL | 字典项描述 |
| sortOrder | INT | NOT NULL | 排序号 |
| status | TINYINT | NOT NULL | 状态(1:启用,2:禁用) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_DictionaryItem_DictId (dictId)
- IX_DictionaryItem_ItemValue (itemValue)
- IX_DictionaryItem_Status (status)
- UQ_DictionaryItem_DictId_ItemValue (dictId, itemValue) - 唯一约束

### **8. 部门表(Department)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 部门唯一标识 |
| deptCode | VARCHAR(50) | NOT NULL, UQ | 部门编码 |
| deptName | NVARCHAR(100) | NOT NULL | 部门名称 |
| parentId | UNIQUEIDENTIFIER | FK, NULL | 父部门ID |
| deptPath | VARCHAR(500) | NOT NULL | 部门路径 |
| leader | NVARCHAR(50) | NULL | 负责人 |
| phone | VARCHAR(20) | NULL | 联系电话 |
| email | VARCHAR(100) | NULL | 电子邮箱 |
| sortOrder | INT | NOT NULL | 排序号 |
| status | TINYINT | NOT NULL | 状态(1:正常,2:禁用) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Department_DeptCode (deptCode)
- IX_Department_ParentId (parentId)
- IX_Department_Status (status)

### **9. 员工表(Employee)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 员工唯一标识 |
| employeeCode | VARCHAR(50) | NOT NULL, UQ | 员工编码 |
| employeeName | NVARCHAR(50) | NOT NULL | 员工姓名 |
| gender | TINYINT | NOT NULL | 性别(1:男,2:女,0:未知) |
| birthDate | DATE | NULL | 出生日期 |
| idCard | VARCHAR(20) | NULL | 身份证号 |
| phone | VARCHAR(20) | NULL | 联系电话 |
| email | VARCHAR(100) | NULL | 电子邮箱 |
| deptId | UNIQUEIDENTIFIER | FK, NOT NULL | 所属部门ID |
| position | NVARCHAR(50) | NULL | 职位 |
| entryDate | DATE | NOT NULL | 入职日期 |
| leaveDate | DATE | NULL | 离职日期 |
| status | TINYINT | NOT NULL | 状态(1:在职,2:离职,3:休假) |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_Employee_EmployeeCode (employeeCode)
- IX_Employee_EmployeeName (employeeName)
- IX_Employee_DeptId (deptId)
- IX_Employee_Status (status)

### **10. 操作日志表(OperationLog)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 日志唯一标识 |
| moduleType | VARCHAR(50) | NOT NULL | 模块类型 |
| operationType | VARCHAR(50) | NOT NULL | 操作类型(增删改查) |
| operationDesc | NVARCHAR(200) | NOT NULL | 操作描述 |
| requestMethod | VARCHAR(10) | NULL | 请求方法 |
| requestUrl | VARCHAR(200) | NULL | 请求URL |
| requestParams | NVARCHAR(2000) | NULL | 请求参数 |
| responseResult | NVARCHAR(2000) | NULL | 响应结果 |
| operationTime | DATETIME2 | NOT NULL | 操作时间 |
| operationUser | UNIQUEIDENTIFIER | NOT NULL | 操作用户ID |
| operationIp | VARCHAR(50) | NULL | 操作IP |
| executionTime | INT | NULL | 执行时长(毫秒) |
| status | TINYINT | NOT NULL | 状态(1:成功,0:失败) |
| errorMsg | NVARCHAR(2000) | NULL | 错误信息 |

**索引：**
- IX_OperationLog_ModuleType (moduleType)
- IX_OperationLog_OperationType (operationType)
- IX_OperationLog_OperationTime (operationTime)
- IX_OperationLog_OperationUser (operationUser)
- IX_OperationLog_Status (status)

### **11. 系统配置表(SystemConfig)**

| 字段名 | 数据类型 | 约束 | 描述 |
|-------|---------|------|------|
| id | UNIQUEIDENTIFIER | PK | 配置唯一标识 |
| configKey | VARCHAR(100) | NOT NULL, UQ | 配置键 |
| configValue | NVARCHAR(500) | NOT NULL | 配置值 |
| configName | NVARCHAR(100) | NOT NULL | 配置名称 |
| configType | VARCHAR(50) | NOT NULL | 配置类型 |
| isSystem | BIT | NOT NULL | 是否系统配置 |
| status | TINYINT | NOT NULL | 状态(1:启用,0:禁用) |
| createBy | UNIQUEIDENTIFIER | NOT NULL | 创建人 |
| createTime | DATETIME2 | NOT NULL | 创建时间 |
| updateTime | DATETIME2 | NULL | 更新时间 |
| remark | NVARCHAR(200) | NULL | 备注 |

**索引：**
- IX_SystemConfig_ConfigKey (configKey)
- IX_SystemConfig_ConfigType (configType)
- IX_SystemConfig_Status (status)

---

## **十一、数据库整体关系图**

下面是MES系统主要模块之间的关系图：

```
┌───────────────────┐           ┌───────────────────┐
│  系统管理模块     │           │  基础信息模块     │
│                   │           │                   │
│  User             │◀─────────▶│  Department       │
│  Role             │           │  Employee         │
│  Permission       │           │  Product          │
│  Dictionary       │           │  BOM              │
└───────────────────┘           └───────────────────┘
         ▲                               ▲
         │                               │
         │                               │
         ▼                               ▼
┌───────────────────┐           ┌───────────────────┐
│  生产计划模块     │           │  物料追溯模块     │
│                   │           │                   │
│  ProductionOrder  │──────────▶│  MaterialBatch    │
│  WorkOrder        │◀─────────▶│  ProductBatch     │
│  OperationPlan    │           │  BatchRelationship│
└───────────────────┘           └───────────────────┘
         ▲                               ▲
         │                               │
         │                               │
         ▼                               ▼
┌───────────────────┐           ┌───────────────────┐
│  生产执行模块     │           │  质量管理模块     │
│                   │           │                   │
│  WorkOrderExecution│◀────────▶│  InspectionTask   │
│  OperationExecution│◀────────▶│  InspectionResult │
│  ProductionReport │           │  SPCControlChart  │
└───────────────────┘           └───────────────────┘
         ▲                               ▲
         │                               │
         │                               │
         ▼                               ▼
┌───────────────────┐           ┌───────────────────┐
│  设备管理模块     │           │  绩效分析模块     │
│                   │           │                   │
│  Equipment        │──────────▶│  KPI              │
│  MaintenanceOrder │           │  KPIData          │
│  EquipmentStatusLog│◀────────▶│  DailyProductionReport│
└───────────────────┘           └───────────────────┘
```

## **十二、数据库设计注意事项**

### **1. 数据完整性**

- **实体完整性**：通过主键和唯一约束保证
- **参照完整性**：通过外键约束保证
- **域完整性**：通过CHECK约束和默认值保证
- **用户定义完整性**：通过触发器和存储过程保证

### **2. 性能优化考虑**

- **分表策略**：
  - 历史数据表：工序执行、设备状态记录、参数记录等大表
  - 分区表：按时间范围分区，提高查询性能

- **索引优化**：
  - 避免过度索引，每表控制在5-8个索引
  - 复合索引中列的顺序应考虑选择性
  - 定期维护索引碎片

- **查询优化**：
  - 常用统计报表考虑使用物化视图
  - 复杂查询使用存储过程优化

### **3. 安全性考虑**

- **数据加密**：敏感数据如密码采用加密存储
- **权限控制**：基于角色的访问控制
- **审计日志**：关键操作记录到操作日志表

### **4. 可扩展性**

- **预留扩展字段**：考虑添加扩展字段如ext1, ext2等
- **版本控制**：关键主数据支持版本管理
- **多租户设计**：如需支持多工厂，考虑添加tenantId字段

### **5. 数据备份与恢复**

- **备份策略**：
  - 每日增量备份
  - 每周全量备份
  - 保留90天历史备份

- **恢复策略**：
  - 支持时间点恢复
  - 定期恢复测试

---

## **十三、后续演进建议**

1. **数据仓库建设**：
   - 构建数据仓库，支持多维分析
   - 建立ETL流程，整合生产数据

2. **大数据平台**：
   - 对历史数据进行归档和分析
   - 结合机器学习进行预测性分析

3. **实时数据处理**：
   - 引入流处理技术处理实时数据
   - 构建实时监控和预警平台