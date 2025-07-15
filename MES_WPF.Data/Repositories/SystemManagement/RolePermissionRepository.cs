using MES_WPF.Core.Models;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    /// <summary>
    /// 角色权限关联表仓储实现
    /// </summary>
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public RolePermissionRepository(MesDbContext context) : base(context)
        {
        }
        
        // 可以在此添加RolePermission特有的仓储方法实现
    }
} 