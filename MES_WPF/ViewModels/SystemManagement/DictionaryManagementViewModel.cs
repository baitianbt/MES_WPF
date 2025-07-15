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
    public partial class DictionaryManagementViewModel : ObservableObject
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly IDictionaryItemService _dictionaryItemService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private Dictionary? _selectedDictionary;
        
        [ObservableProperty]
        private string _searchKeyword = string.Empty;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private int _totalCount;
        
        [ObservableProperty]
        private string _title = "字典管理";
        
        // 字典对话框相关属性
        [ObservableProperty]
        private bool _isDictionaryDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private Dictionary _editingDictionary = new Dictionary();
        
        // 字典项对话框相关属性
        [ObservableProperty]
        private bool _isDictItemDialogOpen;
        
        [ObservableProperty]
        private DictionaryItem _editingDictItem = new DictionaryItem();
        
        [ObservableProperty]
        private bool _isDictItemEditMode;
        
        [ObservableProperty]
        private DictionaryItem? _selectedDictItem;
        
        public ObservableCollection<Dictionary> Dictionaries { get; } = new();
        
        public ObservableCollection<DictionaryItem> DictionaryItems { get; } = new();
        
        public ICollectionView? DictionariesView { get; private set; }
        
        public ICollectionView? DictionaryItemsView { get; private set; }
        
        partial void OnSearchKeywordChanged(string value)
        {
            DictionariesView?.Refresh();
        }
        
        partial void OnSelectedDictionaryChanged(Dictionary? value)
        {
            if (value != null)
            {
                _ = LoadDictionaryItemsAsync(value.Id);
            }
            else
            {
                DictionaryItems.Clear();
            }
        }
        
        public DictionaryManagementViewModel(
            IDictionaryService dictionaryService,
            IDictionaryItemService dictionaryItemService,
            IDialogService dialogService)
        {
            _dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));
            _dictionaryItemService = dictionaryItemService ?? throw new ArgumentNullException(nameof(dictionaryItemService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilters();
            
            // 加载字典数据
            _ = LoadDictionariesAsync();
        }
        
        private void SetupFilters()
        {
            // 字典过滤器
            DictionariesView = CollectionViewSource.GetDefaultView(Dictionaries);
            if (DictionariesView != null)
            {
                DictionariesView.Filter = DictionaryFilter;
            }
            
            // 字典项过滤器
            DictionaryItemsView = CollectionViewSource.GetDefaultView(DictionaryItems);
        }
        
        private bool DictionaryFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                return true;
            }
            
            if (obj is Dictionary dictionary)
            {
                return (dictionary.DictName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       (dictionary.DictType?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       (dictionary.Remark?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
            }
            
            return false;
        }
        
        private async Task LoadDictionariesAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // 获取所有字典
                var dictionaries = await _dictionaryService.GetAllAsync();
                
                // 在UI线程更新集合
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // 清空现有数据
                    Dictionaries.Clear();
                    
                    // 将字典数据添加到集合
                    foreach (var dictionary in dictionaries)
                    {
                        Dictionaries.Add(dictionary);
                    }
                    
                    TotalCount = Dictionaries.Count;
                    
                    // 刷新视图
                    DictionariesView?.Refresh();
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载字典数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        private async Task LoadDictionaryItemsAsync(int dictId)
        {
            try
            {
                // 获取指定字典的所有字典项
                var items = await _dictionaryItemService.GetByDictIdAsync(dictId);
                
                // 在UI线程更新集合
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // 清空现有数据
                    DictionaryItems.Clear();
                    
                    // 将字典项数据添加到集合，按排序号排序
                    foreach (var item in items.OrderBy(i => i.SortOrder))
                    {
                        DictionaryItems.Add(item);
                    }
                    
                    // 刷新视图
                    DictionaryItemsView?.Refresh();
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载字典项数据失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task RefreshDictionaries()
        {
            // 清除搜索条件
            SearchKeyword = string.Empty;
            
            // 重新加载数据
            await LoadDictionariesAsync();
            
            // 如果已选择字典，则刷新字典项
            if (SelectedDictionary != null)
            {
                await LoadDictionaryItemsAsync(SelectedDictionary.Id);
            }
        }
        
        [RelayCommand]
        private void ResetSearch()
        {
            SearchKeyword = string.Empty;
            
            // 刷新视图显示所有数据
            DictionariesView?.Refresh();
        }
        
        [RelayCommand]
        private void AddDictionary()
        {
            // 创建新字典
            EditingDictionary = new Dictionary
            {
                DictType = "DICT_",
                Status = 1,
                CreateTime = DateTime.Now,
                CreateBy = 1 // 当前用户ID，实际应用中应从认证服务获取
            };
            
            IsEditMode = false;
            IsDictionaryDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditDictionary(Dictionary? dictionary)
        {
            if (dictionary == null)
                return;
                
            // 编辑现有字典
            EditingDictionary = new Dictionary
            {
                Id = dictionary.Id,
                DictType = dictionary.DictType,
                DictName = dictionary.DictName,
                Status = dictionary.Status,
                CreateBy = dictionary.CreateBy,
                CreateTime = dictionary.CreateTime,
                UpdateTime = DateTime.Now,
                Remark = dictionary.Remark
            };
            
            IsEditMode = true;
            IsDictionaryDialogOpen = true;
        }
        
        [RelayCommand]
        private void CancelDictEdit()
        {
            IsDictionaryDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveDictionary()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingDictionary.DictType) || 
                    string.IsNullOrWhiteSpace(EditingDictionary.DictName))
                {
                    await _dialogService.ShowErrorAsync("错误", "字典类型和字典名称不能为空");
                    return;
                }
                
                if (IsEditMode)
                {
                    // 更新字典
                    EditingDictionary.UpdateTime = DateTime.Now;
                    await _dictionaryService.UpdateAsync(EditingDictionary);
                    
                    // 更新列表中对应的项
                    var existingDict = Dictionaries.FirstOrDefault(d => d.Id == EditingDictionary.Id);
                    if (existingDict != null)
                    {
                        existingDict.DictType = EditingDictionary.DictType;
                        existingDict.DictName = EditingDictionary.DictName;
                        existingDict.Status = EditingDictionary.Status;
                        existingDict.UpdateTime = EditingDictionary.UpdateTime;
                        existingDict.Remark = EditingDictionary.Remark;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "字典更新成功");
                }
                else
                {
                    // 检查字典类型是否存在
                    var existingDict = Dictionaries.FirstOrDefault(d => d.DictType.Equals(EditingDictionary.DictType, StringComparison.OrdinalIgnoreCase));
                    if (existingDict != null)
                    {
                        await _dialogService.ShowErrorAsync("错误", "字典类型已存在");
                        return;
                    }
                    
                    // 创建新字典
                    var newDict = await _dictionaryService.AddAsync(EditingDictionary);
                    
                    // 添加到集合
                    Dictionaries.Add(newDict);
                    TotalCount = Dictionaries.Count;
                    
                    await _dialogService.ShowInfoAsync("成功", "字典添加成功");
                }
                
                // 关闭对话框
                IsDictionaryDialogOpen = false;
                
                // 刷新视图
                DictionariesView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存字典失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task DeleteDictionary(Dictionary? dictionary)
        {
            if (dictionary == null)
                return;
                
            // 确认删除
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除字典\"{dictionary.DictName}\"吗？此操作将同时删除所有相关字典项，且不可恢复。");
            if (result)
            {
                try
                {
                    // 删除字典
                    await _dictionaryService.DeleteByIdAsync(dictionary.Id);
                    
                    // 从集合中移除
                    Dictionaries.Remove(dictionary);
                    TotalCount = Dictionaries.Count;
                    
                    // 清空字典项列表
                    if (SelectedDictionary?.Id == dictionary.Id)
                    {
                        SelectedDictionary = null;
                        DictionaryItems.Clear();
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "字典删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除字典失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private void AddDictItem()
        {
            if (SelectedDictionary == null)
            {
                _dialogService.ShowErrorAsync("提示", "请先选择一个字典");
                return;
            }
            
            // 获取当前最大排序号
            int maxSortOrder = DictionaryItems.Any() ? DictionaryItems.Max(i => i.SortOrder) : 0;
            
            // 创建新字典项
            EditingDictItem = new DictionaryItem
            {
                DictId = SelectedDictionary.Id,
                SortOrder = maxSortOrder + 10,
                Status = 1,
                CreateTime = DateTime.Now
            };
            
            IsDictItemEditMode = false;
            IsDictItemDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditDictItem(DictionaryItem? dictItem)
        {
            if (dictItem == null)
                return;
                
            // 编辑现有字典项
            EditingDictItem = new DictionaryItem
            {
                Id = dictItem.Id,
                DictId = dictItem.DictId,
                ItemValue = dictItem.ItemValue,
                ItemText = dictItem.ItemText,
                ItemDesc = dictItem.ItemDesc,
                SortOrder = dictItem.SortOrder,
                Status = dictItem.Status,
                CreateTime = dictItem.CreateTime,
                UpdateTime = DateTime.Now,
                Remark = dictItem.Remark
            };
            
            IsDictItemEditMode = true;
            IsDictItemDialogOpen = true;
        }
        
        [RelayCommand]
        private void CancelDictItemEdit()
        {
            IsDictItemDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveDictItem()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingDictItem.ItemValue) || 
                    string.IsNullOrWhiteSpace(EditingDictItem.ItemText))
                {
                    await _dialogService.ShowErrorAsync("错误", "字典项值和字典项文本不能为空");
                    return;
                }
                
                if (IsDictItemEditMode)
                {
                    // 更新字典项
                    EditingDictItem.UpdateTime = DateTime.Now;
                    await _dictionaryItemService.UpdateAsync(EditingDictItem);
                    
                    // 更新列表中对应的项
                    var existingItem = DictionaryItems.FirstOrDefault(i => i.Id == EditingDictItem.Id);
                    if (existingItem != null)
                    {
                        existingItem.ItemValue = EditingDictItem.ItemValue;
                        existingItem.ItemText = EditingDictItem.ItemText;
                        existingItem.ItemDesc = EditingDictItem.ItemDesc;
                        existingItem.SortOrder = EditingDictItem.SortOrder;
                        existingItem.Status = EditingDictItem.Status;
                        existingItem.UpdateTime = EditingDictItem.UpdateTime;
                        existingItem.Remark = EditingDictItem.Remark;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "字典项更新成功");
                }
                else
                {
                    // 检查字典项值在当前字典下是否存在
                    var existingItem = DictionaryItems.FirstOrDefault(i => 
                        i.DictId == EditingDictItem.DictId && 
                        i.ItemValue.Equals(EditingDictItem.ItemValue, StringComparison.OrdinalIgnoreCase));
                        
                    if (existingItem != null)
                    {
                        await _dialogService.ShowErrorAsync("错误", "字典项值在当前字典下已存在");
                        return;
                    }
                    
                    // 创建新字典项
                    var newItem = await _dictionaryItemService.AddAsync(EditingDictItem);
                    
                    // 添加到集合
                    DictionaryItems.Add(newItem);
                    
                    // 按排序号重新排序
                    DictionaryItemsView?.Refresh();
                    
                    await _dialogService.ShowInfoAsync("成功", "字典项添加成功");
                }
                
                // 关闭对话框
                IsDictItemDialogOpen = false;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存字典项失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task DeleteDictItem(DictionaryItem? dictItem)
        {
            if (dictItem == null)
                return;
                
            // 确认删除
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除字典项\"{dictItem.ItemText}\"吗？");
            if (result)
            {
                try
                {
                    // 删除字典项
                    await _dictionaryItemService.DeleteByIdAsync(dictItem.Id);
                    
                    // 从集合中移除
                    DictionaryItems.Remove(dictItem);
                    
                    await _dialogService.ShowInfoAsync("成功", "字典项删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除字典项失败: {ex.Message}");
                }
            }
        }
        
        [RelayCommand]
        private async Task MoveDictItemUp(DictionaryItem? dictItem)
        {
            if (dictItem == null || DictionaryItems.Count <= 1)
                return;
                
            int currentIndex = DictionaryItems.IndexOf(dictItem);
            if (currentIndex <= 0)
                return;
                
            var previousItem = DictionaryItems[currentIndex - 1];
            
            // 交换排序号
            int tempSortOrder = dictItem.SortOrder;
            dictItem.SortOrder = previousItem.SortOrder;
            previousItem.SortOrder = tempSortOrder;
            
            // 更新数据库
            try
            {
                await _dictionaryItemService.UpdateAsync(dictItem);
                await _dictionaryItemService.UpdateAsync(previousItem);
                
                // 重新排序集合
                DictionaryItems.Move(currentIndex, currentIndex - 1);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动字典项失败: {ex.Message}");
                
                // 恢复排序号
                dictItem.SortOrder = tempSortOrder;
                previousItem.SortOrder = previousItem.SortOrder;
            }
        }
        
        [RelayCommand]
        private async Task MoveDictItemDown(DictionaryItem? dictItem)
        {
            if (dictItem == null || DictionaryItems.Count <= 1)
                return;
                
            int currentIndex = DictionaryItems.IndexOf(dictItem);
            if (currentIndex < 0 || currentIndex >= DictionaryItems.Count - 1)
                return;
                
            var nextItem = DictionaryItems[currentIndex + 1];
            
            // 交换排序号
            int tempSortOrder = dictItem.SortOrder;
            dictItem.SortOrder = nextItem.SortOrder;
            nextItem.SortOrder = tempSortOrder;
            
            // 更新数据库
            try
            {
                await _dictionaryItemService.UpdateAsync(dictItem);
                await _dictionaryItemService.UpdateAsync(nextItem);
                
                // 重新排序集合
                DictionaryItems.Move(currentIndex, currentIndex + 1);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"移动字典项失败: {ex.Message}");
                
                // 恢复排序号
                dictItem.SortOrder = tempSortOrder;
                nextItem.SortOrder = nextItem.SortOrder;
            }
        }
    }
} 