using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Shared;
using Orders.Shared.Entities;
using System.Diagnostics.Metrics;

namespace Orders.Frontend.Pages.States
{
    public partial class StateEdit
    {
        private State? state;
        private FormWithName<State>? stateForm;
        [Inject]
        NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject]
        IRepository Repository { get; set; } = null!;

        [EditorRequired, Parameter]
        public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<State>($"api/states/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo($"/countries/details/{1}");
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                }
            }
            else
            {
                state = responseHttp.Response;
            }
        }

        private async Task UpdateAsync()
        {

        }

        private async Task Return()
        {

        }
    }
}
