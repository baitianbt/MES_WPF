using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES_WPF.Core.Models;
using MES_WPF.Core.Services.EquipmentManagement;
using MES_WPF.Model.EquipmentManagement;
using MES_WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MES_WPF.ViewModels.EquipmentManagement
{
    public partial class MaintenanceExecutionViewModel : ObservableObject
    {
        private readonly IMaintenanceExecutionService _maintenanceExecutionService;
        private readonly IMaintenanceOrderService _maintenanceOrderService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private MaintenanceExecution? _selectedExecution;

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
        private byte _selectedResult = 0; // 0:全部, 1:正常, 2:异常

        [ObservableProperty]
        private int? _selectedEquipmentId;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "维护执行记录";
        
        // 新增/编辑执行记录相关属性
        [ObservableProperty]
        private bool _isExecutionDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private MaintenanceExecution _editingExecution = new MaintenanceExecution();
        
        [ObservableProperty]
        private ObservableCollection<MaintenanceOrder> _orders = new ObservableCollection<MaintenanceOrder>();
        
        [ObservableProperty]
        private int? _selectedOrderId;
        
        [ObservableProperty]
        private ObservableCollection<User> _executors = new ObservableCollection<User>();
        
        [ObservableProperty]
        private int? _selectedExecutorId;
        
        [ObservableProperty]
        private ObservableCollection<string> _imageUrls = new ObservableCollection<string>();
        
        public ObservableCollection<MaintenanceExecution> Executions { get; } = new();
        
        public ICollectionView? ExecutionsView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            ExecutionsView?.Refresh();
        }

        partial void OnSelectedResultChanged(byte value)
        {
            ExecutionsView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            ExecutionsView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            ExecutionsView?.Refresh();
        }
        
        partial void OnSelectedEquipmentIdChanged(int? value)
        {
            ExecutionsView?.Refresh();
        }
        
        public MaintenanceExecutionViewModel(
            IMaintenanceExecutionService maintenanceExecutionService,
            IMaintenanceOrderService maintenanceOrderService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _maintenanceExecutionService = maintenanceExecutionService ?? throw new ArgumentNullException(nameof(maintenanceExecutionService));
            _maintenanceOrderService = maintenanceOrderService ?? throw new ArgumentNullException(nameof(maintenanceOrderService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载维护执行记录数据
            _ = LoadExecutionsAsync();
            
            // 加载维护工单数据
            _ = LoadOrdersAsync();
            
            // 加载执行人数据
            _ = LoadExecutorsAsync();
        }
        
        private void SetupFilter()
        {
            ExecutionsView = CollectionViewSource.GetDefaultView(Executions);
            if (ExecutionsView != null)
            {
                ExecutionsView.Filter = ExecutionFilter;
            }
        }
        
        private bool ExecutionFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                SelectedResult == 0 && 
                !SelectedEquipmentId.HasValue &&
                !StartDate.HasValue && 
                !EndDate.HasValue)
            {
                return true;
            }
            
            if (obj is MaintenanceExecution execution)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (execution.ResultDescription?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (execution.Remark?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesResult = SelectedResult == 0 || 
                                    (execution.ExecutionResult.HasValue && execution.ExecutionResult.Value == SelectedResult);
                
                bool matchesEquipment = !SelectedEquipmentId.HasValue || 
                                       (GetEquipmentIdFromOrder(execution.MaintenanceOrderId) == SelectedEquipmentId.Value);
                
                bool matchesDateRange = true;
                if (StartDate.HasValue && execution.StartTime < StartDate.Value)
                {
                    matchesDateRange = false;
                }
                if (EndDate.HasValue && (execution.EndTime.HasValue && execution.EndTime > EndDate.Value.AddDays(1).AddSeconds(-1)))
                {
                    matchesDateRange = false;
                }
                
                return matchesKeyword && matchesResult && matchesEquipment && matchesDateRange;
            }
            
            return false;
        }
        
        // 从维护工单获取设备ID的辅助方法
        private int GetEquipmentIdFromOrder(int orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            return order?.EquipmentId ?? 0;
        }
        
        private async Task LoadExecutionsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Executions.Clear();
                
                // 获取所有维护执行记录
                var executions = await _maintenanceExecutionService.GetAllAsync();
                
                // 将维护执行记录数据添加到集合
                foreach (var execution in executions)
                {
                    Executions.Add(execution);
                }
                
                TotalCount = Executions.Count;
                
                // 刷新视图
                ExecutionsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护执行记录失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadOrdersAsync()
        {
            try
            {
                // 清空现有数据
                Orders.Clear();
                
                // 从服务获取维护工单数据
                var orders = await _maintenanceOrderService.GetAllAsync();
                
                // 将维护工单数据添加到集合
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护工单数据失败: {ex.Message}");
            }
        }
        
        private async Task LoadExecutorsAsync()
        {
            try
            {
                // 清空现有数据
                Executors.Clear();
                
                // 从服务获取用户数据
                var users = await App.GetService<Core.Services.SystemManagement.IUserService>().GetAllAsync();
                
                // 将用户数据添加到集合
                foreach (var user in users)
                {
                    Executors.Add(user);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载执行人数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshExecutions()
        {
            await LoadExecutionsAsync();
        }
        
        [RelayCommand]
        private async Task SearchExecutions()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                ExecutionsView?.Refresh();
                
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
            SelectedResult = 0;
            SelectedEquipmentId = null;
            StartDate = null;
            EndDate = null;
            
            await SearchExecutions();
        }
        
        [RelayCommand]
        private async Task ExportExecutions()
        {
            await _dialogService.ShowInfoAsync("导出", "维护执行记录导出功能尚未实现");
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
        private void AddExecution()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingExecution = new MaintenanceExecution
            {
                StartTime = DateTime.Now,
                CreateTime = DateTime.Now
            };
            SelectedOrderId = Orders.FirstOrDefault()?.Id;
            SelectedExecutorId = Executors.FirstOrDefault()?.Id;
            ImageUrls.Clear();
            
            // 打开对话框
            IsExecutionDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditExecution(MaintenanceExecution? execution)
        {
            if (execution == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建执行记录对象的副本，避免直接修改原始数据
            EditingExecution = new MaintenanceExecution
            {
                Id = execution.Id,
                MaintenanceOrderId = execution.MaintenanceOrderId,
                ExecutorId = execution.ExecutorId,
                StartTime = execution.StartTime,
                EndTime = execution.EndTime,
                LaborTime = execution.LaborTime,
                ExecutionResult = execution.ExecutionResult,
                ResultDescription = execution.ResultDescription,
                ImageUrls = execution.ImageUrls,
                CreateTime = execution.CreateTime,
                UpdateTime = execution.UpdateTime,
                Remark = execution.Remark
            };
            
            // 设置选中的工单和执行人
            SelectedOrderId = execution.MaintenanceOrderId;
            SelectedExecutorId = execution.ExecutorId;
            
            // 解析图片URL
            ImageUrls.Clear();
            if (!string.IsNullOrEmpty(execution.ImageUrls))
            {
                try
                {
                    var urls = System.Text.Json.JsonSerializer.Deserialize<List<string>>(execution.ImageUrls);
                    if (urls != null)
                    {
                        foreach (var url in urls)
                        {
                            ImageUrls.Add(url);
                        }
                    }
                }
                catch { /* 解析失败时不做处理 */ }
            }
            
            // 打开对话框
            IsExecutionDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteExecution(MaintenanceExecution? execution)
        {
            if (execution == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除该维护执行记录吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _maintenanceExecutionService.DeleteAsync(execution);
                    Executions.Remove(execution);
                    TotalCount = Executions.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护执行记录已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护执行记录失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsExecutionDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveExecution()
        {
            // 验证必填字段
            if (!SelectedOrderId.HasValue)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择维护工单");
                return;
            }
            
            if (!SelectedExecutorId.HasValue)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择执行人");
                return;
            }
            
            try
            {
                // 设置工单和执行人ID
                EditingExecution.MaintenanceOrderId = SelectedOrderId.Value;
                EditingExecution.ExecutorId = SelectedExecutorId.Value;
                
                // 设置图片URL
                if (ImageUrls.Count > 0)
                {
                    EditingExecution.ImageUrls = System.Text.Json.JsonSerializer.Serialize(ImageUrls.ToList());
                }
                
                if (IsEditMode)
                {
                    // 更新执行记录
                    EditingExecution.UpdateTime = DateTime.Now;
                    await _maintenanceExecutionService.UpdateAsync(EditingExecution);
                    
                    // 更新列表中的执行记录数据
                    var existingExecution = Executions.FirstOrDefault(e => e.Id == EditingExecution.Id);
                    if (existingExecution != null)
                    {
                        int index = Executions.IndexOf(existingExecution);
                        Executions[index] = EditingExecution;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护执行记录已更新");
                }
                else
                {
                    // 创建新执行记录
                    var newExecution = await _maintenanceExecutionService.AddAsync(EditingExecution);
                    
                    // 添加到执行记录列表
                    Executions.Add(newExecution);
                    TotalCount = Executions.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "维护执行记录已创建");
                }
                
                // 关闭对话框
                IsExecutionDialogOpen = false;
                
                // 刷新视图
                ExecutionsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存维护执行记录失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task CompleteExecution(MaintenanceExecution? execution)
        {
            if (execution == null) return;
            
            // 如果已经完成，则不允许再次完成
            if (execution.EndTime.HasValue)
            {
                await _dialogService.ShowInfoAsync("提示", "该维护执行记录已经完成");
                return;
            }
            
            // 打开完成对话框
            var result = await _dialogService.ShowConfirmAsync("完成维护", "确定要完成该维护执行记录吗？");
            
            if (result)
            {
                try
                {
                    // 获取执行结果和描述
                    var executionResult = await _dialogService.ShowInputAsync("执行结果", "请选择执行结果(1:正常,2:异常):", "1");
                    byte resultValue = 1;
                    if (!string.IsNullOrEmpty(executionResult))
                    {
                        byte.TryParse(executionResult, out resultValue);
                    }
                    
                    var resultDescription = await _dialogService.ShowInputAsync("结果描述", "请输入结果描述:", "");
                    
                    // 完成执行记录
                    var updatedExecution = await _maintenanceExecutionService.CompleteExecutionAsync(
                        execution.Id, 
                        resultValue, 
                        resultDescription ?? string.Empty);
                    
                    // 更新列表中的执行记录
                    var existingExecution = Executions.FirstOrDefault(e => e.Id == updatedExecution.Id);
                    if (existingExecution != null)
                    {
                        int index = Executions.IndexOf(existingExecution);
                        Executions[index] = updatedExecution;
                    }
                    
                    // 刷新视图
                    ExecutionsView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "维护执行记录已完成");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"完成维护执行记录失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task AddImage()
        {
            // 这里应该调用文件选择对话框，但为简化处理，这里直接添加一个示例URL
            var imageUrl = await _dialogService.ShowInputAsync("添加图片", "请输入图片URL:", "");
            
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ImageUrls.Add(imageUrl);
            }
        }
        
        [RelayCommand]
        private void RemoveImage(string imageUrl)
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                ImageUrls.Remove(imageUrl);
            }
        }
    }
}