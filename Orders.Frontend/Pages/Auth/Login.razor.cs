using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Services;
using Orders.Shared.DTOs;

namespace Orders.Frontend.Pages.Auth
{
    public partial class Login
    {
        private LoginDTO loginDTO = new();
        private bool wasClose = false;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [CascadingParameter] public BlazoredModalInstance BlazoredModal { get; set; } = default!;

        private async Task CloseModalAsync()
        {
            wasClose = true;
            await BlazoredModal.CloseAsync(ModalResult.Ok());
        }
        private async Task SignInAsync()
        {
            if (wasClose)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            var reponseHttp = await Repository.PostAsync<LoginDTO, TokenDTO>("api/Accounts/Login", loginDTO);
            if (reponseHttp.Error)
            {
                var message = await reponseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var token = reponseHttp.Response!.Token;
            await LoginService.LoginAsync(token);
            NavigationManager.NavigateTo("/");
        }

    }
}