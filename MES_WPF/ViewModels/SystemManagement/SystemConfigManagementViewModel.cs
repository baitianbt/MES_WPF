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
    public partial class SystemConfigManagementViewModel : ObservableObject
    {
        private readonly ISystemConfigService _systemConfigService;
        private readonly IDialogService _dialogService;
        
        [ObservableProperty]
        private SystemConfig? _selectedConfig;
        
        [ObservableProperty]
        private string _searchKeyword = string.Empty;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private int _totalCount;
        
        [ObservableProperty]
        private string _title = "系统配置";
        
        [ObservableProperty]
        private string _selectedConfigType = "全部";
        
        // 配置对话框相关属性
        [ObservableProperty]
        private bool _isConfigDialogOpen;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private SystemConfig _editingConfig = new SystemConfig();
        
        public ObservableCollection<SystemConfig> Configs { get; } = new();
        
        public ObservableCollection<string> ConfigTypes { get; } = new();

        public ICollectionView? ConfigsView { get; private set; }
        
        partial void OnSearchKeywordChanged(string value)
        {
            ConfigsView?.Refresh();
        }
        
        partial void OnSelectedConfigTypeChanged(string value)
        {
            ConfigsView?.Refresh();
        }
        
        public SystemConfigManagementViewModel(
            ISystemConfigService systemConfigService,
            IDialogService dialogService)
        {
            _systemConfigService = systemConfigService ?? throw new ArgumentNullException(nameof(systemConfigService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            
            // 设置过滤器
            SetupFilter();
            
            // 加载系统配置数据
            _ = LoadConfigsAsync();
        }
        
        private void SetupFilter()
        {
            // 确保视图与集合绑定
            ConfigsView = CollectionViewSource.GetDefaultView(Configs);
            if (ConfigsView != null)
            {
                ConfigsView.Filter = ConfigFilter;
            }
        }
        
        private bool ConfigFilter(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword) && SelectedConfigType == "全部")
            {
                return true;
            }
            
            if (obj is SystemConfig config)
            {
                bool matchesKeyword = string.IsNullOrWhiteSpace(SearchKeyword) ||
                                     (config.ConfigKey?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (config.ConfigName?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (config.ConfigValue?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                     (config.Remark?.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ?? false);
                
                bool matchesType = SelectedConfigType == "全部" || config.ConfigType == SelectedConfigType;
                
                return matchesKeyword && matchesType;
            }
            
            return false;
        }
        
        private async Task LoadConfigsAsync()
        {
            try
            {
                IsRefreshing = true;

                // 获取所有系统配置
                var configs = await _systemConfigService.GetAllAsync();
                
                // 使用HashSet暂存配置类型，避免重复
                var configTypesSet = new HashSet<string> { "全部" };
                
                // 在UI线程更新集合
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    // 清空现有数据
                    Configs.Clear();
                    ConfigTypes.Clear();
                    
                    // 添加"全部"选项
                    ConfigTypes.Add("全部");
                    SelectedConfigType = "全部";


                    // 将系统配置数据添加到集合
                    foreach (var config in configs)
                    {
                        Configs.Add(config);
                        
                        // 收集配置类型
                        if (!string.IsNullOrEmpty(config.ConfigType) && !configTypesSet.Contains(config.ConfigType))
                        {
                            configTypesSet.Add(config.ConfigType);
                            ConfigTypes.Add(config.ConfigType);
                        }
                    }
                    
                    TotalCount = Configs.Count;
                    
                    // 确保刷新视图
                    ConfigsView?.Refresh();
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载系统配置数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        
        [RelayCommand]
        private async Task RefreshConfigs()
        {
            // 清除筛选条件
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                SearchKeyword = string.Empty;
                SelectedConfigType = "全部";
            });
            
            // 重新加载数据
            await LoadConfigsAsync();
            
           
        }
        
        [RelayCommand]
        private void ResetSearch()
        {
            // 在UI线程上更新搜索条件和刷新视图
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                SearchKeyword = string.Empty;
                SelectedConfigType = "全部";
                
                // 刷新视图显示所有数据
                ConfigsView?.Refresh();
            });
        }
        
        [RelayCommand]
        private void AddConfig()
        {
            // 创建新配置
            EditingConfig = new SystemConfig
            {
                Status = 1,
                CreateTime = DateTime.Now,
                CreateBy = 1, // 当前用户ID，实际应用中应从认证服务获取
                IsSystem = false
            };
            
            IsEditMode = false;
            IsConfigDialogOpen = true;
        }
        
        [RelayCommand]
        private void EditConfig(SystemConfig? config)
        {
            if (config == null)
                return;
                
            // 编辑现有配置
            EditingConfig = new SystemConfig
            {
                Id = config.Id,
                ConfigKey = config.ConfigKey,
                ConfigValue = config.ConfigValue,
                ConfigName = config.ConfigName,
                ConfigType = config.ConfigType,
                IsSystem = config.IsSystem,
                Status = config.Status,
                CreateBy = config.CreateBy,
                CreateTime = config.CreateTime,
                UpdateTime = DateTime.Now,
                Remark = config.Remark
            };
            
            IsEditMode = true;
            IsConfigDialogOpen = true;
        }
        
        [RelayCommand]
        private void CancelEdit()
        {
            IsConfigDialogOpen = false;
        }
        
        [RelayCommand]
        private async Task SaveConfig()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditingConfig.ConfigKey) || 
                    string.IsNullOrWhiteSpace(EditingConfig.ConfigValue) ||
                    string.IsNullOrWhiteSpace(EditingConfig.ConfigName) ||
                    string.IsNullOrWhiteSpace(EditingConfig.ConfigType))
                {
                    await _dialogService.ShowErrorAsync("错误", "配置键、配置值、配置名称和配置类型不能为空");
                    return;
                }
                
                if (IsEditMode)
                {
                    // 更新配置
                    EditingConfig.UpdateTime = DateTime.Now;
                    await _systemConfigService.UpdateAsync(EditingConfig);
                    
                    // 更新列表中对应的项
                    var existingConfig = Configs.FirstOrDefault(c => c.Id == EditingConfig.Id);
                    if (existingConfig != null)
                    {
                        existingConfig.ConfigKey = EditingConfig.ConfigKey;
                        existingConfig.ConfigValue = EditingConfig.ConfigValue;
                        existingConfig.ConfigName = EditingConfig.ConfigName;
                        existingConfig.ConfigType = EditingConfig.ConfigType;
                        existingConfig.Status = EditingConfig.Status;
                        existingConfig.UpdateTime = EditingConfig.UpdateTime;
                        existingConfig.Remark = EditingConfig.Remark;
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "系统配置更新成功");
                }
                else
                {
                    // 检查配置键是否存在
                    var existingConfig = Configs.FirstOrDefault(c => c.ConfigKey.Equals(EditingConfig.ConfigKey, StringComparison.OrdinalIgnoreCase));
                    if (existingConfig != null)
                    {
                        await _dialogService.ShowErrorAsync("错误", "配置键已存在");
                        return;
                    }
                    
                    // 创建新配置
                    var newConfig = await _systemConfigService.AddAsync(EditingConfig);
                    
                    // 添加到集合
                    Configs.Add(newConfig);
                    TotalCount = Configs.Count;
                    
                    // 如果是新的配置类型，添加到类型列表
                    if (!string.IsNullOrEmpty(newConfig.ConfigType) && !ConfigTypes.Contains(newConfig.ConfigType))
                    {
                        ConfigTypes.Add(newConfig.ConfigType);
                    }
                    
                    await _dialogService.ShowInfoAsync("成功", "系统配置添加成功");
                }
                
                // 关闭对话框
                IsConfigDialogOpen = false;
                
                // 刷新视图
                ConfigsView?.Refresh();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存系统配置失败: {ex.Message}");
            }
        }
        
        [RelayCommand]
        private async Task DeleteConfig(SystemConfig? config)
        {
            if (config == null)
                return;
                
            // 系统配置不允许删除
            if (config.IsSystem)
            {
                await _dialogService.ShowErrorAsync("警告", "系统配置不允许删除");
                return;
            }
                
            // 确认删除
            var result = await _dialogService.ShowConfirmAsync("确认删除", $"确定要删除配置\"{config.ConfigName}\"吗？");
            if (result)
            {
                try
                {
                    // 删除配置
                    await _systemConfigService.DeleteByIdAsync(config.Id);
                    
                    // 从集合中移除
                    Configs.Remove(config);
                    TotalCount = Configs.Count;
                    
                    // 更新配置类型列表
                    UpdateConfigTypes();
                    
                    await _dialogService.ShowInfoAsync("成功", "系统配置删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除系统配置失败: {ex.Message}");
                }
            }
        }
        
        private void UpdateConfigTypes()
        {
            // 清空现有类型列表（保留"全部"）
            ConfigTypes.Clear();
            ConfigTypes.Add("全部");
            
            // 重新收集配置类型
            foreach (var config in Configs)
            {
                if (!string.IsNullOrEmpty(config.ConfigType) && !ConfigTypes.Contains(config.ConfigType))
                {
                    ConfigTypes.Add(config.ConfigType);
                }
            }
        }
        
        [RelayCommand]
        private async Task ToggleConfigStatus(SystemConfig? config)
        {
            if (config == null)
                return;
                
            try
            {
                // 切换状态
                config.Status = config.Status == 1 ? (byte)0 : (byte)1;
                config.UpdateTime = DateTime.Now;
                
                // 更新数据库
                await _systemConfigService.UpdateAsync(config);
                
                // 刷新视图
                ConfigsView?.Refresh();
                
                await _dialogService.ShowInfoAsync("成功", $"系统配置已{(config.Status == 1 ? "启用" : "禁用")}");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"更新系统配置状态失败: {ex.Message}");
                
                // 恢复状态
                config.Status = config.Status == 1 ? (byte)0 : (byte)1;
            }
        }
    }
} 