using Microsoft.AspNetCore.Components;
using Orders.Frontend.Helpers;

namespace Orders.Frontend.Shared
{
    public partial class MultipleSelector
    {
        private string addAllText = ">>";
        private string removeAllText = "<<";
        [Parameter] public List<MultipleSelectorModel> NonSelected { get; set; } = new();
        [Parameter] public List<MultipleSelectorModel> Selected { get; set; } = new();

        private void Select(MultipleSelectorModel item)
        {
            NonSelected.Remove(item);
            Selected.Add(item);
        }

        private void Unselect(MultipleSelectorModel item)
        {
            NonSelected.Add(item);
            Selected.Remove(item);
        }

        private void SelectAll()
        {
            Selected.AddRange(NonSelected);
            NonSelected.Clear();
        }

        private void UnselectAll()
        {
            NonSelected.AddRange(Selected);
            Selected.Clear();
        }
    }
}