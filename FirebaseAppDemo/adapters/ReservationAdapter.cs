using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace FirebaseAppDemo
{
    public class ReservationAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<DetailModel> arrayList;
        private RecyclerView recyclerView;
        private DashboardActivity activity;
        private string book_status = "";

        public ReservationAdapter()
        {
        }

        public ReservationAdapter(Context context, List<DetailModel> arrayList, RecyclerView recyclerView, DashboardActivity activity)
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
                return arrayList.Count;
            }
        }


        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyView myholder = holder as MyView;

            if (arrayList[position].bookstatus.Equals(book_status))
            {
                myholder.txtEmpName.Visibility = ViewStates.Visible;
                myholder.txtTime.Visibility = ViewStates.Visible;
                myholder.lnrMain.Visibility = ViewStates.Visible;

                myholder.txtEmpName.Text = arrayList[position].cuse_name;
                myholder.txtTime.Text = arrayList[position].worktime;
            }
            else
            {
                myholder.txtEmpName.Visibility = ViewStates.Gone;
                myholder.txtTime.Visibility = ViewStates.Gone;
                myholder.lnrMain.Visibility = ViewStates.Gone;
            }

        }

        private void Mainview_Click(object sender, EventArgs e)
        {
            int position = recyclerView.GetChildAdapterPosition((View)sender);
            activity.moveToSummary(arrayList[position]);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(context)
                                          .Inflate(Resource.Layout.RawLayoutreservation, parent, false);


            TextView txtEmpName = itemView.FindViewById<TextView>(Resource.Id.txt_employee_reservation);
            TextView txtTime = itemView.FindViewById<TextView>(Resource.Id.txt_time_reservation);
            LinearLayout lnrMain = itemView.FindViewById<LinearLayout>(Resource.Id.lnrMain);

            MyView view = new MyView(itemView)
            {
                txtEmpName = txtEmpName,
                txtTime = txtTime,
                lnrMain = lnrMain
            };


            return view;
        }

        public void BookStatus(string status)
        {
            this.book_status = status;
        }



        public class MyView : RecyclerView.ViewHolder
        {
            public View mainview
            {
                get;
                set;
            }

            public LinearLayout lnrMain
            {
                get;
                set;
            }


            public TextView txtEmpName
            {
                get;
                set;
            }
            public TextView txtTime
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
