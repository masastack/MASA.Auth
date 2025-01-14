namespace Masa.Stack.Components;

public class SListItem : MListItem
{
    [Parameter] public bool Medium { get; set; }

    protected override IEnumerable<string> BuildComponentClass()
    {
        if (Medium)
        {
            return base.BuildComponentClass().Concat(new[] { "m-list-item" });
        }
        
        return base.BuildComponentClass();
    }
}