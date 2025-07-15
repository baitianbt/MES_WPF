using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES_WPF.Core.Models;
using MES_WPF.Core.Services.SystemManagement;
using MES_WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MES_WPF.ViewModels.SystemManagement
{
    public partial class RoleManagementViewModel : ObservableObject
    {
        private readonly IRoleService _roleService;
        private readonly IPermissionService _permissionService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private Role? _selectedRole;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:启用, 2:禁用

        [ObservableProperty]
        private byte _selectedRoleType = 0; // 0:全部, 1:系统角色, 2:业务角色

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "角色管理";
        
        // 新增/编辑角色相关属性
        [ObservableProperty]
        private bool _isRoleDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Role _editingRole = new Role();
        
        // 角色权限分配属性
        [ObservableProperty]
        private bool _isPermissionDialogOpen;
        
        [ObservableProperty]
        private Role? _currentRole;
        
        [ObservableProperty]
        private ObservableCollection<Permission> _permissions = new ObservableCollection<Permission>();
        
        [ObservableProperty]
        private ObservableCollection<Permission> _selectedPermissions = new ObservableCollection<Permission>();
        
        public ObservableCollection<Role> Roles { get; } = new();
        
        public ICollectionView? RolesView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            RolesView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            RolesView?.Refresh();
        }
        
        partial void OnSelectedRoleTypeChanged(byte value)
        {
            RolesView?.Refresh();
        }
        
        public RoleManagementViewModel(
            IRoleService roleService,
            IPermissionService permissionService,
            IDialogService dialogService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载角色数据
            _ = LoadRolesAsync();
        }
        
        private void SetupFilter()
        {
            RolesView = CollectionViewSource.GetDefaultView(Roles);
            if (RolesView != null)
            {
                RolesView.Filter = RoleFilter;
            }
        }
        
        private bool RoleFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedStatus == 0 && SelectedRoleType == 0)
            {
                return true;
            }
            
            if (obj is Role role)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (role.RoleName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (role.RoleCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (role.Remark?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesStatus = SelectedStatus == 0 || role.Status == SelectedStatus;
                bool matchesRoleType = SelectedRoleType == 0 || role.RoleType == SelectedRoleType;
                
                return matchesKeyword && matchesStatus && matchesRoleType;
            }
            
            return false;
        }
        
        private async Task LoadRolesAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Roles.Clear();
                
                // 获取所有角色
                var roles = await _roleService.GetAllAsync();
                
                // 将角色数据添加到集合
                foreach (var role in roles)
                {
                    Roles.Add(role);
                }
                
                TotalCount = Roles.Count;
                
                // 刷新视图
                RolesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载角色数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadPermissionsAsync(int roleId)
        {
            try
            {
                // 清空现有数据
                Permissions.Clear();
                SelectedPermissions.Clear();
                
                // 获取所有权限
                var allPermissions = await _permissionService.GetAllAsync();
                
                // 获取角色已有权限
                var rolePermissions = await _roleService.GetRolePermissionsAsync(roleId);
                var rolePermissionIds = rolePermissions.Select(p => p.Id).ToList();
                
                // 将权限数据添加到集合
                foreach (var permission in allPermissions)
                {
                    Permissions.Add(permission);
                    
                    // 如果是角色已有权限，添加到已选权限集合
                    if (rolePermissionIds.Contains(permission.Id))
                    {
                        SelectedPermissions.Add(permission);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载权限数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshRoles()
        {
            await LoadRolesAsync();
        }
        
        [RelayCommand]
        private async Task SearchRoles()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                RolesView?.Refresh();
                
                // 重置到第一页
                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task ResetSearch()
        {
            SearchKeyword = string.Empty;
            SelectedStatus = 0;
            SelectedRoleType = 0;
            
            await SearchRoles();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的角色
            var selectedRoles = Roles.Where(r => r == SelectedRole).ToList();
            
            if (selectedRoles.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的角色");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedRoles.Count} 个角色吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var role in selectedRoles)
                    {
                        await _roleService.DeleteAsync(role);
                        Roles.Remove(role);
                    }
                    
                    TotalCount = Roles.Count;
                    await _dialogService.ShowInfoAsync("成功", "角色已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除角色失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportRoles()
        {
            await _dialogService.ShowInfoAsync("导出", "角色导出功能尚未实现");
        }
        
        [RelayCommand]
        private async Task GoToPage(int page)
        {
            if (page < 1 || page > (TotalCount + PageSize - 1) / PageSize)
            {
                return;
            }
            
            CurrentPage = page;
        }
        
        [RelayCommand]
        private void AddRole()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingRole = new Role
            {
                Status = 1, // 默认启用状态
                RoleType = 2, // 默认业务角色
                CreateTime = DateTime.Now,
                CreateBy = 1, // 假设是当前用户
                SortOrder = 1,
                RoleCode = string.Empty,
                RoleName = string.Empty,
                Remark = string.Empty
            };
            
            // 打开对话框
            IsRoleDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditRole(Role? role)
        {
            if (role == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建角色对象的副本，避免直接修改原始数据
            EditingRole = new Role
            {
                Id = role.Id,
                RoleCode = role.RoleCode,
                RoleName = role.RoleName,
                RoleType = role.RoleType,
                Status = role.Status,
                SortOrder = role.SortOrder,
                CreateBy = role.CreateBy,
                CreateTime = role.CreateTime,
                UpdateTime = role.UpdateTime,
                Remark = role.Remark
            };
            
            // 打开对话框
            IsRoleDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteRole(Role? role)
        {
            if (role == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除角色\"{role.RoleName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _roleService.DeleteAsync(role);
                    Roles.Remove(role);
                    TotalCount = Roles.Count;
                    await _dialogService.ShowInfoAsync("成功", "角色已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除角色失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsRoleDialogOpen = false;
        }
        
        [RelayCommand]
        private void CancelPermissionEdit()
        {
            // 关闭权限对话框
            IsPermissionDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveRole()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingRole.RoleName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入角色名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingRole.RoleCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入角色编码");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新角色
                    EditingRole.UpdateTime = DateTime.Now;
                    await _roleService.UpdateAsync(EditingRole);
                    
                    // 更新列表中的角色数据
                    var existingRole = Roles.FirstOrDefault(r => r.Id == EditingRole.Id);
                    if (existingRole != null)
                    {
                        int index = Roles.IndexOf(existingRole);
                        Roles[index] = EditingRole;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "角色信息已更新");
                }
                else
                {
                    // 创建新角色
                    var newRole = await _roleService.AddAsync(EditingRole);
                    
                    // 添加到角色列表
                    Roles.Add(newRole);
                    TotalCount = Roles.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "角色已创建");
                }
                
                // 关闭对话框
                IsRoleDialogOpen = false;
                
                // 刷新视图
                RolesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存角色失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task SavePermissions()
        {
            if (CurrentRole == null) return;
            
            try
            {
                // 获取选中的权限ID
                var selectedPermissionIds = SelectedPermissions.Select(p => p.Id).ToList();
                
                // 保存权限
                await _roleService.AssignPermissionsAsync(CurrentRole.Id, selectedPermissionIds, 1); // 1为操作人ID
                
                // 关闭对话框
                IsPermissionDialogOpen = false;
                
                await _dialogService.ShowInfoAsync("成功", "角色权限已更新");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存权限失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task AssignPermissions(Role? role)
        {
            if (role == null) return;
            
            CurrentRole = role;
            
            // 加载权限数据
            await LoadPermissionsAsync(role.Id);
            
            // 打开权限分配对话框
            IsPermissionDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DisableRole(Role? role)
        {
            if (role == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用角色", $"确定要禁用角色\"{role.RoleName}\"吗？");
            
            if (result)
            {
                try
                {
                    role.Status = 2; // 禁用状态
                    await _roleService.UpdateAsync(role);
                    RolesView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "角色已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用角色失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnableRole(Role? role)
        {
            if (role == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用角色", $"确定要启用角色\"{role.RoleName}\"吗？");
            
            if (result)
            {
                try
                {
                    role.Status = 1; // 启用状态
                    await _roleService.UpdateAsync(role);
                    RolesView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "角色已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用角色失败: {ex.Message}");
                }
            }
        }
    }
} 