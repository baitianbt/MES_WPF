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
    public partial class PermissionManagementViewModel : ObservableObject
    {
        private readonly IPermissionService _permissionService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private PermissionNode? _selectedPermission;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private byte _selectedPermissionType = 0; // 0:全部, 1:菜单, 2:按钮, 3:数据

        [ObservableProperty]
        private string _title = "权限管理";
        
        // 新增/编辑权限相关属性
        [ObservableProperty]
        private bool _isPermissionDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Permission _editingPermission = new Permission();
        
        [ObservableProperty]
        private int? _selectedParentId;
        
        [ObservableProperty]
        private ObservableCollection<Permission> _parentPermissions = new ObservableCollection<Permission>();
        
        // 权限树
        [ObservableProperty]
        private ObservableCollection<PermissionNode> _permissionTree = new ObservableCollection<PermissionNode>();
        
        // 权限列表（平铺）
        public ObservableCollection<Permission> Permissions { get; } = new();
        
        public ICollectionView? PermissionsView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            PermissionsView?.Refresh();
        }

        partial void OnSelectedPermissionTypeChanged(byte value)
        {
            PermissionsView?.Refresh();
        }
        
        public PermissionManagementViewModel(
            IPermissionService permissionService,
            IDialogService dialogService)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载权限数据
            _ = LoadPermissionsAsync();
        }
        
        private void SetupFilter()
        {
            PermissionsView = CollectionViewSource.GetDefaultView(Permissions);
            if (PermissionsView != null)
            {
                PermissionsView.Filter = PermissionFilter;
            }
        }
        
        private bool PermissionFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedPermissionType == 0)
            {
                return true;
            }
            
            if (obj is Permission permission)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (permission.PermissionName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (permission.PermissionCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (permission.Path?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (permission.Component?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedPermissionType == 0 || permission.PermissionType == SelectedPermissionType;
                
                return matchesKeyword && matchesType;
            }
            
            return false;
        }
        
        private async Task LoadPermissionsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Permissions.Clear();
                PermissionTree.Clear();
                
                // 获取所有权限
                var permissions = await _permissionService.GetAllAsync();
                
                // 将权限数据添加到集合
                foreach (var permission in permissions)
                {
                    Permissions.Add(permission);
                }
                
                TotalCount = Permissions.Count;
                
                // 构建权限树
                BuildPermissionTree();
                
                // 刷新视图
                PermissionsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载权限数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private void BuildPermissionTree()
        {
            var topLevelPermissions = Permissions.Where(p => p.ParentId == null || p.ParentId == 0).OrderBy(p => p.SortOrder);
            
            foreach (var permission in topLevelPermissions)
            {
                var node = new PermissionNode(permission);
                BuildPermissionNode(node);
                PermissionTree.Add(node);
            }
        }
        
        private void BuildPermissionNode(PermissionNode parentNode)
        {
            var children = Permissions.Where(p => p.ParentId == parentNode.Permission.Id).OrderBy(p => p.SortOrder);
            
            foreach (var childPermission in children)
            {
                var childNode = new PermissionNode(childPermission);
                parentNode.Children.Add(childNode);
                BuildPermissionNode(childNode);
            }
        }
        
        private async Task LoadParentPermissionsAsync()
        {
            try
            {
                // 清空现有数据
                ParentPermissions.Clear();
                
                // 添加一个顶级权限选项
                ParentPermissions.Add(new Permission { Id = 0, PermissionName = "顶级菜单", ParentId = null });
                
                // 获取所有菜单权限（过滤掉按钮和数据权限）
                var menuPermissions = Permissions.Where(p => p.PermissionType == 1);
                
                if (IsEditMode)
                {
                    // 编辑模式下过滤掉当前权限及其子权限，避免循环引用
                    var childPermissionIds = GetChildPermissionIds(EditingPermission.Id);
                    menuPermissions = menuPermissions.Where(p => p.Id != EditingPermission.Id && !childPermissionIds.Contains(p.Id));
                }
                
                // 将菜单权限数据添加到集合
                foreach (var permission in menuPermissions.OrderBy(p => p.SortOrder))
                {
                    ParentPermissions.Add(permission);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载父级菜单数据失败: {ex.Message}");
            }
        }
        
        private List<int> GetChildPermissionIds(int permissionId)
        {
            var result = new List<int>();
            
            // 直接子权限
            var children = Permissions.Where(p => p.ParentId == permissionId);
            
            foreach (var child in children)
            {
                result.Add(child.Id);
                // 递归获取子权限的子权限
                result.AddRange(GetChildPermissionIds(child.Id));
            }
            
            return result;
        }
        
        [RelayCommand]
        private async Task RefreshPermissions()
        {
            await LoadPermissionsAsync();
        }
        
        [RelayCommand]
        private async Task SearchPermissions()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                PermissionsView?.Refresh();
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
            SelectedPermissionType = 0;
            
            await SearchPermissions();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            if (SelectedPermission == null)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的权限");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的权限及其子权限吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    // 获取所有子权限ID
                    var childIds = GetChildPermissionIds(SelectedPermission.Permission.Id);
                    
                    // 先删除子权限
                    foreach (var id in childIds)
                    {
                        var childPermission = Permissions.FirstOrDefault(p => p.Id == id);
                        if (childPermission != null)
                        {
                            await _permissionService.DeleteAsync(childPermission);
                            Permissions.Remove(childPermission);
                        }
                    }
                    
                    // 删除当前权限
                    await _permissionService.DeleteAsync(SelectedPermission.Permission);
                    Permissions.Remove(SelectedPermission.Permission);
                    
                    TotalCount = Permissions.Count;
                    
                    // 重建权限树
                    PermissionTree.Clear();
                    BuildPermissionTree();
                    
                    await _dialogService.ShowInfoAsync("成功", "权限已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除权限失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task AddPermission(Permission? parentPermission = null)
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingPermission = new Permission
            {
                Status = 1, // 默认启用状态
                IsVisible = true, // 默认可见
                PermissionType = 1, // 默认菜单类型
                CreateTime = DateTime.Now,
                SortOrder = 1,
                PermissionCode = string.Empty,
                PermissionName = string.Empty,
                Path = string.Empty,
                Component = string.Empty,
                Icon = string.Empty
            };
            
            // 设置父权限
            if (parentPermission != null)
            {
                EditingPermission.ParentId = parentPermission.Id;
            }
            
            await LoadParentPermissionsAsync();
            
            // 设置选中的父权限
            SelectedParentId = EditingPermission.ParentId;
            
            // 打开对话框
            IsPermissionDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task EditPermission(Permission? permission)
        {
            if (permission == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建权限对象的副本，避免直接修改原始数据
            EditingPermission = new Permission
            {
                Id = permission.Id,
                PermissionCode = permission.PermissionCode,
                PermissionName = permission.PermissionName,
                PermissionType = permission.PermissionType,
                ParentId = permission.ParentId,
                Path = permission.Path,
                Component = permission.Component,
                Icon = permission.Icon,
                SortOrder = permission.SortOrder,
                IsVisible = permission.IsVisible,
                Status = permission.Status,
                CreateTime = permission.CreateTime,
                UpdateTime = permission.UpdateTime,
                Remark = permission.Remark
            };
            
            await LoadParentPermissionsAsync();
            
            // 设置选中的父权限
            SelectedParentId = EditingPermission.ParentId;
            
            // 打开对话框
            IsPermissionDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeletePermission(Permission? permission)
        {
            if (permission == null) return;
            
            // 检查是否有子权限
            var childIds = GetChildPermissionIds(permission.Id);
            if (childIds.Any())
            {
                var result = await _dialogService.ShowConfirmAsync("确认删除", $"该权限存在 {childIds.Count} 个子权限，删除该权限将同时删除所有子权限。是否继续？");
                if (!result) return;
            }
            else
            {
                var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除权限\"{permission.PermissionName}\"吗？此操作不可撤销。");
                if (!result) return;
            }
            
            try
            {
                // 先删除子权限
                foreach (var id in childIds)
                {
                    var childPermission = Permissions.FirstOrDefault(p => p.Id == id);
                    if (childPermission != null)
                    {
                        await _permissionService.DeleteAsync(childPermission);
                        Permissions.Remove(childPermission);
                    }
                }
                
                // 删除当前权限
                await _permissionService.DeleteAsync(permission);
                Permissions.Remove(permission);
                
                TotalCount = Permissions.Count;
                
                // 重建权限树
                PermissionTree.Clear();
                BuildPermissionTree();
                
                await _dialogService.ShowInfoAsync("成功", "权限已删除");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除权限失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsPermissionDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SavePermission()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingPermission.PermissionName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入权限名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingPermission.PermissionCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入权限编码");
                return;
            }
            
            try
            {
                // 设置父权限ID
                EditingPermission.ParentId = SelectedParentId == 0 ? null : SelectedParentId;
                
                if (IsEditMode)
                {
                    // 更新权限
                    EditingPermission.UpdateTime = DateTime.Now;
                    await _permissionService.UpdateAsync(EditingPermission);
                    
                    // 更新列表中的权限数据
                    var existingPermission = Permissions.FirstOrDefault(p => p.Id == EditingPermission.Id);
                    if (existingPermission != null)
                    {
                        int index = Permissions.IndexOf(existingPermission);
                        Permissions[index] = EditingPermission;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "权限信息已更新");
                }
                else
                {
                    // 创建新权限
                    var newPermission = await _permissionService.AddAsync(EditingPermission);
                    
                    // 添加到权限列表
                    Permissions.Add(newPermission);
                    TotalCount = Permissions.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "权限已创建");
                }
                
                // 关闭对话框
                IsPermissionDialogOpen = false;
                
                // 重建权限树
                PermissionTree.Clear();
                BuildPermissionTree();
                
                // 刷新视图
                PermissionsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存权限失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task DisablePermission(Permission? permission)
        {
            if (permission == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用权限", $"确定要禁用权限\"{permission.PermissionName}\"吗？");
            
            if (result)
            {
                try
                {
                    permission.Status = 2; // 禁用状态
                    await _permissionService.UpdateAsync(permission);
                    PermissionsView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "权限已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用权限失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnablePermission(Permission? permission)
        {
            if (permission == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用权限", $"确定要启用权限\"{permission.PermissionName}\"吗？");
            
            if (result)
            {
                try
                {
                    permission.Status = 1; // 启用状态
                    await _permissionService.UpdateAsync(permission);
                    PermissionsView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "权限已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用权限失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task AddChildPermission(Permission? permission)
        {
            if (permission == null) return;
            await AddPermission(permission);
        }
    }
    
    public partial class PermissionNode : ObservableObject
    {
        public Permission Permission { get; }
        
        public ObservableCollection<PermissionNode> Children { get; } = new ObservableCollection<PermissionNode>();
        
        [ObservableProperty]
        private bool _isExpanded = true;
        
        [ObservableProperty]
        private bool _isSelected;
        
        public PermissionNode(Permission permission)
        {
            Permission = permission;
        }
    }
} 