using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Timers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemoryGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Uri> cards;
        private bool[] isMatched = new bool[16];

        private Uri flipped1;
        private Uri flipped2;
        private int posflipped1;
        private int posflipped2;

        private System.Timers.Timer fliptimer;
        private System.Timers.Timer gametimer = new System.Timers.Timer(1000);

        private Uri[] cardsLocation = new Uri[16];

        public Uri backOfCard = new Uri("backofCard.png" , UriKind.Relative);

        private int timeTaken = 0;

        private int[] topscores = new int[3] { 100000000, 10000000, 10000000 };

        private StreamWriter fileWrite;

        private StreamReader fileRead;

        //private BinaryFormatter formatter = new BinaryFormatter();

        public MainWindow()
        {
            InitializeComponent();
            gametimer.Elapsed += new ElapsedEventHandler(gametick);
            loadGame();
        }


        private void rankings()
        {
            for (int i = 0; i < topscores.Length; i++)
            {
                if (timeTaken < topscores[i])
                {
                    if (i == 2)
                    {
                        topscores[i] = timeTaken;
                        score3lbl.Content = "User: " + usernametxt.Text + " has time: " + timeTaken + " level: " + levellbl.Content;
                    }
                    else if (i == 1)
                    {
                        topscores[i++] = topscores[i];
                        topscores[i] = timeTaken;
                        score3lbl.Content = score2lbl.Content;
                        score2lbl.Content = "User: " + usernametxt.Text + " has time: " + timeTaken + " level: " + levellbl.Content;

                    }
                    else if (i == 0)
                    {
                        topscores[2] = topscores[i++];
                        topscores[i++] = topscores[i];
                        topscores[i] = timeTaken;
                        score3lbl.Content = score2lbl.Content;
                        score2lbl.Content = score1lbl.Content;
                        score1lbl.Content = "User: " + usernametxt.Text + " has time: " + timeTaken + " level: " + levellbl.Content;

                    }
                }
            }
        }

        private void InitializeGame()
        {
        cards = new List<Uri>()
        {
        new Uri("aceOfSpades.png", UriKind.Relative),
        new Uri("clubs6.png", UriKind.Relative),
        new Uri("diamond10.png", UriKind.Relative),
        new Uri("hearts2.png", UriKind.Relative),
        new Uri("jackOfDiamonds.png", UriKind.Relative),
        new Uri("kingOfHearts.png", UriKind.Relative),
        new Uri("queenOfClubs.png", UriKind.Relative),
        new Uri("spades8.png", UriKind.Relative),
        new Uri("aceOfSpades.png", UriKind.Relative),
        new Uri("clubs6.png", UriKind.Relative),
        new Uri("diamond10.png", UriKind.Relative),
        new Uri("hearts2.png", UriKind.Relative),
        new Uri("jackOfDiamonds.png", UriKind.Relative),
        new Uri("kingOfHearts.png", UriKind.Relative),
        new Uri("queenOfClubs.png", UriKind.Relative),
        new Uri("spades8.png", UriKind.Relative)
        };

            for(int i = 0; i < isMatched.Length; i++)
            {
                isMatched[i] = false;
            }

            AssignCard();
            AssignBackOfCard();
            removeAllCardEvents();

            acendlvlbtn.IsEnabled = true;
            decendlvlbtn.IsEnabled = true;
            stopbtn.IsEnabled = false;
            startbtn.IsEnabled = true;
            gametimer.Stop();
            usernametxt.IsReadOnly = false;
            timeTaken = 0;
            posflipped1 = -1;
            posflipped2 = -1;
            Timekeeplbl.Content = timeTaken;
        }

        private void startAllCardEvents()
        {
            Card1x1.MouseLeftButtonDown += Clickcard1x1;
            Card1x2.MouseLeftButtonDown += ClickCard1x2;
            Card1x3.MouseLeftButtonDown += ClickCard1x3;
            Card1x4.MouseLeftButtonDown += ClickCard1x4;
            Card2x1.MouseLeftButtonDown += ClickCard2x1;
            Card2x2.MouseLeftButtonDown += ClickCard2x2;
            Card2x3.MouseLeftButtonDown += ClickCard2x3;
            Card2x4.MouseLeftButtonDown += ClickCard2x4;
            Card3x1.MouseLeftButtonDown += ClickCard3x1;
            Card3x2.MouseLeftButtonDown += ClickCard3x2;
            Card3x3.MouseLeftButtonDown += ClickCard3x3;
            Card3x4.MouseLeftButtonDown += ClickCard3x4;
            Card4x1.MouseLeftButtonDown += ClickCard4x1;
            Card4x2.MouseLeftButtonDown += ClickCard4x2;
            Card4x3.MouseLeftButtonDown += ClickCard4x3;
            Card4x4.MouseLeftButtonDown += ClickCard4x4;
        }
        private void removeAllCardEvents()
        {
            Card1x1.MouseLeftButtonDown -= Clickcard1x1;
            Card1x2.MouseLeftButtonDown -= ClickCard1x2;
            Card1x3.MouseLeftButtonDown -= ClickCard1x3;
            Card1x4.MouseLeftButtonDown -= ClickCard1x4;
            Card2x1.MouseLeftButtonDown -= ClickCard2x1;
            Card2x2.MouseLeftButtonDown -= ClickCard2x2;
            Card2x3.MouseLeftButtonDown -= ClickCard2x3;
            Card2x4.MouseLeftButtonDown -= ClickCard2x4;
            Card3x1.MouseLeftButtonDown -= ClickCard3x1;
            Card3x2.MouseLeftButtonDown -= ClickCard3x2;
            Card3x3.MouseLeftButtonDown -= ClickCard3x3;
            Card3x4.MouseLeftButtonDown -= ClickCard3x4;
            Card4x1.MouseLeftButtonDown -= ClickCard4x1;
            Card4x2.MouseLeftButtonDown -= ClickCard4x2;
            Card4x3.MouseLeftButtonDown -= ClickCard4x3;
            Card4x4.MouseLeftButtonDown -= ClickCard4x4;
        }

        private void AssignCard()
        {
            Random rand = new Random();
            for(int i=0; i < cardsLocation.Length ; i++)
            {
                int loc = rand.Next(0, cards.Count);
                cardsLocation[i] = cards[loc];
                cards.RemoveAt(loc);
            }

        }

        private void AssignBackOfCard()
        {
            foreach (Image card in gridGame.Children)
            {
                card.Source = new BitmapImage(backOfCard);
           
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!usernametxt.Text.Equals(""))
            {
                startAllCardEvents();
                acendlvlbtn.IsEnabled = false;
                decendlvlbtn.IsEnabled = false;
                stopbtn.IsEnabled = true;
                startbtn.IsEnabled = false;
                gametimer.Enabled = true;
                gametimer.Start();
                usernametxt.IsReadOnly = true;
            }
            else
            {
                System.Windows.MessageBox.Show("Enter username");
            }
        }

 
        private void gametick(object sender, EventArgs e)
        {
            timeTaken = timeTaken +1;
            Dispatcher.Invoke(() => { Timekeeplbl.Content = timeTaken; });
        }
    

        private void stop_click(object sender, RoutedEventArgs e)
        {
            removeAllCardEvents();
            stopbtn.IsEnabled = false;
            startbtn.IsEnabled = true;
            gametimer.Stop();
        }

        private void Resetbtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();

        }




        private void mouseEnter(object sender, StylusEventArgs e)
        {

        }

        private void hasWon()
        {
            bool winCheck = true;
            for(int i = 0; i < isMatched.Length; i++)
            {
                if(isMatched[i] == false)
                {
                    winCheck = false;
                }
            }
            if(winCheck)
            {
                gametimer.Stop();
                System.Windows.MessageBox.Show("You won. Time: " + timeTaken);
                rankings();
                removeAllCardEvents();
                stopbtn.IsEnabled = false;
                startbtn.IsEnabled = false;
                acendlvlbtn.IsEnabled = false;
                decendlvlbtn.IsEnabled = false;
            }
        }

        private void Clickcard1x1(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[0] == false)
            {
                if (flipped1 == null)
                {
                    Card1x1.Source = new BitmapImage(cardsLocation[0]);
                    flipped1 = cardsLocation[0];
                    posflipped1 = 0;
                }
                else if (flipped2 == null && posflipped1 != 0)
                {
                    Card1x1.Source = new BitmapImage(cardsLocation[0]);
                    flipped2 = cardsLocation[0];
                    posflipped2 = 0;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard1x2(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[1] == false)
            {
                if (flipped1 == null)
                {
                    Card1x2.Source = new BitmapImage(cardsLocation[1]);
                    flipped1 = cardsLocation[1];
                    posflipped1 = 1;
                }
                else if (flipped2 == null && posflipped1 != 1)
                {
                    Card1x2.Source = new BitmapImage(cardsLocation[1]);

                    flipped2 = cardsLocation[1];
                    posflipped2 = 1;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard1x3(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[2] == false)
            {
                if (flipped1 == null)
                {

                    Card1x3.Source = new BitmapImage(cardsLocation[2]);
                    flipped1 = cardsLocation[2];
                    posflipped1 = 2;
                }
                else if (flipped2 == null && posflipped1 != 2)
                {
                    Card1x3.Source = new BitmapImage(cardsLocation[2]);
                    flipped2 = cardsLocation[2];
                    posflipped2 = 2;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard1x4(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[3] == false)
            {
                if (flipped1 == null)
                {

                    Card1x4.Source = new BitmapImage(cardsLocation[3]);
                    flipped1 = cardsLocation[3];
                    posflipped1 = 3;
                }
                else if (flipped2 == null && posflipped1 != 3)
                {
                    Card1x4.Source = new BitmapImage(cardsLocation[3]);
                    flipped2 = cardsLocation[3];
                    posflipped2 = 3;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard2x1(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[4] == false)
            {
                if (flipped1 == null)
                {

                    Card2x1.Source = new BitmapImage(cardsLocation[4]);
                    flipped1 = cardsLocation[4];
                    posflipped1 = 4;
                }
                else if (flipped2 == null && posflipped1 != 4)
                {
                    Card2x1.Source = new BitmapImage(cardsLocation[4]);
                    flipped2 = cardsLocation[4];
                    posflipped2 = 4;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard2x2(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[5] == false)
            {
                if (flipped1 == null)
                {
                    Card2x2.Source = new BitmapImage(cardsLocation[5]);
                    flipped1 = cardsLocation[5];
                    posflipped1 = 5;
                }
                else if (flipped2 == null && posflipped1 != 5)
                {
                    Card2x2.Source = new BitmapImage(cardsLocation[5]);
                    flipped2 = cardsLocation[5];
                    posflipped2 = 5;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard2x3(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[6] == false)
            {
                if (flipped1 == null)
                {
                    Card2x3.Source = new BitmapImage(cardsLocation[6]);
                    flipped1 = cardsLocation[6];
                    posflipped1 = 6;
                }
                else if (flipped2 == null && posflipped1 != 6)
                {
                    Card2x3.Source = new BitmapImage(cardsLocation[6]);
                    flipped2 = cardsLocation[6];
                    posflipped2 = 6;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard2x4(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[7] == false)
            {
                if (flipped1 == null)
                {
                    Card2x4.Source = new BitmapImage(cardsLocation[7]);
                    flipped1 = cardsLocation[7];
                    posflipped1 = 7;
                }
                else if (flipped2 == null && posflipped1 != 7)
                {
                    Card2x4.Source = new BitmapImage(cardsLocation[7]);
                    flipped2 = cardsLocation[7];
                    posflipped2 = 7;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard3x1(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[8] == false)
            {
                if (flipped1 == null)
                {
                    Card3x1.Source = new BitmapImage(cardsLocation[8]);
                    flipped1 = cardsLocation[8];
                    posflipped1 = 8;
                }
                else if (flipped2 == null && posflipped1 != 8)
                {
                    Card3x1.Source = new BitmapImage(cardsLocation[8]);
                    flipped2 = cardsLocation[8];
                    posflipped2 = 8;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard3x2(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[9] == false)
            {
                if (flipped1 == null)
                {
                    Card3x2.Source = new BitmapImage(cardsLocation[9]);
                    flipped1 = cardsLocation[9];
                    posflipped1 = 9;
                }
                else if (flipped2 == null && posflipped1 != 9)
                {
                    Card3x2.Source = new BitmapImage(cardsLocation[9]);
                    flipped2 = cardsLocation[9];
                    posflipped2 = 9;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard3x3(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[10] == false)
            {
                if (flipped1 == null)
                {
                    Card3x3.Source = new BitmapImage(cardsLocation[10]);
                    flipped1 = cardsLocation[10];
                    posflipped1 = 10;
                }
                else if (flipped2 == null && posflipped1 != 10)
                {
                    Card3x3.Source = new BitmapImage(cardsLocation[10]);
                    flipped2 = cardsLocation[10];
                    posflipped2 = 10;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard3x4(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[11] == false)
            {
                if (flipped1 == null)
                {
                    Card3x4.Source = new BitmapImage(cardsLocation[11]);
                    flipped1 = cardsLocation[11];
                    posflipped1 = 11;
                }
                else if (flipped2 == null && posflipped1 != 11)
                {
                    Card3x4.Source = new BitmapImage(cardsLocation[11]);
                    flipped2 = cardsLocation[11];
                    posflipped2 = 11;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard4x1(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[12] == false)
            {
                if (flipped1 == null)
                {
                    Card4x1.Source = new BitmapImage(cardsLocation[12]);
                    flipped1 = cardsLocation[12];
                    posflipped1 = 12;
                }
                else if (flipped2 == null && posflipped1 != 12)
                {
                    Card4x1.Source = new BitmapImage(cardsLocation[12]);
                    flipped2 = cardsLocation[12];
                    posflipped2 = 12;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard4x2(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[13] == false)
            {
                if (flipped1 == null)
                {
                    Card4x2.Source = new BitmapImage(cardsLocation[13]);
                    flipped1 = cardsLocation[13];
                    posflipped1 = 13;
                }
                else if (flipped2 == null && posflipped1 != 13)
                {
                    Card4x2.Source = new BitmapImage(cardsLocation[13]);
                    flipped2 = cardsLocation[13];
                    posflipped2 = 13;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void ClickCard4x3(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[14] == false)
            {
                if (flipped1 == null)
                {
                    Card4x3.Source = new BitmapImage(cardsLocation[14]);
                    flipped1 = cardsLocation[14];
                    posflipped1 = 14;
                }
                else if (flipped2 == null && posflipped1 != 14)
                {
                    Card4x3.Source = new BitmapImage(cardsLocation[14]);
                    flipped2 = cardsLocation[14];
                    posflipped2 = 14;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }

                }
            }
        }

        private void ClickCard4x4(object sender, MouseButtonEventArgs e)
        {
            if (isMatched[15] == false)
            {
                if (flipped1 == null)
                {
                    Card4x4.Source = new BitmapImage(cardsLocation[15]);
                    flipped1 = cardsLocation[15];
                    posflipped1 = 15;
                }
                else if (flipped2 == null & posflipped1 != 15)
                {
                    Card4x4.Source = new BitmapImage(cardsLocation[15]);
                    flipped2 = cardsLocation[15];
                    posflipped2 = 15;
                    string stringflipped1 = flipped1.ToString();
                    string stringflipped2 = flipped2.ToString();
                    if (stringflipped1.Equals(stringflipped2))
                    {
                        haveMatched();
                        hasWon();
                    }
                    else
                    {
                        setTimer();
                    }
                }
            }
        }

        private void haveMatched()
        {
            isMatched[posflipped1] = true;
            isMatched[posflipped2] = true;
            flipped1 = null;
            flipped2 = null;
            posflipped1 = -1;
            posflipped2 = -1;

        }



        private void setTimer()
        {
            if ( Convert.ToInt32(levellbl.Content) == 1)
            {
                fliptimer = new System.Timers.Timer(1500);
                fliptimer.Elapsed += new ElapsedEventHandler(tick);
                fliptimer.Enabled = true;
                fliptimer.Start();
            }
            else if (Convert.ToInt32(levellbl.Content) == 2)
            {
                fliptimer = new System.Timers.Timer(1000);
                fliptimer.Elapsed += new ElapsedEventHandler(tick);
                fliptimer.Enabled = true;
                fliptimer.Start();
            }
            else if (Convert.ToInt32(levellbl.Content) == 3)
            {
                fliptimer = new System.Timers.Timer(500);
                fliptimer.Elapsed += new ElapsedEventHandler(tick);
                fliptimer.Enabled = true;
                fliptimer.Start();
            }
        }
        private void tick(object sender,EventArgs e)
        {
            fliptimer.Stop();
            Dispatcher.Invoke(new ThreadStart(() => compare()));
            
        }
        private void compare()
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).UpdateLayout();
            foreach (Image card in gridGame.Children)
            {
                if( card == gridGame.Children[posflipped1] || card == gridGame.Children[posflipped2])
                {
                    card.Source = new BitmapImage(backOfCard);
                }
            }
                posflipped1 = -1;
                posflipped2 = -1;
                flipped1 = null;
                flipped2 = null;
        }

        private void decendlvlbtn_Click(object sender, RoutedEventArgs e)
        {
            if (stopbtn.IsEnabled == false)
            {
                if (Convert.ToInt32(levellbl.Content) == 3)
                {
                    levellbl.Content = 2;
                }
                else if (Convert.ToInt32(levellbl.Content) == 2)
                {
                    levellbl.Content = 1;
                }
            }
        }

        private void acendlvlbtn_Click(object sender, RoutedEventArgs e)
        {
            if (stopbtn.IsEnabled == false)
            {
                if (Convert.ToInt32(levellbl.Content) == 1)
                {
                    levellbl.Content = 2;
                }
                else if (Convert.ToInt32(levellbl.Content) == 2)
                {
                    levellbl.Content = 3;
                }
            }
        }

        private void loadGame()
        {
            try
            {
                FileStream savingData = new FileStream("gamedata", FileMode.Open, FileAccess.Read);
                fileRead = new StreamReader(savingData);
                for (int i = 0; i < cardsLocation.Length; i++)
                {
                    cardsLocation[i] = new Uri (fileRead.ReadLine(), UriKind.Relative);
                }
                for (int i = 0; i < isMatched.Length; i++)
                {
                    isMatched[i] = Convert.ToBoolean(fileRead.ReadLine());
                }
                String flippedcheck = fileRead.ReadLine();
                if (flippedcheck.Equals("null"))
                {
                    flipped1 = null;
                }
                else
                {
                    flipped1 = new Uri(fileRead.ReadLine(), UriKind.Relative);
                }
                posflipped1 = Convert.ToInt32(fileRead.ReadLine());
                timeTaken = Convert.ToInt32(fileRead.ReadLine());
                for (int i = 0; i < topscores.Length; i++)
                {
                    topscores[i] = Convert.ToInt32(fileRead.ReadLine());
                }
                score1lbl.Content = fileRead.ReadLine();
                score2lbl.Content = fileRead.ReadLine();
                score3lbl.Content = fileRead.ReadLine();
                usernametxt.Text = fileRead.ReadLine();
                fileRead.Close();

   
                stopbtn.IsEnabled = false;
                startbtn.IsEnabled = true;
                if(timeTaken > 0)
                {
                    usernametxt.IsReadOnly = true;
                    acendlvlbtn.IsEnabled = false;
                    decendlvlbtn.IsEnabled = false;
                }
                else
                {
                    usernametxt.IsReadOnly = false;
                    acendlvlbtn.IsEnabled = true;
                    decendlvlbtn.IsEnabled = true;
                }
      
                gametimer.Stop();
                posflipped2 = -1;

                int count = 0;
                foreach (Image card in gridGame.Children)
                {
                    if(isMatched[count])
                    {
                        card.Source = new BitmapImage(cardsLocation[count]);
                    }
                    else
                    {
                        card.Source = new BitmapImage(backOfCard);
                    }
                    count++;
                }
                removeAllCardEvents();
                Timekeeplbl.Content = timeTaken;
            }
            catch(Exception)
            {

            }
            finally
            {
                fileRead.Close();
            }
        }

        private bool exitWinCheck()
        {
            bool winCheck = true;
            for (int i = 0; i < isMatched.Length; i++)
            {
                if (isMatched[i] == false)
                {
                    winCheck = false;
                }
            }
            return winCheck;
        }



        private void clickclosed(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if(exitWinCheck())
                {
                    InitializeGame();
                }
                FileStream savingData = new FileStream("gamedata", FileMode.Create, FileAccess.Write);
                fileWrite = new StreamWriter(savingData);
                foreach (Uri img in cardsLocation)
                {
                    fileWrite.WriteLine(img.ToString());
                }
                for (int i = 0; i < isMatched.Length; i++)
                {
                    fileWrite.WriteLine(isMatched[i].ToString());
                }
                if (flipped1 == null)
                {
                    fileWrite.WriteLine("null");
                    fileWrite.WriteLine(-1);
                }
                else
                {
                    fileWrite.WriteLine(flipped1.ToString());
                    fileWrite.WriteLine(posflipped1.ToString());
                }
                fileWrite.WriteLine(timeTaken.ToString());
                for (int i = 0; i < topscores.Length; i++)
                {
                    fileWrite.WriteLine(topscores[i].ToString());
                }
                fileWrite.WriteLine(score1lbl.Content);
                fileWrite.WriteLine(score2lbl.Content);
                fileWrite.WriteLine(score3lbl.Content);
                fileWrite.WriteLine(usernametxt.Text);
                fileWrite.Close();

               


            }
            catch (Exception)
            {

            }
            finally
            {
                fileWrite.Close();
            }
        }
    }
}
