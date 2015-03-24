using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ConnectionLogger.Controls
{
    public class LogCell : ViewCell
    {
        Label timeLabel;
        Label statusLabel;
        Label messageLabel;
        Label speedLabel;
        Label ipLabel;

        Image icon;

        Label border;

        public LogCell()
        {
            Grid grid = new Grid
            {
                Padding = new Thickness(5, 10, 0, 0),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) },
                   new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) },
                },
                RowDefinitions =
                {
                    new RowDefinition(){Height=50},
                    new RowDefinition(),
                    new RowDefinition(){Height=5}
                }
            };

            timeLabel = new Label()
            {
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,

            };
            ipLabel = new Label()
            {
                YAlign = TextAlignment.Center,
                XAlign = TextAlignment.Center,

            };
            statusLabel = new Label()
            {
                YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Start,
				Font = Font.SystemFontOfSize(NamedSize.Small,FontAttributes.Bold)
            };
            speedLabel = new Label()
            {
                YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Start,
				Font = Font.SystemFontOfSize(NamedSize.Medium,FontAttributes.Bold)
            };
            messageLabel = new Label()
            {
                YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Start
            };

            icon = new Image() 
            {
                 Aspect= Aspect.AspectFit,
                 HeightRequest=32,
                 WidthRequest=32
            };

            border = new Label() 
            {
                BackgroundColor=Color.Blue
            };

            timeLabel.SetBinding(Label.TextProperty, "Title");
            statusLabel.SetBinding(Label.TextProperty, "Status");
            messageLabel.SetBinding(Label.TextProperty, "Message");
            speedLabel.SetBinding(Label.TextProperty, "Speed");
            ipLabel.SetBinding(Label.TextProperty, "IP");
            icon.SetBinding(Image.SourceProperty, "IconSource");

            grid.Children.Add(icon, 0, 0);
            grid.Children.Add(timeLabel, 1, 0);
            grid.Children.Add(ipLabel, 2, 0);
            grid.Children.Add(statusLabel, 0, 1);
            grid.Children.Add(messageLabel, 1, 1);
            grid.Children.Add(speedLabel, 2, 1);
            grid.Children.Add(border, 0, 3,2,3);

            this.View = grid;
            this.View.HorizontalOptions = LayoutOptions.Center;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            
        }
    }
}
