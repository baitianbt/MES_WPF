using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Core.Services.EquipmentManagement;
using MES_WPF.Model.BasicInformation;
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
    public partial class ParameterLogViewModel : ObservableObject
    {
        private readonly IEquipmentParameterLogService _parameterLogService;
        private readonly IEquipmentService _equipmentService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private EquipmentParameterLog? _selectedParameterLog;

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
        private int? _selectedEquipmentId;

        [ObservableProperty]
        private string _selectedParameterCode = string.Empty;

        [ObservableProperty]
        private bool? _isAlarm;

        [ObservableProperty]
        private byte _selectedAlarmLevel = 0; // 0:全部, 1:提示, 2:警告, 3:严重

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "设备参数记录";
        
        // 新增/编辑参数记录相关属性
        [ObservableProperty]
        private bool _isParameterLogDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private EquipmentParameterLog _editingParameterLog = new EquipmentParameterLog();
        
        [ObservableProperty]
        private ObservableCollection<Equipment> _equipments = new ObservableCollection<Equipment>();
        
        [ObservableProperty]
        private ObservableCollection<string> _parameterCodes = new ObservableCollection<string>();
        
        // 参数趋势图相关属性
        [ObservableProperty]
        private bool _isTrendDialogOpen;
        
        [ObservableProperty]
        private List<EquipmentParameterLog> _trendData = new List<EquipmentParameterLog>();
        
        [ObservableProperty]
        private string _trendTitle = string.Empty;
        
        public ObservableCollection<EquipmentParameterLog> ParameterLogs { get; } = new();
        
        public ICollectionView? ParameterLogsView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            ParameterLogsView?.Refresh();
        }

        partial void OnSelectedEquipmentIdChanged(int? value)
        {
            ParameterLogsView?.Refresh();
            // 当设备ID变化时，加载该设备的参数代码列表
            if (value.HasValue)
            {
                _ = LoadParameterCodesAsync(value.Value);
            }
            else
            {
                ParameterCodes.Clear();
            }
        }

        partial void OnSelectedParameterCodeChanged(string value)
        {
            ParameterLogsView?.Refresh();
        }

        partial void OnIsAlarmChanged(bool? value)
        {
            ParameterLogsView?.Refresh();
        }

        partial void OnSelectedAlarmLevelChanged(byte value)
        {
            ParameterLogsView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            ParameterLogsView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            ParameterLogsView?.Refresh();
        }
        
        public ParameterLogViewModel(
            IEquipmentParameterLogService parameterLogService,
            IEquipmentService equipmentService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _parameterLogService = parameterLogService ?? throw new ArgumentNullException(nameof(parameterLogService));
            _equipmentService = equipmentService ?? throw new ArgumentNullException(nameof(equipmentService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载设备参数记录数据
            _ = LoadParameterLogsAsync();
            
            // 加载设备数据
            _ = LoadEquipmentsAsync();
        }
        
        private void SetupFilter()
        {
            ParameterLogsView = CollectionViewSource.GetDefaultView(ParameterLogs);
            if (ParameterLogsView != null)
            {
                ParameterLogsView.Filter = ParameterLogFilter;
            }
        }
        
        private bool ParameterLogFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                !SelectedEquipmentId.HasValue &&
                string.IsNullOrWhiteSpace(SelectedParameterCode) &&
                !IsAlarm.HasValue &&
                SelectedAlarmLevel == 0 &&
                !StartDate.HasValue && 
                !EndDate.HasValue)
            {
                return true;
            }
            
            if (obj is EquipmentParameterLog log)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (log.ParameterName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (log.ParameterValue?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (log.Unit?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesEquipment = !SelectedEquipmentId.HasValue || log.EquipmentId == SelectedEquipmentId.Value;
                
                bool matchesParameterCode = string.IsNullOrWhiteSpace(SelectedParameterCode) || 
                                          log.ParameterCode == SelectedParameterCode;
                
                bool matchesAlarmStatus = !IsAlarm.HasValue || log.IsAlarm == IsAlarm.Value;
                
                bool matchesAlarmLevel = SelectedAlarmLevel == 0 || 
                                       (log.AlarmLevel.HasValue && log.AlarmLevel.Value == SelectedAlarmLevel);
                
                bool matchesDateRange = true;
                if (StartDate.HasValue && log.CollectTime < StartDate.Value)
                {
                    matchesDateRange = false;
                }
                if (EndDate.HasValue && log.CollectTime > EndDate.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesDateRange = false;
                }
                
                return matchesKeyword && matchesEquipment && matchesParameterCode && 
                       matchesAlarmStatus && matchesAlarmLevel && matchesDateRange;
            }
            
            return false;
        }
        
        private async Task LoadParameterLogsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                ParameterLogs.Clear();
                
                // 获取所有设备参数记录
                var logs = await _parameterLogService.GetAllAsync();
                
                // 将设备参数记录数据添加到集合
                foreach (var log in logs)
                {
                    ParameterLogs.Add(log);
                }
                
                TotalCount = ParameterLogs.Count;
                
                // 刷新视图
                ParameterLogsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载设备参数记录失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadEquipmentsAsync()
        {
            try
            {
                // 清空现有数据
                Equipments.Clear();
                
                // 从服务获取设备数据
                var equipments = await _equipmentService.GetAllAsync();
                
                // 将设备数据添加到集合
                foreach (var equipment in equipments)
                {
                    Equipments.Add(equipment);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载设备数据失败: {ex.Message}");
                
                // 加载失败时添加一些默认设备
                Equipments.Clear();
                Equipments.Add(new Equipment { Id = 1, ResourceId = 1, SerialNumber = "EQ001" });
                Equipments.Add(new Equipment { Id = 2, ResourceId = 2, SerialNumber = "EQ002" });
                Equipments.Add(new Equipment { Id = 3, ResourceId = 3, SerialNumber = "EQ003" });
            }
        }
        
        private async Task LoadParameterCodesAsync(int equipmentId)
        {
            try
            {
                // 清空现有数据
                ParameterCodes.Clear();
                
                // 获取特定设备的参数代码列表
                var logs = await _parameterLogService.GetByEquipmentIdAsync(equipmentId);
                var codes = logs.Select(l => l.ParameterCode).Distinct().ToList();
                
                // 将参数代码添加到集合
                foreach (var code in codes)
                {
                    ParameterCodes.Add(code);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载参数代码失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshParameterLogs()
        {
            await LoadParameterLogsAsync();
        }
        
        [RelayCommand]
        private async Task SearchParameterLogs()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                ParameterLogsView?.Refresh();
                
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
            SelectedEquipmentId = null;
            SelectedParameterCode = string.Empty;
            IsAlarm = null;
            SelectedAlarmLevel = 0;
            StartDate = null;
            EndDate = null;
            
            await SearchParameterLogs();
        }
        
        [RelayCommand]
        private async Task ExportParameterLogs()
        {
            await _dialogService.ShowInfoAsync("导出", "设备参数记录导出功能尚未实现");
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
        private void AddParameterLog()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingParameterLog = new EquipmentParameterLog
            {
                CollectTime = DateTime.Now,
                CreateTime = DateTime.Now,
                IsAlarm = false
            };
            
            // 打开对话框
            IsParameterLogDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditParameterLog(EquipmentParameterLog? log)
        {
            if (log == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建参数记录对象的副本，避免直接修改原始数据
            EditingParameterLog = new EquipmentParameterLog
            {
                Id = log.Id,
                EquipmentId = log.EquipmentId,
                ParameterCode = log.ParameterCode,
                ParameterName = log.ParameterName,
                ParameterValue = log.ParameterValue,
                Unit = log.Unit,
                CollectTime = log.CollectTime,
                IsAlarm = log.IsAlarm,
                AlarmLevel = log.AlarmLevel,
                CreateTime = log.CreateTime
            };
            
            // 打开对话框
            IsParameterLogDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteParameterLog(EquipmentParameterLog? log)
        {
            if (log == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除该设备参数记录吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _parameterLogService.DeleteAsync(log);
                    ParameterLogs.Remove(log);
                    TotalCount = ParameterLogs.Count;
                    await _dialogService.ShowInfoAsync("成功", "设备参数记录已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除设备参数记录失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsParameterLogDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveParameterLog()
        {
            // 验证必填字段
            if (EditingParameterLog.EquipmentId <= 0)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择设备");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingParameterLog.ParameterCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入参数代码");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingParameterLog.ParameterName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入参数名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingParameterLog.ParameterValue))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入参数值");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新参数记录
                    await _parameterLogService.UpdateAsync(EditingParameterLog);
                    
                    // 更新列表中的参数记录数据
                    var existingLog = ParameterLogs.FirstOrDefault(l => l.Id == EditingParameterLog.Id);
                    if (existingLog != null)
                    {
                        int index = ParameterLogs.IndexOf(existingLog);
                        ParameterLogs[index] = EditingParameterLog;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "设备参数记录已更新");
                }
                else
                {
                    // 创建新参数记录
                    var newLog = await _parameterLogService.AddAsync(EditingParameterLog);
                    
                    // 添加到参数记录列表
                    ParameterLogs.Add(newLog);
                    TotalCount = ParameterLogs.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "设备参数记录已创建");
                }
                
                // 关闭对话框
                IsParameterLogDialogOpen = false;
                
                // 刷新视图
                ParameterLogsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存设备参数记录失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task ViewTrend(EquipmentParameterLog? log)
        {
            if (log == null) return;
            
            try
            {
                IsRefreshing = true;
                
                // 获取过去30天的趋势数据
                var startDate = DateTime.Now.AddDays(-30);
                var endDate = DateTime.Now;
                
                // 获取趋势数据
                var trendData = await _parameterLogService.GetParameterTrendAsync(
                    log.EquipmentId, 
                    log.ParameterCode, 
                    startDate, 
                    endDate);
                
                // 设置趋势数据
                TrendData = trendData.ToList();
                
                // 设置趋势图标题
                TrendTitle = $"{log.ParameterName} 趋势图";
                
                // 打开趋势图对话框
                IsTrendDialogOpen = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"获取趋势数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private void CloseTrend()
        {
            // 关闭趋势图对话框
            IsTrendDialogOpen = false;
        }
    }
} 