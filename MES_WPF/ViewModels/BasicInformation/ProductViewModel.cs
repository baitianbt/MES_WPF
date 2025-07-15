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
    /// <summary>
    /// 产品管理视图模型
    /// </summary>
    public partial class ProductViewModel : ObservableObject
    {
        private readonly IProductService _productService;
        private readonly IDialogService _dialogService;
        
        // 扩展Product类以添加IsSelected属性
        public partial class ProductWithSelection : Product
        {
            public bool IsSelected { get; set; }
        }

        #region 属性

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        public ObservableCollection<Product> ProductsView => _products;

        [ObservableProperty]
        private Product _selectedProduct;

        [ObservableProperty]
        private Product _editingProduct;

        [ObservableProperty]
        private string _searchKeyword;

        [ObservableProperty]
        private int _selectedProductType = 0;

        [ObservableProperty]
        private int _selectedStatus = 0;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isProductDialogOpen;

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
        public ProductViewModel(IProductService productService, IDialogService dialogService)
        {
            _productService = productService;
            _dialogService = dialogService;

            // 加载产品数据
            Task.Run(async () => await LoadProductsAsync());
        }

        // 可执行条件方法
        private bool CanPreviousPage() => CurrentPage > 1;
        private bool CanNextPage() => CurrentPage < TotalPages;

        /// <summary>
        /// 加载产品数据
        /// </summary>
        [RelayCommand]
        private async Task LoadProductsAsync()
        {
            try
            {
                IsRefreshing = true;
                var products = await _productService.GetAllAsync();
                TotalCount = products.Count();

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
            finally
            {
                IsRefreshing = false;
            }
        }

        /// <summary>
        /// 搜索产品
        /// </summary>
        [RelayCommand]
        private async Task SearchProductsAsync()
        {
            try
            {
                IsRefreshing = true;

                var products = await _productService.GetAllAsync();

                // 按关键字筛选
                if (!string.IsNullOrWhiteSpace(SearchKeyword))
                {
                    products = products.Where(p => 
                        p.ProductCode.Contains(SearchKeyword) || 
                        p.ProductName.Contains(SearchKeyword));
                }

                // 按产品类型筛选
                if (SelectedProductType > 0)
                {
                    byte productType = (byte)SelectedProductType;
                    products = products.Where(p => p.ProductType == productType);
                }

                // 按状态筛选
                if (SelectedStatus > 0)
                {
                    bool isActive = SelectedStatus == 1;
                    products = products.Where(p => p.IsActive == isActive);
                }

                TotalCount = products.Count();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });

                CurrentPage = 1;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"搜索产品失败: {ex.Message}");
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
            SelectedProductType = 0;
            SelectedStatus = 0;
            
            Task.Run(async () => await LoadProductsAsync());
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        [RelayCommand]
        private void AddProduct()
        {
            EditingProduct = new Product
            {
                CreateTime = DateTime.Now,
                IsActive = true
            };
            
            IsEditMode = false;
            IsProductDialogOpen = true;
        }

        /// <summary>
        /// 编辑产品
        /// </summary>
        [RelayCommand]
        private void EditProduct(Product product)
        {
            if (product == null) return;

            // 创建编辑对象的副本，以免直接修改原对象
            EditingProduct = new Product
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                ProductType = product.ProductType,
                Specification = product.Specification,
                Unit = product.Unit,
                Description = product.Description,
                IsActive = product.IsActive,
                CreateTime = product.CreateTime,
                UpdateTime = DateTime.Now
            };
            
            IsEditMode = true;
            IsProductDialogOpen = true;
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        [RelayCommand]
        private async Task DeleteProductAsync(Product product)
        {
            if (product == null) return;

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除产品 {product.ProductName} 吗？");
            if (result)
            {
                try
                {
                    await _productService.DeleteByIdAsync(product.Id);
                    Products.Remove(product);
                    TotalCount--;
                    await _dialogService.ShowInfoAsync("成功", "产品删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"删除产品失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 批量删除产品
        /// </summary>
        [RelayCommand]
        private async Task BatchDeleteAsync()
        {
            // 注：这里应该使用ProductWithSelection类型，但当前数据模型不支持，需要调整模型设计
            // 临时解决方案，跳过此方法
            await _dialogService.ShowInfoAsync("提示", "批量删除功能需要模型支持，暂不可用");
            return;
            
            /*
            var selectedProducts = Products.Where(p => (p as ProductWithSelection)?.IsSelected ?? false).ToList();
            if (selectedProducts.Count == 0)
            {
                await _dialogService.ShowInfoAsync("提示", "请选择要删除的产品");
                return;
            }

            var result = await _dialogService.ShowConfirmAsync("确认", $"确定要删除选中的 {selectedProducts.Count} 个产品吗？");
            if (result)
            {
                try
                {
                    foreach (var product in selectedProducts)
                    {
                        await _productService.DeleteByIdAsync(product.Id);
                        Products.Remove(product);
                    }
                    TotalCount -= selectedProducts.Count;
                    await _dialogService.ShowInfoAsync("成功", "产品批量删除成功");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("错误", $"批量删除产品失败: {ex.Message}");
                }
            }
            */
        }

        /// <summary>
        /// 导出产品数据
        /// </summary>
        [RelayCommand]
        private void ExportProducts()
        {
            _dialogService.ShowInfoAsync("提示", "导出功能待实现");
        }

        /// <summary>
        /// 保存产品
        /// </summary>
        [RelayCommand]
        private async Task SaveProductAsync()
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(EditingProduct.ProductCode))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入产品编码");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingProduct.ProductName))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入产品名称");
                return;
            }

            if (string.IsNullOrWhiteSpace(EditingProduct.Unit))
            {
                await _dialogService.ShowInfoAsync("提示", "请输入计量单位");
                return;
            }

            try
            {
                // 检查产品编码是否已存在
                if (!IsEditMode)
                {
                    bool exists = await _productService.IsProductCodeExistsAsync(EditingProduct.ProductCode);
                    if (exists)
                    {
                        await _dialogService.ShowInfoAsync("提示", $"产品编码 {EditingProduct.ProductCode} 已存在");
                        return;
                    }
                }

                if (IsEditMode)
                {
                    // 更新
                    EditingProduct.UpdateTime = DateTime.Now;
                    await _productService.UpdateAsync(EditingProduct);

                    // 更新列表中的对象
                    var existingProduct = Products.FirstOrDefault(p => p.Id == EditingProduct.Id);
                    if (existingProduct != null)
                    {
                        var index = Products.IndexOf(existingProduct);
                        Products[index] = EditingProduct;
                    }

                    await _dialogService.ShowInfoAsync("成功", "产品更新成功");
                }
                else
                {
                    // 新增
                    EditingProduct.CreateTime = DateTime.Now;
                    var newProduct = await _productService.AddAsync(EditingProduct);
                    Products.Add(newProduct);
                    TotalCount++;

                    await _dialogService.ShowInfoAsync("成功", "产品添加成功");
                }

                IsProductDialogOpen = false;
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("错误", $"保存产品失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消编辑
        /// </summary>
        [RelayCommand]
        private void CancelEdit()
        {
            IsProductDialogOpen = false;
        }

        /// <summary>
        /// 查看产品BOM
        /// </summary>
        [RelayCommand]
        private void ViewBOM(Product product)
        {
            if (product == null) return;
            
            _dialogService.ShowInfoAsync("提示", $"查看产品 {product.ProductName} 的BOM功能待实现");
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