using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;

namespace Orders.Frontend.Pages.Cities
{
    public partial class CityEdit
    {
        [Parameter]
        public int Id { get; set; }
        private City? city;
        private FormWithName<City>? cityForm;
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject]
        IRepository Repository { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<City>($"/api/cities/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo($"/countries");
                    return;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(message: message, icon: SweetAlertIcon.Error);
                return;
            }

            city = responseHttp.Response;
        }

        private async Task UpdateAsync()
        {
            var responseHttp = await Repository.PutAsync($"api/cities", city);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync(message: message, icon: SweetAlertIcon.Error);
            }
            Return();

            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Timer = 3000,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
            });
            await toast.FireAsync(message: "Registro actualizado correctamente.", icon: SweetAlertIcon.Success);

        }

        private void Return()
        {
            cityForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/states/details/{city!.StateId}");
        }
    }
}
