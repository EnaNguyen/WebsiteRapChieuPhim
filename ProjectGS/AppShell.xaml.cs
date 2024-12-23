using ProjectGS.Pages;

namespace ProjectGS
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();       
        }
        private readonly static Type[] _routablePageTypes =
            [
                typeof(Signin),
                typeof(Signup),
                typeof(MyOrderPage),
                typeof(OrderDetailPage),
                typeof(DetailPage),
            ]; 
            
        private void RegisterRoutes()
        {
            foreach (var pageType in _routablePageTypes) 
            {
                Routing.RegisterRoute(pageType.Name, pageType);
            }
/*            Routing.RegisterRoute("signin", typeof(Signin));
            Routing.RegisterRoute("signup", typeof(Signup));*/
        }
        private async void SignOut_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.DisplayAlert("Thông Báo", "Bạn có muốn đăng xuất không?", "OK");
        }
    }
}
