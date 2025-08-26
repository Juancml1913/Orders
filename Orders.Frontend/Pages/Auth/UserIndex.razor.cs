using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Runtime.InteropServices;

namespace Orders.Frontend.Pages.Auth
{
    [Authorize(Roles = "Admin")]
    public partial class UserIndex
    {
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        public List<User>? Users { get; set; }
        private int currentPage = 1;
        private int totalPages;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int RecordsNumber { get; set; } = 10;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task SelectedRecordsNumberAsync(int recordsNumber)
        {
            RecordsNumber = recordsNumber;
            int page = 1;
            await LoadAsync(page);
        }

        private async Task FilterCallBack(string filter)
        {
            Filter = filter;
            await ApplyFilterAsync();
            StateHasChanged();
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task ApplyFilterAsync()
        {
            await LoadAsync();
        }

        private void ValidateRecordsNumber(int recordsNumber)
        {
            if (recordsNumber == 0)
            {
                RecordsNumber = 10;
            }
        }

        private async Task LoadAsync(int page = 1)
        {
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

        private async Task<bool> LoadListAsync(int page)
        {
            ValidateRecordsNumber(RecordsNumber);
            var url = $"api/accounts/all?page={page}&recordsNumber={RecordsNumber}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }
            var reponseHttp = await Repository.GetAsync<List<User>>(url);
            if (reponseHttp.Error)
            {
                var message = await reponseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }

            Users = reponseHttp.Response!;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/accounts/totalpages?recordsNumber={RecordsNumber}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }
            var reponseHttp = await Repository.GetAsync<int>(url);
            if (reponseHttp.Error)
            {
                var message = await reponseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            totalPages = reponseHttp.Response!;
        }
    }
}