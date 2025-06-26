using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.States
{
    public partial class StateDetails
    {
        private int totalPages;
        private int currentPage = 1;
        private List<City>? cities;
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject]
        IRepository Repository { get; set; } = null!;
        private State? state;

        [Parameter]
        public int StateId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync(int page = 1)
        {
            var ok = await LoadStatesAsync();
            if (ok)
            {
                ok = await LoadCitiesAsync(page);
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
            var responseHttp = await Repository.GetAsync<int>($"/api/cities/totalpages?id={StateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPages = responseHttp.Response;
        }

        private async Task<bool> LoadCitiesAsync(int page)
        {
            var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities?id={StateId}&page={page}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            cities = responseHttp.Response;
            return true;
        }

        private async Task<bool> LoadStatesAsync()
        {
            var responseHttp = await Repository.GetAsync<State>($"/api/states/{StateId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo($"/countries");
                    return false;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            state = responseHttp.Response;
            return true;
        }

        private async Task DeleteAsync(City city)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Deseas eliminar la ciudad {city.Name}?",
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
            var responseHttp = await Repository.DeleteAsync<City>($"/api/cities/{city.Id}");
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
                Timer = 3000,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,

            });

            await toast.FireAsync(message: "Eliminación exitosa.", icon: SweetAlertIcon.Success);
        }
    }
}
