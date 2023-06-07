using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[,] IntMap;
        public int[,] MoveMap;
        public int[,] ButtonNumMap;
        public int[,] FiguresValueMap;
        public string[,] StrMap;
        public double turn = 0;
        public int MoveCntFlag = 0;
        public int RateMoveCnt = 0;
        public int MateCheck = 0;
        public bool ChoosingSide = true; //false - чёрные  true - белые
        public int cnt = 0;

        private Dictionary<int, int> boardRules = new Dictionary<int, int>();
        private Dictionary<int, Action<int, int, bool, int>> chessRules = new Dictionary<int, Action<int, int, bool, int>>();
        private Dictionary<int, List<Move>> movies = new Dictionary<int, List<Move>>();

        //        Расшифровка номеров фигур
        //
        // Король Игрока   - 1       Король ИИ   - 7
        // Королева Игрока - 2       Королева ИИ - 8
        // Слон Игрока     - 3       Слон ИИ     - 9
        // Конь Игрока     - 4       Конь ИИ     - 10
        // Ладья Игрока    - 5       Ладья ИИ    - 11
        // Пешка Игрока    - 6       Пешка ИИ    - 12

        public MainWindow()
        {
            InitializeComponent();
            SetStartingLocation();
            RedriwingFigure();
            boardRules.Add(1, -900);
            boardRules.Add(2, -90);
            boardRules.Add(3, -30);
            boardRules.Add(4, -30);
            boardRules.Add(5, -50);
            boardRules.Add(6, -10);
            boardRules.Add(7, 900);
            boardRules.Add(8, 90);
            boardRules.Add(9, 30);
            boardRules.Add(10, 30);
            boardRules.Add(11, 50);
            boardRules.Add(12, 10);

            chessRules.Add(1, KingRules);
            chessRules.Add(2, QueenRules);
            chessRules.Add(3, BishopRules);
            chessRules.Add(4, KnightRules);
            chessRules.Add(5, RookRules);
            chessRules.Add(6, PawnRules);

            chessRules.Add(7, KingRules);
            chessRules.Add(8, QueenRules);
            chessRules.Add(9, BishopRules);
            chessRules.Add(10, KnightRules);
            chessRules.Add(11, RookRules);
            chessRules.Add(12, PawnRules);

            movies.Add(1, new List<Move>());
            movies.Add(2, new List<Move>());
            movies.Add(3, new List<Move>());
            movies.Add(4, new List<Move>());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MoveCntFlag += 1;
            var button = (UIElement)sender;
            var col = Grid.GetColumn(button) - 1;
            var row = 8 - Grid.GetRow(button);
            //MessageBox.Show($"col {col} row {row} 123 {ButtonNumMap[row, col]}");
            CheckMove(row, col, ButtonNumMap[row, col], 0);
        }

        private void SetStartingLocation()
        {
            MoveMap = getClearArray();
            IntMap = new int[8, 8]
            {
                { 5,  4,  3,  2,  1,  3,  4,  5  },
                { 6,  6,  6,  6,  6,  6,  6,  6  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 12, 12, 12, 12, 12, 12, 12, 12 },
                { 11, 10, 9,  8,  7,  9,  10, 11 }
            };
            ButtonNumMap = new int[8, 8]
            {
                { 0,  8,   16,  24,  32,  40,  48,  56  },
                { 1,  9,   17,  25,  33,  41,  49,  57  },
                { 2,  10,  18,  26,  34,  42,  50,  58  },
                { 3,  11,  19,  27,  35,  43,  51,  59  },
                { 4,  12,  20,  28,  36,  44,  52,  60  },
                { 5,  13,  21,  29,  37,  45,  53,  61  },
                { 6,  14,  22,  30,  38,  46,  54,  62  },
                { 7,  15,  23,  31,  39,  47,  55,  63  }
            };
            StrMap = new string[8, 8]
            {
                {"Images/WhiteRook.png","Images/WhiteKnight.png","Images/WhiteBishop.png","Images/WhiteQueen.png","Images/WhiteKing.png","Images/WhiteBishop.png","Images/WhiteKnight.png","Images/WhiteRook.png" },
                {"Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png","Images/WhitePawn.png"},
                {"","","","","","","","" },
                {"","","","","","","","" },
                {"","","","","","","","" },
                {"","","","","","","","" },
                {"Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png","Images/BlackPawn.png"},
                {"Images/BlackRook.png","Images/BlackKnight.png","Images/BlackBishop.png","Images/BlackQueen.png","Images/BlackKing.png","Images/BlackBishop.png","Images/BlackKnight.png","Images/BlackRook.png" }
            };
            FiguresValueMap = new int[8, 8]
            {
                { 50,    30,   30,   90,   900,   30,   30,    50  },
                { 10,    10,   10,   10,   10,    10,   10,    10  },
                { 0,     0,    0,    0,    0,     0,    0,     0   },
                { 0,     0,    0,    0,    0,     0,    0,     0   },
                { 0,     0,    0,    0,    0,     0,    0,     0   },
                { 0,     0,    0,    0,    0,     0,    0,     0   },
                { -10,  -10,  -10,  -10,  -10,   -10,  -10,   -10  },
                { -50,  -30,  -30,  -90,  -900,  -30,  -30,   -50  }
            };
            //RateBoard();
        }

        private void ChengingSide_Click(object sender, RoutedEventArgs e)
        {
            ChoosingSide = !ChoosingSide;
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int intbuff = IntMap[i, j];
                    string strbuff = StrMap[i, j];
                    StrMap[i, j] = StrMap[i + 7, j];
                    StrMap[i + 7, j] = strbuff;

                    intbuff = IntMap[i + 1, j];
                    strbuff = StrMap[i + 1, j];
                    StrMap[i + 1, j] = StrMap[i + 6, j];
                    StrMap[i + 6, j] = strbuff;
                }
            }
            RedriwingFigure();
        }

        private void RedriwingFigure()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var row = 7 - i;
                    GetChildren<Image>(MainGrid, i + 1, j + 1).Source = new BitmapImage(new Uri(StrMap[row, j], UriKind.Relative));
                }
            }
        }

        private void Surrender_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите сдаться?", "Сдаться", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Button> listButtons = new List<Button>();
                GetLogicalChildCollection(this, listButtons);
                //кнопка смены стороны
                listButtons[64].IsEnabled = true;
                SetStartingLocation();
                RedriwingFigure();
            }
        }

        private void CheckMove(int x, int y, int num, int depth)
        {
            if (MoveCntFlag == 1)
            {
                MoveMap[x, y] = 1;
                CheckRules(x, y, num, true, depth);
            }
            else
            {
                MoveMap[x, y] = 2;
                int ClickCnt = 0;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (MoveMap[i, j] == 1 || MoveMap[i, j] == 2)
                        {
                            ClickCnt++;
                        }
                    }
                }
                if (ClickCnt == 1)
                {
                    MoveMap = getClearArray();
                    MoveCntFlag = 0;
                    OnOffButtons(true, 65);
                }
                if (MoveCntFlag == 2)
                    MoveFigure();
            }
        }

        private void MoveFigure()
        {
            OnOffButtons(true, 0);
            List<Button> listButtons = new List<Button>();
            GetLogicalChildCollection(this, listButtons);
            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (MoveMap[i, j] == 1)
                    {
                        x1 = i;
                        y1 = j;
                    }
                    if (MoveMap[i, j] == 2)
                    {
                        x2 = i;
                        y2 = j;
                    }
                }
            }
            string ImgBuf = StrMap[x1, y1];
            int IntBuff = IntMap[x1, y1];
            int ValBuff = FiguresValueMap[x1, y1];
            StrMap[x1, y1] = "";
            IntMap[x1, y1] = 0;
            FiguresValueMap[x1, y1] = 0;
            StrMap[x2, y2] = ImgBuf;
            IntMap[x2, y2] = IntBuff;
            FiguresValueMap[x2, y2] = ValBuff;
            RedriwingFigure();
            MoveCntFlag = 0;
            MoveMap = getClearArray();
            CheckTurn();
            OnOffButtons(true, 0);
        }

        private void CheckTurn()
        {
            if (turn % 2 == 0)
            {
                MessageBox.Show("igrok hod");
                turn++;
                CheckTurn();
                //CheckEndGame();
            }
            else
            {
                turn++;
                foreach (var item in movies.Values)
                {
                    item.Clear();
                }
                cnt = 0;
                var watch = Stopwatch.StartNew();
                var MaxRate = Minimax(4, true, int.MinValue, int.MaxValue);
                watch.Stop();
                MessageBox.Show("Обработано ходов:" + cnt.ToString()+" Время выполнения " + watch.Elapsed+ " maxvalue "+ MaxRate);
                IntMap[MaxRate.Item3, MaxRate.Item4] = IntMap[MaxRate.Item1, MaxRate.Item2];
                StrMap[MaxRate.Item3, MaxRate.Item4] = StrMap[MaxRate.Item1, MaxRate.Item2];
                IntMap[MaxRate.Item1, MaxRate.Item2] = 0;
                StrMap[MaxRate.Item1, MaxRate.Item2] = "";
                RedriwingFigure();
                //CheckEndGame();
            }
        }

        private void CheckEndGame()
        {
            int cnt = 0;
            bool flag = false, BlackKing = false, WhiteKing = false, End = false;
            for (int i = 0; i < 8 && flag == false; i++)
            {
                for (int j = 0; j < 8 && flag == false; j++)
                {
                    if (IntMap[i, j] == 1)
                    {
                        WhiteKing = true;
                        cnt++;
                    }
                    if (IntMap[i, j] == 7)
                    {
                        BlackKing = true;
                        cnt++;
                    }
                    if (cnt == 2)
                    {
                        flag = true;
                    }
                }
            }
            if (!BlackKing)
            {
                MessageBox.Show("Вы победили");
                End = true;
            }
            if (!WhiteKing)
            {
                MessageBox.Show("Вы проиграли");
                End = true;
            }
            if (End == true)
            {
                List<Button> listButtons = new List<Button>();
                GetLogicalChildCollection(this, listButtons);
                listButtons[64].IsEnabled = true;
                SetStartingLocation();
                RedriwingFigure();
            }
        }
        private void CheckRules(int x, int y, int num, bool flag, int depth)
        {
            List<Button> listButtons = new List<Button>();
            GetLogicalChildCollection(this, listButtons);
            OnOffButtons(false, num);
            chessRules.TryGetValue(IntMap[x, y], out Action<int, int, bool, int> popedFunction);
            if (popedFunction != null)
                popedFunction(x, y, IntMap[x, y] < 7, depth);
        }

        private void PawnRules(int x, int y, bool side, int depth)
        {
            int mod = side ? 1 : -1;
            //Вверх
            if (x + mod < 8 && x + mod > -1)
            {
                if (IntMap[x + mod, y] == 0)
                {
                    OnAndFillButtons(x + mod, y);
                    AddMove(x, y, x + mod, y, depth);
                }
                if (y + 1 < 8)
                {
                    if ((IntMap[x + mod, y + 1] >= 7 && mod > 0) || (IntMap[x + mod, y + 1] <= 6 && IntMap[x + mod, y + 1] != 0 && mod < 0))
                    {
                        OnAndFillAttackButtons(x + mod, y + 1);
                        AddMove(x, y, x + mod, y + 1, depth);
                    }
                }
                if (y - 1 > -1)
                {
                    if ((IntMap[x + mod, y - 1] >= 7 && mod > 0) || (IntMap[x + mod, y - 1] <= 6 && IntMap[x + mod, y - 1] != 0 && mod < 0))
                    {
                        OnAndFillAttackButtons(x + mod, y - 1);
                        AddMove(x, y, x + mod, y - 1, depth);
                    }
                }
            }
            if (mod > 0)
                mod++;
            else
                mod--;
            //Первых ход пешки
            if (((x == 1 && mod > 0) || (x == 6 && mod < 0)) && IntMap[x + mod, y] == 0)
            {
                if (IntMap[x + mod, y] == 0)
                {
                    OnAndFillButtons(x + mod, y);
                    AddMove(x, y, x + mod, y, depth);
                }
            }
        }

        private void RookRules(int x, int y, bool side, int depth)
        {
            for (int i = x + 1; i < 8; i++)
            {
                if (IntMap[i, y] == 0)
                {
                    OnAndFillButtons(i, y);
                    AddMove(x, y, i, y, depth);
                }
                else if ((IntMap[i, y] >= 7 && side) || (IntMap[i, y] <= 6 && IntMap[i, y] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, y);
                    AddMove(x, y, i, y, depth);
                    break;
                }
                else
                    break;

            }
            //Вниз
            for (int i = x - 1; i > -1; i--)
            {
                if (IntMap[i, y] == 0)
                {
                    OnAndFillButtons(i, y);
                    AddMove(x, y, i, y, depth);
                }
                else if ((IntMap[i, y] >= 7 && side) || (IntMap[i, y] <= 6 && IntMap[i, y] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, y);
                    AddMove(x, y, i, y, depth);
                    break;
                }
                else
                    break;
            }
            //Вправо
            for (int i = y + 1; i < 8; i++)
            {
                if (IntMap[x, i] == 0)
                {
                    OnAndFillButtons(x, i);
                    AddMove(x, y, x, i, depth);
                }
                else if ((IntMap[x, i] >= 7 && side) || (IntMap[x, i] <= 6 && IntMap[x, i] != 0 && !side))
                {
                    OnAndFillAttackButtons(x, i);
                    AddMove(x, y, x, i, depth);
                    break;
                }
                else
                    break;
            }
            //Влево
            for (int i = y - 1; i > -1; i--)
            {
                if (IntMap[x, i] == 0)
                {
                    OnAndFillButtons(x, i);
                    AddMove(x, y, x, i, depth);
                }
                else if ((IntMap[x, i] >= 7 && side) || (IntMap[x, i] <= 6 && IntMap[x, i] != 0 && !side))
                {
                    OnAndFillAttackButtons(x, i);
                    AddMove(x, y, x, i, depth);
                    break;
                }
                else
                    break;
            }
        }

        private void KnightRules(int x, int y, bool side, int depth)
        {
            //Вверх-вправо
            if (x + 2 < 8 && y + 1 < 8)
            {
                if (IntMap[x + 2, y + 1] == 0)
                {
                    OnAndFillButtons(x + 2, y + 1);
                    AddMove(x, y, x + 2, y + 1, depth);
                }
                if (IntMap[x + 2, y + 1] >= 7 && side || (IntMap[x + 2, y + 1] <= 6 && IntMap[x + 2, y + 1] != 0 && !side))
                {
                    OnAndFillAttackButtons(x + 2, y + 1);
                    AddMove(x, y, x + 2, y + 1, depth);
                }
            }
            //Вверх-влево
            if (x + 2 < 8 && y - 1 > -1)
            {
                if (IntMap[x + 2, y - 1] == 0)
                {
                    OnAndFillButtons(x + 2, y - 1);
                    AddMove(x, y, x + 2, y - 1, depth);
                }
                if (IntMap[x + 2, y - 1] >= 7 && side || (IntMap[x + 2, y - 1] <= 6 && IntMap[x + 2, y - 1] != 0 && !side))
                {
                    OnAndFillAttackButtons(x + 2, y - 1);
                    AddMove(x, y, x + 2, y - 1, depth);
                }
            }
            //Вниз-вправо
            if (x - 2 > -1 && y + 1 < 8)
            {
                if (IntMap[x - 2, y + 1] == 0)
                {
                    OnAndFillButtons(x - 2, y + 1);
                    AddMove(x, y, x - 2, y + 1, depth);
                }
                if (IntMap[x - 2, y + 1] >= 7 && side || (IntMap[x - 2, y + 1] <= 6 && IntMap[x - 2, y + 1] != 0 && !side))
                {
                    OnAndFillAttackButtons(x - 2, y + 1);
                    AddMove(x, y, x - 2, y + 1, depth);
                }
            }
            //Вниз-влево
            if (x - 2 > -1 && y - 1 > -1)
            {
                if (IntMap[x - 2, y - 1] == 0)
                {
                    OnAndFillButtons(x - 2, y - 1);
                    AddMove(x, y, x - 2, y - 1, depth);
                }
                if (IntMap[x - 2, y - 1] >= 7 && side || (IntMap[x - 2, y - 1] <= 6 && IntMap[x - 2, y - 1] != 0 && !side))
                {
                    OnAndFillAttackButtons(x - 2, y - 1);
                    AddMove(x, y, x - 2, y - 1, depth);
                }
            }
            //Вправо-вверх
            if (x + 1 < 8 && y + 2 < 8)
            {
                if (IntMap[x + 1, y + 2] == 0)
                {
                    OnAndFillButtons(x + 1, y + 2);
                    AddMove(x, y, x + 1, y + 2, depth);
                }
                if (IntMap[x + 1, y + 2] >= 7 && side || (IntMap[x + 1, y + 2] <= 6 && IntMap[x + 1, y + 2] != 0 && !side))
                {
                    OnAndFillAttackButtons(x + 1, y + 2);
                    AddMove(x, y, x + 1, y + 2, depth);
                }
            }
            //Вправо-вниз
            if (x - 1 > -1 && y + 2 < 8)
            {
                if (IntMap[x - 1, y + 2] == 0)
                {
                    OnAndFillButtons(x - 1, y + 2);
                    AddMove(x, y, x - 1, y + 2, depth);
                }
                if (IntMap[x - 1, y + 2] >= 7 && side || (IntMap[x - 1, y + 2] <= 6 && IntMap[x - 1, y + 2] != 0 && !side))
                {
                    OnAndFillAttackButtons(x - 1, y + 2);
                    AddMove(x, y, x - 1, y + 2, depth);
                }
            }
            //Влево-вверх
            if (x + 1 < 8 && y - 2 > -1)
            {
                if (IntMap[x + 1, y - 2] == 0)
                {
                    OnAndFillButtons(x + 1, y - 2);
                    AddMove(x, y, x + 1, y - 2, depth);
                }
                if (IntMap[x + 1, y - 2] >= 7 && side || (IntMap[x + 1, y - 2] <= 6 && IntMap[x + 1, y - 2] != 0 && !side))
                {
                    OnAndFillAttackButtons(x + 1, y - 2);
                    AddMove(x, y, x + 1, y - 2, depth);
                }
            }
            //Влево-вниз
            if (x - 1 > -1 && y - 2 > -1)
            {
                if (IntMap[x - 1, y - 2] == 0)
                {
                    OnAndFillButtons(x - 1, y - 2);
                    AddMove(x, y, x - 1, y - 2, depth);
                }
                if (IntMap[x - 1, y - 2] >= 7 && side || (IntMap[x - 1, y - 2] <= 6 && IntMap[x - 1, y - 2] != 0 && !side))
                {
                    OnAndFillAttackButtons(x - 1, y - 2);
                    AddMove(x, y, x - 1, y - 2, depth);
                }
            }
        }

        private void BishopRules(int x, int y, bool side, int depth)
        {
            //Вверх-вправо
            int i = x + 1;
            int j = y + 1;
            while (i <= 7 && j <= 7)
            {
                if (IntMap[i, j] == 0)
                {
                    OnAndFillButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    i++;
                    j++;
                }
                else if (IntMap[i, j] >= 7 && side || (IntMap[i, j] <= 6 && IntMap[i, j] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    break;
                }
                else { break; }
            }
            //Вверх-влево
            i = x + 1;
            j = y - 1;
            while (i <= 7 && j >= 0)
            {
                if (IntMap[i, j] == 0)
                {
                    OnAndFillButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    i++;
                    j--;
                }
                else if (IntMap[i, j] >= 7 && side || (IntMap[i, j] <= 6 && IntMap[i, j] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    break;
                }
                else { break; }
            }
            //Вниз-вправо
            i = x - 1;
            j = y + 1;
            while (i >= 0 && j <= 7)
            {
                if (IntMap[i, j] == 0)
                {
                    OnAndFillButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    i--;
                    j++;
                }
                else if (IntMap[i, j] >= 7 && side || (IntMap[i, j] <= 6 && IntMap[i, j] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    break;
                }
                else { break; }
            }
            //Вниз-влево
            i = x - 1;
            j = y - 1;
            while (i >= 0 && j >= 0)
            {
                if (IntMap[i, j] == 0)
                {
                    OnAndFillButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    i--;
                    j--;
                }
                else if (IntMap[i, j] >= 7 && side || (IntMap[i, j] <= 6 && IntMap[i, j] != 0 && !side))
                {
                    OnAndFillAttackButtons(i, j);
                    AddMove(x, y, i, j, depth);
                    break;
                }
                else { break; }
            }
        }

        private void QueenRules(int x, int y, bool side, int depth)
        {
            RookRules(x, y, side, depth);
            BishopRules(x, y, side, depth);
        }

        private void KingRules(int x, int y, bool side, int depth)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i >= 0 && i <= 7 && j >= 0 && j <= 7)
                    {
                        if (IntMap[i, j] == 0)
                        {
                            OnAndFillButtons(i, j);
                            AddMove(x, y, i, j, depth);
                        }
                        if (IntMap[i, j] >= 7 && side || (IntMap[i, j] <= 6 && IntMap[i, j] != 0 && !side))
                        {
                            OnAndFillAttackButtons(i, j);
                            AddMove(x, y, i, j, depth);
                        }
                    }
                }
            }


        }

        private void SearchAllMoves(bool isAPlayer, int depth)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (isAPlayer)
                    {
                        if (IntMap[i, j] <= 6 && IntMap[i, j] > 0)
                        {
                            CheckRules(i, j, 0, false, depth);
                        }
                    }
                    else
                    {
                        if (IntMap[i, j] >= 7)
                        {
                            CheckRules(i, j, 0, false, depth);
                        }
                    }
                }
            }
        }

        private Tuple<int, int, int, int, int> Minimax(int depth, bool isAPlayer, int alpha, int beta)
        {
            cnt++;
            if (depth == 0)
                return Tuple.Create(0,0,0,0, RateBoard());
            movies.TryGetValue(depth, out List<Move> moves);
            SearchAllMoves(isAPlayer, depth);
            if (isAPlayer)
            {
                int x1max = 0, y1max = 0, x2max = 0, y2max = 0, maxEval = int.MinValue;
                foreach (var move in moves)
                {
                    int IntBuff1 = IntMap[move.X1, move.Y1];
                    int IntBuff2 = IntMap[move.X2, move.Y2];
                    IntMap[move.X2, move.Y2] = IntMap[move.X1, move.Y1];
                    IntMap[move.X1, move.Y1] = 0;
                    if (movies.TryGetValue(depth - 1, out List<Move> nextLevelMove))
                        nextLevelMove.Clear();
                    maxEval = Math.Max(maxEval, Minimax(depth - 1, !isAPlayer, alpha, beta).Item5);
                    if (maxEval > beta)
                    {
                        break;
                        if (depth == 4)
                        {
                            x1max = move.X1; y1max = move.Y1;
                            x2max = move.X2; y2max = move.Y2;
                        }
                        
                    }
                    alpha = Math.Max(alpha, maxEval);
                    IntMap[move.X1, move.Y1] = IntBuff1;
                    IntMap[move.X2, move.Y2] = IntBuff2;
                }
                return Tuple.Create(x1max, y1max, x2max, y2max, maxEval);
            }
            else
            {
                int x1max = 0, y1max = 0, x2max = 0, y2max = 0, minEval = int.MaxValue;
                foreach (var move in moves)
                {
                    int IntBuff1 = IntMap[move.X1, move.Y1];
                    int IntBuff2 = IntMap[move.X2, move.Y2];
                    IntMap[move.X2, move.Y2] = IntMap[move.X1, move.Y1];
                    IntMap[move.X1, move.Y1] = 0;
                    if (movies.TryGetValue(depth - 1, out List<Move> nextLevelMove))
                        nextLevelMove.Clear();
                    minEval = Math.Min(minEval, Minimax(depth - 1, !isAPlayer, alpha, beta).Item5);
                    if (minEval < alpha)
                    {
                        break;
                        if (depth == 4)
                        {
                            x1max = move.X1; y1max = move.Y1;
                            x2max = move.X2; y2max = move.Y2;
                        }
                        break;
                    }
                    beta = Math.Min(beta, minEval);
                    IntMap[move.X1, move.Y1] = IntBuff1;
                    IntMap[move.X2, move.Y2] = IntBuff2;
                }
                return Tuple.Create(x1max, y1max, x2max, y2max, minEval);
            }
        }

        // Оценка доски
        private int RateBoard()
        {
            var map = IntMap;
            int BoardRate = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (boardRules.TryGetValue(map[i, j], out int val))
                        BoardRate += val;
                }
            }
            return BoardRate;
        }

        private void AddMove(int x1, int y1, int x2, int y2, int depth)
        {
            Move move = new Move
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };

            if(movies.TryGetValue(depth, out List<Move> movs))
                movs.Add(move);
        }

        private class Move
        {
            public int X1 { get; set; }
            public int Y1 { get; set; }
            public int X2 { get; set; }
            public int Y2 { get; set; }
        }

        private T GetChildren<T>(Grid grid, int row, int column) where T : UIElement
        {
            return grid.Children
                .Cast<UIElement>().Where(child => child is T && Grid.GetRow(child) == row && Grid.GetColumn(child) == column)
                .Cast<T>()
                .FirstOrDefault();
        }

        private int[,] getClearArray()
        {
            return new int[8, 8]
            {
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  },
                { 0,  0,  0,  0,  0,  0,  0,  0  }
            };
        }

        // На будующее
        private double[,] pawnEvalWhite =
        {
            { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
            { 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0},
            {1.0, 1.0, 2.0, 3.0, 3.0, 2.0, 1.0, 1.0},
            { 0.5, 0.5, 1.0, 2.5, 2.5, 1.0, 0.5, 0.5},
            { 0.0, 0.0, 0.0, 2.0, 2.0, 0.0, 0.0, 0.0},
            { 0.5, -0.5, -1.0, 0.0, 0.0, -1.0, -0.5, 0.5},
            { 0.5, 1.0, 1.0, -2.0, -2.0, 1.0, 1.0, 0.5},
            { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
        };

        //private double[,] pawnEvalBlack = reverseArray(pawnEvalWhite);

        private double[,] knightEval ={
            {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0},
            {-4.0, -2.0, 0.0, 0.0, 0.0, 0.0, -2.0, -4.0},
            {-3.0, 0.0, 1.0, 1.5, 1.5, 1.0, 0.0, -3.0},
            {-3.0, 0.5, 1.5, 2.0, 2.0, 1.5, 0.5, -3.0},
            {-3.0, 0.0, 1.5, 2.0, 2.0, 1.5, 0.0, -3.0},
            {-3.0, 0.5, 1.0, 1.5, 1.5, 1.0, 0.5, -3.0},
            {-4.0, -2.0, 0.0, 0.5, 0.5, 0.0, -2.0, -4.0},
            {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 }
        };

        private double[,] bishopEvalWhite = {
            {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0},
            {-1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 1.0, 1.0, 0.5, 0.0, -1.0},
            {-1.0, 0.5, 0.5, 1.0, 1.0, 0.5, 0.5, -1.0},
            {-1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, -1.0},
            {-1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0},
            {-1.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, -1.0},
            {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0}
        };

        //private double[,] bishopEvalBlack = reverseArray(bishopEvalWhite);

        private double[,] rookEvalWhite = {
            {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0},
            {0.5, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {0.0, 0.0, 0.0, 0.5, 0.5, 0.0, 0.0, 0.0}
        };

        //private double[,] rookEvalBlack = reverseArray(rookEvalWhite);

        private double[,] evalQueen = {
            {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0},
            {-1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0},
            {-0.5, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5},
            {0.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5},
            {-1.0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0}
        };

        private double[,] kingEvalWhite = {
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0},
            {-1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0},
            {2.0, 2.0, 0.0, 0.0, 0.0, 0.0, 2.0, 2.0},
            {2.0, 3.0, 1.0, 0.0, 0.0, 1.0, 3.0, 2.0}
        };

        // Дальше какя-то логика с кнопками
        private void OnAndFillAttackButtons(int x, int y)
        {
            List<Button> listButtons = new List<Button>();
            GetLogicalChildCollection(this, listButtons);
            listButtons[ButtonNumMap[x, y]].IsEnabled = true;
            listButtons[ButtonNumMap[x, y]].Opacity = 0.5;
            listButtons[ButtonNumMap[x, y]].Background = new SolidColorBrush(Colors.Red);
        }

        private void OnAndFillButtons(int x, int y)
        {
            List<Button> listButtons = new List<Button>();
            GetLogicalChildCollection(this, listButtons);
            listButtons[64].IsEnabled = false;
            listButtons[ButtonNumMap[x, y]].IsEnabled = true;
            listButtons[ButtonNumMap[x, y]].Opacity = 0.5;
            listButtons[ButtonNumMap[x, y]].Background = new SolidColorBrush(Colors.Green);
        }

        private void OnOffButtons(bool flag, int num)
        {
            List<Button> listButtons = new List<Button>();
            GetLogicalChildCollection(this, listButtons);
            for (int i = 0; i < 64; i++)
            {
                listButtons[i].IsEnabled = flag;
                listButtons[i].Opacity = 0;
                if (flag == false)
                {
                    listButtons[num].IsEnabled = true;
                    listButtons[num].Opacity = 0.5;
                    listButtons[num].Background = new SolidColorBrush(Colors.Orange);
                }
            }
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }
    }

}
