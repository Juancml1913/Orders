using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Cities
{
    public partial class CityCreate
    {
        private City city = new();

        private FormWithName<City>? formCity;
        [Parameter]
        public int StateId { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        [Inject] SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] IRepository Repository { get; set; } = null!;

        private async Task SaveAsync()
        {
            city.StateId = StateId;
            var responseHttp = await Repository.PostAsync($"/api/cities", city);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(message: message, icon: SweetAlertIcon.Error);
                return;
            }
            Return();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer=3000,
            });

            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro creado con éxito.");
        }

        private void Return()
        {
            formCity!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/states/details/{StateId}");
        }

    }
}
