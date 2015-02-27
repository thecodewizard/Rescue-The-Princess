using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rescue_The_Princess
{
    class Round2
    {
        #region declarations

        //Declare on module level
            //Arrays
        private static String[] _library = new String[0];
        private String[] _history = new String[0];
        private static String[] _typebuffer = new String[0];
            //Variabelen
        public static String _thrownword = "";
        public static String _correct_word = "";
        public static int _numberbuffer = 0;
            //Objecten
        private Random moRandomGenerator = new Random();
            //fields
        private String _assignment;

        #endregion

        #region init

        //Set up properties
        public String Assignment
        {
            get { return _assignment; }
            set { _assignment = value; }
        }

        //Declare Constructor(s)
        public Round2()
        {

        }

        #endregion

        //Static methods of the class
        #region static

        public static void ThinkAboutWords(String file)
        {
            using(StreamReader importer = new StreamReader(file))
            {
                String sLine = importer.ReadLine();

                while(sLine != null)
                {
                    //add the line to the library
                    AddToArray(ref _library, sLine);

                    //initialize next loop
                    sLine = importer.ReadLine();
                }
            }//This method will read the lines of the requested file and save it in the library
        }

        public static void Reset()
        {
            Array.Resize(ref _library, 0);
            _thrownword = "";
            _numberbuffer = 0;
            Array.Resize(ref _typebuffer, 0);
        } //This method resets the data for round 2

        public static bool Remember(String c, bool bforget = false)
        {
            String buffer = "";

            if (bforget == false) AddToArray(ref _typebuffer, c.ToLower());
            else Array.Resize(ref _typebuffer, 0);

            if((_thrownword != null) && (_thrownword != ""))
            {
                foreach (String s in _typebuffer)
                {
                    buffer += s.ToLower();
                }

                if (buffer.Contains(_thrownword.ToLower()))
                {
                    return true;
                }
                else { return false; }
            }
            else
            {
                return false;
            }
        }

        public static bool IsCompleted(int _max)
        {
            _numberbuffer++;
            
            if(_numberbuffer == _max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static String IsCorrect()
        {
            String buffer = "";
            Int16 ibuffer = 0;

            for(int i = 0; i<=_typebuffer.GetUpperBound(0); i++)
            {
                String letter = _typebuffer[i];

                if(ibuffer == 0 && (_thrownword.StartsWith(letter.ToLower()) == true))
                {
                    buffer = letter;
                    ibuffer++;
                }
                else if (_thrownword.StartsWith(buffer.ToLower() + letter.ToLower()))
                {
                    buffer += letter;
                    ibuffer++;
                }
                else
                {
                    buffer = "";
                    ibuffer = 0;
                }
            }

            return buffer.ToLower();
        }

        #endregion

        //Public methods of the class
        #region public

        public String GiveWord(ref String[] hist)
        {
            _history = hist;

            String word;
            int upperbound = _library.GetUpperBound(0) - 1;
            do
            {
                int wordnr = moRandomGenerator.Next(0, upperbound);
                word = _library[wordnr];
            } while (IsWordThrown(word.ToLower()) == true);

            AddToArray(ref _history, word.ToLower());
            _thrownword = word;
            hist = _history;

            return word;
        }

        public bool IsWordThrown(String word)
        {
            foreach(String sword in _history)
            {
                if (sword.ToLower().ToString() == word.ToLower().ToString())
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        //Private method of the class
        #region private

        private static void AddToArray(ref String[] arrs, String toadd)
        {
            Array.Resize(ref arrs, arrs.Length + 1);
            arrs[arrs.GetUpperBound(0)] = toadd;
        }

        #endregion
    }
}
