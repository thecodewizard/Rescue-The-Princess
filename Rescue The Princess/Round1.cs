using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Rescue_The_Princess
{
    class Round1
    {
        #region declarations
        //Declareren op moduleniveau
            //Arrays
        private static Round1[] _enemiesRAW = new Round1[0];
        private static Image[] _enemies = new Image[0];
            //Fields
        private double _Xlocation;
        private double _Ylocation;
        private double _Xmovement;
        private double _Ymovement;
        private double _speed;
        private double _dimention;
            //Objects
        public Random moRandomGenerator = new Random();

        protected static Image _princes = new Image();

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

        //Define Constructor
        public Round1()
        {

        }
        
        public Round1(double dimention, double cnvheight, double cnvwidth, String imgloc, double[] bufferx, double[] buffery)
        {
            //Make new values
            this.GenerateNewValues(dimention, cnvwidth, cnvheight, bufferx, buffery);

            //Generate UI Element
                //Make an new enemy
            Image imgEnemy = new Image();
            imgEnemy.Source = new BitmapImage(new Uri(imgloc));

                //Define graphics
            imgEnemy.Width = dimention;
            imgEnemy.Height = dimention;
            imgEnemy.SetValue(Canvas.TopProperty, this.XLocation);
            imgEnemy.SetValue(Canvas.LeftProperty, this.YLocation);

                //Add To Array
            AddToArray(ref _enemies, imgEnemy);
            AddToArray(ref _enemiesRAW, this);

        } //This constructor makes a new enemy image and adds it to the array

        #endregion  //This region contains the intialize methods of this class

        #region statics

        public static void SetRound(double dimention ,double cnvheight, double cnvwidth, String imgprinces)
        {
            //Generate the princess
            _princes.Source = new BitmapImage(new Uri(imgprinces));
            _princes.Width = dimention;
            _princes.Height = dimention;
            _princes.SetValue(Canvas.TopProperty, cnvheight / 2);
            _princes.SetValue(Canvas.LeftProperty, cnvwidth / 2);

        } //This method will generate all the enemies and display the princess.

        public static void DeleteEnemy(int iEnemy)
        {
            //Delete enemy from imagearray
            Image[] buffer = _enemies;
            Array.Resize(ref _enemies, 0);

            for (int i = 0; i < buffer.Length; i++)
            {
                Image img = buffer[i];
                if (i != iEnemy)
                {
                    AddToArray(ref _enemies, img);
                }
            }

            //Delete enemy from RAW array
            Round1[] bufferraw = _enemiesRAW;
            Array.Resize(ref _enemiesRAW, 0);

            for( int i = 0; i < bufferraw.Length; i++)
            {
                Round1 enemy = bufferraw[i];
                if(i != iEnemy)
                {
                    Array.Resize(ref _enemiesRAW, _enemiesRAW.Length + 1);
                    _enemiesRAW[_enemiesRAW.GetUpperBound(0)] = enemy;
                }
            }

        }   //This will delete the enemy requested

        public static Image GivePrinces()
        {
            return _princes;
        } //This method will return the princes image.

        public static Image[] GiveEnemies()
        {
            return _enemies;
        } //This method will return the complete enemy array.

        public static Round1[] GiveRAW()
        {
            return _enemiesRAW;
        } //This method will return the RAW object array.

        private static void AddToArray(ref Image[] arr, Image toadd)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = toadd;
        }

        public static bool IsWon()
        {
            if(_enemies.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Reset()
        {
            Array.Resize(ref _enemies, 0);
            Array.Resize(ref _enemiesRAW, 0);
        }

        #endregion //This region contains the static methods of this class

        #region publics

        public Image[] NextStep(double dimention, int i, double cnvwidth, double cnvheight)
        {
            //When a tick arrived, move all enemies
            MoveEnemies();

            //Next, check if a collision took place
            CollisionCheck(dimention / 2 );

            //Recalculate the movement
            this.XMovement = GenerateXMovement(this.GetXpos(), cnvwidth);
            this.YMovement = GenerateYMovement(this.GetYpos(), cnvheight);

            //Generate the new images
            Image img = _enemies[i];
            img.SetValue(Canvas.TopProperty, this.YLocation);
            img.SetValue(Canvas.LeftProperty, this.XLocation);

            //If everyting was OK, return the new enemies
            return GiveEnemies();

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

        private void GenerateNewValues(double dimention, double cnvwidth, double cnvheight, double[] bufferx, double[] buffery)
        {
            //Generate new location
            double xpos; double ypos;
            double xhalf = cnvwidth / 2; double yhalf = cnvheight / 2;
            //Make sure that the image doesn't spawn ON the princess... Derp.
            do
            {
                xpos = GenerateXPosition(cnvwidth);
            } while (((xpos >= xhalf - (dimention * 2)) && (xpos <= xhalf + (dimention * 2)))
                || (bufferx.Contains(xpos) == true));

            do
            {
                ypos = GenerateYPosition(cnvheight);
            } while (((ypos >= yhalf - dimention) && (ypos <= yhalf + dimention))
                || (buffery.Contains(ypos) == true));

            //Set Properties
            this.XLocation = xpos;
            this.YLocation = ypos;
            this.XMovement = GenerateXMovement(this.XLocation, cnvwidth / 2);
            this.YMovement = GenerateYMovement(this.YLocation, cnvheight / 2);
            this.Speed = GenerateSpeed();
            this.Dimention = dimention;
        }

        private void MoveEnemies()
        {
            this.XLocation = this.XLocation + (this.XMovement * this.Speed);
            this.YLocation = this.YLocation + (this.YMovement * this.Speed);
        } //This method will move the enemies according to their position

        private void CollisionCheck(double dimention)
        {
            //int errormargin = (Int32)Math.Round(dimention/10, 0);
            int errormargin = 30;
            double xloc = (double)_princes.GetValue(Canvas.LeftProperty);
            double yloc = (double)_princes.GetValue(Canvas.TopProperty);

            /*Because it's always the left-up corner of the image that crosses the line, this has to be taken into account while calculating.
                For the pictures from the left -> xloc - dimention
                from above -> yloc - dimention
                from right -> xloc + dimention
                from bottom -> yloc + dimention
            We can combine this to two if statements */
           
            if((this.XLocation > (xloc - dimention) + errormargin) && (this.XLocation < ((xloc + dimention) - errormargin)))
            {
                if ((this.YLocation > (yloc - dimention) + errormargin) && (this.YLocation < (yloc + (dimention/2*3)) - errormargin))
                {
                   MessageBox.Show("Too bad, the enemy kidnapped the princes. Better luck next time.", "game over", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                   if (Application.Current != null) Application.Current.Shutdown();
                   System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private double GenerateXPosition(double cnvwidth)
        {
            int iminX;
            int imaxX;
            bool bTest = Int32.TryParse(Math.Round(cnvwidth - _dimention, 0).ToString(), out imaxX);
            bTest = Int32.TryParse(Math.Round(cnvwidth / 4, 0).ToString(), out iminX);
            double x = moRandomGenerator.Next(-iminX, imaxX - 2); // -2 -> -1 for max correction, -1 for nextdouble.
            x += moRandomGenerator.NextDouble();

            return x;
        }

        private double GenerateYPosition(double cnvheight)
        {
            int iminY;
            int imaxY;
            bool bTest = Int32.TryParse(Math.Round(cnvheight - _dimention, 0).ToString(), out imaxY);
            bTest = Int32.TryParse(Math.Round(cnvheight / 4, 0).ToString(), out iminY);
            double y = moRandomGenerator.Next(-iminY, imaxY - 2); //see GenerateXPosition for comment.
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
                if (dXpostion <= quartX) dXmov = 0.7;
                else dXmov = 1;
            }
            else
            {
                if (dXpostion >= quartX + halfX) dXmov = -0.7;
                else dXmov = -1;
            }
            return dXmov;
        }

        private double GenerateYMovement(double dYposition, double cnvheight)
        {
            double dYmov;
            double halfY = cnvheight / 2;
            double quartY = halfY / 2;

            if (dYposition <= halfY)
            {
                if (dYposition <= quartY) dYmov = 1;
                else dYmov = 0.5;
            }
            else
            {
                if (dYposition >= quartY + halfY) dYmov = -1;
                else dYmov = -0.5;
            }
            return dYmov;
        }

        private double GenerateSpeed()
        {
            int imaxSpeed = 5;
            double speed = moRandomGenerator.Next(3, imaxSpeed - 2);
            speed += moRandomGenerator.NextDouble();

            return speed/2.8;
        }

        private void AddToArray(ref Round1[] arr, Round1 toadd)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = toadd;
        }

        #endregion //This region contains the private helping methods of this class.
    }
}
