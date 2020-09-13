using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BoxExLogic;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BoxHw
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BoxManger boxManger;
        Action filter;
        public MainPage()
        {
            this.InitializeComponent();
             boxManger = new BoxManger(BoxManger_DateRemoved);
            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size((double)1280, (double)720);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Application.Current.Suspending += Current_Suspending;
        }
        private void BoxManger_DateRemoved(object sender, DateRemovedEventArgs e)
        {
            screen_txtblock.Text = "this boxes were deleted because they werent bought for too long\n";
            foreach (var item in e.Deleted)
            {
               screen_txtblock.Text+=$"box amount: {item.Amount} , box width: {item.X}, box height: {item.Y}";
            }
        }
        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            boxManger.Save();
        }
        private void buy_btn_Click(object sender, RoutedEventArgs e)
        {
            filter = Action.Buy;
            enter_btn.Content = "buy";
            amount_txtbox.Visibility = Visibility.Visible;
        }
        private void Show_btn_Click(object sender, RoutedEventArgs e)
        {
            filter = Action.Show;
            enter_btn.Content = "show";
            amount_txtbox.Visibility = Visibility.Collapsed;
        }
        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            filter = Action.Add;
            enter_btn.Content = "add";
            amount_txtbox.Visibility = Visibility.Visible;
        }
        private async void enter_btn_Click(object sender, RoutedEventArgs e)
        {
            if(width_txtbox.Text==""||heighet_txtbox.Text=="")
            {
                MessageDialog message = new MessageDialog("you must enetr all the details");
                await message.ShowAsync();
                return;
            }
            if (!double.TryParse(width_txtbox.Text,out double x) ||! double.TryParse(heighet_txtbox.Text, out double y))
            {
                MessageDialog message = new MessageDialog("you must enetr numbers");
                await message.ShowAsync();
                return;
            }
            if (x==0 || y==0)
            {
                MessageDialog message = new MessageDialog("you must enetr numbers greater then 0");
                await message.ShowAsync();
                return;
            }
            Box box;
            switch (filter)
            { 
                case Action.Buy:
                    if (!int.TryParse(amount_txtbox.Text, out int amount1) || amount1 <= 0)
                    {
                        MessageDialog message = new MessageDialog("you must enetr numbers");
                        await message.ShowAsync();
                        return;
                    }
                    if (amount1==1)
                    {
                        try
                        {
                            box = boxManger.Buy(x, y, out string s2);
                            if (box == null) { screen_txtblock.Text = "we arent able to find box that match your sizes"; return; }
                            if (x == box.X && y == box.Y)
                            {
                                screen_txtblock.Text = "you succssesfully bought the box " + s2;
                            }
                            else
                            {
                                screen_txtblock.Text = $"we unfortunaly dont have box with your exact sizes so we gave you box with width {box.X} and heighet {box.Y}";
                            }
                        }
                        catch (BoxException aae)
                        {

                            screen_txtblock.Text = aae.Message;
                        } 
                    }
                    else
                    {
                        enter_btn.HorizontalAlignment = HorizontalAlignment.Right;
                        yes_btn.Visibility = Visibility.Visible;
                        no_btn.Visibility = Visibility.Visible;
                        string answer = boxManger.BuyManyOrder(x, y, amount1, out bool isSucces);
                        screen_txtblock.Text = answer;
                        if(!isSucces)screen_txtblock.Text += "\nclick yes to aprrove and no to dismiss";                          
                    }
                    break;
                case Action.Show:
                     box = boxManger.Show(x, y);
                    if(box==null)
                    {
                        screen_txtblock.Text = "we unfortunaly dont have box with your exact sizes ";
                    }
                    else
                    {
                        screen_txtblock.Text = $"width: {box.X} heighet: {box.Y} amount: {box.Amount} last time ordrerd: {box.LastTimeOrder.ToShortDateString()}";
                    }
                    break;
                case Action.Add:
                    if (!int.TryParse(amount_txtbox.Text,out int amount)||amount<=0)
                    {
                        MessageDialog message = new MessageDialog("you must enetr numbers");
                        await message.ShowAsync();
                        return;
                    }
                    try
                    {
                        boxManger.Add(x, y, amount, out string s);
                        screen_txtblock.Text = s;
                        break;
                    }
                    catch (ArgumentException ae)
                    {

                        MessageDialog message = new MessageDialog(ae.Message);
                        await message.ShowAsync();
                        return;
                        
                    }
                    
            }
            amount_txtbox.Text = "";
            heighet_txtbox.Text = "";
            width_txtbox.Text = "";
        }
        private void YES_btn_Click(object sender, RoutedEventArgs e)
        {
            boxManger.BuyManyChoise(true);
            screen_txtblock.Height = 200;
            screen_txtblock.Text = "order approved";
            enter_btn.HorizontalAlignment = HorizontalAlignment.Center;
            yes_btn.Visibility = Visibility.Collapsed;
            no_btn.Visibility = Visibility.Collapsed;
        }
        private void NO_btn_Click(object sender, RoutedEventArgs e)
        {
            screen_txtblock.Height = 200;
            screen_txtblock.Text = "order dissmissed";
            enter_btn.HorizontalAlignment = HorizontalAlignment.Center;
            yes_btn.Visibility = Visibility.Collapsed;
            no_btn.Visibility = Visibility.Collapsed;
        }
        private void width_txtbox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c)&&!char.IsPunctuation(c));
        }
    }
    public enum Action
    {
        Buy=1,
        Show=2,
        Add=3
    }
}
