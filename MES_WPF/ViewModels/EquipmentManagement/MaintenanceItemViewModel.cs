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
    public partial class MaintenanceItemViewModel : ObservableObject
    {
        private readonly IMaintenanceItemService _itemService;
        private readonly IEquipmentMaintenancePlanService _planService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private MaintenanceItem? _selectedItem;

        [ObservableProperty]
        private string _searchKeyword = string.Empty;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private byte _selectedItemType = 0; // 0:全部, 1:检查, 2:清洁, 3:润滑, 4:更换, 5:调整

        [ObservableProperty]
        private int? _selectedPlanId;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "维护项目管理";
        
        // 新增/编辑维护项目相关属性
        [ObservableProperty]
        private bool _isItemDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private MaintenanceItem _editingItem = new MaintenanceItem();
        
        [ObservableProperty]
        private ObservableCollection<EquipmentMaintenancePlan> _maintenancePlans = new ObservableCollection<EquipmentMaintenancePlan>();
        
        public ObservableCollection<MaintenanceItem> Items { get; } = new();
        
        public ICollectionView? ItemsView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            ItemsView?.Refresh();
        }

        partial void OnSelectedItemTypeChanged(byte value)
        {
            ItemsView?.Refresh();
        }

        partial void OnSelectedPlanIdChanged(int? value)
        {
            ItemsView?.Refresh();
        }
        
        public MaintenanceItemViewModel(
            IMaintenanceItemService itemService,
            IEquipmentMaintenancePlanService planService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _planService = planService ?? throw new ArgumentNullException(nameof(planService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载维护项目数据
            _ = LoadItemsAsync();
            
            // 加载维护计划数据
            _ = LoadMaintenancePlansAsync();
        }
        
        private void SetupFilter()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            if (ItemsView != null)
            {
                ItemsView.Filter = ItemFilter;
            }
        }
        
        private bool ItemFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedItemType == 0 && !SelectedPlanId.HasValue)
            {
                return true;
            }
            
            if (obj is MaintenanceItem item)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (item.ItemCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (item.ItemName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (item.Method?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedItemType == 0 || item.ItemType == SelectedItemType;
                
                bool matchesPlan = !SelectedPlanId.HasValue || item.MaintenancePlanId == SelectedPlanId.Value;
                
                return matchesKeyword && matchesType && matchesPlan;
            }
            
            return false;
        }
        
        private async Task LoadItemsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Items.Clear();
                
                // 获取所有维护项目
                var items = await _itemService.GetAllAsync();
                
                // 将维护项目数据添加到集合
                foreach (var item in items)
                {
                    Items.Add(item);
                }
                
                TotalCount = Items.Count;
                
                // 刷新视图
                ItemsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护项目数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadMaintenancePlansAsync()
        {
            try
            {
                // 清空现有数据
                MaintenancePlans.Clear();
                
                // 添加"全部"选项
                MaintenancePlans.Add(new EquipmentMaintenancePlan { Id = 0, PlanName = "全部" });
                
                // 从服务获取维护计划数据
                var plans = await _planService.GetAllAsync();
                
                // 将维护计划数据添加到集合
                foreach (var plan in plans)
                {
                    MaintenancePlans.Add(plan);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载维护计划数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshItems()
        {
            await LoadItemsAsync();
        }
        
        [RelayCommand]
        private async Task SearchItems()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                ItemsView?.Refresh();
                
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
            SelectedItemType = 0;
            SelectedPlanId = null;
            
            await SearchItems();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的维护项目
            var selectedItems = Items.Where(i => i == SelectedItem).ToList();
            
            if (selectedItems.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的维护项目");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedItems.Count} 个维护项目吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var item in selectedItems)
                    {
                        await _itemService.DeleteAsync(item);
                        Items.Remove(item);
                    }
                    
                    TotalCount = Items.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护项目已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护项目失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportItems()
        {
            await _dialogService.ShowInfoAsync("导出", "维护项目导出功能尚未实现");
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
        private void AddItem()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingItem = new MaintenanceItem
            {
                CreateTime = DateTime.Now,
                ItemType = 1, // 默认检查
                SequenceNo = 1,
                IsRequired = true,
                ItemCode = GenerateNewItemCode(),
                ItemName = string.Empty
            };
            
            // 打开对话框
            IsItemDialogOpen = true;
        }
        
        private string GenerateNewItemCode()
        {
            // 生成新的项目编码，格式：MI + 当前年月日 + 4位序号
            string dateStr = DateTime.Now.ToString("yyyyMMdd");
            int count = Items.Count + 1;
            return $"MI{dateStr}{count:D4}";
        }
        
        [RelayCommand]
        private void EditItem(MaintenanceItem? item)
        {
            if (item == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
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
                    await _itemService.DeleteAsync(item);
                    Items.Remove(item);
                    TotalCount = Items.Count;
                    await _dialogService.ShowInfoAsync("成功", "维护项目已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除维护项目失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsItemDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveItem()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingItem.ItemName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入项目名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingItem.ItemCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入项目编码");
                return;
            }
            
            if (EditingItem.MaintenancePlanId <= 0)
            {
                await _dialogService.ShowErrorAsync("错误", "请选择维护计划");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新维护项目
                    EditingItem.UpdateTime = DateTime.Now;
                    await _itemService.UpdateAsync(EditingItem);
                    
                    // 更新列表中的维护项目数据
                    var existingItem = Items.FirstOrDefault(i => i.Id == EditingItem.Id);
                    if (existingItem != null)
                    {
                        int index = Items.IndexOf(existingItem);
                        Items[index] = EditingItem;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "维护项目信息已更新");
                }
                else
                {
                    // 创建新维护项目
                    EditingItem.CreateTime = DateTime.Now;
                    var newItem = await _itemService.AddAsync(EditingItem);
                    
                    // 添加到维护项目列表
                    Items.Add(newItem);
                    TotalCount = Items.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "维护项目已创建");
                }
                
                // 关闭对话框
                IsItemDialogOpen = false;
                
                // 刷新视图
                ItemsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存维护项目失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task MoveUp(MaintenanceItem? item)
        {
            if (item == null) return;
            
            // 获取相同计划的项目并按序号排序
            var planItems = Items.Where(i => i.MaintenancePlanId == item.MaintenancePlanId)
                                .OrderBy(i => i.SequenceNo)
                                .ToList();
            
            int index = planItems.IndexOf(item);
            if (index <= 0) return; // 已经是第一个，无法上移
            
            // 交换序号
            var prevItem = planItems[index - 1];
            int tempSeq = item.SequenceNo;
            item.SequenceNo = prevItem.SequenceNo;
            prevItem.SequenceNo = tempSeq;
            
            try
            {
                // 更新数据库
                await _itemService.UpdateAsync(item);
                await _itemService.UpdateAsync(prevItem);
                
                // 刷新列表
                await LoadItemsAsync();
                
                // 保持当前项选中
                SelectedItem = item;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动项目失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task MoveDown(MaintenanceItem? item)
        {
            if (item == null) return;
            
            // 获取相同计划的项目并按序号排序
            var planItems = Items.Where(i => i.MaintenancePlanId == item.MaintenancePlanId)
                                .OrderBy(i => i.SequenceNo)
                                .ToList();
            
            int index = planItems.IndexOf(item);
            if (index >= planItems.Count - 1) return; // 已经是最后一个，无法下移
            
            // 交换序号
            var nextItem = planItems[index + 1];
            int tempSeq = item.SequenceNo;
            item.SequenceNo = nextItem.SequenceNo;
            nextItem.SequenceNo = tempSeq;
            
            try
            {
                // 更新数据库
                await _itemService.UpdateAsync(item);
                await _itemService.UpdateAsync(nextItem);
                
                // 刷新列表
                await LoadItemsAsync();
                
                // 保持当前项选中
                SelectedItem = item;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动项目失败: {ex.Message}");
            }
        }
    }
}