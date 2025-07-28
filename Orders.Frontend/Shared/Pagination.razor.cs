using Microsoft.AspNetCore.Components;
using System.Security;

namespace Orders.Frontend.Shared
{
    public partial class Pagination
    {
        private List<PageModel> links = [];
        private List<OptionModel> options = [];
        private int selectedOptionValue = 10;
        [Parameter] public int CurrentPage { get; set; } = 1;
        [Parameter] public int TotalPage { get; set; } = 1;
        [Parameter] public int Radio { get; set; } = 10;
        [Parameter] public EventCallback<int> SelectedPage { get; set; }
        [Parameter] public EventCallback<int> RecordsNumber { get; set; }
        [Parameter] public bool IsHome { get; set; }

        protected override void OnParametersSet()
        {
            BuildPages();
            BuildOptions();

        }

        private void BuildOptions()
        {
            if (IsHome)
            {
                options = [new OptionModel { Value = 8, Name = "8" },
                new OptionModel { Value = 16, Name = "16" },
                new OptionModel { Value = 32, Name = "32" },
                new OptionModel { Value = int.MaxValue, Name = "Todos" }];
            }
            else
            {
                options = [new OptionModel { Value = 10, Name = "10" },
                new OptionModel { Value = 25, Name = "25" },
                new OptionModel { Value = 50, Name = "50" },
                new OptionModel { Value = int.MaxValue, Name = "Todos" }];
            }
        }

        private void BuildPages()
        {
            links = new List<PageModel>();
            links.Add(new PageModel
            {
                Text = "Anterior",
                Page = CurrentPage - 1,
                Enable = CurrentPage != 1,
            });

            for (int i = 1; i<= TotalPage; i++)
            {
                if (TotalPage <= Radio)
                {
                    links.Add(new PageModel
                    {
                        Text = i.ToString(),
                        Page = i,
                        Enable = true,
                        Active = i == CurrentPage
                    });
                }
                if (TotalPage > Radio && i<= Radio && CurrentPage <= Radio)
                {
                    links.Add(new PageModel
                    {
                        Text = i.ToString(),
                        Page = i,
                        Enable = true,
                        Active = i == CurrentPage
                    });
                }
                if (CurrentPage > Radio && i> CurrentPage - Radio && i <= CurrentPage)
                {
                    links.Add(new PageModel
                    {
                        Text = i.ToString(),
                        Page = i,
                        Enable = true,
                        Active = i == CurrentPage
                    });
                }

            }

            links.Add(new PageModel
            {
                Text = "Siguiente",
                Page =CurrentPage != TotalPage ? CurrentPage + 1 : CurrentPage,
                Enable = CurrentPage != TotalPage,
            });
        }
        private async Task InternalRecordsNumberSelected(ChangeEventArgs e)
        {
            if (e.Value != null)
            {
                selectedOptionValue = Convert.ToInt32(e.Value.ToString());
            }
            await RecordsNumber.InvokeAsync(selectedOptionValue);
        }

        private async Task InternalSelectedPage(PageModel pageModel)
        {
            if (pageModel.Page == CurrentPage || pageModel.Page == 0)
            {
                return;
            }
            await SelectedPage.InvokeAsync(pageModel.Page);
        }
        private class OptionModel
        {
            public string Name { get; set; } = null!;
            public int Value { get; set; }
        }
        private class PageModel
        {
            public string Text { get; set; } = null!;
            public int Page { get; set; }
            public bool Enable { get; set; }
            public bool Active { get; set; }
        }
    }
}
