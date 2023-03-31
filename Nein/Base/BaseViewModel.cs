using ReactiveUI;

namespace Nein.Base;

public class BaseViewModel : ReactiveObject, IActivatableViewModel
{
    public ViewModelActivator Activator { get; protected init; }
}