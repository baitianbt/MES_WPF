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
    public partial class EmployeeManagementViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private Employee? _selectedEmployee;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private DateTime? _entryDateStart;

        [ObservableProperty]
        private DateTime? _entryDateEnd;

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:在职, 2:离职, 3:休假

        [ObservableProperty]
        private int? _selectedDepartmentId;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "员工管理";
        
        // 新增/编辑员工相关属性
        [ObservableProperty]
        private bool _isEmployeeDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Employee _editingEmployee = new Employee();
        
        [ObservableProperty]
        private ObservableCollection<Department> _departments = new ObservableCollection<Department>();
        
        // 员工调动相关属性
        [ObservableProperty]
        private bool _isTransferDialogOpen;
        
        [ObservableProperty]
        private Employee? _transferEmployee;
        
        [ObservableProperty]
        private int _newDepartmentId;
        
        [ObservableProperty]
        private string _newPosition = string.Empty;
        
        // 员工离职相关属性
        [ObservableProperty]
        private bool _isLeaveDialogOpen;
        
        [ObservableProperty]
        private Employee? _leaveEmployee;
        
        [ObservableProperty]
        private DateTime _leaveDate = DateTime.Now;
        
        public ObservableCollection<Employee> Employees { get; } = new();
        
        public ICollectionView? EmployeesView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            EmployeesView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            EmployeesView?.Refresh();
        }

        partial void OnSelectedDepartmentIdChanged(int? value)
        {
            EmployeesView?.Refresh();
        }

        partial void OnEntryDateStartChanged(DateTime? value)
        {
            EmployeesView?.Refresh();
        }

        partial void OnEntryDateEndChanged(DateTime? value)
        {
            EmployeesView?.Refresh();
        }
        
        public EmployeeManagementViewModel(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IDialogService dialogService)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载员工数据
            _ = LoadEmployeesAsync();
            
            // 加载部门数据
            _ = LoadDepartmentsAsync();
        }
        
        private void SetupFilter()
        {
            EmployeesView = CollectionViewSource.GetDefaultView(Employees);
            if (EmployeesView != null)
            {
                EmployeesView.Filter = EmployeeFilter;
            }
        }
        
        private bool EmployeeFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                SelectedStatus == 0 && 
                SelectedDepartmentId == null &&
                !EntryDateStart.HasValue &&
                !EntryDateEnd.HasValue)
            {
                return true;
            }
            
            if (obj is Employee employee)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (employee.EmployeeCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (employee.EmployeeName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (employee.Position?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (employee.Phone?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (employee.Email?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesStatus = SelectedStatus == 0 || employee.Status == SelectedStatus;
                
                bool matchesDepartment = !SelectedDepartmentId.HasValue || employee.DeptId == SelectedDepartmentId;
                
                bool matchesEntryDate = true;
                if (EntryDateStart.HasValue && employee.EntryDate < EntryDateStart.Value)
                {
                    matchesEntryDate = false;
                }
                if (EntryDateEnd.HasValue && employee.EntryDate > EntryDateEnd.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesEntryDate = false;
                }
                
                return matchesKeyword && matchesStatus && matchesDepartment && matchesEntryDate;
            }
            
            return false;
        }
        
        private async Task LoadEmployeesAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Employees.Clear();
                
                // 获取所有员工
                var employees = await _employeeService.GetAllAsync();
                
                // 将员工数据添加到集合
                foreach (var employee in employees)
                {
                    Employees.Add(employee);
                }
                
                TotalCount = Employees.Count;
                
                // 刷新视图
                EmployeesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载员工数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadDepartmentsAsync()
        {
            try
            {
                // 清空现有数据
                Departments.Clear();
                
                // 获取所有部门
                var departments = await _departmentService.GetAllAsync();
                
                // 将部门数据添加到集合
                foreach (var department in departments.OrderBy(d => d.SortOrder))
                {
                    Departments.Add(department);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载部门数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshEmployees()
        {
            await LoadEmployeesAsync();
        }
        
        [RelayCommand]
        private async Task SearchEmployees()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                EmployeesView?.Refresh();
                
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
            SelectedDepartmentId = null;
            EntryDateStart = null;
            EntryDateEnd = null;
            
            await SearchEmployees();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的员工
            var selectedEmployees = Employees.Where(e => e == SelectedEmployee).ToList();
            
            if (selectedEmployees.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的员工");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedEmployees.Count} 个员工吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var employee in selectedEmployees)
                    {
                        // 检查是否有关联用户
                        var user = await _employeeService.GetRelatedUserAsync(employee.Id);
                        if (user != null)
                        {
                            await _dialogService.ShowErrorAsync("错误", $"员工 {employee.EmployeeName} 存在关联用户，无法删除。");
                            continue;
                        }
                        
                        await _employeeService.DeleteAsync(employee);
                        Employees.Remove(employee);
                    }
                    
                    TotalCount = Employees.Count;
                    await _dialogService.ShowInfoAsync("成功", "员工已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除员工失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportEmployees()
        {
            await _dialogService.ShowInfoAsync("导出", "员工导出功能尚未实现");
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
        private async Task AddEmployee()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingEmployee = new Employee
            {
                Status = 1, // 默认在职状态
                Gender = 0, // 默认未知
                CreateTime = DateTime.Now,
                EntryDate = DateTime.Now,
                EmployeeCode = string.Empty,
                EmployeeName = string.Empty,
                Position = string.Empty,
                Phone = string.Empty,
                Email = string.Empty
            };
            
            // 设置默认部门
            if (Departments.Count > 0)
            {
                EditingEmployee.DeptId = Departments.First().Id;
            }
            
            // 打开对话框
            IsEmployeeDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task EditEmployee(Employee? employee)
        {
            if (employee == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建员工对象的副本，避免直接修改原始数据
            EditingEmployee = new Employee
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                EmployeeName = employee.EmployeeName,
                Gender = employee.Gender,
                BirthDate = employee.BirthDate,
                IdCard = employee.IdCard,
                Phone = employee.Phone,
                Email = employee.Email,
                DeptId = employee.DeptId,
                Position = employee.Position,
                EntryDate = employee.EntryDate,
                LeaveDate = employee.LeaveDate,
                Status = employee.Status,
                CreateTime = employee.CreateTime,
                UpdateTime = employee.UpdateTime,
                Remark = employee.Remark
            };
            
            // 打开对话框
            IsEmployeeDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteEmployee(Employee? employee)
        {
            if (employee == null) return;
            
            // 检查是否有关联用户
            var user = await _employeeService.GetRelatedUserAsync(employee.Id);
            if (user != null)
            {
                await _dialogService.ShowErrorAsync("错误", $"员工 {employee.EmployeeName} 存在关联用户，无法删除。");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除员工\"{employee.EmployeeName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _employeeService.DeleteAsync(employee);
                    Employees.Remove(employee);
                    TotalCount = Employees.Count;
                    await _dialogService.ShowInfoAsync("成功", "员工已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除员工失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsEmployeeDialogOpen = false;
        }
        
        [RelayCommand]
        private void CancelTransfer()
        {
            // 关闭对话框
            IsTransferDialogOpen = false;
        }
        
        [RelayCommand]
        private void CancelLeave()
        {
            // 关闭对话框
            IsLeaveDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveEmployee()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingEmployee.EmployeeName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入员工姓名");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingEmployee.EmployeeCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入员工编码");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新员工
                    EditingEmployee.UpdateTime = DateTime.Now;
                    await _employeeService.UpdateAsync(EditingEmployee);
                    
                    // 更新列表中的员工数据
                    var existingEmployee = Employees.FirstOrDefault(e => e.Id == EditingEmployee.Id);
                    if (existingEmployee != null)
                    {
                        int index = Employees.IndexOf(existingEmployee);
                        Employees[index] = EditingEmployee;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "员工信息已更新");
                }
                else
                {
                    // 创建新员工
                    var newEmployee = await _employeeService.AddAsync(EditingEmployee);
                    
                    // 添加到员工列表
                    Employees.Add(newEmployee);
                    TotalCount = Employees.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "员工已创建");
                }
                
                // 关闭对话框
                IsEmployeeDialogOpen = false;
                
                // 刷新视图
                EmployeesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存员工失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task TransferEmployeeMethod(Employee? employee)
        {
            if (employee == null) return;

            // 设置当前员工
            TransferEmployee = employee;
            
            // 设置默认值
            NewDepartmentId = employee.DeptId;
            NewPosition = employee.Position;
            
            // 打开对话框
            IsTransferDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task SaveTransfer()
        {
            if (SelectedEmployee == null) return;
            
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(NewPosition))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入新职位");
                return;
            }
            
            try
            {
                // 调动员工
                await _employeeService.TransferAsync(SelectedEmployee.Id, NewDepartmentId, NewPosition);
                
                // 更新员工信息
                var employee = await _employeeService.GetByIdAsync(SelectedEmployee.Id);
                if (employee != null)
                {
                    // 更新列表中的员工数据
                    var existingEmployee = Employees.FirstOrDefault(e => e.Id == employee.Id);
                    if (existingEmployee != null)
                    {
                        int index = Employees.IndexOf(existingEmployee);
                        Employees[index] = employee;
                    }
                }
                
                // 关闭对话框
                IsTransferDialogOpen = false;
                
                // 刷新视图
                EmployeesView?.Refresh();
                
                await _dialogService.ShowInfoAsync("成功", "员工调动已完成");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"员工调动失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task EmployeeLeave(Employee? employee)
        {
            if (employee == null) return;
            
            // 设置当前员工
            LeaveEmployee = employee;
            
            // 设置默认离职日期
            LeaveDate = DateTime.Now;
            
            // 打开对话框
            IsLeaveDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task SaveLeave()
        {
            if (LeaveEmployee == null) return;
            
            try
            {
                // 设置员工离职
                await _employeeService.LeaveAsync(LeaveEmployee.Id, LeaveDate);
                
                // 更新员工信息
                var employee = await _employeeService.GetByIdAsync(LeaveEmployee.Id);
                if (employee != null)
                {
                    // 更新列表中的员工数据
                    var existingEmployee = Employees.FirstOrDefault(e => e.Id == employee.Id);
                    if (existingEmployee != null)
                    {
                        int index = Employees.IndexOf(existingEmployee);
                        Employees[index] = employee;
                    }
                }
                
                // 关闭对话框
                IsLeaveDialogOpen = false;
                
                // 刷新视图
                EmployeesView?.Refresh();
                
                await _dialogService.ShowInfoAsync("成功", "员工已设置为离职");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"设置员工离职失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task SetOnLeave(Employee? employee)
        {
            if (employee == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("设置休假", $"确定要将员工\"{employee.EmployeeName}\"设置为休假状态吗？");
            
            if (result)
            {
                try
                {
                    employee.Status = 3; // 休假状态
                    await _employeeService.UpdateAsync(employee);
                    EmployeesView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "员工已设置为休假状态");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"设置员工状态失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task SetActive(Employee? employee)
        {
            if (employee == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("设置在职", $"确定要将员工\"{employee.EmployeeName}\"设置为在职状态吗？");
            
            if (result)
            {
                try
                {
                    employee.Status = 1; // 在职状态
                    await _employeeService.UpdateAsync(employee);
                    EmployeesView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "员工已设置为在职状态");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"设置员工状态失败: {ex.Message}");
                }
            }
        }
    }
} 