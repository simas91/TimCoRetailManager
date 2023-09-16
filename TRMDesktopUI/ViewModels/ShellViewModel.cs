using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using TRMDesktopUI.EventModels;

namespace TRMDesktopUI.ViewModels
{
    // SalesViewModel is used here to save the data in case the user exits the form with sales
    // once the user comes back it will still have data filled for sales
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private SimpleContainer _container;

        // SimpleContainer lets us request new instances
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, 
            SimpleContainer container)
        {
            _events = events;
            _salesVM = salesVM;
            _container = container;

            _events.SubscribeOnUIThread(this);

            // makes LoginViewModel not a singleton and gives new instance on request
            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }


        public Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            ActivateItemAsync(_salesVM);
            return Task.CompletedTask;
        }
    }
}
