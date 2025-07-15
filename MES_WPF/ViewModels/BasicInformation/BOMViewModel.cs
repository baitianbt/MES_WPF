
using MES_WPF.Core.Services.BasicInformation;
using MES_WPF.Model.BasicInformation;
using MES_WPF.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MES_WPF.ViewModels.BasicInformation
{
    /// <summary>
    /// BOM管理视图模型
    /// </summary>
    public partial class BOMViewModel : ObservableObject
    {
        private readonly IBOMService _bomService;
        private readonly IProductService _productService;
        private readonly IDialogService _dialogService;

        #region 属性

        [ObservableProperty]
        private ObservableCollection<BOM> _boms = new();
        
        // 扩展BOM类以添加IsSelected属性
        public partial class BOMWithSelection : BOM
        {
            public bool IsSelected { get; set; }
        }

        public ObservableCollection<BOM> BOMs => _boms;
        public ObservableCollection<BOM> BOMsView => _boms;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private BOM _selectedBOM;

        [ObservableProperty]
        private BOM _editingBOM;

        [ObservableProperty]
        private string _searchKeyword;

        [ObservableProperty]
        private int? _selectedProductId;

        [ObservableProperty]
        private int _selectedStatus = 0;

        [ObservableProperty]
        private bool _onlyDefaultBOM;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isBOMDialogOpen;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _totalCount;

        [ObservableProperty]
        private int _currentPage = 1;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public BOMViewModel(IBOMService bomService, IProductService productService, IDialogService dialogService)
        {
            _bomService = bomService;
            _productService = productService;
            _dialogService = dialogService;

            // 加载产品和BOM数据
            Task.Run(async () => 
            {
                await LoadProductsAsync();
                await LoadBOMsAsync();
            });
        }

        // 可执行条件方法
        private bool CanPreviousPage() => CurrentPage > 1;

        private bool CanNextPage() => CurrentPage < TotalPages;

        /// <summary>
        /// 加载产品数据
        /// </summary>
        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _productService.GetAllAsync();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载产品数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载BOM数据
        /// </summary>
        private async Task LoadBOMsAsync()
        {
            try
            {
                IsRefreshing = true;
                var boms = await _bomService.GetAllAsync();
                TotalCount = boms.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _boms.Clear();
                    foreach (var bom in boms)
                    {
                        _boms.Add(bom);
                    }
                });
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"加载BOM数据失败: {ex.Message}");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 搜索BOM
        /// </summary>
        [RelayCommand]
        private async Task SearchBOMsAsync()
        {
            try
            {
                IsRefreshing = true;

                var boms = await _bomService.GetAllAsync();

                // 按关键字筛选
                if (!string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    boms = boms.Where(b => 
                        b.BomCode.Contains(SearchKeyword) || 
                        b.BomName.Contains(SearchKeyword));
                }

                // 按产品筛选
                if (SelectedProductId.HasValue)
                {
                    boms = boms.Where(b => b.ProductId == SelectedProductId.Value);
                }

                // 按状态筛选
                if (SelectedStatus > 0)
                {
                    byte status = (byte)SelectedStatus;
                    boms = boms.Where(b => b.Status == status);
                }

                // 是否只显示默认BOM
                if (OnlyDefaultBOM)
                {
                    boms = boms.Where(b => b.IsDefault);
                }

                TotalCount = boms.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _boms.Clear();
                    foreach (var bom in boms)
                    {
                        _boms.Add(bom);
                    }
                });

                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索BOM失败: {ex.Message}");
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
            SelectedProductId = null;
            SelectedStatus = 0;
            OnlyDefaultBOM = false;
            
            Task.Run(async () => await LoadBOMsAsync());
        }

        /// <summary>
        /// 添加BOM
        /// </summary>
        [RelayCommand]
        private async Task AddBOMAsync()
        {
            // 如果没有产品，提示先添加产品
            if (Products.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请先添加产品，再创建BOM");
                return;
            }

            EditingBOM = new BOM
            {
                CreateTime = DateTime.Now,
                EffectiveDate = DateTime.Now,
                Status = 1, // 默认为草稿状态
                IsDefault = false
            };
            
            IsEditMode = false;
            IsBOMDialogOpen = true;
        }

        /// <summary>
        /// 编辑BOM
        /// </summary>
        [RelayCommand]
        private void EditBOM(BOM bom)
        {
            if (bom == null) return;

            // 创建编辑对象的副本，以免直接修改原对象
            EditingBOM = new BOM
            {
                Id = bom.Id,
                BomCode = bom.BomCode,
                BomName = bom.BomName,
                ProductId = bom.ProductId,
                Version = bom.Version,
                Status = bom.Status,
                EffectiveDate = bom.EffectiveDate,
                ExpiryDate = bom.ExpiryDate,
                IsDefault = bom.IsDefault,
                CreateTime = bom.CreateTime,
                UpdateTime = DateTime.Now
            };
            
            IsEditMode = true;
            IsBOMDialogOpen = true;
        }

        /// <summary>
        /// 删除BOM
        /// </summary>
        [RelayCommand]
        private async Task DeleteBOMAsync(BOM bom)
        {
            if (bom == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除BOM {bom.BomName} 吗？");
            if (result)
            {
                try
                {
                    // 如果是默认BOM，需要提示
                    if (bom.IsDefault)
                    {
                        var confirmDefault = await _dialogService.ShowConfirmAsync("警告", "当前BOM是默认版本，删除后可能影响其他功能。确定要删除吗？");
                        if (!confirmDefault) return;
                    }

                    await _bomService.DeleteByIdAsync(bom.Id);
                    _boms.Remove(bom);
                    TotalCount--;
                    await _dialogService.ShowInfoAsync("成功", "BOM删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除BOM失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 批量删除BOM
        /// </summary>
        [RelayCommand]
        private async Task BatchDeleteAsync()
        {
            // 注：这里应该使用BOMWithSelection类型，但当前数据模型不支持，需要调整模型设计
            // 临时解决方案，跳过此方法
            await _dialogService.ShowInfoAsync("提示", "批量删除功能需要模型支持，暂不可用");
            return;
            
            /*
            var selectedBOMs = _boms.Where(b => (b as BOMWithSelection)?.IsSelected ?? false).ToList();
            if (selectedBOMs.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择要删除的BOM");
                return;
            }

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除选中的 {selectedBOMs.Count} 个BOM吗？");
            if (result)
            {
                try
                {
                    // 检查是否包含默认BOM
                    var hasDefault = selectedBOMs.Any(b => b.IsDefault);
                    if (hasDefault)
                    {
                        var confirmDefault = await _dialogService.ShowConfirmAsync("警告", "选中的BOM中包含默认版本，删除后可能影响其他功能。确定要删除吗？");
                        if (!confirmDefault) return;
                    }

                    foreach (var bom in selectedBOMs)
                    {
                        await _bomService.DeleteByIdAsync(bom.Id);
                        _boms.Remove(bom);
                    }
                    
                    TotalCount -= selectedBOMs.Count;
                    await _dialogService.ShowInfoAsync("成功", "BOM批量删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"批量删除BOM失败: {ex.Message}");
                }
            }
            */
        }

        /// <summary>
        /// 导出BOM数据
        /// </summary>
        [RelayCommand]
        private void ExportBOMs()
        {
            _dialogService.ShowInfoAsync("提示", "导出功能待实现");
        }

        /// <summary>
        /// 保存BOM
        /// </summary>
        [RelayCommand]
        private async Task SaveBOMAsync()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingBOM.BomCode))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入BOM编码");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingBOM.BomName))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入BOM名称");
                return;
            }

            if (EditingBOM.ProductId <= 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择产品");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingBOM.Version))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入版本号");
                return;
            }

            if (EditingBOM.EffectiveDate == default)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择生效日期");
                return;
            }

            try
            {
                // 检查BOM编码是否已存在
                if (!IsEditMode)
                {
                    bool exists = await _bomService.IsBOMCodeExistsAsync(EditingBOM.BomCode);
                    if (exists)
                    {
                        await _dialogService.ShowInfoAsync("提示", $"BOM编码 {EditingBOM.BomCode} 已存在");
                        return;
                    }
                }

                // 处理默认BOM逻辑
                if (EditingBOM.IsDefault)
                {
                    // 如果设置为默认，需要取消同一产品下其他BOM的默认状态
                    await _bomService.SetDefaultBOMAsync(EditingBOM.Id, EditingBOM.ProductId);
                }

                if (IsEditMode)
                {
                    // 更新
                    EditingBOM.UpdateTime = DateTime.Now;
                    await _bomService.UpdateAsync(EditingBOM);

                    // 更新列表中的对象
                    var existingBOM = _boms.FirstOrDefault(b => b.Id == EditingBOM.Id);
                    if (existingBOM != null)
                    {
                        var index = _boms.IndexOf(existingBOM);
                        _boms[index] = EditingBOM;
                    }

                    await _dialogService.ShowInfoAsync("成功", "BOM更新成功");
                }
                else
                {
                    // 新增
                    EditingBOM.CreateTime = DateTime.Now;
                    var newBOM = await _bomService.AddAsync(EditingBOM);
                    _boms.Add(newBOM);
                    TotalCount++;

                    await _dialogService.ShowInfoAsync("成功", "BOM添加成功");
                }

                IsBOMDialogOpen = false;
                
                // 重新加载数据以更新默认BOM状态
                await LoadBOMsAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存BOM失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        [RelayCommand]
        private void CancelEdit()
        {
            IsBOMDialogOpen = false;
        }

        /// <summary>
        /// 查看BOM明细
        /// </summary>
        [RelayCommand]
        private void ViewBOMItems(BOM bom)
        {
            if (bom == null) return;
            
            _dialogService.ShowInfoAsync("提示", $"查看BOM {bom.BomName} 明细功能待实现");
        }

        /// <summary>
        /// 设置默认BOM
        /// </summary>
        [RelayCommand]
        private async Task SetDefaultBOMAsync(BOM bom)
        {
            if (bom == null) return;

            try
            {
                // 如果已经是默认BOM，无需操作
                if (bom.IsDefault)
                {
                    await _dialogService.ShowInfoAsync("提示", "该BOM已经是默认版本");
                    return;
                }

                // 如果不是已发布状态，提示不能设为默认
                if (bom.Status != 3) // 3-已发布
                {
                    await _dialogService.ShowInfoAsync("提示", "只有已发布的BOM才能设为默认版本");
                    return;
                }

                await _bomService.SetDefaultBOMAsync(bom.Id, bom.ProductId);
                
                // 重新加载数据
                await LoadBOMsAsync();
                
                await _dialogService.ShowInfoAsync("成功", $"已将BOM {bom.BomName} 设为默认版本");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"设置默认BOM失败: {ex.Message}");
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
}