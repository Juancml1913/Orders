using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Runtime.InteropServices;

namespace Orders.Frontend.Pages.Countries
{
    public partial class CountryDetails
    {
        private Country? country;
        private List<State>? states;
        private int currentPage = 1;
        private int totalPage;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject]
        private IRepository Repository { get; set; } = null!;
        [Parameter]
        public int CountryId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync(int page = 1)
        {
            var ok = await LoadCountryAsync();
            if (ok)
            {
                ok = await LoadStateAsync(page);
                if (ok)
                {
                    await LoadPageAsync();
                }
            }
        }

        private async Task SelectedPage(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadPageAsync()
        {
            var responseHttp = await Repository.GetAsync<int>($"/api/states/totalpages?id={CountryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPage = responseHttp.Response;
        }
        private async Task<bool> LoadStateAsync(int page)
        {
            var responseHttp = await Repository.GetAsync<List<State>>($"/api/states?id={CountryId}&page={page}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            states = responseHttp.Response;
            return true;
        }
        private async Task<bool> LoadCountryAsync()
        {
            var responseHttp = await Repository.GetAsync<Country>($"/api/countries/{CountryId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/countries");
                    return false;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            country = responseHttp.Response;
            return true;
        }

        private async Task DeteleAsync(State state)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Deseas eliminar el departamento/estado {state.Name}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                CancelButtonText = "No",
                ConfirmButtonText = "Sí"
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<State>($"/api/states/{state.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
            }

            await LoadAsync();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000,
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: $"El departamento/estado {state.Name} ha sido eliminado correctamente.");
        }
    }
}
