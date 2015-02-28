using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Rescue_The_Princess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool DEVELOPINGMODE = false;
        private bool OLDGAME = true;

        #region declarations

        //Declareren op moduleniveau
            //Arrays
        protected String[] _hist = new String[0];
            //Variabelen
        public String _datafolder = AppDomain.CurrentDomain.BaseDirectory;
        protected String _princes = "/princess.jpg";
        protected String _enemy_round_1 = "/enemy1.jpg";
        protected String _enemy_round_2 = "/enemy2.jpg";
        protected String _enemy_round_3 = "/enemy3.jpg";
        protected String _projectiles = "/projectile.png";
        protected String _wordfile = "/words.txt";
        private String _musicMain = "/main_theme.mp3";
        private String _musicRound1 = "/round1.mp3";
        private String _musicRound2 = "/round2.mp3";
        private String _musicRound3 = "/round3.mp3";
        protected int _numberenemies = 30;
        protected int _dimention = 200;   //Height in pixels
        private int _enemyspeed = 30;    //speed in milliseconds
        private int _typeSpeed = 1000;   //speed in milliseconds
        private int _maxtypetime = 10;  //%down per speedunit (see up)
        private int _numberwords = 10;
        private int _throwspeed = 10;  //Speed in milliseconds per egg refresh speed
        private int _spawnspeed = 350;  //recommended Spawnspeed of the eggs
        private int _numbereggs = 25;
        private int _buffer = 0;
        private bool _music = true;
            //Objects
        private DispatcherTimer tmr = new DispatcherTimer();
        private DispatcherTimer tmr2 = new DispatcherTimer();
        private DispatcherTimer tmr3 = new DispatcherTimer();
        private DispatcherTimer Spawn = new DispatcherTimer();
        private MediaPlayer ambient = new MediaPlayer();

        #endregion

        #region inits

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTmr();
            InitTmr2();
            InitTmr3();
            InitSpawn();
            SetContent();
            SetGameVariables();
            ShowHome();
        }

        private void SetGameVariables(String difficulty="edium")
        {
            if(difficulty == "easy")
            {
                //Ronde 1
                _numberenemies = 15;     //#enemies to defeat   - s30
                _enemyspeed = 20;        //Speed in milliseconds (improve costs lag) -s30

                //Ronde 2
                _numberwords = 8;       //#enemies to defeat    -s12
                _typeSpeed = 1500;         //Speed in milliseconds (improve costs lag) -s1000
                _maxtypetime = 5;       //%down in total typetime per tick  -s10

                //Ronde 3
                _numbereggs = 15;        //#enemies to defeat    - s30
                _spawnspeed = 150;        //speed of egg -s350
                _throwspeed = 100;        //Movement of thrower -s10

            }
            if(difficulty == "medium")
            {
                //Ronde 1
                _numberenemies = 30;     //#enemies to defeat   - s30
                _enemyspeed = 25;        //Speed in milliseconds (improve costs lag) -s30

                //Ronde 2
                _numberwords = 10;       //#enemies to defeat    -s12
                _typeSpeed = 1100;         //Speed in milliseconds (improve costs lag) -s1000
                _maxtypetime = 10;       //%down in total typetime per tick  -s10

                //Ronde 3
                _numbereggs = 30;        //#enemies to defeat    - s30
                _spawnspeed = 250;        //Hint of release speed (this is approx. generated) -s350
                _throwspeed = 30;        //Speed in milliseconds (improve costs lag) -s10
            }
            if(difficulty == "hard")
            {
                //Ronde 1
                _numberenemies = 40;     //#enemies to defeat   - s30
                _enemyspeed = 35;        //Speed in milliseconds (improve costs lag) -s30

                //Ronde 2
                _numberwords = 13;       //#enemies to defeat    -s12
                _typeSpeed = 800;         //Speed in milliseconds (improve costs lag) -s1000
                _maxtypetime = 10;       //%down in total typetime per tick  -s10

                //Ronde 3
                _numbereggs = 50;        //#enemies to defeat    - s30
                _spawnspeed = 350;        //Hint of release speed (this is approx. generated) -s350
                _throwspeed = 50;        //Speed in milliseconds (improve costs lag) -s10
            }
            if (DEVELOPINGMODE == true)
            {
                _numbereggs = 4;
                _numberenemies = 2;
                _numberwords = 1;
            }

            if (OLDGAME == true)
            {
                _enemy_round_1 = "/OLD_enemy1.jpg";
                _enemy_round_2 = "/OLD_enemy2.jpg";
                _enemy_round_3 = "/OLD_enemy3.jpg";
                _princes = "/OLD_princess.jpg";
                _projectiles = "/OLD_projectile.png";
            }
        }

        private void SetContent()
        {
            lblRound2Discr.Content = "The evil TyperHacker has kidnapped the princess." + System.Environment.NewLine + "Rescue her by typing what the evil son of a b*tch says.";
            lblPart3Descr.Content = "Defeat the evil kidnapper!" + System.Environment.NewLine + "Click the items before they reach you!";
            lblIntroDescr.Content = "Oh no, our beloved princess has been kidnapped!" + System.Environment.NewLine +
                "It's your soul duty to save the princess and gain her eternal love." + System.Environment.NewLine +
                "But elas. Three evil masterminds stand in your way of saving your beloved princess." + System.Environment.NewLine +
                "Nothing as simple for a hero like you. Each enemy has a weakness." + System.Environment.NewLine +
                "Complete these and the princess shall be yours. But fail, and she's lost." + System.Environment.NewLine + "Success, my heroes";
            lblFinishDescr.Content = "Nicely done, the princess is saved and can now safely return to helping NMCT-students learn how to code C#." + System.Environment.NewLine + "Good work, my young padawan.";
        }

        private void ShowHome()
        {
            ResetAll();
            grdHome.Visibility = Visibility.Visible;
            lblHome.Visibility = Visibility.Visible;
            btnStartGame.Visibility = Visibility.Visible;
            btnShutdown_app.Visibility = Visibility.Visible;
            LoadMusicMain();
            if (_music == true) StartMusic();
        }

        private void ResetAll()
        {
            //Reset UI Elements
            grdHome.Visibility = Visibility.Hidden;
            grdOne.Visibility = Visibility.Hidden;
            grdThree.Visibility = Visibility.Hidden;
            grdTwo.Visibility = Visibility.Hidden;
            grdGame.Visibility = Visibility.Hidden;
            grdCompleted.Visibility = Visibility.Hidden;
            grdIntro.Visibility = Visibility.Hidden;

            //Reset Arrays
            Array.Resize(ref _hist, 0);
            _buffer = 0;

            //Timers stoppen
            StopTimerRound1();
            StopTimerRound2();
            StopTimerRound3();
            Spawn.Stop();

            //ReInitTimers
            InitTmr();
            InitTmr2();
            InitTmr3();

            //Reset classes
            Round1.Reset();
            Round2.Reset();
            Round3.Reset();

            //Canvas cleanup
            cnvEnemy.Children.Clear();
            cnvTyper.Children.Clear();
            cnvEggThrower.Children.Clear();

        }

        private void ShowIntro()
        {
            grdHome.Visibility = Visibility.Hidden;
            lblHome.Visibility = Visibility.Hidden;
            btnStartGame.Visibility = Visibility.Hidden;
            btnShutdown_app.Visibility = Visibility.Hidden;
            grdIntro.Visibility = Visibility.Visible;
            lblIntroDescr.Visibility = Visibility.Visible;
            lblIntroTitle.Visibility = Visibility.Visible;
        }

        private void TerminateMenu()
        {
            grdIntro.Visibility = Visibility.Hidden;
            lblIntroTitle.Visibility = Visibility.Hidden;
            lblIntroDescr.Visibility = Visibility.Hidden;
            ShowMenuRound1();
        }

        private void InitTmr()
        {
            tmr.Interval = TimeSpan.FromMilliseconds(_enemyspeed);
            tmr.Tick += tmr_Tick;
        }

        private void InitTmr2()
        {
            tmr2.Interval = TimeSpan.FromMilliseconds(_typeSpeed);
            tmr2.Tick += tmr2_Tick;
        }

        private void InitTmr3()
        {
            tmr3.Interval = TimeSpan.FromMilliseconds(_throwspeed);
            tmr3.Tick += tmr3_Tick;
        }

        private void InitSpawn()
        {
            Spawn.Interval = TimeSpan.FromMilliseconds(Round3.GetSpawnInterval(_spawnspeed));
            Spawn.Tick += Spawn_Tick;
        }

        private void StartTimerRound1()
        {
            tmr.Start();
        }

        private void StopTimerRound1()
        {
            tmr.Stop();
        }

        private void StartTimerRound2()
        {
            tmr2.Start();
        }

        private void StopTimerRound2()
        {
            tmr2.Stop();
        }

        private void StartTimerRound3()
        {
            tmr3.Start();
        }

        private void StopTimerRound3()
        {
            tmr3.Stop();
        }

        private void LoadMusicMain()
        {
            ambient.Open(new Uri(_datafolder + _musicMain));
        }

        private void LoadMusicRound1()
        {
            ambient.Open(new Uri(_datafolder + _musicRound1));
        }

        private void LoadMusicRound2()
        {
            ambient.Open(new Uri(_datafolder + _musicRound2));
        }

        private void LoadMusicRound3()
        {
            ambient.Open(new Uri(_datafolder + _musicRound3));
        }

        private void StartMusic()
        {
            ambient.Volume = 100;
            ambient.Play();
        }

        private void StopMusic()
        {
            ambient.Stop();
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            ShowIntro();
        }

        private void btnShutdown_app_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region ControlSaveThePrinces

        private void ShowMenuRound1()
        {
            grdOne.Visibility = Visibility.Visible;
            lblRound1Descr.Visibility = Visibility.Visible;
            lblRound1Title.Visibility = Visibility.Visible;
            btnRound1Start.Visibility = Visibility.Visible;
            StopMusic();
            LoadMusicRound1();
            if (_music == true) StartMusic();
        } // This method will show the menu for Round 1.

        private void StartRound1()
        {
            cnvEnemy.Visibility = Visibility.Visible;
            lblRound1Descr.Visibility = Visibility.Hidden;
            lblRound1Title.Visibility = Visibility.Hidden;
            btnRound1Start.Visibility = Visibility.Hidden;
            InitRound1();
            StartTimerRound1();
        } //This method will start Round 1.

        private void TerminateRound1()
        {
            grdOne.Visibility = Visibility.Hidden;
            cnvEnemy.Visibility = Visibility.Hidden;
            cnvEnemy.Children.Clear();
            ShowMenuRound2();
            StopTimerRound1();
        } //This method will halt Round 1.

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartRound1();
        }

        #endregion

        #region savetheprinces
        void tmr_Tick(object sender, EventArgs e)
        {
            //When ticked, generate the next step
            Image[] imgs = null;
            double[] _bufx = new double[0];
            double[] _bufy = new double[0];

            Round1[] RAW = Round1.GiveRAW();
            for(int i = 0; i<= RAW.GetUpperBound(0); i++ )
            {
                Round1 enemy = RAW[i];
                double x = enemy.GetXpos();
                imgs = enemy.NextStep(_dimention, i, cnvEnemy.ActualWidth, cnvEnemy.ActualHeight);
                AddToArray(ref _bufx, enemy.GetXpos());
                AddToArray(ref _bufy, enemy.GetYpos());
                double x2 = enemy.GetXpos();
            }

            //Check if the game has been won.
            CheckIfWon();
            
            if(Round1.IsWon() == false)
            {
                //Fill the screen with new values
                cnvEnemy.Children.Clear();

                //Display the princes
                Image Princes = Round1.GivePrinces();
                cnvEnemy.Children.Add(Princes);

                //Reset the enemies
                for (int i = 0; i <=  imgs.GetUpperBound(0); i++)
                {
                    Image img = imgs[i];
                    img.SetValue(Canvas.TopProperty, _bufy[i]);
                    img.SetValue(Canvas.LeftProperty, _bufx[i]);
                    cnvEnemy.Children.Add(img);
                    img.Tag = i;
                }
            }
        }

        private void InitRound1()
        {
            //Set Buffers
            double[] _xpos = new double[_numberenemies];
            double[] _ypos = new double[_numberenemies];

            //Set the controls
            cnvEnemy.Children.Clear();
            StartTimerRound1();
            Round1.SetRound(_dimention, cnvEnemy.ActualHeight, cnvEnemy.ActualWidth,_datafolder + _princes);

            //Generate all the enemies
            for (int i = 0; i < _numberenemies; i++)
            {
                Round1 enemy = new Round1(_dimention, cnvEnemy.ActualHeight, cnvEnemy.ActualWidth, _datafolder + _enemy_round_1, _xpos, _ypos);
                _xpos[i] = enemy.GetXpos();
                _ypos[i] = enemy.GetYpos();
            }

            //Display the princes
            Image Princes = Round1.GivePrinces();
            cnvEnemy.Children.Add(Princes);

            //Get The enemies
            Image[] enemies = Round1.GiveEnemies();
            for(int i=0; i<= enemies.GetUpperBound(0); i++)
            {
                Image enemy = enemies[i];
                enemy.SetValue(Canvas.TopProperty, _ypos[i]);
                enemy.SetValue(Canvas.LeftProperty, _xpos[i]);
                cnvEnemy.Children.Add(enemy);
                enemy.Tag = i;
                enemy.MouseUp += enemy_MouseUp;
            }
        }

        private void CheckIfWon()
        {
            if(Round1.IsWon() == true)
            {
                TerminateRound1();
            }
        } // This method will check if the user has eliminated the threats.

        void imgEnemy_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cnvEnemy.Children.Remove((Image)sender);
        }

        void enemy_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image enemy = (Image)sender;
            cnvEnemy.Children.Remove(enemy);
            Round1.DeleteEnemy((Int32)enemy.Tag);
        } //This will initialize the objects for round 1.

        private void btnStartRound1Easy_Click(object sender, RoutedEventArgs e)
        {
            SetGameVariables("easy");
            TerminateMenu();
        }

        private void btnStartRound1Medium_Click(object sender, RoutedEventArgs e)
        {
            SetGameVariables("medium");
            TerminateMenu();
        }

        private void btnStartRound1Hard_Click(object sender, RoutedEventArgs e)
        {
            SetGameVariables("hard");
            TerminateMenu();
        }

        #endregion

        #region ControlTyperHacker

        private void ShowMenuRound2()
        {
            grdTwo.Visibility = Visibility.Visible;
            lblRound2Discr.Visibility = Visibility.Visible;
            lblRound2Title.Visibility = Visibility.Visible;
            btnRound2Start.Visibility = Visibility.Visible;
            grdGame.Visibility = Visibility.Hidden;
            pgb.Visibility = Visibility.Hidden;
            cnvTyper.Visibility = Visibility.Hidden;
            StopMusic();
            LoadMusicRound2();
            if (_music == true) StartMusic();
        } //This method will show the menu for Round 2

        private void StartRound2()
        {
            lblRound2Discr.Visibility = Visibility.Hidden;
            lblRound2Title.Visibility = Visibility.Hidden;
            btnRound2Start.Visibility = Visibility.Hidden;
            grdGame.Visibility = Visibility.Visible;
            pgb.Visibility = Visibility.Visible;
            cnvTyper.Visibility = Visibility.Visible;

            InitRound2();
            StartTimerRound2();
        } //This Method will show 

        private void TerminateRound2()
        {
            grdTwo.Visibility = Visibility.Hidden;
            grdGame.Visibility = Visibility.Hidden;
            pgb.Visibility = Visibility.Hidden;
            cnvTyper.Visibility = Visibility.Hidden;
            cnvTyper.Children.Clear();
            ShowMenuRound3();
            StopTimerRound2();
        }

        private void btnRound2Start_Click(object sender, RoutedEventArgs e)
        {
            StartRound2();
        }

        #endregion

        #region TyperHacker

        private void InitRound2()
        {
            //Load Words
            Round2.ThinkAboutWords(_datafolder + _wordfile);

            //Start Timer
            StartTimerRound2();

            //SetPicture
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(_datafolder + _enemy_round_2));
            img.Height = _dimention;
            img.Width = _dimention;
            img.SetValue(Canvas.TopProperty, cnvTyper.ActualHeight / 2);
            img.SetValue(Canvas.LeftProperty, cnvTyper.ActualWidth / 2);

            //SetPrinces
            Image princes = new Image();
            princes.Source = new BitmapImage(new Uri(_datafolder + _princes));
            princes.Height = _dimention /2;
            princes.Width = _dimention /2;
            double x = Convert.ToDouble(img.GetValue(Canvas.LeftProperty)) + _dimention;
            princes.SetValue(Canvas.LeftProperty, x - _dimention / 8);
            double y = Convert.ToDouble(img.GetValue(Canvas.TopProperty)) + _dimention;
            princes.SetValue(Canvas.TopProperty, y -  _dimention / 8);

            //Add the images to the screen
            cnvTyper.Children.Add(princes);
            cnvTyper.Children.Add(img);

            //UI Activate
            spnlword.Visibility = Visibility.Visible;
            elli.Visibility = Visibility.Visible;
            pgb.Value = 100;

            //Throw the inital word
            ThrowWord();
        }

        private void tmr2_Tick(object sender, EventArgs e)
        {
            pgb.Value -= _maxtypetime;
            CheckGameOver();
            Round2 mqs = new Round2();
            mqs.IsWordThrown(" ");
        } //This method will handle the tick for round 2

        private void CheckGameOver()
        {
            if(pgb.Value <= 0)
            {
                MessageBox.Show("The enemy has defeated you and the princess was kidnapped by the evil typewizard. Try better next time", "Game Over", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                ShowHome();
            }
        }

        private void ThrowWord()
        {
            Round2 newword = new Round2();
            String word = newword.GiveWord(ref _hist);
            MakeWord();
        }

        private void MakeWord()
        {
            //Initialize the stackpanel
            spnlword.Children.Clear();

            //Fill in the correct word.
            bool bgood = true;
            String buffer = "";
            foreach (Char c in Round2._thrownword)
            {
                buffer += c.ToString().ToLower();

                //Initialize the UI Elements
                TextBlock txb = new TextBlock();
                SolidColorBrush nmct = new SolidColorBrush();
                txb.FontFamily = new FontFamily("Ubuntu");
                txb.Foreground = nmct;
                txb.FontSize = 15;

                if ((Round2.IsCorrect().Contains(buffer) == true) && bgood == true)
                {
                    //Assign colors
                    nmct.Color = Color.FromRgb(0, 230, 0);
                }
                else
                {
                    bgood = false;

                    //assign colors
                    nmct.Color = Color.FromRgb(232, 94, 0);
                }

                //adding to stackpanel
                txb.Text = c.ToString();
                txb.FontWeight = FontWeights.DemiBold;
                spnlword.Orientation = Orientation.Horizontal;
                spnlword.Children.Add(txb);                
            }
        }

        private void key_pressed(object sender, KeyEventArgs e)
        {
            //Catch the pressed control.
            String cchar = e.Key.ToString();
            if(cchar == "Space")
            {
                cchar = " ";
            }

            //Get whether the round has been won or not.
            bool bguessed = Round2.Remember(cchar);
            if(bguessed == true)
            {
                Guessed();
                Round2.Remember(" ", true);
            }

            //Re_initialize the word
            MakeWord();
        }

        private void Guessed()
        {
            pgb.Value = 101;
            ThrowWord();
            bool bcompleted = Round2.IsCompleted(_numberwords);
            if(bcompleted == true)
            {
                TerminateRound2();
            }

        } //This methods invokes a new word.

        #endregion

        #region controlcatcheggs

        private void ShowMenuRound3()
        {
            grdThree.Visibility = Visibility.Visible;
            lblPart3Descr.Visibility = Visibility.Visible;
            lblPart3Title.Visibility = Visibility.Visible;
            btnPart3Start.Visibility = Visibility.Visible;
            cnvEggThrower.Visibility = Visibility.Hidden;
            StopMusic();
            LoadMusicRound3();
            if (_music == true) StartMusic();
        }

        private void StartRound3()
        {
            lblPart3Descr.Visibility = Visibility.Hidden;
            lblPart3Title.Visibility = Visibility.Hidden;
            btnPart3Start.Visibility = Visibility.Hidden;
            cnvEggThrower.Visibility = Visibility.Visible;
            StartTimerRound3();
            InitRound3();
        }

        private void TerminateRound3()
        {
            StopTimerRound3();
            grdThree.Visibility = Visibility.Hidden;
            cnvEggThrower.Visibility = Visibility.Hidden;
            cnvEggThrower.Children.Clear();
            ShowCompleted();
        }

        private void btnPart3Start_Click(object sender, RoutedEventArgs e)
        {
            StartRound3();
        }

        #endregion

        #region catcheggs

        private void InitRound3()
        {
            InitRound3Controls();
            StartTimerRound3();
            Spawn.Start();
        }

        void tmr3_Tick(object sender, EventArgs e)
        {
            //When ticked, generate the next step
            Image[] imgs = new Image[0];
            double[] _bufx = new double[0];
            double[] _bufy = new double[0];

            Round3[] RAW = Round3.GiveRAW();
            for (int i = 0; i <= RAW.GetUpperBound(0); i++)
            {
                Round3 egg = RAW[i];
                imgs = egg.NextStep(_dimention, i, cnvEnemy.ActualWidth, cnvEnemy.ActualHeight, _spawnspeed);
                AddToArray(ref _bufx, egg.GetXpos());
                AddToArray(ref _bufy, egg.GetYpos());
            }

            //Check if the game has been won.
            CheckIfWonRound3();

            if (Round3.IsWon(_numbereggs) == false)
            {
                //Fill the screen with new values
                cnvEggThrower.Children.Clear();

                //Display the princes
                Image Princes = Round3.GivePrinces();

                cnvEggThrower.Children.Add(Princes);

                //Display the thrower
                double cnvwidth = cnvEggThrower.ActualWidth;
                Image Thrower = Round3.GiveThrower();

                Thrower.Height = _dimention;
                Thrower.Width = _dimention;
                Thrower.SetValue(Canvas.TopProperty, Round3._yposthrower);
                Thrower.SetValue(Canvas.LeftProperty, cnvwidth - (cnvwidth / 10));
                cnvEggThrower.Children.Add(Thrower);

                //Reset the enemies
                for (int i = 0; i <= imgs.GetUpperBound(0); i++)
                {
                    Image img = imgs[i];
                    img.SetValue(Canvas.TopProperty, _bufy[i]);
                    img.SetValue(Canvas.LeftProperty, _bufx[i]);
                    cnvEggThrower.Children.Add(img);
                    img.Tag = i;
                }
            }
        }

        void Spawn_Tick(object sender, EventArgs e)
        {
            SpawnImage();
            if (_buffer >= _numbereggs)
            {
                Spawn.Stop();
            }
        }

        private void SpawnImage()
        {
            if (_buffer >= _numbereggs)
            {
                Spawn.Stop();
            }
            else
            {
                //Generate the egg.
                Round3 eggRAW = new Round3(_dimention, cnvEggThrower.ActualHeight, cnvEggThrower.ActualWidth, _datafolder + _projectiles, _spawnspeed);

                //Display the generated egg
                Image egg = eggRAW.Picture;
                egg.SetValue(Canvas.TopProperty, eggRAW.YLocation);
                egg.SetValue(Canvas.LeftProperty, eggRAW.XLocation);
                cnvEggThrower.Children.Add(egg);
                egg.Tag = _buffer;
                egg.MouseUp += egg_MouseUp;

                //If the max count not yet exceeded, spawn next
                _buffer++;
            }
        }
        
        void egg_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Round3.CountDeletedEgg();
            Image egg = (Image)sender;
            cnvEggThrower.Children.Remove(egg);
            Round3.DeleteEgg((Int32)egg.Tag);
        }

        private void InitRound3Controls()
        {
            //Set the controls
            cnvEggThrower.Children.Clear();
            StartTimerRound3();
            Round3.SetRound(_dimention, cnvEggThrower.ActualHeight, cnvEggThrower.ActualWidth, _datafolder + _princes, _datafolder + _enemy_round_3);

            //Display the princes
            Image Princes = Round3.GivePrinces();
            cnvEggThrower.Children.Add(Princes);

            //Display the thrower
            Image Thrower = Round3.GiveThrower();
            Thrower.SetValue(Canvas.TopProperty, cnvEggThrower.ActualHeight / 2);
            Thrower.SetValue(Canvas.LeftProperty, cnvEggThrower.ActualWidth - _dimention);
            cnvEggThrower.Children.Add(Thrower);

            //Start SpawnTimer
            Spawn.Start();

            //Spawn First Image
            SpawnImage();
        }

        private void CheckIfWonRound3()
        {
            if (Round3.IsWon(_numbereggs) == true)
            {
                TerminateRound3();
            }
        }

        #endregion

        #region completed

        private void ShowCompleted()
        {
            grdCompleted.Visibility = Visibility.Visible;
            lblFinishDescr.Visibility = Visibility.Visible;
            lblFinish.Visibility = Visibility.Visible;
            btnfinish.Visibility = Visibility.Visible;
            StopMusic();
            LoadMusicMain();
            if (_music == true) StartMusic();
        }

        private void Terminate()
        {
            grdCompleted.Visibility = Visibility.Hidden;
            lblFinish.Visibility = Visibility.Hidden;
            lblFinishDescr.Visibility = Visibility.Hidden;
            btnfinish.Visibility = Visibility.Hidden;
            ShowHome();
        }

        private void btnfinish_Click(object sender, RoutedEventArgs e)
        {
            Terminate();
        }
        #endregion

        #region additional helper methods

        private void AddToArray(ref double[] arr, double toadd)
        {
            Array.Resize(ref arr, arr.Length + 1);
            arr[arr.GetUpperBound(0)] = toadd;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(_music == true)
            {
                _music = false;
                StopMusic();
                btnSound.Background = Brushes.Red;
            }
            else
            {
                _music = true;
                if(ambient.Source != null) StartMusic();
                btnSound.Background = Brushes.Green;
            }
        }

        #endregion
    }
}
