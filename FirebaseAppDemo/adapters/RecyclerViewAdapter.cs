using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace FirebaseAppDemo
{
    public class RecyclerViewAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<DetailModel> arrayList;
        private RecyclerView recyclerView;
        private DashboardActivity activity;


        public RecyclerViewAdapter(Context context, List<DetailModel> arrayList, RecyclerView recyclerView, DashboardActivity activity)
        {
            this.context = context;
            this.arrayList = arrayList;
            this.recyclerView = recyclerView;
            this.activity = activity;
        }


        public override int ItemCount
        {
            get
            {
                if (arrayList != null)
                    return arrayList.Count;
                else
                    return 0;
            }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;


            myholder.txtTime.Text = arrayList[position].time;

            if (arrayList[position].money != null && !arrayList[position].money.Equals(""))
            {
                myholder.imgMoney.Visibility = ViewStates.Visible;
                myholder.txtMoney.Visibility = ViewStates.Visible;
                myholder.txtMoney.Text = arrayList[position].money;
            }
            else
            {
                myholder.txtMoney.Visibility = ViewStates.Gone;
                myholder.imgMoney.Visibility = ViewStates.Gone;
            }

            myholder.txtStatus.Text = arrayList[position].showtime;


            if (arrayList[position].location != null && !arrayList[position].location.Equals(""))
            {
                myholder.imgLocation.Visibility = ViewStates.Visible;
                myholder.txtLocation.Visibility = ViewStates.Visible;

                myholder.txtLocation.Text = arrayList[position].location;

                myholder.lnrLayout.SetBackgroundColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorwhite)));

            }
            else
            {
                myholder.imgLocation.Visibility = ViewStates.Gone;
                myholder.txtLocation.Visibility = ViewStates.Gone;
                myholder.lnrLayout.SetBackgroundColor(new Android.Graphics.Color(ContextCompat.GetColor(context, Resource.Color.colorRawBack)));
            }

            myholder.lnrLayout.Click += Mainview_Click;
        }

        private void Mainview_Click(object sender, EventArgs e)
        {
            int position = recyclerView.GetChildAdapterPosition((View)sender);

            if (arrayList[position].location != null && !arrayList[position].location.Equals(""))
            {
                activity.moveToEmployeeDetail(arrayList[position]);
            }
            else
            {

            }

        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(context)
                                          .Inflate(Resource.Layout.raw_layout, parent, false);

            TextView txtTime = itemView.FindViewById<TextView>(Resource.Id.txt_raw_time);
            TextView txtStatus = itemView.FindViewById<TextView>(Resource.Id.txt_raw_status);
            TextView txtLocation = itemView.FindViewById<TextView>(Resource.Id.txt_raw_location);
            TextView txtMoney = itemView.FindViewById<TextView>(Resource.Id.txt_raw_money);
            ImageView imgMoney = itemView.FindViewById<ImageView>(Resource.Id.imgMoney);
            ImageView imgLocation = itemView.FindViewById<ImageView>(Resource.Id.imgLocation);
            LinearLayout layout = itemView.FindViewById<LinearLayout>(Resource.Id.lnrLayout);

            MyView view = new MyView(itemView)
            {
                txtTime = txtTime,
                txtStatus = txtStatus,
                txtMoney = txtMoney,
                txtLocation = txtLocation,
                imgMoney = imgMoney,
                imgLocation = imgLocation,
                lnrLayout = layout
            };


            return view;

        }


        public class MyView : RecyclerView.ViewHolder
        {
            public View mainview
            {
                get;
                set;
            }

            public LinearLayout lnrLayout
            {
                get;
                set;
            }

            public TextView txtTime
            {
                get;
                set;
            }
            public TextView txtStatus
            {
                get;
                set;
            }
            public TextView txtLocation
            {
                get;
                set;
            }
            public TextView txtMoney
            {
                get;
                set;
            }
            public ImageView imgMoney
            {
                get;
                set;
            }
            public ImageView imgLocation
            {
                get;
                set;
            }

            public MyView(View view) : base(view)
            {
                mainview = view;
            }
        }

    }



}
