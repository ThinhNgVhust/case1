using CaseStudy1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy1.ViewModel
{
  public class BallonViewModel : ObservableBase
    {
        private Ballon myBallon = null;

        public BallonViewModel()
        {
            this.myBallon = new Ballon();
        }
        public double Angle
        {
            get { return myBallon.Angle; }
            set
            {
                if (myBallon.Angle != value)
                {
                    myBallon.Angle = value;
                    OnPropertyChange("Angle");
                }
            }
        }

        public double Length
        {
            get { return myBallon.Length; }
            set
            {
                if (myBallon.Length != value)
                {
                    myBallon.Length = value;
                    OnPropertyChange("Length");
                }
            }
        }

        public double Radius
        {
            get { return myBallon.Radius; }
            set
            {
                if (myBallon.Radius != value)
                {
                    myBallon.Radius = value;
                    OnPropertyChange("Radius");
                }
            }
        }


        public int ColorIndex
        {
            get
            {
                return myBallon.ColorIndex;
            }
            set
            {
                if (value != myBallon.ColorIndex)
                {
                    myBallon.ColorIndex = value;
                    OnPropertyChange("ColorIndex");
                }
            }
        }


        public string LayerName
        {
            get { return myBallon.LayerName; }
            set
            {
                if (value != myBallon.LayerName)
                {
                    myBallon.LayerName = value;
                    OnPropertyChange("LayerName");
                }
            }
        }
        public string CharInput
        {
            get { return myBallon.CharInput; }
            set
            {
                if (value != myBallon.CharInput)
                {
                    myBallon.CharInput = value;
                    OnPropertyChange("CharInput");
                }
            }
        }

    }
}
