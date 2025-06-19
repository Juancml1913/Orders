using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;
using System.Diagnostics.Metrics;

namespace Orders.Frontend.Pages.States
{
    public partial class StateCreate
    {
        public FormWithName<State>? stateForm;

        private State state = new();
        [Inject]
        private NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject]
        private IRepository Repository { get; set; } = null!;
        [Parameter]
        public int CountryId { get; set; }

        private async Task CreateAsync()
        {
            state.CountryId = CountryId;
            var responseHttp = await Repository.PostAsync("api/states", state);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
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
            stateForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/countries/details/{CountryId}");
        }

    }
}
