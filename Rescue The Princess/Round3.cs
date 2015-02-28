using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rescue_The_Princess
{
    class Round3
    {
        #region declarations
        //Declareren op moduleniveau
            //Arrays
        private static Round3[] _eggs = new Round3[0];
            //Fields
        private double _Xlocation;
        private double _Ylocation;
        private double _Xmovement;
        private double _Ymovement;
        private double _speed;
        private double _dimention;
        private Image _picture;
            //Objects
        public static Random moRandomGenerator = new Random();
        protected static Image _princes = new Image();
        protected static Image _thrower = new Image();
            //Variables
        public static int _count = 0;
        public static double _xposthrower = 0.0;
        public static double _yposthrower = 0.0;
        public static bool _directionup = false;

        #endregion

        #region inits

        //Make Properties

        public double XLocation
        {
            get { return _Xlocation; }
            set { _Xlocation = value; }
        }

        public double YLocation
        {
            get { return _Ylocation; }
            set { _Ylocation = value; }
        }

        public double XMovement
        {
            get { return _Xmovement; }
            set { _Xmovement = value; }
        }

        public double YMovement
        {
            get { return _Ymovement; }
            set { _Ymovement = value; }
        }

        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public double Dimention
        {
            get { return _dimention; }
            set { _dimention = value; }
        }
        public Image Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }

        //Define Constructor
        public Round3()
        {

        }
        
        public Round3(double dimention, double cnvheight, double cnvwidth, String eggimg, double givenspeed)
        {
            //Make new values
            this.GenerateNewValues(dimention, cnvwidth, cnvheight, givenspeed);

            //Generate UI Element
                //Make an new enemy
            Image imgEgg = new Image();
            imgEgg.Source = new BitmapImage(new Uri(eggimg));

                //Define graphics
            imgEgg.Width = dimention / 3  * 2 ;
            imgEgg.Height = dimention / 3 * 2;
            imgEgg.SetValue(Canvas.TopProperty, this.XLocation);
            imgEgg.SetValue(Canvas.LeftProperty, this.YLocation);
            this._picture = imgEgg;

                //Add To Array
            AddToArray(ref _eggs, this);

        } //This constructor makes a new enemy image and adds it to the array

        #endregion  //This region contains the intialize methods of this class

        #region statics

        public static void SetRound(double dimention ,double cnvheight, double cnvwidth, String imgprinces, String imgEnemy)
        {
            //Generate the princess
            _princes.Source = new BitmapImage(new Uri(imgprinces));
            _princes.Width = dimention;
            _princes.Height = dimention;
            _princes.SetValue(Canvas.TopProperty, cnvheight / 2);
            _princes.SetValue(Canvas.LeftProperty, cnvwidth - (cnvwidth - (cnvwidth / 20)));

            //Generate the thrower
            _thrower.Source = new BitmapImage(new Uri(imgEnemy));
            _thrower.Width = dimention;
            _thrower.Height = dimention;
            _yposthrower = cnvheight / 2;
            _xposthrower = cnvwidth - (cnvwidth / 10);

        } //This method will generate all the enemies and display the princess.

        public static void DeleteEgg(int iEgg)
        {
            //Delete enemy from RAW array
            Round3[] bufferraw = _eggs;
            Array.Resize(ref _eggs, 0);

            for( int i = 0; i < bufferraw.Length; i++)
            {
                Round3 egg = bufferraw[i];
                if(i != iEgg)
                {
                    Array.Resize(ref _eggs, _eggs.Length + 1);
                    _eggs[_eggs.GetUpperBound(0)] = egg;
                }
            }

        }   //This will delete the enemy requested

        public static Image GivePrinces()
        {
            return _princes;
        } //This method will return the princes image.

        public static Image GiveThrower()
        {
            return _thrower;
        }

        public static Image[] GiveEggs()
        {
            Image[] eggs = new Image[0];
            foreach(Round3 egg in _eggs)
            {
                AddToArray(ref eggs, egg.Picture);
            }
            return eggs;

        } //This method will return the complete enemy array.

        public static Round3[] GiveRAW()
        {
            return _eggs;
        } //This method will return the RAW object array.

        private static void AddToArray(ref Image[] arr, Image toadd)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = toadd;
        }

        public static bool IsWon(int objectstocatch)
        {
            if(_count == objectstocatch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CountDeletedEgg()
        {
            _count = _count + 1;
        }

        public static void Reset()
        {
            Array.Resize(ref _eggs, 0);
            _count = 0;
        }

        public static int GetSpawnInterval(int iRecommendedSpawnInterval)
        {
            int iBuffer = (Int32)Math.Round((Double)(iRecommendedSpawnInterval / 6), 0);
            int iSpawnInterval = moRandomGenerator.Next(iRecommendedSpawnInterval - iBuffer, iRecommendedSpawnInterval + iBuffer);
            return iSpawnInterval;
        }

        #endregion //This region contains the static methods of this class

        #region publics

        public Image[] NextStep(double dimention, int i, double cnvwidth, double cnvheight, double tickspeed)
        {
            //When a tick arrived, move all enemies
            MoveEggs();

            //Move The Thrower
            MoveThrower(dimention, cnvwidth, cnvheight, tickspeed);

            //Next, check if a collision took place
            CollisionCheck(dimention);

            //Recalculate the movement
            this.XMovement = GenerateXMovement(this.GetXpos(), cnvwidth);
            this.YMovement = GenerateYMovement(this.GetYpos(), cnvheight);

            //Generate the new images
            Image img = this.Picture;
            img.SetValue(Canvas.TopProperty, this.YLocation);
            img.SetValue(Canvas.LeftProperty, this.XLocation);

            //If everyting was OK, return the new enemies
            return GiveEggs();

        } //This method will pass on the tick from mainwindow

        public double GetXpos()
        {
            return this.XLocation;
        }

        public double GetYpos()
        {
            return this.YLocation;
        }

        #endregion //This region contains the public methods of this class

        #region privates

        private void MoveThrower(double dimention, double cnvwidth, double cnvHeight, double tickspeedmillisec)
        {
            double throwerspeed = ((dimention / 4) / tickspeedmillisec) * 20;
            Double xpos = (Double)_thrower.GetValue(Canvas.TopProperty);

            //Calculate the new values based on position. If the thrower is in the upper or lower 1/4 of the canvas, revert.
            double ymove;
            if (xpos <= (cnvHeight / 8))
            {
                //if the thrower is in the upper 1/4 of the canvas.
                ymove = xpos + throwerspeed;
                _directionup = false;
            } 
            else if (xpos >= (cnvHeight - (cnvHeight / 4)))
            {
                //if the thrower is in the lower 1/4 of the canvas.
                ymove = xpos - throwerspeed;
                _directionup = true;
            }
            else
            {
                //if the thrower is in the lower part
                if (_directionup == true) ymove = xpos - throwerspeed;
                else ymove = xpos + throwerspeed;
            }

            _xposthrower = cnvwidth - (cnvwidth / 10);
            _yposthrower = ymove;
        }

        private void GenerateNewValues(double dimention, double cnvwidth, double cnvheight, double givenspeed)
        {
            //Generate new location
            double xpos; double ypos;
            double xhalf = cnvwidth / 2; double yhalf = cnvheight / 2;
            xpos = Convert.ToDouble(_thrower.GetValue(Canvas.LeftProperty));
            ypos = Convert.ToDouble(_thrower.GetValue(Canvas.TopProperty));

            //Set Properties
            this.XLocation = xpos;
            this.YLocation = ypos;
            this.XMovement = GenerateXMovement(this.XLocation, cnvwidth / 2);
            this.YMovement = GenerateYMovement(this.YLocation, cnvheight / 2);
            this.Speed = GenerateSpeed(givenspeed);
            this.Dimention = dimention;
        }

        private void MoveEggs()
        {
            this.XLocation = this.XLocation + (this.XMovement * this.Speed);
            this.YLocation = this.YLocation + (this.YMovement * this.Speed);
        } //This method will move the enemies according to their position

        private void CollisionCheck(double dimention)
        {
            double xloc = (double)_princes.GetValue(Canvas.LeftProperty);
            double yloc = (double)_princes.GetValue(Canvas.TopProperty);
            if(this.XLocation <=  dimention / 6)
            {
                MessageBox.Show("Too bad, the enemy kidnapped the princes. Better luck next time.", "game over", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                if(Application.Current != null) Application.Current.Shutdown();
            }
        }

        private double GenerateYPosition(double cnvheight)
        {
            int imaxY;
            bool bTest = Int32.TryParse(Math.Round(cnvheight - _dimention, 0).ToString(), out imaxY);
            double y = moRandomGenerator.Next(0, imaxY - 2); //see GenerateXPosition for comment.
            y += moRandomGenerator.NextDouble();

            return y;
        }

        private double GenerateXMovement(double dXpostion, double cnvwidth)
        {
            //De beweging bepalen op basis van de positie van de princes.
            //Nauwkeurigheid beperkt tot achtsten.

            double dXmov;
            double halfX = cnvwidth / 2;
            double quartX = halfX / 2;


            if (dXpostion <= halfX)
            {
                if (dXpostion <= quartX) dXmov = -1.2;
                else dXmov = -1.6;
            }
            else
            {
                if (dXpostion >= quartX + halfX) dXmov = -1.2;
                else dXmov = -1.6;
            }
            return dXmov * 2;
        }

        private double GenerateYMovement(double dYposition, double cnvheight)
        {
            double dYmov;
            double halfY = cnvheight / 2;
            double quartY = halfY / 2;

            if (dYposition <= halfY)
            {
                if (dYposition <= quartY) dYmov = 1;
                else dYmov = 0.6;
            }
            else
            {
                if (dYposition >= quartY + halfY) dYmov = -1;
                else dYmov = -0.6;
            }
            return dYmov;
        }

        private double GenerateSpeed(double givenspeed)
        {
            double speed = moRandomGenerator.NextDouble();
            speed += givenspeed / 300;
            return speed;
        }

        private void AddToArray(ref Round3[] arr, Round3 toadd)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = toadd;
        }

        #endregion //This region contains the private helping methods of this class.
    }
}
