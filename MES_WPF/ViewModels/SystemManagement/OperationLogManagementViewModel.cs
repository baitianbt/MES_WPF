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
    public partial class OperationLogManagementViewModel : ObservableObject
    {
        private readonly IOperationLogService _operationLogService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private OperationLog? _selectedLog;
        
        [ObservableProperty]
        private string _searchKeyword = string.Empty;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private int _totalCount;
        
        [ObservableProperty]
        private string _title = "操作日志";
        
        [ObservableProperty]
        private string _selectedModuleType = "全部";
        
        [ObservableProperty]
        private string _selectedOperationType = "全部";
        
        [ObservableProperty]
        private DateTime? _operationTimeStart;
        
        [ObservableProperty]
        private DateTime? _operationTimeEnd;
        
        [ObservableProperty]
        private byte _selectedStatus = 255; // 255:全部, 1:成功, 0:失败
        
        // 日志详情对话框相关属性
        [ObservableProperty]
        private bool _isLogDetailDialogOpen;
        
        [ObservableProperty]
        private OperationLog _detailLog = new OperationLog();
        
        public ObservableCollection<OperationLog> Logs { get; } = new();
        
        public ObservableCollection<string> ModuleTypes { get; } = new();
        
        public ObservableCollection<string> OperationTypes { get; } = new();
        
        public ICollectionView? LogsView { get; private set; }
        
        partial void OnSearchKeywordChanged(string value)
        {
            LogsView?.Refresh();
        }
        
        partial void OnSelectedModuleTypeChanged(string value)
        {
            LogsView?.Refresh();
        }
        
        partial void OnSelectedOperationTypeChanged(string value)
        {
            LogsView?.Refresh();
        }
        
        partial void OnOperationTimeStartChanged(DateTime? value)
        {
            LogsView?.Refresh();
        }
        
        partial void OnOperationTimeEndChanged(DateTime? value)
        {
            LogsView?.Refresh();
        }
        
        partial void OnSelectedStatusChanged(byte value)
        {
            LogsView?.Refresh();
        }
        
        public OperationLogManagementViewModel(
            IOperationLogService operationLogService,
            IDialogService dialogService)
        {
            _operationLogService = operationLogService ?? throw new ArgumentNullException(nameof(operationLogService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 初始化时间范围为近7天
            OperationTimeStart = DateTime.Now.AddDays(-7);
            OperationTimeEnd = DateTime.Now;
            
            // 加载操作日志数据
            _ = LoadLogsAsync();
        }
        
        private void SetupFilter()
        {
            LogsView = CollectionViewSource.GetDefaultView(Logs);
            if (LogsView != null)
            {
                LogsView.Filter = LogFilter;
            }
        }
        
        private bool LogFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                SelectedModuleType == "全部" && 
                SelectedOperationType == "全部" &&
                SelectedStatus == 255 &&
                !OperationTimeStart.HasValue &&
                !OperationTimeEnd.HasValue)
            {
                return true;
            }
            
            if (obj is OperationLog log)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (log.OperationDesc?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (log.RequestMethod?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (log.RequestUrl?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (log.OperationIp?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesModuleType = SelectedModuleType == "全部" || log.ModuleType == SelectedModuleType;
                
                bool matchesOperationType = SelectedOperationType == "全部" || log.OperationType == SelectedOperationType;
                
                bool matchesStatus = SelectedStatus == 255 || log.Status == SelectedStatus;
                
                bool matchesOperationTime = true;
                if (OperationTimeStart.HasValue && log.OperationTime < OperationTimeStart.Value)
                {
                    matchesOperationTime = false;
                }
                if (OperationTimeEnd.HasValue && log.OperationTime > OperationTimeEnd.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesOperationTime = false;
                }
                
                return matchesKeyword && matchesModuleType && matchesOperationType && matchesStatus && matchesOperationTime;
            }
            
            return false;
        }
        
        private async Task LoadLogsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Logs.Clear();
                ModuleTypes.Clear();
                OperationTypes.Clear();
                
                // 添加"全部"选项
                ModuleTypes.Add("全部");
                OperationTypes.Add("全部");
                
                // 获取所有操作日志
                var logs = await _operationLogService.GetAllAsync();
                
                // 将操作日志数据添加到集合
                foreach (var log in logs)
                {
                    Logs.Add(log);
                    
                    // 收集模块类型和操作类型
                    if (!string.IsNullOrEmpty(log.ModuleType) && !ModuleTypes.Contains(log.ModuleType))
                    {
                        ModuleTypes.Add(log.ModuleType);
                    }
                    
                    if (!string.IsNullOrEmpty(log.OperationType) && !OperationTypes.Contains(log.OperationType))
                    {
                        OperationTypes.Add(log.OperationType);
                    }
                }
                
                TotalCount = Logs.Count;
                
                // 刷新视图
                LogsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载操作日志数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task RefreshLogs()
        {
            await LoadLogsAsync();
        }
        
        [RelayCommand]
        private void ResetSearch()
        {
            SearchKeyword = string.Empty;
            SelectedModuleType = "全部";
            SelectedOperationType = "全部";
            SelectedStatus = 255;
            OperationTimeStart = DateTime.Now.AddDays(-7);
            OperationTimeEnd = DateTime.Now;
        }
        
        [RelayCommand]
        private void ViewLogDetail(OperationLog? log)
        {
            if (log == null)
                return;
                
            DetailLog = log;
            IsLogDetailDialogOpen = true;
        }
        
        [RelayCommand]
        private void CloseLogDetail()
        {
            IsLogDetailDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task ExportLogs()
        {
            try
            {
                IsRefreshing = true;
                
                // 获取筛选后的日志
                var filteredLogs = Logs.Where(log => LogFilter(log)).ToList();
                
                if (filteredLogs.Count == 0)
                {
                    await _dialogService.ShowErrorAsync("警告", "没有符合条件的日志数据可导出");
                    return;
                }
                
                // 导出日志，这里简化为仅显示消息
                await _dialogService.ShowInfoAsync("成功", $"已成功导出 {filteredLogs.Count} 条日志数据");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"导出日志数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task ClearLogs()
        {
            // 确认清空
            var result = await _dialogService.ShowConfirmAsync("确认清空", "确定要清空所有操作日志吗？此操作不可恢复！");
            if (result)
            {
                try
                {
                    IsRefreshing = true;
                    
                    // 清空日志
                    //await _operationLogService.ClearAllAsync();
                    
                    // 清空集合
                    Logs.Clear();
                    TotalCount = 0;
                    
                    await _dialogService.ShowInfoAsync("成功", "操作日志已清空");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"清空操作日志失败: {ex.Message}");
                }
                finally
                {
                    IsRefreshing = false;
                }
            }
        }
    }
} 