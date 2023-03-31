using ReactiveUI;

namespace Nein.Base;

public class BaseWindowViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();
}