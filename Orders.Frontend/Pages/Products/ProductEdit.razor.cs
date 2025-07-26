using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using System.Runtime.InteropServices;

namespace Orders.Frontend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductEdit
    {
        private ProductDTO productDTO = new()
        {
            ProductCategoryIds = new List<int>(),
            ProductImages = new List<string>(),
        };
        private Product product = new();
        private ProductForm? productForm;
        private List<Category> selectedCategories = new();
        private List<Category> nonselectedCategories = new();
        private bool loading = true;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter, EditorRequired] public int ProductId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
            await LoadCategoriesAsync();
        }

        private async Task AddImageAsync()
        {
            if (productDTO.ProductImages is null || productDTO.ProductImages.Count == 0)
            {
                return;
            }
            var imageDTO = new ImageDTO
            {
                ProductId = ProductId,
                Images = productDTO.ProductImages
            };
            var responseHttp = await Repository.PostAsync<ImageDTO, ImageDTO>("api/products/addimages", imageDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            productDTO.ProductImages = responseHttp.Response!.Images;
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                Timer = 3000,
                ShowConfirmButton = true,
            });
            await toast.FireAsync(message: "Imagenes añadidas con éxíto.", icon: SweetAlertIcon.Success);
        }

        private async Task RemoveImageAsync()
        {
            if (productDTO.ProductImages is null || productDTO.ProductImages.Count == 0)
            {
                return;
            }
            var imageDTO = new ImageDTO
            {
                ProductId = ProductId,
                Images = productDTO.ProductImages
            };
            var responseHttp = await Repository.PostAsync<ImageDTO, ImageDTO>("api/products/removeLastImage", imageDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            productDTO.ProductImages = responseHttp.Response!.Images;
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                Timer = 3000,
                ShowConfirmButton = true,
            });
            await toast.FireAsync(message: "Imagén eliminada con éxíto.", icon: SweetAlertIcon.Success);
        }

        private async Task LoadProductAsync()
        {
            loading = true;
            var httpActionResponse = await Repository.GetAsync<Product>($"api/products/{ProductId}");
            loading = false;
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            product = httpActionResponse.Response!;
            productDTO = ToProductDTO(product);
        }

        private ProductDTO ToProductDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ProductCategoryIds = product.ProductCategories!.Select(c => c.Id).ToList(),
                ProductImages = product.ProductImages!.Select(i => i.Image).ToList()
            };
        }

        private async Task LoadCategoriesAsync()
        {
            var httpActionResponse = await Repository.GetAsync<List<Category>>("api/categories/combo");
            loading = false;
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var categories = httpActionResponse.Response!;
            foreach (var category in categories!)
            {
                var found = product!.ProductCategories!.FirstOrDefault(x => x.CategoryId == category.Id);
                if (found is null)
                {
                    nonselectedCategories.Add(category);
                }
                else
                {
                    selectedCategories.Add(category);
                }
            }
        }

        private async Task SaveProductAsync()
        {
            var responseHttp = await Repository.PutAsync("api/products/full", productDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                Timer= 3000,
                ShowConfirmButton = true,
            });
            await toast.FireAsync(message: "Producto modificado con exíto.", icon: SweetAlertIcon.Success);
            Return();
        }

        private void Return()
        {
            productForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/products");
        }
    }
}