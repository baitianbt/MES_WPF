using MES_WPF.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_WPF.Data.Repositories.SystemManagement
{
    /// <summary>
    /// 角色权限关联表仓储接口
    /// </summary>
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        // 可以在此添加RolePermission特有的仓储方法
    }
}
