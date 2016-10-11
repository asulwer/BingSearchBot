using System;
using System.ComponentModel;

namespace BingSearchBot.Model
{
    public enum Mode
    {
        Mobile,
        Desktop
    }    
    public class SettingsModel : INotifyPropertyChanged
    {
        private string username;
        private string password;
        private Mode mode;
        private int min;
        private int max;
        private int count;
        private string filepath;

        public SettingsModel()
        {
            Id = Guid.NewGuid();
            username = string.Empty;
            password = string.Empty;
            mode = Mode.Desktop;
            min = 0;
            max = 0;
            count = 0;
            filepath = string.Empty;
        }

        [SQLite.PrimaryKey]
        public Guid Id { get; protected set; }

        public string UserName
        {
            get
            {
                return username;
            }
            set
            {
                if (value == username)
                    return;

                username = value;
                this.OnPropertyChanged("UserName");
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                if (value == password)
                    return;

                password = value;
                this.OnPropertyChanged("Password");
            }
        }
        public Mode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if (value == mode)
                    return;

                mode = value;
                this.OnPropertyChanged("Mode");
            }
        }
        public int Min
        {
            get
            {
                return min;
            }
            set
            {
                if (value == min)
                    return;

                min = value;
                this.OnPropertyChanged("Min");
            }
        }
        public int Max
        {
            get
            {
                return max;
            }
            set
            {
                if (value == max)
                    return;

                max = value;
                this.OnPropertyChanged("Max");
            }
        }
        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                if (value == count)
                    return;

                count = value;
                this.OnPropertyChanged("Count");
            }
        }
        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {
                if (value == filepath)
                    return;

                filepath = value;
                this.OnPropertyChanged("FilePath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
