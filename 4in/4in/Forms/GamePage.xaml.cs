using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using yk.ConnectFour.Logics;
using yk.ConnectFour.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace yk.ConnectFour.Forms
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainPageModel m_Model;
        private Game m_Game;

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            m_Model = new MainPageModel();
        }

        /// <summary>
        ///     Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">
        ///     Event data that describes how this page was reached.
        ///     This parameter is typically used to configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var b = new GameBoard(new GameBoard.BoardBuilder
            {
                Columns = 7,
                Rows = 6
            });
            m_Game = new Game(b, ChipColor.Black);
            m_Model.Mode = (CounterParty) e.Parameter;

            RedrawBoard();
        }

        private void RedrawBoard()
        {
            var ellipses = GameField.Children.Where(c => c is Ellipse).ToList();
            foreach (var uiElement in ellipses)
            {
                GameField.Children.Remove(uiElement);
            }
        }

        private async void DoMove(int Column)
        {
            var move = m_Game.DropChip(Column);
            if (move.State == MoveState.OutOfBounds)
            {
                if (m_Model.RobotsMove)
                    return;
                await new MessageDialog("No more place in this column to put the chip").ShowAsync();
                return;
            }

            DrawChip(move);

            if (move.State == MoveState.Victory)
            {
                var dlg =
                    new MessageDialog(
                        string.Format("Victory! {0} won", move.Color == ChipColor.Black ? "Blue" : "Red"), "Game over");
                dlg.Commands.Add(new UICommand("I know")
                {
                    Invoked = Exit
                });
                m_Model.RobotsMove = false;
                await dlg.ShowAsync();
                return;
            }

            if (move.State == MoveState.OutOfBounds)
            {
                if (m_Model.RobotsMove)
                    return;

                await new MessageDialog("No more place to put the chip").ShowAsync();
                return;
            }

            if (m_Model.Mode == CounterParty.Robot)
            {
                m_Model.RobotsMove = !m_Model.RobotsMove;
            }
        }

        private void DrawChip(Move move)
        {
            var e = new Ellipse
            {
                Fill = new SolidColorBrush(move.Color == ChipColor.Black ? Colors.DarkBlue : Colors.Maroon),
                Width = 50,
                Height = 50
            };
            Grid.SetColumn(e, move.Column);
            Grid.SetRow(e, 5 - move.Row);
            GameField.Children.Add(e);
        }

        private void UIElement_OnTapped(object Sender, TappedRoutedEventArgs E)
        {
            var sender = (Rectangle) Sender;
            var col = Grid.GetColumn(sender);
            DoMove(col);

            if (m_Model.RobotsMove)
            {
                var stuckTimes = 0;
                int min = Math.Max(0, col - 1), max = Math.Min(col + 1, 7);
                while (m_Model.RobotsMove)
                    // if the bot made an erroneous move, it should re-move. Though, potentially this behaviour might lead to an infinite
                    // cycle, the bot will lose much faster. It's because I am told don't mess with the winning strategy.
                {
                    var randomCol = new Random(DateTime.Now.Millisecond).Next(min, max);
                    DoMove(randomCol);
                    stuckTimes += 1;
                    if (stuckTimes == 3)
                    {
                        min = 0;
                        max = 7;
                    }

                    if (stuckTimes > 10)
                    {
                        // Houston, we have problems...
                    }
                }
            }
        }

        private void Exit(IUICommand Command)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
                rootFrame.Navigate(typeof (ModeSelector));
        }
    }
}