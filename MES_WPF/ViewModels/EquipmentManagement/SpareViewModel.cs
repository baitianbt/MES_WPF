
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
    public partial class SpareViewModel : ObservableObject
    {
        private readonly ISpareService _spareService;
        private readonly ISpareUsageService _spareUsageService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        
        [ObservableProperty]
        private Spare? _selectedSpare;

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
        private byte _selectedSpareType = 0; // 0:全部, 1:易耗品, 2:维修件, 3:备用件

        [ObservableProperty]
        private bool _showLowStockOnly = false;

        [ObservableProperty]
        private bool _showActiveOnly = false;

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private string _title = "备件管理";
        
        // 新增/编辑备件相关属性
        [ObservableProperty]
        private bool _isSpareDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Spare _editingSpare = new Spare();
        
        // 库存调整相关属性
        [ObservableProperty]
        private bool _isStockAdjustDialogOpen;
        
        [ObservableProperty]
        private decimal _adjustQuantity = 0;
        
        [ObservableProperty]
        private string _adjustReason = string.Empty;
        
        // 备件使用记录相关属性
        [ObservableProperty]
        private bool _isUsageHistoryDialogOpen;
        
        [ObservableProperty]
        private ObservableCollection<SpareUsage> _usageHistory = new ObservableCollection<SpareUsage>();
        
        public ObservableCollection<Spare> Spares { get; } = new();
        
        public ICollectionView? SparesView { get; private set; }

        partial void OnSearchKeywordChanged(string value)
        {
            SparesView?.Refresh();
        }

        partial void OnSelectedSpareTypeChanged(byte value)
        {
            SparesView?.Refresh();
        }

        partial void OnShowLowStockOnlyChanged(bool value)
        {
            SparesView?.Refresh();
        }

        partial void OnShowActiveOnlyChanged(bool value)
        {
            SparesView?.Refresh();
        }

        partial void OnStartDateChanged(DateTime? value)
        {
            SparesView?.Refresh();
        }

        partial void OnEndDateChanged(DateTime? value)
        {
            SparesView?.Refresh();
        }
        
        public SpareViewModel(
            ISpareService spareService,
            ISpareUsageService spareUsageService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _spareService = spareService ?? throw new ArgumentNullException(nameof(spareService));
            _spareUsageService = spareUsageService ?? throw new ArgumentNullException(nameof(spareUsageService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载备件数据
            _ = LoadSparesAsync();
        }
        
        private void SetupFilter()
        {
            SparesView = CollectionViewSource.GetDefaultView(Spares);
            if (SparesView != null)
            {
                SparesView.Filter = SpareFilter;
            }
        }
        
        private bool SpareFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && 
                SelectedSpareType == 0 && 
                !ShowLowStockOnly && 
                !ShowActiveOnly)
            {
                return true;
            }
            
            if (obj is Spare spare)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (spare.SpareCode?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (spare.SpareName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (spare.Specification?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedSpareType == 0 || spare.SpareType == SelectedSpareType;
                
                bool matchesStock = !ShowLowStockOnly || spare.StockQuantity <= spare.MinimumStock;
                
                bool matchesActive = !ShowActiveOnly || spare.IsActive;
                
                bool matchesDate = true;
                if (StartDate.HasValue && spare.CreateTime < StartDate.Value)
                {
                    matchesDate = false;
                }
                if (EndDate.HasValue && spare.CreateTime > EndDate.Value.AddDays(1).AddSeconds(-1))
                {
                    matchesDate = false;
                }
                
                return matchesKeyword && matchesType && matchesStock && matchesActive && matchesDate;
            }
            
            return false;
        }
        
        private async Task LoadSparesAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 清空现有数据
                Spares.Clear();
                
                // 获取所有备件
                var spares = await _spareService.GetAllAsync();
                
                // 将备件数据添加到集合
                foreach (var spare in spares)
                {
                    Spares.Add(spare);
                }
                
                TotalCount = Spares.Count;
                
                // 刷新视图
                SparesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载备件数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task RefreshSpares()
        {
            await LoadSparesAsync();
        }
        
        [RelayCommand]
        private async Task SearchSpares()
        {
            try
            {
                IsRefreshing = true;
                
                // 刷新视图
                SparesView?.Refresh();
                
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
            SelectedSpareType = 0;
            ShowLowStockOnly = false;
            ShowActiveOnly = false;
            StartDate = null;
            EndDate = null;
            
            await SearchSpares();
        }
        
        [RelayCommand]
        private async Task BatchDelete()
        {
            // 获取选中的备件
            var selectedSpares = Spares.Where(s => s == SelectedSpare).ToList();
            
            if (selectedSpares.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先选择要删除的备件");
                return;
            }
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除选中的 {selectedSpares.Count} 个备件吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    foreach (var spare in selectedSpares)
                    {
                        await _spareService.DeleteAsync(spare);
                        Spares.Remove(spare);
                    }
                    
                    TotalCount = Spares.Count;
                    await _dialogService.ShowInfoAsync("成功", "备件已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除备件失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task ExportSpares()
        {
            await _dialogService.ShowInfoAsync("导出", "备件导出功能尚未实现");
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
        private void AddSpare()
        {
            // 重置编辑状态
            IsEditMode = false;
            EditingSpare = new Spare
            {
                IsActive = true, // 默认有效状态
                CreateTime = DateTime.Now,
                SpareCode = string.Empty,
                SpareName = string.Empty,
                SpareType = 1, // 默认为易耗品
                Unit = "个", // 默认单位
                StockQuantity = 0,
                MinimumStock = 0
            };
            
            // 打开对话框
            IsSpareDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditSpare(Spare? spare)
        {
            if (spare == null) return;
            
            // 设置编辑状态
            IsEditMode = true;
            
            // 创建备件对象的副本，避免直接修改原始数据
            EditingSpare = new Spare
            {
                Id = spare.Id,
                SpareCode = spare.SpareCode,
                SpareName = spare.SpareName,
                SpareType = spare.SpareType,
                Specification = spare.Specification,
                Unit = spare.Unit,
                StockQuantity = spare.StockQuantity,
                MinimumStock = spare.MinimumStock,
                Price = spare.Price,
                Supplier = spare.Supplier,
                LeadTime = spare.LeadTime,
                Location = spare.Location,
                IsActive = spare.IsActive,
                CreateTime = spare.CreateTime,
                UpdateTime = spare.UpdateTime,
                Remark = spare.Remark
            };
            
            // 打开对话框
            IsSpareDialogOpen = true;
        }
        
        [RelayCommand]
        private async Task DeleteSpare(Spare? spare)
        {
            if (spare == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除备件\"{spare.SpareName}\"吗？此操作不可撤销。");
            
            if (result)
            {
                try
                {
                    await _spareService.DeleteAsync(spare);
                    Spares.Remove(spare);
                    TotalCount = Spares.Count;
                    await _dialogService.ShowInfoAsync("成功", "备件已删除");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除备件失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            // 关闭对话框
            IsSpareDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveSpare()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingSpare.SpareCode))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入备件编码");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingSpare.SpareName))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入备件名称");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(EditingSpare.Unit))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入单位");
                return;
            }
            
            try
            {
                if (IsEditMode)
                {
                    // 更新备件
                    EditingSpare.UpdateTime = DateTime.Now;
                    await _spareService.UpdateAsync(EditingSpare);
                    
                    // 更新列表中的备件数据
                    var existingSpare = Spares.FirstOrDefault(s => s.Id == EditingSpare.Id);
                    if (existingSpare != null)
                    {
                        int index = Spares.IndexOf(existingSpare);
                        Spares[index] = EditingSpare;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "备件信息已更新");
                }
                else
                {
                    // 创建新备件
                    var newSpare = await _spareService.AddAsync(EditingSpare);
                    
                    // 添加到备件列表
                    Spares.Add(newSpare);
                    TotalCount = Spares.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "备件已创建");
                }
                
                // 关闭对话框
                IsSpareDialogOpen = false;
                
                // 刷新视图
                SparesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存备件失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void OpenStockAdjustDialog(Spare? spare)
        {
            if (spare == null) return;
            
            // 设置当前编辑的备件
            SelectedSpare = spare;
            
            // 重置调整数量和原因
            AdjustQuantity = 0;
            AdjustReason = string.Empty;
            
            // 打开对话框
            IsStockAdjustDialogOpen = true;
        }
        
        [RelayCommand]
        private void CancelStockAdjust()
        {
            // 关闭对话框
            IsStockAdjustDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveStockAdjust()
        {
            if (SelectedSpare == null) return;
            
            if (AdjustQuantity == 0)
            {
                await _dialogService.ShowErrorAsync("错误", "调整数量不能为0");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(AdjustReason))
            {
                await _dialogService.ShowErrorAsync("错误", "请输入调整原因");
                return;
            }
            
            try
            {
                // 更新库存
                var updatedSpare = await _spareService.UpdateStockQuantityAsync(SelectedSpare.Id, AdjustQuantity);
                
                // 更新列表中的备件数据
                var existingSpare = Spares.FirstOrDefault(s => s.Id == updatedSpare.Id);
                if (existingSpare != null)
                {
                    int index = Spares.IndexOf(existingSpare);
                    Spares[index] = updatedSpare;
                }
                
                // 关闭对话框
                IsStockAdjustDialogOpen = false;
                
                // 刷新视图
                SparesView?.Refresh();
                
                await _dialogService.ShowInfoAsync("成功", "库存已调整");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"调整库存失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task ViewUsageHistory(Spare? spare)
        {
            if (spare == null) return;
            
            try
            {
                // 设置当前选中的备件
                SelectedSpare = spare;
                
                // 清空现有数据
                UsageHistory.Clear();
                
                // 获取备件使用记录
                var usageRecords = await _spareUsageService.GetBySpareIdAsync(spare.Id);
                
                // 将使用记录添加到集合
                foreach (var record in usageRecords)
                {
                    UsageHistory.Add(record);
                }
                
                // 打开对话框
                IsUsageHistoryDialogOpen = true;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"获取使用记录失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private void CloseUsageHistory()
        {
            // 关闭对话框
            IsUsageHistoryDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task DisableSpare(Spare? spare)
        {
            if (spare == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("禁用备件", $"确定要禁用备件\"{spare.SpareName}\"吗？");
            
            if (result)
            {
                try
                {
                    var updatedSpare = await _spareService.SetActiveStatusAsync(spare.Id, false);
                    
                    // 更新列表中的备件数据
                    var existingSpare = Spares.FirstOrDefault(s => s.Id == updatedSpare.Id);
                    if (existingSpare != null)
                    {
                        int index = Spares.IndexOf(existingSpare);
                        Spares[index] = updatedSpare;
                    }
                    
                    // 刷新视图
                    SparesView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "备件已禁用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"禁用备件失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task EnableSpare(Spare? spare)
        {
            if (spare == null) return;
            
            var result = await _dialogService.ShowConfirmAsync("启用备件", $"确定要启用备件\"{spare.SpareName}\"吗？");
            
            if (result)
            {
                try
                {
                    var updatedSpare = await _spareService.SetActiveStatusAsync(spare.Id, true);
                    
                    // 更新列表中的备件数据
                    var existingSpare = Spares.FirstOrDefault(s => s.Id == updatedSpare.Id);
                    if (existingSpare != null)
                    {
                        int index = Spares.IndexOf(existingSpare);
                        Spares[index] = updatedSpare;
                    }
                    
                    // 刷新视图
                    SparesView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "备件已启用");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"启用备件失败: {ex.Message}");
                }
            }
        }
    }
}