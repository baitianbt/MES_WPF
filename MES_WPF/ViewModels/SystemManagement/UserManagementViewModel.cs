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
    public partial class UserManagementViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        [ObservableProperty]
        private DateTime? _createTimeStart;

        [ObservableProperty]
        private DateTime? _createTimeEnd;

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:正常, 2:锁定, 3:禁用

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "用户管理";
        
        // 新增/编辑用户相关属性
        [ObservableProperty]
        private bool _isUserDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private User _editingUser = new User();
        
        [ObservableProperty]
        private string _password = string.Empty;
        
        [ObservableProperty]
        private string _confirmPassword = string.Empty;
        
        [ObservableProperty]
        private int? _selectedRoleId;
        
        [ObservableProperty]
        private ObservableCollection<Role> _roles = new ObservableCollection<Role>();
        
        public ObservableCollection<User> Users { get; } = new();
        
        public ICollectionView? UsersView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            UsersView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            UsersView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            UsersView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            UsersView?.Refresh();
        }

        partial void OnCreateTimeStartChanged(DateTime? value)
        {
            UsersView?.Refresh();
        }

        partial void OnCreateTimeEndChanged(DateTime? value)
        {
            UsersView?.Refresh();
        }
        
        public UserManagementViewModel(
            IUserService userService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载用户数据
            _ = LoadUsersAsync();
            
            // 加载角色数据
            _ = LoadRolesAsync();
        }
        
        private void SetupFilter()
        {
            UsersView = CollectionViewSource.GetDefaultView(Users);
            if (UsersView != null)
            {
                UsersView.Filter = UserFilter;
            }
        }
        
        private bool UserFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedStatus == 0)
            {
                return true;
            }
            
            if (obj is User user)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (user.Username?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (user.RealName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (user.Email?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (user.Mobile?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesStatus = SelectedStatus == 0 || user.Status == SelectedStatus;
                
                bool matchesCreateTime = true;
                if (CreateTimeStart.HasValue && user.CreateTime < CreateTimeStart.Value)
                {
                    matchesCreateTime = false;
                }
                if (CreateTimeEnd.HasValue && user.CreateTime > CreateTimeEnd.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesCreateTime = false;
                }
                
                bool matchesDate = true;
                if (StartDate.HasValue && user.LastLoginTime.HasValue && user.LastLoginTime < StartDate.Value)
                {
                    matchesDate = false;
                }
                if (EndDate.HasValue && user.LastLoginTime.HasValue && user.LastLoginTime > EndDate.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesDate = false;
                }
                
                return matchesKeyword && matchesStatus && matchesCreateTime && matchesDate;
            }
            
            return false;
        }
        
        private async Task LoadUsersAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Users.Clear();
                
                // 获取所有用户
                var users = await _userService.GetAllAsync();
                
                // 将用户数据添加到集合
                foreach (var user in users)
                {
                    Users.Add(user);
                }
                
                TotalCount = Users.Count;
                
                // 刷新视图
                UsersView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载用户数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadRolesAsync()
        {
            try
            {
                // 清空现有数据
                Roles.Clear();
                
                // 从服务获取角色数据
                var roles = await App.GetService<IRoleService>().GetAllAsync();
                
                // 将角色数据添加到集合
                foreach (var role in roles)
                {
                    Roles.Add(role);
                }
                
                // 如果没有角色数据，添加一些示例角色
                if (Roles.Count == 0)
                {
                    Roles.Add(new Role { Id = 1, RoleName = "管理员", RoleCode = "admin" });
                    Roles.Add(new Role { Id = 2, RoleName = "操作员", RoleCode = "operator" });
                    Roles.Add(new Role { Id = 3, RoleName = "访客", RoleCode = "guest" });
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载角色数据失败: {ex.Message}");
                
                // 加载失败时添加一些默认角色
                Roles.Clear();
                Roles.Add(new Role { Id = 1, RoleName = "管理员", RoleCode = "admin" });
                Roles.Add(new Role { Id = 2, RoleName = "操作员", RoleCode = "operator" });
                Roles.Add(new Role { Id = 3, RoleName = "访客", RoleCode = "guest" });
            }
        }
        
        [RelayCommand]
        private async Task RefreshUsers()
        {
            await LoadUsersAsync();
        }
        
        [RelayCommand]
        private async Task SearchUsers()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                UsersView?.Refresh();
                
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
            StartDate = null;
            EndDate = null;
            CreateTimeStart = null;
            CreateTimeEnd = null;
            
            await SearchUsers();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的用户
            var selectedUsers = Users.Where(u => u == SelectedUser).ToList();
            
            if (selectedUsers.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的用户");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedUsers.Count} 个用户吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var user in selectedUsers)
                    {
                        await _userService.DeleteAsync(user);
                        Users.Remove(user);
                    }
                    
                    TotalCount = Users.Count;
                    await _dialogService.ShowInfoAsync("成功", "用户已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除用户失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportUsers()
        {
            await _dialogService.ShowInfoAsync("导出", "用户导出功能尚未实现");
        }
        
        [RelayCommand]
        private async Task ViewPassword(User? user)
        {
            if (user == null) return;
            
            // 实际应用中，这里应该调用API获取密码或显示重置密码的对话框
            await _dialogService.ShowInfoAsync("查看密码", $"用户 {user.Username} 的密码为: ******");
        }
        
        [RelayCommand]
        private async Task GoToPage(int page)
        {
            if (page < 1 || page > (TotalCount + PageSize - 1) / PageSize)
            {
                return;
            }
            
            CurrentPage = page;
            
            // 实际应用中，这里应该根据页码加载对应的数据
            // 这里简单处理，不做实际操作
        }
        
        [RelayCommand]
        private void AddUser()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingUser = new User
            {
                Status = 1, // 默认正常状态
                CreateTime = DateTime.Now,
                Username = string.Empty,
                RealName = string.Empty,
                Email = string.Empty,
                Mobile = string.Empty,
                Password = string.Empty
            };
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            SelectedRoleId = Roles.FirstOrDefault()?.Id;
            
            // 打开对话框
            IsUserDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditUser(User? user)
        {
            if (user == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建用户对象的副本，避免直接修改原始数据
            EditingUser = new User
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Email = user.Email,
                Mobile = user.Mobile,
                Status = user.Status,
                Remark = user.Remark,
                CreateTime = user.CreateTime,
                LastLoginTime = user.LastLoginTime,
                LastLoginIp = user.LastLoginIp
            };
            
            // TODO: 设置角色 (实际项目中应该通过用户ID获取用户的角色)
            SelectedRoleId = null;
            
            // 打开对话框
            IsUserDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteUser(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除用户\"{user.RealName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _userService.DeleteAsync(user);
                    Users.Remove(user);
                    TotalCount = Users.Count;
                    await _dialogService.ShowInfoAsync("成功", "用户已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除用户失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsUserDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveUser()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingUser.Username))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入用户名");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingUser.RealName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入真实姓名");
                return;
            }
            
            if (!IsEditMode)
            {
                // 新增模式需要验证密码
                if (string.IsNullOrWhiteSpace(Password))
                {
                    await _dialogService.ShowErrorAsync("错误", "请输入密码");
                    return;
                }
                
                if (Password != ConfirmPassword)
                {
                    await _dialogService.ShowErrorAsync("错误", "两次输入的密码不一致");
                    return;
                }
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新用户
                    await _userService.UpdateAsync(EditingUser);
                    
                    // 更新列表中的用户数据
                    var existingUser = Users.FirstOrDefault(u => u.Id == EditingUser.Id);
                    if (existingUser != null)
                    {
                        int index = Users.IndexOf(existingUser);
                        Users[index] = EditingUser;
                    }
                    
                    // 如果选择了角色，则分配角色
                    if (SelectedRoleId.HasValue)
                    {
                        await _userService.AssignRolesAsync(EditingUser.Id, new List<int> { SelectedRoleId.Value }, 1);
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "用户信息已更新");
                }
                else
                {
                    // 创建新用户
                    EditingUser.Password = Password; // 注意: 实际应用中应该在服务层加密密码
                    var newUser = await _userService.AddAsync(EditingUser);
                    
                    // 添加到用户列表
                    Users.Add(newUser);
                    TotalCount = Users.Count;
                    
                    // 如果选择了角色，则分配角色
                    if (SelectedRoleId.HasValue)
                    {
                        await _userService.AssignRolesAsync(newUser.Id, new List<int> { SelectedRoleId.Value }, 1);
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "用户已创建");
                }
                
                // 关闭对话框
                IsUserDialogOpen = false;
                
                // 刷新视图
                UsersView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存用户失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task ResetPassword(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("重置密码", $"确定要重置用户\"{user.RealName}\"的密码吗？");
            
            if (result)
            {
                var newPassword = await _dialogService.ShowInputAsync("新密码", "请输入新密码:", "123456");
                
                if (!string.IsNullOrEmpty(newPassword))
                {
                    try
                    {
                        await _userService.ResetPasswordAsync(user.Id, newPassword, 1); // 1为操作人ID，实际应从当前登录用户获取
                        await _dialogService.ShowInfoAsync("成功", "密码已重置");
                    }
                    catch (Exception ex)
                    {
                        await _dialogService.ShowErrorAsync("错误", $"重置密码失败: {ex.Message}");
                    }
                }
            }
        }
        
        [RelayCommand]
        private async Task LockUser(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("锁定用户", $"确定要锁定用户\"{user.RealName}\"吗？");
            
            if (result)
            {
                try
                {
                    user.Status = 2; // 锁定状态
                    await _userService.UpdateAsync(user);
                    UsersView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "用户已锁定");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"锁定用户失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task UnlockUser(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("解锁用户", $"确定要解锁用户\"{user.RealName}\"吗？");
            
            if (result)
            {
                try
                {
                    user.Status = 1; // 正常状态
                    await _userService.UpdateAsync(user);
                    UsersView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "用户已解锁");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"解锁用户失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task DisableUser(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用用户", $"确定要禁用用户\"{user.RealName}\"吗？");
            
            if (result)
            {
                try
                {
                    user.Status = 3; // 禁用状态
                    await _userService.UpdateAsync(user);
                    UsersView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "用户已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用用户失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnableUser(User? user)
        {
            if (user == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用用户", $"确定要启用用户\"{user.RealName}\"吗？");
            
            if (result)
            {
                try
                {
                    user.Status = 1; // 正常状态
                    await _userService.UpdateAsync(user);
                    UsersView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "用户已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用用户失败: {ex.Message}");
                }
            }
        }
    }
    
    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RealName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string Remark { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIp { get; set; } = string.Empty;
        public List<RoleModel> Roles { get; set; } = new List<RoleModel>();
    }
}