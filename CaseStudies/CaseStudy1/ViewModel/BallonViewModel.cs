using CaseStudy1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CaseStudy1.ViewModel
{
    public class BallonViewModel : ObservableBase
    {
        private Ballon _myBallon;
        public Ballon MyBallon
        {
            get { return _myBallon; }
            set { _myBallon = value; }
        }

        public RelayCommand GetData { get; set; }
        public BallonViewModel()
        {
            MyBallon = new Ballon();
            Angle = 0;
            Length = 0;
            Radius = 0;
            StringInput = "String input";
            ColorIndex = 0;
            LayerName = "";
            Colors = new List<string>();
            Colors.Add("Red");
            Colors.Add("Green");
            Colors.Add("Blue");
        }

        public double Angle
        {
            get { return MyBallon.Angle; }
            set
            {
                if (MyBallon.Angle != value)
                {
                    MyBallon.Angle = value;
                    OnPropertyChange("Angle");
                }
            }
        }

        public double Length
        {
            get { return MyBallon.Length; }
            set
            {
                if (MyBallon.Length != value)
                {
                    MyBallon.Length = value;
                    OnPropertyChange("Length");
                }
            }
        }

        public double Radius
        {
            get { return MyBallon.Radius; }
            set
            {
                if (MyBallon.Radius != value)
                {
                    MyBallon.Radius = value;
                    OnPropertyChange("Radius");
                }
            }
        }



        public List<string> Colors
        {
            get;
            set;
        }
        public int ColorIndex
        {
            get
            {
                return MyBallon.ColorIndex;
            }
            set
            {
                if (value != MyBallon.ColorIndex)
                {
                    MyBallon.ColorIndex = value + 1;
                    OnPropertyChange("ColorIndex");
                }
            }
        }

        public List<string> Layers
        {
            get;
            set;
        }
        public string LayerName
        {
            get { return MyBallon.LayerName; }
            set
            {
                if (value != MyBallon.LayerName)
                {
                    MyBallon.LayerName = value;
                    OnPropertyChange("LayerName");
                }
            }
        }
        public string StringInput
        {
            get { return MyBallon.StringInput; }
            set
            {
                if (value != MyBallon.StringInput)
                {
                    MyBallon.StringInput = value;
                    OnPropertyChange("StringInput");
                }
            }
        }


        public bool Validate(ref string error)
        {
            if(Length < 0.0001)
            {
                error = "Failed1";
                return false;
            }


            return true;
        }

    }
}
