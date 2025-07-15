using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MES_WPF.ViewModels.BasicInformation
{
    public partial class EquipmentViewModel : ObservableObject
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IResourceService _resourceService;
        private readonly IDialogService _dialogService;

        // 设备列表
        [ObservableProperty]
        private ObservableCollection<Equipment> _equipments = new();

        // 资源列表（用于下拉选择）
        [ObservableProperty]
        private ObservableCollection<Resource> _resources = new();

        // 选中的设备
        [ObservableProperty]
        private Equipment _selectedEquipment;

        // 新建/编辑的设备
        [ObservableProperty]
        private Equipment _editingEquipment;

        // 搜索条件
        [ObservableProperty]
        private string _searchText;

        // 是否显示需要维护的设备
        [ObservableProperty]
        private bool _showMaintenanceRequired;

        // 是否编辑模式
        [ObservableProperty]
        private bool _isEditing;

        // 是否正在忙碌
        [ObservableProperty]
        private bool _isBusy;

        partial void OnShowMaintenanceRequiredChanged(bool value)
        {
            Task.Run(() => LoadEquipmentsAsync());
        }

        public EquipmentViewModel(
            IEquipmentService equipmentService,
            IResourceService resourceService,
            IDialogService dialogService)
        {
            _equipmentService = equipmentService;
            _resourceService = resourceService;
            _dialogService = dialogService;

            // 初始化时加载数据
            Task.Run(async () =>
            {
                await LoadResourcesAsync();
                await LoadEquipmentsAsync();
            });
        }

        // 加载设备列表
        [RelayCommand]
        private async Task LoadEquipmentsAsync()
        {
            try
            {
                IsBusy = true;
                var equipments = ShowMaintenanceRequired
                    ? await _equipmentService.GetMaintenanceRequiredEquipmentsAsync()
                    : await _equipmentService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Equipments.Clear();
                    foreach (var equipment in equipments)
                    {
                        Equipments.Add(equipment);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载设备信息失败：{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 加载资源列表
        private async Task LoadResourcesAsync()
        {
            try
            {
                var resources = await _resourceService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Resources.Clear();
                    foreach (var resource in resources)
                    {
                        Resources.Add(resource);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载资源信息失败：{ex.Message}");
            }
        }

        // 搜索设备
        [RelayCommand]
        private async Task SearchEquipmentsAsync()
        {
            try
            {
                IsBusy = true;
                var equipments = await _equipmentService.GetAllAsync();

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    equipments = equipments.Where(e => 
                        e.SerialNumber?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        e.EquipmentModel?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        e.Manufacturer?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                        e.Resource?.ResourceName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true
                    );
                }

                if (ShowMaintenanceRequired)
                {
                    var today = DateTime.Today;
                    equipments = equipments.Where(e => e.NextMaintenanceDate.HasValue && e.NextMaintenanceDate.Value <= today);
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Equipments.Clear();
                    foreach (var equipment in equipments)
                    {
                        Equipments.Add(equipment);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索设备失败：{ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // 添加设备
        [RelayCommand]
        private void AddEquipment()
        {
            EditingEquipment = new Equipment
            {
                PurchaseDate = DateTime.Today,
                LastMaintenanceDate = DateTime.Today,
                NextMaintenanceDate = DateTime.Today.AddDays(30),
                MaintenanceCycle = 30
            };
            IsEditing = true;
        }

        // 编辑设备
        [RelayCommand]
        private void EditEquipment()
        {
            if (SelectedEquipment == null) return;

            // 创建副本以便取消编辑时恢复
            EditingEquipment = new Equipment
            {
                Id = SelectedEquipment.Id,
                ResourceId = SelectedEquipment.ResourceId,
                EquipmentModel = SelectedEquipment.EquipmentModel,
                Manufacturer = SelectedEquipment.Manufacturer,
                SerialNumber = SelectedEquipment.SerialNumber,
                PurchaseDate = SelectedEquipment.PurchaseDate,
                WarrantyPeriod = SelectedEquipment.WarrantyPeriod,
                MaintenanceCycle = SelectedEquipment.MaintenanceCycle,
                LastMaintenanceDate = SelectedEquipment.LastMaintenanceDate,
                NextMaintenanceDate = SelectedEquipment.NextMaintenanceDate,
                IpAddress = SelectedEquipment.IpAddress,
                OpcUaEndpoint = SelectedEquipment.OpcUaEndpoint
            };
            IsEditing = true;
        }

        // 保存设备
        [RelayCommand]
        private async Task SaveEquipmentAsync()
        {
            if (EditingEquipment == null) return;

            try
            {
                // 验证必填字段
                if (EditingEquipment.ResourceId <= 0)
                {
                    await _dialogService.ShowErrorAsync("验证错误", "请选择关联资源");
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditingEquipment.SerialNumber))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "序列号不能为空");
                    return;
                }

                // 检查序列号是否已存在（如果不是编辑现有设备）
                if (EditingEquipment.Id == 0 &&
                    await _equipmentService.IsSerialNumberExistsAsync(EditingEquipment.SerialNumber))
                {
                    await _dialogService.ShowErrorAsync("验证错误", "该序列号已存在");
                    return;
                }

                // 保存
                Equipment savedEquipment;
                if (EditingEquipment.Id == 0)
                {
                    savedEquipment = await _equipmentService.AddAsync(EditingEquipment);
                    await _dialogService.ShowInfoAsync("成功", "设备添加成功");
                }
                else
                {
                    savedEquipment = await _equipmentService.UpdateAsync(EditingEquipment);
                    await _dialogService.ShowInfoAsync("成功", "设备更新成功");
                }

                // 刷新列表
                await LoadEquipmentsAsync();
                
                // 选中保存的设备
                SelectedEquipment = Equipments.FirstOrDefault(e => e.Id == savedEquipment.Id);
                
                IsEditing = false;
                EditingEquipment = null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存设备失败：{ex.Message}");
            }
        }

        // 删除设备
        [RelayCommand]
        private async Task DeleteEquipmentAsync()
        {
            if (SelectedEquipment == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", "确定要删除此设备吗？");
            if (!result) return;

            try
            {
                await _equipmentService.DeleteByIdAsync(SelectedEquipment.Id);
                await _dialogService.ShowInfoAsync("成功", "设备删除成功");
                await LoadEquipmentsAsync();
                SelectedEquipment = null;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"删除设备失败：{ex.Message}");
            }
        }

        // 记录设备维护
        [RelayCommand]
        private async Task RecordMaintenanceAsync()
        {
            if (SelectedEquipment == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", "确定要记录设备维护吗？此操作将更新上次维护日期和下次维护日期。");
            if (!result) return;

            try
            {
                var updatedEquipment = await _equipmentService.RecordMaintenanceAsync(SelectedEquipment.Id);
                await _dialogService.ShowInfoAsync("成功", "设备维护记录更新成功");
                await LoadEquipmentsAsync();
                SelectedEquipment = Equipments.FirstOrDefault(e => e.Id == updatedEquipment.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"记录设备维护失败：{ex.Message}");
            }
        }

        // 更新设备保养周期
        [RelayCommand]
        private async Task UpdateMaintenanceCycleAsync()
        {
            if (SelectedEquipment == null) return;

            var cycleString = await _dialogService.ShowInputAsync("更新保养周期", "请输入新的保养周期（天数）：", 
                SelectedEquipment.MaintenanceCycle?.ToString() ?? "30");
            
            if (string.IsNullOrEmpty(cycleString)) return;

            if (!int.TryParse(cycleString, out int cycle) || cycle <= 0)
            {
                await _dialogService.ShowErrorAsync("错误", "请输入有效的天数");
                return;
            }

            try
            {
                var updatedEquipment = await _equipmentService.UpdateMaintenanceCycleAsync(SelectedEquipment.Id, cycle);
                await _dialogService.ShowInfoAsync("成功", "设备保养周期更新成功");
                await LoadEquipmentsAsync();
                SelectedEquipment = Equipments.FirstOrDefault(e => e.Id == updatedEquipment.Id);
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"更新设备保养周期失败：{ex.Message}");
            }
        }

        // 取消编辑
        [RelayCommand]
        private void CancelEdit()
        {
            EditingEquipment = null;
            IsEditing = false;
        }
    }
}