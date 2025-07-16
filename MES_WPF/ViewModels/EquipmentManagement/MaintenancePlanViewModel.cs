using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public partial class MaintenancePlanViewModel : ObservableObject
    {
        private readonly IEquipmentMaintenancePlanService _maintenancePlanService;
        private readonly IMaintenanceItemService _maintenanceItemService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private EquipmentMaintenancePlan? _selectedPlan;

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
        private byte _selectedMaintenanceType = 0; // 0:全部, 1:日常保养, 2:定期维护, 3:预防性维护

        [ObservableProperty]
        private byte _selectedStatus = 0; // 0:全部, 1:启用, 2:禁用

        [ObservableProperty]
        private bool _showDuePlansOnly = false;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "设备维护计划";
        
        // 新增/编辑计划相关属性
        [ObservableProperty]
        private bool _isPlanDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private EquipmentMaintenancePlan _editingPlan = new EquipmentMaintenancePlan();
        
        // 维护项目相关属性
        [ObservableProperty]
        private bool _isItemsDialogOpen;
        
        [ObservableProperty]
        private ObservableCollection<MaintenanceItem> _maintenanceItems = new ObservableCollection<MaintenanceItem>();
        
        // 新增/编辑维护项目相关属性
        [ObservableProperty]
        private bool _isItemDialogOpen;
        
        [ObservableProperty]
        private bool _isItemEditMode;
        
        [ObservableProperty]
        private MaintenanceItem _editingItem = new MaintenanceItem();
        
        public ObservableCollection<EquipmentMaintenancePlan> Plans { get; } = new();
        
        public ICollectionView? PlansView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            PlansView?.Refresh();
        }

        partial void OnSelectedMaintenanceTypeChanged(byte value)
        {
            PlansView?.Refresh();
        }

        partial void OnSelectedStatusChanged(byte value)
        {
            PlansView?.Refresh();
        }

        partial void OnShowDuePlansOnlyChanged(bool value)
        {
            PlansView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            PlansView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            PlansView?.Refresh();
        }
        
        public MaintenancePlanViewModel(
            IEquipmentMaintenancePlanService maintenancePlanService,
            IMaintenanceItemService maintenanceItemService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _maintenancePlanService = maintenancePlanService ?? throw new ArgumentNullException(nameof(maintenancePlanService));
            _maintenanceItemService = maintenanceItemService ?? throw new ArgumentNullException(nameof(maintenanceItemService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载维护计划数据
            _ = LoadPlansAsync();
        }
        
        private void SetupFilter()
        {
            PlansView = CollectionViewSource.GetDefaultView(Plans);
            if (PlansView != null)
            {
                PlansView.Filter = PlanFilter;
            }
        }
        
        private bool PlanFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                SelectedMaintenanceType == 0 && 
                SelectedStatus == 0 &&
                !ShowDuePlansOnly)
            {
                return true;
            }
            
            if (obj is EquipmentMaintenancePlan plan)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (plan.PlanCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (plan.PlanName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedMaintenanceType == 0 || plan.MaintenanceType == SelectedMaintenanceType;
                
                bool matchesStatus = SelectedStatus == 0 || plan.Status == SelectedStatus;
                
                bool matchesDue = !ShowDuePlansOnly || 
                                 (plan.NextExecuteDate.HasValue && plan.NextExecuteDate.Value <= DateTime.Now);
                
                bool matchesDate = true;
                if (StartDate.HasValue && plan.CreateTime < StartDate.Value)
                {
                    matchesDate = false;
                }
                if (EndDate.HasValue && plan.CreateTime > EndDate.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesDate = false;
                }
                
                return matchesKeyword && matchesType && matchesStatus && matchesDue && matchesDate;
            }
            
            return false;
        }
        
        private async Task LoadPlansAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Plans.Clear();
                
                // 获取所有维护计划
                var plans = await _maintenancePlanService.GetAllAsync();
                
                // 将维护计划数据添加到集合
                foreach (var plan in plans)
                {
                    Plans.Add(plan);
                }
                
                TotalCount = Plans.Count;
                
                // 刷新视图
                PlansView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护计划数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task RefreshPlans()
        {
            await LoadPlansAsync();
        }
        
        [RelayCommand]
        private async Task SearchPlans()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                PlansView?.Refresh();
                
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
            SelectedMaintenanceType = 0;
            SelectedStatus = 0;
            ShowDuePlansOnly = false;
            StartDate = null;
            EndDate = null;
            
            await SearchPlans();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的维护计划
            var selectedPlans = Plans.Where(p => p == SelectedPlan).ToList();
            
            if (selectedPlans.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的维护计划");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedPlans.Count} 个维护计划吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var plan in selectedPlans)
                    {
                        await _maintenancePlanService.DeleteAsync(plan);
                        Plans.Remove(plan);
                    }
                    
                    TotalCount = Plans.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护计划已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护计划失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportPlans()
        {
            await _dialogService.ShowInfoAsync("导出", "维护计划导出功能尚未实现");
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
        private void AddPlan()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingPlan = new EquipmentMaintenancePlan
            {
                Status = 1, // 默认启用状态
                CreateTime = DateTime.Now,
                CreateBy = 1, // 默认创建人ID，实际应用中应该从登录用户获取
                PlanCode = string.Empty,
                PlanName = string.Empty,
                MaintenanceType = 1, // 默认为日常保养
                CycleType = 1, // 默认为天
                CycleValue = 1, // 默认为1
                StandardTime = 30 // 默认30分钟
            };
            
            // 打开对话框
            IsPlanDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditPlan(EquipmentMaintenancePlan? plan)
        {
            if (plan == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建维护计划对象的副本，避免直接修改原始数据
            EditingPlan = new EquipmentMaintenancePlan
            {
                Id = plan.Id,
                PlanCode = plan.PlanCode,
                PlanName = plan.PlanName,
                EquipmentId = plan.EquipmentId,
                MaintenanceType = plan.MaintenanceType,
                CycleType = plan.CycleType,
                CycleValue = plan.CycleValue,
                StandardTime = plan.StandardTime,
                LastExecuteDate = plan.LastExecuteDate,
                NextExecuteDate = plan.NextExecuteDate,
                Status = plan.Status,
                CreateBy = plan.CreateBy,
                CreateTime = plan.CreateTime,
                UpdateTime = plan.UpdateTime,
                Remark = plan.Remark
            };
            
            // 打开对话框
            IsPlanDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeletePlan(EquipmentMaintenancePlan? plan)
        {
            if (plan == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除维护计划\"{plan.PlanName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _maintenancePlanService.DeleteAsync(plan);
                    Plans.Remove(plan);
                    TotalCount = Plans.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护计划已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护计划失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsPlanDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SavePlan()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingPlan.PlanCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入计划编码");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingPlan.PlanName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入计划名称");
                return;
            }
            
            if (EditingPlan.EquipmentId <= 0)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择设备");
                return;
            }
            
            try
            {
                // 计算下次执行日期
                if (!EditingPlan.NextExecuteDate.HasValue)
                {
                    EditingPlan.NextExecuteDate = CalculateNextExecuteDate(DateTime.Now, EditingPlan.CycleType, EditingPlan.CycleValue);
                }
                
                if (IsEditMode)
                {
                    // 更新维护计划
                    EditingPlan.UpdateTime = DateTime.Now;
                    await _maintenancePlanService.UpdateAsync(EditingPlan);
                    
                    // 更新列表中的维护计划数据
                    var existingPlan = Plans.FirstOrDefault(p => p.Id == EditingPlan.Id);
                    if (existingPlan != null)
                    {
                        int index = Plans.IndexOf(existingPlan);
                        Plans[index] = EditingPlan;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护计划信息已更新");
                }
                else
                {
                    // 创建新维护计划
                    var newPlan = await _maintenancePlanService.AddAsync(EditingPlan);
                    
                    // 添加到维护计划列表
                    Plans.Add(newPlan);
                    TotalCount = Plans.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "维护计划已创建");
                }
                
                // 关闭对话框
                IsPlanDialogOpen = false;
                
                // 刷新视图
                PlansView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存维护计划失败: {ex.Message}");
            }
        }
        
        private DateTime CalculateNextExecuteDate(DateTime baseDate, byte cycleType, int cycleValue)
        {
            return cycleType switch
            {
                1 => baseDate.AddDays(cycleValue), // 天
                2 => baseDate.AddDays(cycleValue * 7), // 周
                3 => baseDate.AddMonths(cycleValue), // 月
                4 => baseDate.AddMonths(cycleValue * 3), // 季度
                5 => baseDate.AddYears(cycleValue), // 年
                _ => baseDate.AddDays(cycleValue)
            };
        }
        
        [RelayCommand]
        private async Task DisablePlan(EquipmentMaintenancePlan? plan)
        {
            if (plan == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用计划", $"确定要禁用维护计划\"{plan.PlanName}\"吗？");
            
            if (result)
            {
                try
                {
                    plan.Status = 2; // 禁用状态
                    plan.UpdateTime = DateTime.Now;
                    await _maintenancePlanService.UpdateAsync(plan);
                    PlansView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "维护计划已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用维护计划失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnablePlan(EquipmentMaintenancePlan? plan)
        {
            if (plan == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用计划", $"确定要启用维护计划\"{plan.PlanName}\"吗？");
            
            if (result)
            {
                try
                {
                    plan.Status = 1; // 启用状态
                    plan.UpdateTime = DateTime.Now;
                    await _maintenancePlanService.UpdateAsync(plan);
                    PlansView?.Refresh();
                    await _dialogService.ShowInfoAsync("成功", "维护计划已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用维护计划失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ViewItems(EquipmentMaintenancePlan? plan)
        {
            if (plan == null) return;
            
            try
            {
                // 设置当前选中的维护计划
                SelectedPlan = plan;
                
                // 清空现有数据
                MaintenanceItems.Clear();
                
                // 获取维护项目
                var items = await _maintenanceItemService.GetSortedItemsByPlanIdAsync(plan.Id);
                
                // 将维护项目添加到集合
                foreach (var item in items)
                {
                    MaintenanceItems.Add(item);
                }
                
                // 打开对话框
                IsItemsDialogOpen = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"获取维护项目失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void CloseItems()
        {
            // 关闭对话框
            IsItemsDialogOpen = false;
        }
        
        [RelayCommand]
        private void AddItem()
        {
            if (SelectedPlan == null) return;
            
            // 重置编辑状态
            IsItemEditMode = false;
            EditingItem = new MaintenanceItem
            {
                MaintenancePlanId = SelectedPlan.Id,
                CreateTime = DateTime.Now,
                ItemCode = string.Empty,
                ItemName = string.Empty,
                ItemType = 1, // 默认为检查
                IsRequired = true, // 默认为必填
                SequenceNo = MaintenanceItems.Count + 1 // 默认序号为当前项目数量+1
            };
            
            // 打开对话框
            IsItemDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditItem(MaintenanceItem? item)
        {
            if (item == null) return;
            
            // 设置编辑状态
            IsItemEditMode = true;
            
            // 创建维护项目对象的副本，避免直接修改原始数据
            EditingItem = new MaintenanceItem
            {
                Id = item.Id,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                MaintenancePlanId = item.MaintenancePlanId,
                ItemType = item.ItemType,
                StandardValue = item.StandardValue,
                UpperLimit = item.UpperLimit,
                LowerLimit = item.LowerLimit,
                Unit = item.Unit,
                Method = item.Method,
                Tool = item.Tool,
                SequenceNo = item.SequenceNo,
                IsRequired = item.IsRequired,
                CreateTime = item.CreateTime,
                UpdateTime = item.UpdateTime,
                Remark = item.Remark
            };
            
            // 打开对话框
            IsItemDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteItem(MaintenanceItem? item)
        {
            if (item == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除维护项目\"{item.ItemName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _maintenanceItemService.DeleteAsync(item);
                    MaintenanceItems.Remove(item);
                    
                    // 重新排序
                    int sequenceNo = 1;
                    foreach (var existingItem in MaintenanceItems.OrderBy(i => i.SequenceNo))
                    {
                        existingItem.SequenceNo = sequenceNo++;
                        await _maintenanceItemService.UpdateAsync(existingItem);
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护项目已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护项目失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelItemEdit()
        {
            // 关闭对话框
            IsItemDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveItem()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingItem.ItemCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入项目编码");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingItem.ItemName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入项目名称");
                return;
            }
            
            try
            {
                if (IsItemEditMode)
                {
                    // 更新维护项目
                    EditingItem.UpdateTime = DateTime.Now;
                    await _maintenanceItemService.UpdateAsync(EditingItem);
                    
                    // 更新列表中的维护项目数据
                    var existingItem = MaintenanceItems.FirstOrDefault(i => i.Id == EditingItem.Id);
                    if (existingItem != null)
                    {
                        int index = MaintenanceItems.IndexOf(existingItem);
                        MaintenanceItems[index] = EditingItem;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护项目信息已更新");
                }
                else
                {
                    // 创建新维护项目
                    var newItem = await _maintenanceItemService.AddAsync(EditingItem);
                    
                    // 添加到维护项目列表
                    MaintenanceItems.Add(newItem);
                    
                    await _dialogService.ShowInfoAsync("成功", "维护项目已创建");
                }
                
                // 关闭对话框
                IsItemDialogOpen = false;
                
                // 重新排序列表
                var sortedItems = new ObservableCollection<MaintenanceItem>(
                    MaintenanceItems.OrderBy(i => i.SequenceNo));
                MaintenanceItems.Clear();
                foreach (var item in sortedItems)
                {
                    MaintenanceItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存维护项目失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task MoveItemUp(MaintenanceItem? item)
        {
            if (item == null) return;
            
            try
            {
                // 查找前一个项目
                var prevItem = MaintenanceItems
                    .OrderBy(i => i.SequenceNo)
                    .LastOrDefault(i => i.SequenceNo < item.SequenceNo);
                
                if (prevItem != null)
                {
                    // 交换序号
                    int tempSeq = item.SequenceNo;
                    item.SequenceNo = prevItem.SequenceNo;
                    prevItem.SequenceNo = tempSeq;
                    
                    // 更新数据库
                    await _maintenanceItemService.UpdateAsync(item);
                    await _maintenanceItemService.UpdateAsync(prevItem);
                    
                    // 重新排序列表
                    var sortedItems = new ObservableCollection<MaintenanceItem>(
                        MaintenanceItems.OrderBy(i => i.SequenceNo));
                    MaintenanceItems.Clear();
                    foreach (var sortedItem in sortedItems)
                    {
                        MaintenanceItems.Add(sortedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动项目失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task MoveItemDown(MaintenanceItem? item)
        {
            if (item == null) return;
            
            try
            {
                // 查找后一个项目
                var nextItem = MaintenanceItems
                    .OrderBy(i => i.SequenceNo)
                    .FirstOrDefault(i => i.SequenceNo > item.SequenceNo);
                
                if (nextItem != null)
                {
                    // 交换序号
                    int tempSeq = item.SequenceNo;
                    item.SequenceNo = nextItem.SequenceNo;
                    nextItem.SequenceNo = tempSeq;
                    
                    // 更新数据库
                    await _maintenanceItemService.UpdateAsync(item);
                    await _maintenanceItemService.UpdateAsync(nextItem);
                    
                    // 重新排序列表
                    var sortedItems = new ObservableCollection<MaintenanceItem>(
                        MaintenanceItems.OrderBy(i => i.SequenceNo));
                    MaintenanceItems.Clear();
                    foreach (var sortedItem in sortedItems)
                    {
                        MaintenanceItems.Add(sortedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动项目失败: {ex.Message}");
            }
        }
    }
}