using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Orders.Frontend.Pages.Auth;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages
{
    public partial class Home
    {
        private int currentPage = 1;
        private int totalPages;
        private int counter = 0;
        private bool isAuthenticated;

        public List<Product>? Products { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int RecordNumber { get; set; } = 8;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        protected async override Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        protected async override Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
            await LoadCounterAsync();
        }

        private async Task LoadCounterAsync()
        {
            if (!isAuthenticated)
            {
                return;

            }
            var responseHttp = await Repository.GetAsync<int>("api/temporalorders/count");
            if (responseHttp.Error)
            {
                return;
            }
            counter = responseHttp.Response;

        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await AuthenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        private void ValidateRecordsNumber()
        {
            if (RecordNumber == 0)
            {
                RecordNumber = 8;
            }
        }

        private async Task SelectedRecordsNumberAsync(int recordsNumber)
        {
            RecordNumber = recordsNumber;
            int page = 1;
            await SelectedPageAsync(page);
        }

        private async Task FilterCallBack(string filter)
        {
            Filter = filter;
            await ApplyFilterAsync();
            StateHasChanged();
        }

        private async Task AddToCartAsync(int productId)
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
                await toast1.FireAsync(icon: SweetAlertIcon.Error, message: "Deber iniciar sesión para poder agregar productos al carrito de compras.");
                return;
            }
            var temporalOrderDTO = new TemporalOrderDTO
            {
                ProductId = productId,
            };

            var responseHttp = await Repository.PostAsync("api/temporalorders/full", temporalOrderDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await LoadCounterAsync();
            var toast2 = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = false,
                Timer = 3000,
            });
            await toast2.FireAsync(icon: SweetAlertIcon.Success, message: "Producto añadido al carrito correctamente.");
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await SelectedPageAsync(page);
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            ValidateRecordsNumber();
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/products/totalpages?recordsnumber={RecordNumber}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            totalPages = responseHttp.Response;
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/products?page={page}&recordsnumber={RecordNumber}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<List<Product>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }

            Products = responseHttp.Response;
            return true;
        }
    }
}
