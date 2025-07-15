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
using MES_WPF.Core.Services.SystemManagement;
using MES_WPF.Core.Models;

namespace MES_WPF.ViewModels.BasicInformation
{
    /// <summary>
    /// 资源管理视图模型
    /// </summary>
    public partial class ResourceViewModel : ObservableObject
    {
        private readonly IResourceService _resourceService;
        private readonly IEquipmentService _equipmentService;
        private readonly IDepartmentService _departmentService;
        private readonly IDialogService _dialogService;

        #region 属性

        [ObservableProperty]
        private ObservableCollection<Resource> _resources = new();

        public ObservableCollection<Resource> ResourcesView => _resources;

        [ObservableProperty]
        private ObservableCollection<Department> _departments = new();

        [ObservableProperty]
        private Resource _selectedResource;

        [ObservableProperty]
        private Resource _editingResource;

        [ObservableProperty]
        private Equipment _editingEquipment;

        [ObservableProperty]
        private string _searchKeyword;

        [ObservableProperty]
        private int _selectedResourceType = 0;

        [ObservableProperty]
        private int _selectedStatus = 0;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isResourceDialogOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private bool _isEquipmentResource;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _currentPage = 1;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResourceViewModel(
            IResourceService resourceService,
            IEquipmentService equipmentService,
            IDepartmentService departmentService,
            IDialogService dialogService)
        {
            _resourceService = resourceService;
            _equipmentService = equipmentService;
            _departmentService = departmentService;
            _dialogService = dialogService;

            // 加载资源和部门数据
            Task.Run(async () =>
            {
                await LoadDepartmentsAsync();
                await LoadResourcesAsync();
            });
        }

        partial void OnEditingResourceChanged(Resource value)
        {
            if (value != null)
            {
                IsEquipmentResource = value.ResourceType == 1; // 资源类型为1时是设备
            }
        }

        // 可执行条件方法
        private bool CanPreviousPage() => CurrentPage > 1;
        private bool CanNextPage() => CurrentPage < TotalPages;

        /// <summary>
        /// 加载部门数据
        /// </summary>
        private async Task LoadDepartmentsAsync()
        {
            try
            {
                var departments = await _departmentService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Departments.Clear();
                    foreach (var department in departments)
                    {
                        Departments.Add(department);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载部门数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载资源数据
        /// </summary>
        [RelayCommand]
        private async Task LoadResourcesAsync()
        {
            try
            {
                IsRefreshing = true;
                var resources = await _resourceService.GetAllAsync();
                TotalCount = resources.Count();

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
                await _dialogService.ShowErrorAsync("错误", $"加载资源数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 搜索资源
        /// </summary>
        [RelayCommand]
        private async Task SearchResourcesAsync()
        {
            try
            {
                IsRefreshing = true;

                var resources = await _resourceService.GetAllAsync();

                // 按关键字筛选
                if (!string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    resources = resources.Where(r => 
                        r.ResourceCode.Contains(SearchKeyword) || 
                        r.ResourceName.Contains(SearchKeyword));
                }

                // 按资源类型筛选
                if (SelectedResourceType > 0)
                {
                    byte resourceType = (byte)SelectedResourceType;
                    resources = resources.Where(r => r.ResourceType == resourceType);
                }

                // 按状态筛选
                if (SelectedStatus > 0)
                {
                    byte status = (byte)SelectedStatus;
                    resources = resources.Where(r => r.Status == status);
                }

                TotalCount = resources.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Resources.Clear();
                    foreach (var resource in resources)
                    {
                        Resources.Add(resource);
                    }
                });

                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索资源失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 重置搜索条件
        /// </summary>
        [RelayCommand]
        private void ResetSearch()
        {
            SearchKeyword = string.Empty;
            SelectedResourceType = 0;
            SelectedStatus = 0;
            
            Task.Run(async () => await LoadResourcesAsync());
        }

        /// <summary>
        /// 添加资源
        /// </summary>
        [RelayCommand]
        private void AddResource()
        {
            EditingResource = new Resource
            {
                ResourceType = 1, // 默认为设备类型
                Status = 1, // 默认为可用状态
                CreateTime = DateTime.Now
            };

            EditingEquipment = new Equipment(); // 初始化设备信息
            
            IsEditMode = false;
            IsEquipmentResource = true; // 默认为设备资源
            IsResourceDialogOpen = true;
        }

        /// <summary>
        /// 编辑资源
        /// </summary>
        [RelayCommand]
        private async Task EditResourceAsync(Resource resource)
        {
            if (resource == null) return;

            // 创建编辑对象的副本，以免直接修改原对象
            EditingResource = new Resource
            {
                Id = resource.Id,
                ResourceCode = resource.ResourceCode,
                ResourceName = resource.ResourceName,
                ResourceType = resource.ResourceType,
                DepartmentId = resource.DepartmentId,
                Status = resource.Status,
                Description = resource.Description,
                CreateTime = resource.CreateTime,
                UpdateTime = DateTime.Now
            };
            
            // 如果是设备类型，加载设备信息
            if (resource.ResourceType == 1) // 1:设备
            {
                try
                {
                    var equipment = await _equipmentService.GetByResourceIdAsync(resource.Id);
                    if (equipment != null)
                    {
                        EditingEquipment = new Equipment
                        {
                            ResourceId = equipment.ResourceId,
                            EquipmentModel = equipment.EquipmentModel,
                            Manufacturer = equipment.Manufacturer,
                            SerialNumber = equipment.SerialNumber,
                            PurchaseDate = equipment.PurchaseDate,
                            WarrantyPeriod = equipment.WarrantyPeriod,
                            MaintenanceCycle = equipment.MaintenanceCycle,
                            LastMaintenanceDate = equipment.LastMaintenanceDate,
                            NextMaintenanceDate = equipment.NextMaintenanceDate,
                            IpAddress = equipment.IpAddress,
                            OpcUaEndpoint = equipment.OpcUaEndpoint
                        };
                    }
                    else
                    {
                        EditingEquipment = new Equipment { ResourceId = resource.Id };
                    }
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"加载设备信息失败: {ex.Message}");
                    EditingEquipment = new Equipment { ResourceId = resource.Id };
                }
            }
            else
            {
                EditingEquipment = new Equipment();
            }
            
            IsEditMode = true;
            IsResourceDialogOpen = true;
        }

        /// <summary>
        /// 删除资源
        /// </summary>
        [RelayCommand]
        private async Task DeleteResourceAsync(Resource resource)
        {
            if (resource == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除资源 {resource.ResourceName} 吗？");
            if (result)
            {
                try
                {
                    // 如果是设备类型，可能需要先删除设备信息
                    if (resource.ResourceType == 1)
                    {
                        var equipment = await _equipmentService.GetByResourceIdAsync(resource.Id);
                        if (equipment != null)
                        {
                            await _equipmentService.DeleteByIdAsync(equipment.Id);
                        }
                    }

                    await _resourceService.DeleteByIdAsync(resource.Id);
                    Resources.Remove(resource);
                    TotalCount--;
                    await _dialogService.ShowInfoAsync("成功", "资源删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除资源失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 批量删除资源
        /// </summary>
        [RelayCommand]
        private async Task BatchDeleteAsync()
        {
            // 注：这里应该使用ResourceWithSelection类型，但当前数据模型不支持，需要调整模型设计
            await _dialogService.ShowInfoAsync("提示", "批量删除功能需要模型支持，暂不可用");
        }

        /// <summary>
        /// 导出资源数据
        /// </summary>
        [RelayCommand]
        private void ExportResources()
        {
            _dialogService.ShowInfoAsync("提示", "导出功能待实现");
        }

        /// <summary>
        /// 保存资源
        /// </summary>
        [RelayCommand]
        private async Task SaveResourceAsync()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingResource.ResourceCode))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入资源编码");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingResource.ResourceName))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入资源名称");
                return;
            }

            if (EditingResource.ResourceType <= 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择资源类型");
                return;
            }

            try
            {
                // 检查资源编码是否已存在
                if (!IsEditMode)
                {
                    bool exists = await _resourceService.IsResourceCodeExistsAsync(EditingResource.ResourceCode);
                    if (exists)
                    {
                        await _dialogService.ShowInfoAsync("提示", $"资源编码 {EditingResource.ResourceCode} 已存在");
                        return;
                    }
                }

                Resource savedResource;
                if (IsEditMode)
                {
                    // 更新
                    EditingResource.UpdateTime = DateTime.Now;
                    savedResource = await _resourceService.UpdateAsync(EditingResource);

                    // 更新列表中的对象
                    var existingResource = Resources.FirstOrDefault(r => r.Id == EditingResource.Id);
                    if (existingResource != null)
                    {
                        int index = Resources.IndexOf(existingResource);
                        Resources[index] = savedResource;
                    }
                }
                else
                {
                    // 新增
                    EditingResource.CreateTime = DateTime.Now;
                    savedResource = await _resourceService.AddAsync(EditingResource);
                    Resources.Add(savedResource);
                    TotalCount++;
                }

                // 如果是设备类型，保存设备信息
                if (EditingResource.ResourceType == 1 && EditingEquipment != null)
                {
                    EditingEquipment.ResourceId = savedResource.Id;

                    // 判断是新增还是更新设备信息
                    var existingEquipment = await _equipmentService.GetByResourceIdAsync(savedResource.Id);
                    if (existingEquipment != null)
                    {
                        // 更新现有设备信息
                        EditingEquipment.Id = existingEquipment.Id;
                        await _equipmentService.UpdateAsync(EditingEquipment);
                    }
                    else
                    {
                        // 新增设备信息
                        await _equipmentService.AddAsync(EditingEquipment);
                    }
                }

                await _dialogService.ShowInfoAsync("成功", IsEditMode ? "资源更新成功" : "资源添加成功");
                IsResourceDialogOpen = false;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存资源失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        [RelayCommand]
        private void CancelEdit()
        {
            IsResourceDialogOpen = false;
        }

        /// <summary>
        /// 查看设备详情
        /// </summary>
        [RelayCommand]
        private async Task ViewEquipmentAsync(Resource resource)
        {
            if (resource == null || resource.ResourceType != 1) return;

            try
            {
                var equipment = await _equipmentService.GetByResourceIdAsync(resource.Id);
                if (equipment != null)
                {
                    // 这里可以直接使用设备视图模型跳转到设备详情页，或者弹出设备详情对话框
                    // 暂时使用简单的消息框显示设备信息
                    string info = $"设备详情:\n" +
                        $"型号: {equipment.EquipmentModel}\n" +
                        $"制造商: {equipment.Manufacturer}\n" +
                        $"序列号: {equipment.SerialNumber}\n" +
                        $"购买日期: {equipment.PurchaseDate?.ToString("yyyy-MM-dd")}\n" +
                        $"保修期: {equipment.WarrantyPeriod} 个月\n" +
                        $"保养周期: {equipment.MaintenanceCycle} 天\n" +
                        $"上次保养: {equipment.LastMaintenanceDate?.ToString("yyyy-MM-dd")}\n" +
                        $"下次保养: {equipment.NextMaintenanceDate?.ToString("yyyy-MM-dd")}\n" +
                        $"IP地址: {equipment.IpAddress}";
                    
                    await _dialogService.ShowInfoAsync("设备详情", info);
                }
                else
                {
                    await _dialogService.ShowInfoAsync("提示", "该资源没有关联的设备信息");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载设备详情失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 上一页
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanPreviousPage))]
        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                // 实现分页逻辑
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanNextPage))]
        private void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                // 实现分页逻辑
            }
        }

        /// <summary>
        /// 跳转到指定页
        /// </summary>
        [RelayCommand]
        private void GoToPage(object pageObj)
        {
            if (pageObj is int pageInt)
            {
                if (pageInt >= 1 && pageInt <= TotalPages)
                {
                    CurrentPage = pageInt;
                    // 实现分页逻辑
                }
            }
            else if (pageObj is string pageStr)
            {
                if (int.TryParse(pageStr, out int pageVal) && pageVal >= 1 && pageVal <= TotalPages)
                {
                    CurrentPage = pageVal;
                    // 实现分页逻辑
                }
            }
        }

        private int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        private int PageSize => 10; // 每页显示数量
    }

    // 扩展Resource类以添加IsSelected属性
    public partial class ResourceWithSelection : Resource
    {
        public bool IsSelected { get; set; }
    }
}