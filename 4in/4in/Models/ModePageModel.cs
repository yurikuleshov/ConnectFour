using System.ComponentModel;
using System.Runtime.CompilerServices;
using _4in.Annotations;

namespace yk.ConnectFour.Models
{
    public enum CounterParty
    {
        Human,
        Robot
    }

    public class ModePageModel : INotifyPropertyChanged
    {
        private CounterParty m_Mode;

        public CounterParty Mode
        {
            get { return m_Mode; }
            set
            {
                m_Mode = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}