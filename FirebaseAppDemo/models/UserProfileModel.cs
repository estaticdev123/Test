using System;
using Java.Util;

namespace FirebaseAppDemo
{
    public class UserProfileModel
    {
        private HashMap accountDataMap;

        public UserProfileModel()
        {

        }

        public UserProfileModel(HashMap accountDataMap)
        {
            this.accountDataMap = accountDataMap;
            this.name = (string)accountDataMap.Get("name");
            this.email = (string)accountDataMap.Get("email");
            this.password = (string)accountDataMap.Get("password");
            this.mobileno = (string)accountDataMap.Get("mobileno");
            this.location = (string)accountDataMap.Get("location");
            this.uid = (string)accountDataMap.Get("uid");

        }

        public string name
        {
            get;
            set;
        }

        public string email
        {
            get;
            set;
        }

        public string password
        {
            get;
            set;
        }

        public string mobileno
        {
            get;
            set;
        }

        public string location
        {
            get;
            set;
        }

        public string uid
        {
            get;
            set;  
        }

    }
}
