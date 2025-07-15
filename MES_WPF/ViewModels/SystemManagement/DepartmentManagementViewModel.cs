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
    public partial class DepartmentManagementViewModel : ObservableObject
    {
        private readonly IDepartmentService _departmentService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private Department? _selectedDepartment;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:正常, 2:禁用

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "部门管理";
        
        // 新增/编辑部门相关属性
        [ObservableProperty]
        private bool _isDepartmentDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Department _editingDepartment = new Department();
        
        [ObservableProperty]
        private int? _selectedParentDepartmentId;
        
        [ObservableProperty]
        private ObservableCollection<Department> _parentDepartments = new ObservableCollection<Department>();
        
        public ObservableCollection<Department> Departments { get; } = new();
        
        public ICollectionView? DepartmentsView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            DepartmentsView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            DepartmentsView?.Refresh();
        }
        
        public DepartmentManagementViewModel(
            IDepartmentService departmentService,
            IDialogService dialogService)
        {
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载部门数据
            _ = LoadDepartmentsAsync();
        }
        
        private void SetupFilter()
        {
            DepartmentsView = CollectionViewSource.GetDefaultView(Departments);
            if (DepartmentsView != null)
            {
                DepartmentsView.Filter = DepartmentFilter;
            }
        }
        
        private bool DepartmentFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedStatus == 0)
            {
                return true;
            }
            
            if (obj is Department department)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (department.DeptName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (department.DeptCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (department.Leader?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesStatus = SelectedStatus == 0 || department.Status == SelectedStatus;
                
                return matchesKeyword && matchesStatus;
            }
            
            return false;
        }
        
        private async Task LoadDepartmentsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Departments.Clear();
                
                // 获取所有部门
                var departments = await _departmentService.GetAllAsync();
                
                // 将部门数据添加到集合
                foreach (var department in departments)
                {
                    Departments.Add(department);
                }
                
                TotalCount = Departments.Count;
                
                // 刷新视图
                DepartmentsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载部门数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadParentDepartmentsAsync()
        {
            try
            {
                // 清空现有数据
                ParentDepartments.Clear();
                
                // 添加一个根部门选项
                ParentDepartments.Add(new Department { Id = 0, DeptName = "顶级部门", ParentId = null });
                
                // 获取所有部门（编辑时排除当前部门及其子部门）
                var departments = await _departmentService.GetAllAsync();
                
                if (IsEditMode)
                {
                    var childDepts = await _departmentService.GetDepartmentAndChildrenAsync(EditingDepartment.Id);
                    var childIds = childDepts.Select(d => d.Id).ToList();
                    
                    // 过滤掉当前部门及其子部门
                    departments = departments.Where(d => !childIds.Contains(d.Id) && d.Id != EditingDepartment.Id);
                }
                
                // 将部门数据添加到集合
                foreach (var department in departments)
                {
                    ParentDepartments.Add(department);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载父部门数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshDepartments()
        {
            await LoadDepartmentsAsync();
        }
        
        [RelayCommand]
        private async Task SearchDepartments()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                DepartmentsView?.Refresh();
                
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
            
            await SearchDepartments();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的部门
            var selectedDepartments = Departments.Where(d => d == SelectedDepartment).ToList();
            
            if (selectedDepartments.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的部门");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedDepartments.Count} 个部门吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var department in selectedDepartments)
                    {
                        // 检查是否有子部门
                        var children = await _departmentService.GetChildDepartmentsAsync(department.Id);
                        if (children.Any())
                        {
                            await _dialogService.ShowErrorAsync("错误", $"部门 {department.DeptName} 存在子部门，无法删除。请先删除子部门。");
                            continue;
                        }
                        
                        // 检查是否有员工
                        var employees = await _departmentService.GetDepartmentEmployeesAsync(department.Id);
                        if (employees.Any())
                        {
                            await _dialogService.ShowErrorAsync("错误", $"部门 {department.DeptName} 存在员工，无法删除。请先移除部门下的员工。");
                            continue;
                        }
                        
                        await _departmentService.DeleteAsync(department);
                        Departments.Remove(department);
                    }
                    
                    TotalCount = Departments.Count;
                    await _dialogService.ShowInfoAsync("成功", "部门已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除部门失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportDepartments()
        {
            await _dialogService.ShowInfoAsync("导出", "部门导出功能尚未实现");
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
        private async Task AddDepartment()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingDepartment = new Department
            {
                Status = 1, // 默认正常状态
                CreateTime = DateTime.Now,
                DeptCode = string.Empty,
                DeptName = string.Empty,
                DeptPath = string.Empty,
                Leader = string.Empty,
                Phone = string.Empty,
                Email = string.Empty,
                ParentId = null,
                SortOrder = 1
            };
            
            await LoadParentDepartmentsAsync();
            SelectedParentDepartmentId = 0; // 默认顶级部门
            
            // 打开对话框
            IsDepartmentDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task EditDepartment(Department? department)
        {
            if (department == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建部门对象的副本，避免直接修改原始数据
            EditingDepartment = new Department
            {
                Id = department.Id,
                DeptCode = department.DeptCode,
                DeptName = department.DeptName,
                DeptPath = department.DeptPath,
                Leader = department.Leader,
                Phone = department.Phone,
                Email = department.Email,
                ParentId = department.ParentId,
                SortOrder = department.SortOrder,
                Status = department.Status,
                CreateTime = department.CreateTime,
                UpdateTime = department.UpdateTime,
                Remark = department.Remark
            };
            
            await LoadParentDepartmentsAsync();
            SelectedParentDepartmentId = EditingDepartment.ParentId ?? 0;
            
            // 打开对话框
            IsDepartmentDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteDepartment(Department? department)
        {
            if (department == null) return;
            
            // 检查是否有子部门
            var children = await _departmentService.GetChildDepartmentsAsync(department.Id);
            if (children.Any())
            {
                await _dialogService.ShowErrorAsync("错误", $"部门 {department.DeptName} 存在子部门，无法删除。请先删除子部门。");
                return;
            }
            
            // 检查是否有员工
            var employees = await _departmentService.GetDepartmentEmployeesAsync(department.Id);
            if (employees.Any())
            {
                await _dialogService.ShowErrorAsync("错误", $"部门 {department.DeptName} 存在员工，无法删除。请先移除部门下的员工。");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除部门\"{department.DeptName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _departmentService.DeleteAsync(department);
                    Departments.Remove(department);
                    TotalCount = Departments.Count;
                    await _dialogService.ShowInfoAsync("成功", "部门已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除部门失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsDepartmentDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveDepartment()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingDepartment.DeptName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入部门名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingDepartment.DeptCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入部门编码");
                return;
            }
            
            try
            {
                // 设置父部门ID
                EditingDepartment.ParentId = SelectedParentDepartmentId == 0 ? null : SelectedParentDepartmentId;
                
                if (IsEditMode)
                {
                    // 更新部门
                    EditingDepartment.UpdateTime = DateTime.Now;
                    await _departmentService.UpdateAsync(EditingDepartment);
                    
                    // 更新部门路径
                    await _departmentService.UpdateDepartmentPathAsync(EditingDepartment.Id);
                    
                    // 更新列表中的部门数据
                    var existingDepartment = Departments.FirstOrDefault(d => d.Id == EditingDepartment.Id);
                    if (existingDepartment != null)
                    {
                        int index = Departments.IndexOf(existingDepartment);
                        Departments[index] = EditingDepartment;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "部门信息已更新");
                }
                else
                {
                    // 创建新部门
                    var newDepartment = await _departmentService.AddAsync(EditingDepartment);
                    
                    // 更新部门路径
                    await _departmentService.UpdateDepartmentPathAsync(newDepartment.Id);
                    
                    // 添加到部门列表
                    Departments.Add(newDepartment);
                    TotalCount = Departments.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "部门已创建");
                }
                
                // 关闭对话框
                IsDepartmentDialogOpen = false;
                
                // 刷新视图
                DepartmentsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存部门失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task DisableDepartment(Department? department)
        {
            if (department == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用部门", $"确定要禁用部门\"{department.DeptName}\"吗？");
            
            if (result)
            {
                try
                {
                    department.Status = 2; // 禁用状态
                    await _departmentService.UpdateAsync(department);
                    DepartmentsView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "部门已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用部门失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnableDepartment(Department? department)
        {
            if (department == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用部门", $"确定要启用部门\"{department.DeptName}\"吗？");
            
            if (result)
            {
                try
                {
                    department.Status = 1; // 正常状态
                    await _departmentService.UpdateAsync(department);
                    DepartmentsView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "部门已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用部门失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ViewEmployees(Department? department)
        {
            if (department == null) return;
            
            try
            {
                var employees = await _departmentService.GetDepartmentEmployeesAsync(department.Id);
                if (!employees.Any())
                {
                    await _dialogService.ShowInfoAsync("提示", $"部门 {department.DeptName} 暂无员工");
                    return;
                }
                
                // TODO: 打开员工列表对话框或导航到员工列表页面
                await _dialogService.ShowInfoAsync("查看员工", $"部门 {department.DeptName} 有 {employees.Count()} 名员工");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"获取部门员工失败: {ex.Message}");
            }
        }
    }
} 