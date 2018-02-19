
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using FirebaseAppDemo.models;
using Java.Util;
using Newtonsoft.Json;

namespace FirebaseAppDemo
{



    public class CalanderFragment : Android.Support.V4.App.Fragment, TabLayout.IOnTabSelectedListener, IValueEventListener
    {

        String[] Week = new String[] { "", "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
        private TabLayout tabLayout;

        private RecyclerView recyclerView;
        private RecyclerViewAdapter recyclerAdapter;
        private List<DetailModel> arrayList;
        private ProgressBar progressBar;
        private string currentDate = "";
        private TextView txtLocation;

        public CalanderFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.Main, container, false);
            return view;
        }



        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);


            //toolbar
            Android.Support.V7.Widget.Toolbar toolbar = (Android.Support.V7.Widget.Toolbar)((DashboardActivity)Activity).FindViewById(Resource.Id.toolbar);
            TextView mTitle = (TextView)toolbar.FindViewById(Resource.Id.toolbar_title);
            if (mTitle != null)
                mTitle.Text = "March 2018";

            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycler_view);

            tabLayout = view.FindViewById<TabLayout>(Resource.Id.tab_layout);

            ImageView imgMinusHours = view.FindViewById<ImageView>(Resource.Id.img_minus_hours);
            ImageView imgPlusHours = view.FindViewById<ImageView>(Resource.Id.img_plus_hours);

            TextView txtNoOfHours = view.FindViewById<TextView>(Resource.Id.txt_no_of_hours);
            txtLocation = view.FindViewById<TextView>(Resource.Id.txt_your_location);

            progressBar = (ProgressBar)view.FindViewById(Resource.Id.progressBar);

            imgPlusHours.Click += delegate
            {

                String hou = txtNoOfHours.Text;

                if (hou.Equals("4"))
                {
                    Toast.MakeText(Activity, "Max 4 Hours", ToastLength.Short).Show();
                }
                else
                {
                    int nHour = int.Parse(hou);
                    int newHour = nHour + 1;
                    txtNoOfHours.Text = (newHour.ToString());
                }
            };


            imgMinusHours.Click += delegate
            {

                String hou = txtNoOfHours.Text;


                if (hou.Equals("1"))
                {
                    Toast.MakeText(Activity, "Min 1 Hour Required", ToastLength.Short).Show();
                }
                else
                {

                    int nHour = int.Parse(hou);
                    int newHour = nHour - 1;
                    txtNoOfHours.Text = (newHour.ToString());

                }
            };


            setCustomTabs();

            arrayList = new List<DetailModel>();


            //for (int i = 0; i < 20; i++)
            //{
            //    DetailModel detailModel = new DetailModel();
            //    detailModel.time = ("9:" + i);
            //    detailModel.status = ("status" + i);
            //    detailModel.location = ("9" + i + " m");
            //    detailModel.money = ("9" + i + " Euro");
            //    arrayList.Add(detailModel);
            //}

            recyclerAdapter = new RecyclerViewAdapter(Activity, arrayList, recyclerView, (FirebaseAppDemo.DashboardActivity)Activity);
            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(mLayoutManager);
            recyclerView.SetAdapter(recyclerAdapter);

            tabLayout.AddOnTabSelectedListener(this);

            getCalanderData(Calendar.Instance.Get(Calendar.Date).ToString());
            currentDate = getCurrentDateNow(Calendar.Instance.Get(Calendar.Date).ToString());



            setAddress();
            setLocationData();
        }


        public void setAddress()
        {
            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            var query = database.Child("User").Child(FirebaseAuth.Instance.CurrentUser.Uid);

            query.AddListenerForSingleValueEvent(new OnProfileData());
        }

        public class OnProfileData : Java.Lang.Object, Firebase.Database.IValueEventListener
        {
            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {

                if (snapshot.Exists())
                {
                    string location = snapshot.Child("location")?.GetValue(true)?.ToString();

                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString("location", location);
                    editor.Apply();
                }

            }
        }

        public void setLocationData()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Application.Context);
            txtLocation.Text = prefs.GetString("location", "Ahmedabad");
        }


        public string getCurrentDateNow(string date)
        {
            var dateToGet = new DateTime(2018, 2, int.Parse(date), 0, 0, 0);
            var format = "dd-MM-yyyy";
            var dtString = dateToGet.ToString(format);
            return dtString;
        }


        private void setCustomTabs()
        {

            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("First"));

            Calendar calendar = Calendar.Instance;
            for (int i = 0; i < tabLayout.TabCount; i++)
            {
                TabLayout.Tab tab = tabLayout.GetTabAt(i);
                View tabView = ((ViewGroup)tabLayout.GetChildAt(0)).GetChildAt(i);
                tabView.RequestLayout();
                var view = LayoutInflater.From(Context).Inflate(Resource.Layout.custom_tab, null);

                TextView txt_cal_day = view.FindViewById<TextView>(Resource.Id.txt_cal_day);
                TextView txt_date = view.FindViewById<TextView>(Resource.Id.txt_date);

                View viewCircle = view.FindViewById<View>(Resource.Id.view_circle);

                if (i == 2 || i == 3 || i == 4)
                {
                    viewCircle.Visibility = ViewStates.Visible;
                }
                else
                {
                    viewCircle.Visibility = ViewStates.Gone;
                }

                if (i != 0)
                    calendar.Add(Calendar.Date, 1);

                txt_date.Text = ("" + calendar.Get(Calendar.Date));
                txt_cal_day.Text = (Week[calendar.Get(Calendar.DayOfWeek)]);
                tab.SetCustomView(view);
                //tab.Text = (i + "");
            }

        }


        private void getCalanderData()
        {

            //var user = FirebaseAuth.Instance.CurrentUser;

            DatabaseReference database = FirebaseDatabase.Instance.Reference;
            Query query = database.Child("Booked").OrderByChild("cust_id").EqualTo(FirebaseAuth.Instance.CurrentUser.Uid);
            query.AddListenerForSingleValueEvent(this);
        }


        private void getCalanderData(string date)
        {

            //var user = FirebaseAuth.Instance.CurrentUser;

            var dateToGet = new DateTime(2018, 2, int.Parse(date), 0, 0, 0);

            //var is24hour = Android.Text.Format.DateFormat.Is24HourFormat(this);
            //var format = is24hour ? "dd/MM/yyyy HH:mm" : "dd/MM/yyyy hh:mm tt";

            var format = "dd-MM-yyyy";
            var dtString = dateToGet.ToString(format);


            DatabaseReference database = FirebaseDatabase.Instance.Reference;

            Query query = database.Child("employee");
            query.AddListenerForSingleValueEvent(this);
        }



        public void OnTabReselected(TabLayout.Tab tab)
        {

        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            View view = tab.CustomView;

            TextView txt_cal_day = view.FindViewById<TextView>(Resource.Id.txt_cal_day);
            txt_cal_day.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.colorwhite)));

            TextView txt_date = view.FindViewById<TextView>(Resource.Id.txt_date);
            //getCalanderData(txt_date.Text);

            currentDate = getCurrentDateNow(txt_date.Text);
            getCalanderData(txt_date.Text);

        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            View view = tab.CustomView;

            TextView txt_cal_day = view.FindViewById<TextView>(Resource.Id.txt_cal_day);
            txt_cal_day.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.colorGrayDark)));



        }

        public void OnCancelled(DatabaseError error)
        {
        }



        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                arrayList.Clear();

                string date = currentDate;

                for (int i = 9; i < 15; i++)
                {
                    DetailModel model = new DetailModel();
                    model.time = i.ToString();
                    model.showtime = "Not Available";
                    model.location = "";
                    model.money = "";
                    model.date = date;
                    arrayList.Add(model);
                }

                var obj = snapshot.Children;

                List<DetailModel> arrayListTemp = new List<DetailModel>();
                for (int i = 0; i < arrayList.Count; i++)
                {
                    arrayListTemp.Add(arrayList[i]);
                }


                foreach (DataSnapshot snapshotChild in obj.ToEnumerable())
                {
                    if (snapshotChild.GetValue(true) == null) continue;

                    DetailModel model = new DetailModel();

                    model.showtime = "Available";
                    model.location = snapshotChild.Child("distance")?.GetValue(true)?.ToString();
                    model.money = snapshotChild.Child("net_price")?.GetValue(true)?.ToString();
                    model.emp_id = snapshotChild.Child("emp_id")?.GetValue(true)?.ToString();
                    var data = snapshotChild.Child("avaibility").Value;


                    string output = JsonConvert.SerializeObject(data);
                    var arraycombination = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AvailablityModelClass>>(output);

                    model.availableslotList = arraycombination;


                    for (int i = 0; i < model.availableslotList.Count; i++)
                    {

                        for (int j = 0; j < arrayList.Count(); j++)
                        {
                            if (arrayList[j].time.Equals(model.availableslotList[i].availabletime) && date.Equals(model.availableslotList[i].availabledate) && model.availableslotList[i].status.Equals("true"))
                            {
                                model.time = model.availableslotList[i].availabletime;
                                model.availabledate = model.availableslotList[i].availabledate;
                                model.availabletime = model.availableslotList[i].availabletime;
                                model.status = model.availableslotList[i].status;

                                arrayListTemp[j] = model;
                                break;
                            }
                        }
                    }
                }

                arrayList.Clear();
                arrayList = arrayListTemp;

                try
                {
                    recyclerAdapter = new RecyclerViewAdapter(Activity, arrayList, recyclerView, (FirebaseAppDemo.DashboardActivity)Activity);
                    recyclerView.SetAdapter(recyclerAdapter);
                }
                catch (Exception e)
                {

                }


                recyclerAdapter.NotifyDataSetChanged();


            }
            else
            {
                arrayList.Clear();

                for (int i = 9; i < 15; i++)
                {
                    DetailModel model = new DetailModel();
                    model.time = i.ToString();
                    model.status = "Not Available";
                    model.location = "";
                    model.money = "";
                    arrayList.Add(model);
                }

                recyclerAdapter.NotifyDataSetChanged();
            }

            if (progressBar != null)
                progressBar.Visibility = ViewStates.Gone;
        }


        public class MyValueEventListener : Java.Lang.Object, Firebase.Database.IValueEventListener
        {

            private List<DetailModel> list;
            private RecyclerViewAdapter adapter;
            private string time;

            public MyValueEventListener(List<DetailModel> list, RecyclerViewAdapter adapter, string time)
            {
                this.list = list;
                this.adapter = adapter;
                this.time = time;
            }


            public void OnCancelled(DatabaseError error)
            {

            }

            public void OnDataChange(DataSnapshot snapshot)
            {


                DetailModel model = new DetailModel();



                model.time = time;
                model.status = "Available";
                model.location = snapshot.Child("distance")?.GetValue(true)?.ToString();
                model.money = snapshot.Child("net_price")?.GetValue(true)?.ToString();


                for (int i = 0; i < list.Count(); i++)
                {

                    if (list[i].time.Equals(time))
                    {
                        list[i] = model;
                        break;
                    }

                }

                adapter.NotifyDataSetChanged();
            }

        }
    }
}



//public void OnDataChange(DataSnapshot snapshot)
//{
//    if (snapshot.Exists())
//    {
//        arrayList.Clear();

//        for (int i = 9; i < 15; i++)
//        {
//            DetailModel model = new DetailModel();
//            model.time = i.ToString();
//            model.status = "Not Available";
//            model.location = "";
//            model.money = "";
//            arrayList.Add(model);
//        }


//        var obj = snapshot.Children;

//        foreach (DataSnapshot snapshotChild in obj.ToEnumerable())
//        {
//            if (snapshotChild.GetValue(true) == null) continue;
//            DetailModel model = new DetailModel();
//            model.time = snapshotChild.Child("time")?.GetValue(true)?.ToString();
//            model.cust_id = snapshotChild.Child("cust_id")?.GetValue(true)?.ToString();
//            model.date = snapshotChild.Child("date")?.GetValue(true)?.ToString();
//            model.emp_id = snapshotChild.Child("emp_id")?.GetValue(true)?.ToString();
//            model.id = snapshotChild.Child("id")?.GetValue(true)?.ToString();


//            DatabaseReference database = FirebaseDatabase.Instance.Reference;


//            database.Child("employee").Child(model.emp_id).AddListenerForSingleValueEvent(new MyValueEventListener(arrayList, recyclerAdapter, model.time));


//            // Use type conversions as required. I have used string properties only
//        }


//        // Use type conversions as required. I have used string properties only

//    }

//    if (progressBar != null)
//        progressBar.Visibility = ViewStates.Gone;
//}



//model.availabletime = snapshotChild.Child("availabletime")?.GetValue(true)?.ToString();
//model.availabledate = snapshotChild.Child("availabledate")?.GetValue(true)?.ToString();

//for (int i = 0; i < arrayList.Count(); i++)
//{
//    if (arrayList[i].time.Equals(model.time) && arrayList[i].date.Equals(model.date))
//    {
//        arrayList[i] = model;
//        break;
//    }
//}