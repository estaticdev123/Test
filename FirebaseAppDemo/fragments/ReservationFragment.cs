
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;

namespace FirebaseAppDemo
{
    public class ReservationFragment : Android.Support.V4.App.Fragment, IValueEventListener, TabLayout.IOnTabSelectedListener
    {

        private RecyclerView recyclerView;
        private ReservationAdapter reservationAdapter;
        private List<DetailModel> arrayListReservation;
        private TabLayout tabLayout;


        public ReservationFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ReservationFragment, container, false);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            //toolbar
            Android.Support.V7.Widget.Toolbar toolbar = (Android.Support.V7.Widget.Toolbar)((DashboardActivity)Activity).FindViewById(Resource.Id.toolbar);
            TextView mTitle = (TextView)toolbar.FindViewById(Resource.Id.toolbar_title);
            if (mTitle != null)
                mTitle.Text = "Reservations";

            onInit(view);

            getreservationData();
        }


        private void onInit(View view)
        {

            recyclerView = (RecyclerView)view.FindViewById(Resource.Id.recyclerView);
            tabLayout = (TabLayout)view.FindViewById(Resource.Id.tab_layout);
            arrayListReservation = new List<DetailModel>();

            reservationAdapter = new ReservationAdapter(Activity, arrayListReservation, recyclerView, (FirebaseAppDemo.DashboardActivity)Activity);
            reservationAdapter.BookStatus("complete");

            RecyclerView.LayoutManager mLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(mLayoutManager);
            recyclerView.SetAdapter(reservationAdapter);


            tabLayout.AddTab(tabLayout.NewTab().SetText("Complete"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("Pending"));
            tabLayout.AddTab(tabLayout.NewTab().SetText("Cancel"));

            //setCustomTabs(0,0,0);

            tabLayout.AddOnTabSelectedListener(this);
        }

        private void getreservationData()
        {

            DatabaseReference database = FirebaseDatabase.Instance.Reference;
            Query query = database.Child("Booked").OrderByChild("cust_id").EqualTo(FirebaseAuth.Instance.CurrentUser.Uid);
            query.AddListenerForSingleValueEvent(this);

        }

        public void OnCancelled(DatabaseError error)
        {

        }

        public void OnDataChange(DataSnapshot snapshot)
        {
            if (snapshot.Exists())
            {
                arrayListReservation.Clear();

                var obj = snapshot.Children;

                int complete = 0;
                int pending = 0;
                int cancel = 0;


                foreach (DataSnapshot snapshotChild in obj.ToEnumerable())
                {
                    if (snapshotChild.GetValue(true) == null) continue;
                    DetailModel model = new DetailModel();
                    model.emp_id = snapshotChild.Child("emp_id")?.GetValue(true)?.ToString();
                    model.bookstatus = snapshotChild.Child("book_status")?.GetValue(true)?.ToString();
                    model.worktime = snapshotChild.Child("date")?.GetValue(true)?.ToString();
                    model.cuse_name = snapshotChild.Child("cust_name")?.GetValue(true)?.ToString();


                    if (model.bookstatus.Equals("complete"))
                        complete++;
                    else if (model.bookstatus.Equals("pending"))
                        pending++;
                    else if (model.bookstatus.Equals("cancel"))
                        cancel++;


                    arrayListReservation.Add(model);
                }

                setCustomTabs(complete, pending, cancel);
                reservationAdapter.NotifyDataSetChanged();

            }
        }

        private void setCustomTabs(int complete, int pending, int cancel)
        {

            for (int i = 0; i < tabLayout.TabCount; i++)
            {
                TabLayout.Tab tab = tabLayout.GetTabAt(i);
                View tabView = ((ViewGroup)tabLayout.GetChildAt(0)).GetChildAt(i);
                tabView.RequestLayout();
                var view = LayoutInflater.From(Context).Inflate(Resource.Layout.CustomTabReservation, null);

                TextView txt_day = view.FindViewById<TextView>(Resource.Id.txt_tab_reservation);
                ImageView img = view.FindViewById<ImageView>(Resource.Id.img_tab_reservation);

                if (i == 0)
                {
                    txt_day.Text = (complete.ToString());
                    img.SetImageResource(Resource.Drawable.ic_complete);
                }
                else if (i == 1)
                {
                    txt_day.Text = (pending.ToString());
                    img.SetImageResource(Resource.Drawable.ic_pending);
                }
                else if (i == 2)
                {
                    txt_day.Text = (cancel.ToString());
                    img.SetImageResource(Resource.Drawable.ic_cancel);
                }

                tab.SetCustomView(view);

            }

        }

        public void OnTabReselected(TabLayout.Tab tab)
        {

        }

        public void OnTabSelected(TabLayout.Tab tab)
        {
            View view = tab.CustomView;
            if (view != null)
            {
                TextView txt_day = view.FindViewById<TextView>(Resource.Id.txt_tab_reservation);

                txt_day.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.colorwhite)));

                if (tab.Position == 0)
                {
                    reservationAdapter.BookStatus("complete");
                }
                else if (tab.Position == 1)
                {
                    reservationAdapter.BookStatus("pending");
                }
                else if (tab.Position == 2)
                {
                    reservationAdapter.BookStatus("cancel");
                }

                reservationAdapter.NotifyDataSetChanged();

            }

        }

        public void OnTabUnselected(TabLayout.Tab tab)
        {
            View view = tab.CustomView;

            if (view != null)
            {
                TextView txt_day = view.FindViewById<TextView>(Resource.Id.txt_tab_reservation);

                txt_day.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.colorcyan)));
            }


        }
    }
}
