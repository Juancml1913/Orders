using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Enums;
using System.Net;
using System.Threading.Tasks;

namespace Orders.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin")]
    public partial class OrderDetails
    {
        private Order? order;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int OrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var responseHttp = await Repository.GetAsync<Order>($"api/orders/{OrderId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/orders");
                    return;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            order = responseHttp.Response;
        }

        private async Task CancelOrderAsync()
        {
            await ModifyTemporalOrderAsync("cancelar", OrderStatus.Cancelled);
        }

        private async Task DispathOrderAsync()
        {
            await ModifyTemporalOrderAsync("despachar", OrderStatus.Dispatched);
        }
        private async Task SendOrderAsync()
        {
            await ModifyTemporalOrderAsync("enviar", OrderStatus.Sent);
        }
        private async Task ConfirmOrderAsync()
        {
            await ModifyTemporalOrderAsync("confirmar", OrderStatus.Confirmed);
        }
        private async Task ModifyTemporalOrderAsync(string message, OrderStatus status)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title= "Confirmaci�n",
                Text = $"�Est� seguro que quieres {message} el pedido?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }
            var orderDTO = new OrderDTO
            {
                Id = OrderId,
                OrderStatus = status,
            };
            var responseHttp = await Repository.PutAsync("api/orders", orderDTO);
            if (responseHttp.Error)
            {
                var messageHttp = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", messageHttp, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/orders");
        }
    }
}