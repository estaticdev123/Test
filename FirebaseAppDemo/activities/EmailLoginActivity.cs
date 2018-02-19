
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Auth;

namespace FirebaseAppDemo
{
    [Activity(Label = "EmailLoginActivity", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class EmailLoginActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EmailLoginActivity);
            // Create your application here


            TextView txtLogin = FindViewById<TextView>(Resource.Id.txtLogin);
            txtLogin.Click += async delegate (object sender, EventArgs e)
            {
                await loginAsync(sender, e);
            };

            TextView txtRegister = FindViewById<TextView>(Resource.Id.txtRegister);
            txtRegister.Click += moveToRegister;

        }


        public void AuthStateChanged(object sender, FirebaseAuth.AuthStateEventArgs e)
        {
            var user = e.Auth.CurrentUser;

            if (user != null)
            {
                MoveToDashboard();
                Toast.MakeText(this, "Firebase Signin Success", ToastLength.Short).Show();

            }
            else
            {
                Toast.MakeText(this, "Firebase Signin Failed!!", ToastLength.Short).Show();
            }
        }


        protected override void OnStart()
        {
            base.OnStart();

            FirebaseAuth.Instance.AuthState += AuthStateChanged;
        }


        protected override void OnStop()
        {
            base.OnStop();

            FirebaseAuth.Instance.AuthState -= AuthStateChanged;
        }

        async System.Threading.Tasks.Task loginAsync(object sender, EventArgs e)
        {
            EditText username = FindViewById<EditText>(Resource.Id.edtEmail);
            EditText password = FindViewById<EditText>(Resource.Id.edtPassword);

            String email = username.Text.ToString();
            String passwordText = password.Text.ToString();

            if (email != null && email.Length > 0 && isValidEmail(email))
            {
                if (passwordText != null && passwordText.Length > 6)
                {
                    //Toast.MakeText(this, "Login Call", ToastLength.Short).Show();

                    try
                    {
                        await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, passwordText);
                    }
                    catch (Exception ex)
                    {
                        // Sign-in failed, display a message to the user
                        // If sign in succeeds, the AuthState event handler will
                        //  be notified and logic to handle the signed in user can happen there
                        Toast.MakeText(this, "Sign In failed" + ex.Message, ToastLength.Short).Show();
                    }

                }
                else
                    Toast.MakeText(this, "Enter Password more than 6 character", ToastLength.Short).Show();
            }
            else Toast.MakeText(this, "Enter Valid Email Address", ToastLength.Short).Show();
        }

        void moveToRegister(object sender, EventArgs e)
        {
            Intent register = new Intent(this, typeof(RegisterActivity));
            StartActivity(register);
            Finish();
        }


        public bool isValidEmail(string email)
        {
            return Android.Util.Patterns.EmailAddress.Matcher(email).Matches();
        }

        protected void MoveToDashboard()
        {
            Intent dashboard = new Intent(this, typeof(DashboardActivity));
            StartActivity(dashboard);
            Finish();
        }
    }
}
