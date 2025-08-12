using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Orders.Frontend.Pages.Auth;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Products
{
    public partial class ProductDetails
    {
        private List<string>? categories;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private bool isAuthenticated;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int ProductId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
        [CascadingParameter] private IModalService Modal { get; set; } = default!;
        public TemporalOrderDTO TemporalOrderDTO { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadProductAsync();
        }

        private async Task LoadProductAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<Product>($"api/products/{ProductId}");
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            product = responseHttp.Response;
            categories = product!.ProductCategories!.Select(c => c.Category!.Name).ToList();
            images = product.ProductImages!.Select(i => i.Image).ToList();
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await AuthenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        private async Task AddToCartAsync()
        {
            if (!isAuthenticated)
            {
                Modal.Show<Login>();
                var toast1 = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Toast = true,
                    Position = SweetAlertPosition.BottomEnd,
                    ShowConfirmButton = false,
                    Timer = 3000,
                });
                await toast1.FireAsync(icon: SweetAlertIcon.Error, message: "Debes iniciar sesión para poder agregar productos al carrito de compras.");
                return;
            }
            TemporalOrderDTO.ProductId = ProductId;

            var responseHttp = await Repository.PostAsync("api/temporalorders/full", TemporalOrderDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var toast2 = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = false,
                Timer = 3000,
            });
            await toast2.FireAsync(icon: SweetAlertIcon.Success, message: "Producto añadido al carrito correctamente.");
        }
    }
}