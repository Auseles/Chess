using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    /// <summary>
    /// Логика взаимодействия для ChessWindow.xaml
    /// </summary>
    public partial class ChessWindow : Window
    {
        private int[,] IntMap;
        private int[,] MoveMap;
        private int[,] ButtonNumMap;
        private string[,] StrMap;
        private string[,] PlayerEatArray;
        private string[,] BotEatArray;
        private double turn = 0;
        private int ClickCntFlag = 0;
        private bool ChoosingSide = true; //false - чёрные  true - белые
        private int LastMove1;
        private int LastMove2;
        private int WhiteKingMoveCnt = 0;
        private int BlackKingMoveCnt = 0;
        private int WhiteLeftRookMoveCnt = 0;
        private int WhiteRightRookMoveCnt = 0;
        private int BlackLeftRookMoveCnt = 0;
        private int BlackRightRookMoveCnt = 0;
        private int Complexity = 1;
        private bool ComplexityFlag = false;
        private bool SoundFlag = true;
        private int NoEatMoveCnt = 0;
        private int NoPawnMoveCnt = 0;
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

        public ChessWindow()
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
            var button = (UIElement)sender;
            var col = Grid.GetColumn(button) - 1;
            var row = 8 - Grid.GetRow(button);
            if (IntMap[row, col] != 0 && IntMap[row, col] < 7 || ClickCntFlag == 1)
            {
                ClickCntFlag += 1;
                CheckMove(row, col, ButtonNumMap[row, col], 0);
            }
        }

        private void SetStartingLocation()
        {
            ComplexityFlag = false;
            turn = 0;
            MoveMap = GetClearArray();
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
            PlayerEatArray = new string[2, 8]
            {
                {"","","","","","","","" },
                {"","","","","","","","" }
            };
            BotEatArray = new string[2, 8]
            {
                {"","","","","","","","" },
                {"","","","","","","","" }
            };
            WhiteKingMoveCnt = 0;
            WhiteLeftRookMoveCnt = 0;
            WhiteRightRookMoveCnt = 0;
            BlackKingMoveCnt = 0;
            BlackLeftRookMoveCnt = 0;
            BlackRightRookMoveCnt = 0;
            OnOffButtons(true, 0);
        }

        private void SetComplexity()
        {
            if (ComplexityComboBox.SelectedIndex == 0)
                Complexity = 0;
            if (ComplexityComboBox.SelectedIndex == 1)
                Complexity = 1;
            if (ComplexityComboBox.SelectedIndex == 2)
                Complexity = 2;
        }
        
        private void ChengingSide_Click(object sender, RoutedEventArgs e)
        {
            ChoosingSide = !ChoosingSide;
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string strbuff = StrMap[i, j];
                    StrMap[i, j] = StrMap[i + 7, j];
                    StrMap[i + 7, j] = strbuff;
                    strbuff = StrMap[i + 1, j];
                    StrMap[i + 1, j] = StrMap[i + 6, j];
                    StrMap[i + 6, j] = strbuff;
                }
            }
            RedriwingFigure();
            turn++;
            CheckTurn();
            OnOffButtons(true, 0);
        }
        
        private void RedriwingFigure()
        {
            //Перерисовка положения фигур и взятых фигур
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var row = 7 - i;
                    GetChildren<Image>(MainGrid, i + 1, j + 1).Source = new BitmapImage(new Uri(StrMap[row, j], UriKind.Relative));
                    if (i < 2)
                    {
                        GetChildren<Image>(PlayerGrid, i, j).Source = new BitmapImage(new Uri(PlayerEatArray[i, j], UriKind.Relative));
                        GetChildren<Image>(BotGrid, i, j).Source = new BitmapImage(new Uri(BotEatArray[i, j], UriKind.Relative));
                    }
                }
            }
        }
        
        private void EatAdd(string figure, bool isAPlayer)
        {
            NoEatMoveCnt = 0;
            bool flag = false;
            var map = PlayerEatArray;
            if (!isAPlayer)
                map = BotEatArray;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map[i, j] == "")
                    {
                        map[i, j] = figure;
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    break;
            }
        }
        
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите перезапустить игру?", "Перезапуск игры", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                List<Button> listButtons = new List<Button>();
                GetLogicalChildCollection(this, listButtons);
                //кнопка смены стороны
                listButtons[65].IsEnabled = true;
                Restart.Visibility = Visibility.Hidden;
                ComplexityComboBox.IsEnabled = true;
                SetStartingLocation();
                RedriwingFigure();
            }
        }
        
        private void CheckMove(int x, int y, int num, int depth)
        {
            if (ClickCntFlag == 1)
            {
                MoveMap[x, y] = 1;
                CheckRules(x, y, num, depth);
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
                    MoveMap = GetClearArray();
                    ClickCntFlag = 0;
                    OnOffButtons(true, 65);
                }
                if (ClickCntFlag == 2)
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
            //Выяснение ходили ли ладьи игрока
            if (x1 == 0 && y1 == 0 && IntMap[x1, y1] == 5)
                WhiteLeftRookMoveCnt++;
            if (x1 == 0 && y1 == 7 && IntMap[x1, y1] == 5)
                WhiteRightRookMoveCnt++;
            string ImgBuf = StrMap[x1, y1];
            int IntBuff = IntMap[x1, y1];
            StrMap[x1, y1] = "";
            IntMap[x1, y1] = 0;
            if (StrMap[x2, y2] != "")
                EatAdd(StrMap[x2, y2], true);
            else
                NoEatMoveCnt++;
            StrMap[x2, y2] = ImgBuf;
            IntMap[x2, y2] = IntBuff;
            if (IntMap[x2, y2] == 6)
                NoPawnMoveCnt = 0;
            else
                NoPawnMoveCnt++;
            //Превращение пешки игрока
            if (IntMap[x2, y2] == 6 && x2 == 7)
            {
                IntMap[x2, y2] = 2;
                if (ChoosingSide)
                    StrMap[x2, y2] = "Images/WhiteQueen.png";
                else
                    StrMap[x2, y2] = "Images/BlackQueen.png";
            }
            //Выяснение ходил ли король игрока
            if (IntMap[x2, y2] == 1)
            {
                WhiteKingMoveCnt++;
                //Короткая рокировка
                if (x1 == 0 && y1 == 4 && x2 == 0 && y2 == 6)
                {
                    WhiteRightRookMoveCnt++;
                    ImgBuf = StrMap[0, 7];
                    IntBuff = IntMap[0, 7];
                    StrMap[0, 7] = "";
                    IntMap[0, 7] = 0;
                    StrMap[0, 5] = ImgBuf;
                    IntMap[0, 5] = IntBuff;
                }
                //Длинная рокировка
                if (x1 == 0 && y1 == 4 && x2 == 0 && y2 == 2)
                {
                    WhiteLeftRookMoveCnt++;
                    ImgBuf = StrMap[0, 0];
                    IntBuff = IntMap[0, 0];
                    StrMap[0, 0] = "";
                    IntMap[0, 0] = 0;
                    StrMap[0, 3] = ImgBuf;
                    IntMap[0, 3] = IntBuff;
                }
            }
            RedriwingFigure();
            ClickCntFlag = 0;
            MoveMap = GetClearArray();
            CheckTurn();
            OnOffButtons(true, 0);
        }
        
        private void CheckTurn()
        {
            if (turn % 2 == 0)
            {
                turn++;
                CheckTurn();
                CheckEndGame();
                MuteButton.IsEnabled = true;
            }
            else
            {
                turn++;
                foreach (var item in movies.Values)
                {
                    item.Clear();
                }
                int depth = 0;
                if (Complexity == 0)
                    depth = 1;
                if (Complexity == 1)
                    depth = 2;
                if (Complexity == 2)
                    depth = 3;
                var MaxRate = FirstStepMiniMax(depth, false);
                //Добавление съеденых фигур в список
                if (StrMap[MaxRate.Item3, MaxRate.Item4] != "")
                    EatAdd(StrMap[MaxRate.Item3, MaxRate.Item4], false);
                else
                    NoEatMoveCnt++;
                if (IntMap[MaxRate.Item1, MaxRate.Item2] == 12)
                    NoPawnMoveCnt = 0;
                else
                    NoPawnMoveCnt++;
                //Ходил ли король ИИ
                if (IntMap[MaxRate.Item1, MaxRate.Item2] == 7)
                    BlackKingMoveCnt++;
                //Ходили ли ладьи ИИ
                if (MaxRate.Item1 == 7 && MaxRate.Item2 == 0 && IntMap[MaxRate.Item1, MaxRate.Item2] == 11)
                    BlackLeftRookMoveCnt++;
                if (MaxRate.Item1 == 7 && MaxRate.Item2 == 7 && IntMap[MaxRate.Item1, MaxRate.Item2] == 11)
                    BlackRightRookMoveCnt++;
                //Короткая рокировка
                if (MaxRate.Item1 == 7 && MaxRate.Item2 == 4 && MaxRate.Item3 == 7 && MaxRate.Item4 == 6)
                {
                    BlackRightRookMoveCnt++;
                    int intbuff = IntMap[7, 7];
                    string strbuff = StrMap[7, 7];
                    IntMap[7, 7] = 0;
                    StrMap[7, 7] = "";
                    IntMap[7, 5] = intbuff;
                    StrMap[7, 5] = strbuff;
                }
                //Длинная рокировка
                if (MaxRate.Item1 == 7 && MaxRate.Item2 == 4 && MaxRate.Item3 == 7 && MaxRate.Item4 == 2)
                {
                    BlackLeftRookMoveCnt++;
                    int intbuff = IntMap[7, 0];
                    string strbuff = StrMap[7, 0];
                    IntMap[7, 0] = 0;
                    StrMap[7, 0] = "";
                    IntMap[7, 3] = intbuff;
                    StrMap[7, 3] = strbuff;
                }
                IntMap[MaxRate.Item3, MaxRate.Item4] = IntMap[MaxRate.Item1, MaxRate.Item2];
                StrMap[MaxRate.Item3, MaxRate.Item4] = StrMap[MaxRate.Item1, MaxRate.Item2];
                IntMap[MaxRate.Item1, MaxRate.Item2] = 0;
                StrMap[MaxRate.Item1, MaxRate.Item2] = "";
                //Превращение пешки ИИ
                if (IntMap[MaxRate.Item3, MaxRate.Item4] == 12 && MaxRate.Item3 == 0)
                {
                    IntMap[MaxRate.Item3, MaxRate.Item4] = 8;
                    if (!ChoosingSide)
                        StrMap[MaxRate.Item3, MaxRate.Item4] = "Images/WhiteQueen.png";
                    else
                        StrMap[MaxRate.Item3, MaxRate.Item4] = "Images/BlackQueen.png";
                }
                List<Button> listButtons = new List<Button>();
                GetLogicalChildCollection(this, listButtons);
                LastMove1 = ButtonNumMap[MaxRate.Item1, MaxRate.Item2];
                LastMove2 = ButtonNumMap[MaxRate.Item3, MaxRate.Item4];
                RedriwingFigure();
                CheckEndGame();
                if (SoundFlag)
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.IO.Stream res = assembly.GetManifestResourceStream(@"Chess.1.wav");
                    SoundPlayer simpleSound = new SoundPlayer(res);
                    simpleSound.Play();
                }
                MuteButton.IsEnabled = true;
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
            if (NoPawnMoveCnt >= 50 && NoEatMoveCnt >= 50)
            {
                MessageBox.Show("Ничья");
                End = true;
            }
            if (End == true)
            {
                List<Button> listButtons = new List<Button>();
                GetLogicalChildCollection(this, listButtons);
                listButtons[65].IsEnabled = true;
                ComplexityComboBox.IsEnabled = true;
                SetStartingLocation();
                RedriwingFigure();
            }
        }
        
        private void CheckRules(int x, int y, int num, int depth)
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
            int mod2 = 0;
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
            {
                mod2 = mod;
                mod++;

            }
            else
            {
                mod2 = mod;
                mod--;
            }
            //Первых ход пешки
            if (((x == 1 && mod > 0) || (x == 6 && mod < 0)) && IntMap[x + mod, y] == 0)
            {
                if (IntMap[x + mod, y] == 0 && IntMap[x + mod2, y] == 0)
                {
                    OnAndFillButtons(x + mod, y);
                    AddMove(x, y, x + mod, y, depth);
                }
            }
        }
        
        private void RookRules(int x, int y, bool side, int depth)
        {
            //Вверх
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
            //Рокировка игрока
            if (WhiteKingMoveCnt == 0 && side)
            {
                if (WhiteRightRookMoveCnt == 0)
                {
                    bool flag = true;
                    for (int i = y + 1; i < 8; i++)
                    {
                        if (IntMap[x, i] != 0 && IntMap[x, i] != 5)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        OnAndFillButtons(x, y + 2);
                        AddMove(x, y, x, y + 2, depth);
                    }
                }
                if (WhiteLeftRookMoveCnt == 0)
                {
                    bool flag = true;
                    for (int i = y - 1; i > -1; i--)
                    {
                        if (IntMap[x, i] != 0 && IntMap[x, i] != 5)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        OnAndFillButtons(x, y - 2);
                        AddMove(x, y, x, y - 2, depth);
                    }
                }
            }
            if (BlackKingMoveCnt == 0 && !side)
            {
                if (BlackRightRookMoveCnt == 0)
                {
                    bool flag = true;
                    for (int i = y + 1; i < 8; i++)
                    {
                        if (IntMap[x, i] != 0 && IntMap[x, i] != 11)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        OnAndFillButtons(x, y + 2);
                        AddMove(x, y, x, y + 2, depth);
                    }
                }
                if (BlackLeftRookMoveCnt == 0)
                {
                    bool flag = true;
                    for (int i = y - 1; i > -1; i--)
                    {
                        if (IntMap[x, i] != 0 && IntMap[x, i] != 11)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        OnAndFillButtons(x, y - 2);
                        AddMove(x, y, x, y - 2, depth);
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
                            CheckRules(i, j, 0, depth);
                        }
                    }
                    else
                    {
                        if (IntMap[i, j] >= 7)
                        {
                            CheckRules(i, j, 0, depth);
                        }
                    }
                }
            }
        }
        
        private Tuple<int, int, int, int> FirstStepMiniMax(int depth, bool isAPlayer)
        {
            movies.TryGetValue(depth, out List<Move> moves);
            SearchAllMoves(isAPlayer, depth);
            int x1max = 0, y1max = 0, x2max = 0, y2max = 0;
            double maxEval = int.MinValue;
            foreach (var move in moves)
            {
                int IntBuff1 = IntMap[move.X1, move.Y1];
                int IntBuff2 = IntMap[move.X2, move.Y2];
                IntMap[move.X2, move.Y2] = IntMap[move.X1, move.Y1];
                IntMap[move.X1, move.Y1] = 0;
                double value = Minimax(depth - 1, !isAPlayer, int.MinValue, int.MaxValue);
                if (value >= maxEval)
                {
                    maxEval = value;
                    x1max = move.X1;
                    y1max = move.Y1;
                    x2max = move.X2;
                    y2max = move.Y2;
                }
                IntMap[move.X1, move.Y1] = IntBuff1;
                IntMap[move.X2, move.Y2] = IntBuff2;
            }
            return Tuple.Create(x1max, y1max, x2max, y2max);
        }

        private double Minimax(int depth, bool isAPlayer, double alpha, double beta)
        {
            if (depth == 0)
                return RateBoard();
            movies.TryGetValue(depth, out List<Move> moves);
            SearchAllMoves(isAPlayer, depth);
            if (!isAPlayer)
            {
                double maxEval = double.MinValue;
                foreach (var move in moves)
                {
                    int IntBuff1 = IntMap[move.X1, move.Y1];
                    int IntBuff2 = IntMap[move.X2, move.Y2];
                    IntMap[move.X2, move.Y2] = IntMap[move.X1, move.Y1];
                    IntMap[move.X1, move.Y1] = 0;
                    if (movies.TryGetValue(depth - 1, out List<Move> nextLevelMove))
                        nextLevelMove.Clear();
                    maxEval = Math.Max(maxEval, Minimax(depth - 1, !isAPlayer, alpha, beta));
                    IntMap[move.X1, move.Y1] = IntBuff1;
                    IntMap[move.X2, move.Y2] = IntBuff2;
                    alpha = Math.Max(alpha, maxEval);
                    if (beta <= alpha)
                    {
                        return maxEval;
                    }
                }
                return maxEval;
            }
            else
            {
                double minEval = int.MaxValue;
                foreach (var move in moves)
                {
                    int IntBuff1 = IntMap[move.X1, move.Y1];
                    int IntBuff2 = IntMap[move.X2, move.Y2];
                    IntMap[move.X2, move.Y2] = IntMap[move.X1, move.Y1];
                    IntMap[move.X1, move.Y1] = 0;
                    if (movies.TryGetValue(depth - 1, out List<Move> nextLevelMove))
                        nextLevelMove.Clear();
                    minEval = Math.Min(minEval, Minimax(depth - 1, !isAPlayer, alpha, beta));
                    IntMap[move.X1, move.Y1] = IntBuff1;
                    IntMap[move.X2, move.Y2] = IntBuff2;
                    beta = Math.Min(beta, minEval);
                    if (beta <= alpha)
                    {
                        return minEval;
                    }
                }
                return minEval;
            }
        }

        private double RateBoard()
        {
            var map = IntMap;
            double BoardRate = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    double coef = 0;
                    if (map[i, j] == 1)
                        coef = kingEvalWhite[i, j];
                    if (map[i, j] == 2)
                        coef = evalQueen[i, j];
                    if (map[i, j] == 3)
                        coef = bishopEvalWhite[i, j];
                    if (map[i, j] == 4)
                        coef = knightEval[i, j];
                    if (map[i, j] == 5)
                        coef = rookEvalWhite[i, j];
                    if (map[i, j] == 6)
                        coef = pawnEvalWhite[i, j];
                    if (map[i, j] == 7)
                        coef = kingEvalBlack[i, j];
                    if (map[i, j] == 8)
                        coef = evalQueen[i, j];
                    if (map[i, j] == 9)
                        coef = bishopEvalBlack[i, j];
                    if (map[i, j] == 10)
                        coef = knightEval[i, j];
                    if (map[i, j] == 11)
                        coef = rookEvalBlack[i, j];
                    if (map[i, j] == 12)
                        coef = pawnEvalBlack[i, j];
                    if (boardRules.TryGetValue(map[i, j], out int val))
                        BoardRate += val + coef;
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

            if (movies.TryGetValue(depth, out List<Move> movs))
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

        private int[,] GetClearArray()
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
        
        private static double[,] ReverseArray(double[,] Array)
        {
            double[,] doubles = new double[8, 8];
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    doubles[i, j] = -Array[i, j];
                }
            }
            return doubles;
        }
        
        //Массивы оценок расположения фигур
        private static double[,] pawnEvalWhite =
        {
            { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
            { 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5.0, 5},
            {1.0, 1.0, 2.0, 3.0, 3.0, 2.0, 1.0, 1.0},
            { 0.5, 0.5, 1.0, 2.5, 2.5, 1.0, 0.5, 0.5},
            { 0.0, 0.0, 0.0, 2.0, 2.0, 0.0, 0.0, 0.0},
            { 0.5, -0.5, -1.0, 0.0, 0.0, -1.0, -0.5, 0.5},
            { 0.5, 1.0, 1.0, -2.0, -2.0, 1.0, 1.0, 0.5},
            { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}
        };

        private static double[,] pawnEvalBlack = ReverseArray(pawnEvalWhite);

        private static double[,] knightEval ={
            {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0},
            {-4.0, -2.0,  0.0,  0.0,  0.0,  0.0, -2.0, -4.0},
            {-3.0,  0.0,  1.0,  1.5,  1.5,  1.0,  0.0, -3.0},
            {-3.0,  0.5,  1.5,  2.0,  2.0,  1.5,  0.5, -3.0},
            {-3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0},
            {-3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0},
            {-4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0},
            {-5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0 }
        };

        private static double[,] bishopEvalWhite = {
            {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0},
            {-1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 1.0, 1.0, 0.5, 0.0, -1.0},
            {-1.0, 0.5, 0.5, 1.0, 1.0, 0.5, 0.5, -1.0},
            {-1.0, 0.0, 1.0, 1.0, 1.0, 1.0, 0.0, -1.0},
            {-1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, -1.0},
            {-1.0, 0.5, 0.0, 0.0, 0.0, 0.0, 0.5, -1.0},
            {-2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0}
        };

        private static double[,] bishopEvalBlack = ReverseArray(bishopEvalWhite);

        private static double[,] rookEvalWhite = {
            {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0},
            {0.5, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {-0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5},
            {0.0, 0.0, 0.0, 0.5, 0.5, 0.0, 0.0, 0.0}
        };

        private static double[,] rookEvalBlack = ReverseArray(rookEvalWhite);

        private static double[,] evalQueen = {
            {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0},
            {-1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0},
            {-0.5, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5},
            {0.0, 0.0, 0.5, 0.5, 0.5, 0.5, 0.0, -0.5},
            {-1.0, 0.5, 0.5, 0.5, 0.5, 0.5, 0.0, -1.0},
            {-1.0, 0.0, 0.5, 0.0, 0.0, 0.0, 0.0, -1.0},
            {-2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -2.0}
        };

        private static double[,] kingEvalWhite = {
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0},
            {-2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0},
            {-1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0},
            {2.0,  2.0,  0.0,  0.0,  0.0,  0.0,  2.0,  2.0},
            {2.0,  3.0,  1.0,  0.0,  0.0,  1.0,  3.0,  2.0}
        };

        private static double[,] kingEvalBlack = ReverseArray(kingEvalWhite);

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
            listButtons[65].IsEnabled = false;
            if (!ComplexityFlag)
            {
                SetComplexity();
                ComplexityFlag = true;
            }
            ComplexityComboBox.IsEnabled = false;
            Restart.Visibility = Visibility.Visible;
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
                if (turn > 1)
                {
                    listButtons[LastMove1].Opacity = 0.5;
                    listButtons[LastMove1].Background = new SolidColorBrush(Colors.DarkSlateGray);
                    listButtons[LastMove2].Opacity = 0.5;
                    listButtons[LastMove2].Background = new SolidColorBrush(Colors.DarkSlateGray);
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

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SoundFlag)
                SoundImage.Source = new BitmapImage(new Uri("Images/no-sound.png", UriKind.Relative));
            else
                SoundImage.Source = new BitmapImage(new Uri("Images/sounds.png", UriKind.Relative));
            SoundFlag = !SoundFlag;
        }
    }

}
