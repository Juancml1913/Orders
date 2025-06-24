using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.States
{
    public partial class StateDetails
    {
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

        private async Task LoadAsync()
        {
            var responseHttp = await Repository.GetAsync<State>($"/api/states/{StateId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo($"/countries");
                    return;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            state = responseHttp.Response;
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
                if (responseHttp.HttpResponseMessage.StatusCode != System.Net.HttpStatusCode.NotFound) {
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

            await toast.FireAsync(message:"Eliminación exitosa.", icon: SweetAlertIcon.Success);
        }
    }
}
