using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Java.Lang;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace FirebaseAppDemo
{
    [Activity(MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Android.Support.V7.App.AppCompatActivity, IFacebookCallback
    {

        private ICallbackManager mFBCallManager;
        private MyProfileTracker mprofileTracker;
        private Context mContext;
        const int RC_SIGN_IN = 9001;

        private TextView facebookButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LoginActivity);

            mprofileTracker = new MyProfileTracker();
            mprofileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mprofileTracker.StartTracking();



            // Create your application here


            mContext = this;

            Firebase.FirebaseApp.InitializeApp(this);


            var signedIn = FirebaseAuth.Instance.CurrentUser;

            if (signedIn == null)
            {
                //Toast.MakeText(this, "User Not Signed in", ToastLength.Short).Show();
            }
            else
            {

            }

            TextView txtLogin = FindViewById<TextView>(Resource.Id.txtLogin);
            TextView txtRegister = FindViewById<TextView>(Resource.Id.txtRegister);
            TextView txtGoogleLogin = FindViewById<TextView>(Resource.Id.txtLoginGoogle);



            txtLogin.Click += moveToEmailLogin;
            txtRegister.Click += moveToRegister;


            //facebook

            //var BtnFBLogin = FindViewById<LoginButton>(Resource.Id.txtLoginFacebook);


            //BtnFBLogin.SetReadPermissions(new List<string> {
            //"user_friends",
            // "public_profile",
            //    "email"
            // });

            //mFBCallManager = CallbackManagerFactory.Create();
            //BtnFBLogin.RegisterCallback(mFBCallManager, this);





            mFBCallManager = CallbackManagerFactory.Create();
            LoginManager.Instance.RegisterCallback(mFBCallManager, this);

            facebookButton = FindViewById<TextView>(Resource.Id.txtLoginFacebook);
            facebookButton.Click += OnFacebookButtonClick;



            LoginManager.Instance.LogOut();

        }

        private async void OnFacebookButtonClick(object sender, System.EventArgs e)
        {
            var accessToken = AccessToken.CurrentAccessToken?.Token;

            if (accessToken == null)
            {
                LoginManager.Instance.LogInWithReadPermissions(this, new[] { "public_profile", "email" });
            }
            else
            {
                //Do whatever you want when already logged in
            }
        }



        public void OnCancel() { }
        public void OnError(FacebookException p0) { }
        public void OnSuccess(Java.Lang.Object result)
        {



            LoginResult loginResult = result as LoginResult;


            GraphCallback graphCallBack = new GraphCallback();
            graphCallBack.RequestCompleted += OnGetFriendsResponseAsync;

            Bundle bundle = new Bundle();
            bundle.PutString("fields", "id,name,email");



            var request = new GraphRequest(loginResult.AccessToken, "/" + AccessToken.CurrentAccessToken.UserId, bundle, HttpMethod.Get, graphCallBack).ExecuteAsync();

        }


        class GraphCallback : Java.Lang.Object, GraphRequest.ICallback
        {
            // Event to pass the response when it's completed
            public event EventHandler<GraphResponseEventArgs> RequestCompleted = delegate { };

            public void OnCompleted(GraphResponse reponse)
            {
                this.RequestCompleted(this, new GraphResponseEventArgs(reponse));
            }
        }

        public class GraphResponseEventArgs : EventArgs
        {
            GraphResponse _response;
            public GraphResponseEventArgs(GraphResponse response)
            {
                _response = response;
            }

            public GraphResponse Response { get { return _response; } }
        }


        protected override void OnStart()
        {
            base.OnStart();

            FirebaseAuth.Instance.AuthState += AuthStateChanged;


            //var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
            //if (opr.IsDone)
            //{
            //    // If the user's cached credentials are valid, the OptionalPendingResult will be "done"
            //    // and the GoogleSignInResult will be available instantly.

            //    var result = opr.Get() as GoogleSignInResult;
            //    HandleSignInResult(result);
            //}
            //else
            //{
            //    // If the user has not previously signed in on this device or the sign-in has expired,
            //    // this asynchronous branch will attempt to sign in the user silently.  Cross-device
            //    // single sign-on will occur in this branch.
            //    ShowProgressDialog();
            //    opr.SetResultCallback(new SignInResultCallback { Activity = this });
            //}
        }


        protected override void OnStop()
        {
            base.OnStop();

            FirebaseAuth.Instance.AuthState -= AuthStateChanged;
        }


        public void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;

            if (user != null)
            {
                MoveToDashboard();
                //Toast.MakeText(this, "Firebase Signin Success", ToastLength.Short).Show();
            }
            else
            {
                //Toast.MakeText(this, "Firebase Signin Failed!!", ToastLength.Short).Show();
            }
        }

        private void OnGetFriendsResponseAsync(object sender, GraphResponseEventArgs e)
        {
            string name = e?.Response?.JSONObject.GetString("name");
            string email = e?.Response?.JSONObject.GetString("email");
            string password = "1111111";

            if (name != null && name.Length > 0 && email != null && email.Length > 0)
            {

                registerAsync(name, email, password);
            }
            else
            {

            }

        }


        public void loginAsync(string email, string password)
        {
            try
            {
                FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
               {
                   if (task.IsCompleted)
                   {
                       MoveToDashboard();
                   }
               });
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, "Sign In failed" + ex.Message, ToastLength.Short).Show();
            }
        }


        public void registerAsync(string name, string email, string password)
        {

            try
            {

                FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
                {

                    if (task.IsFaulted)
                    {
                        loginAsync(email, password);
                    }
                    else if (task.IsCompleted)
                    {
                        CreateUser(FirebaseAuth.Instance.CurrentUser.Uid, name, email, password);
                    }


                });

            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }



        //public async System.Threading.Tasks.Task registerAsync(string name, string email, string password)
        //{
        //    try
        //    {
        //        var data = await FirebaseAuth.Instance.CreateUserWithEmailAndPasswordAsync(email, password);

        //        if (data.User != null)
        //        {
        //            CreateUser(data.User.Uid, name, email, password);
        //        }
        //        else
        //        {
        //            Toast.MakeText(this, "Login call", ToastLength.Short).Show();
        //            loginAsync(email, password);
        //        }

        //    }
        //    catch (System.Exception ex)
        //    {
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}

        protected async void CreateUser(string uid, string name, string email, string password)
        {
            LoginManager.Instance.LogOut();

            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            var stringEndPoint = database.Child("User").Child(uid);
            stringEndPoint.Child("name").SetValue(name);
            stringEndPoint.Child("uid").SetValue(uid);
            stringEndPoint.Child("email").SetValue(email);
            stringEndPoint.Child("mobileno").SetValue("919191919191");
            stringEndPoint.Child("location").SetValue("Ahmedabad");
            stringEndPoint.Child("password").SetValue(password);

        }

        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                try
                {
                    string name = e.mProfile.Name;
                    string profileId = e.mProfile.Id;

                }
                catch (Java.Lang.Exception exc)
                {
                }
            }
            else
            {

            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mFBCallManager.OnActivityResult(requestCode, (int)resultCode, data);


        }

        public class MyProfileTracker : ProfileTracker
        {
            public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;
            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
            {
                if (mOnProfileChanged != null)
                {
                    mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
                }
            }
        }

        public class OnProfileChangedEventArgs : EventArgs
        {
            public Profile mProfile;
            public OnProfileChangedEventArgs(Profile profile)
            {
                mProfile = profile;


            }
        }

        void moveToEmailLogin(object sender, EventArgs e)
        {
            Intent emailLogin = new Intent(this, typeof(EmailLoginActivity));
            StartActivity(emailLogin);

        }

        public void moveToRegister(object sender, EventArgs e)
        {
            Intent register = new Intent(this, typeof(RegisterActivity));
            StartActivity(register);
        }

        protected void MoveToDashboard()
        {
            Intent dashboard = new Intent(this, typeof(DashboardActivity));
            StartActivity(dashboard);
            Finish();
        }






        //Google Sign in


    }
}
