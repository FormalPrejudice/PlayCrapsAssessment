using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayCrapsAssessment
{
    public partial class Form1 : Form
    {
        static bool isFirstRoll;
        static bool isFirstRound;
        static bool isGameOver;
        static int pointSet = 0;
        static int RoundId = 0;
        static int GameId = 0;
        static int CurrentRoundId = 0;
        static List<int> Rolls = new List<int>();
        static int SelectedPlayerId = 9999999;

        public Form1()
        {
            InitializeComponent();
            PopulatePlayers();
            rollButton.Enabled = false;
        }

        public void PopulatePlayers()
        {
            using (var _context = new CrapsContext())
            {
                var PlayerNames = _context.Player.Select(x => new ComboBoxItem()
                {
                    Text = x.Name,
                    Value = x.PlayerId
                }).ToList();

                //Adding one here to handle the default value without outOfRanging in the for loop
                ComboBoxItem[] _dataSource = new ComboBoxItem[PlayerNames.Count() + 1];

                _dataSource[0] = new ComboBoxItem { Text = "Please select a player.", Value = 9999999 };

                for (var i = 1; i <= PlayerNames.Count(); i++)
                {
                    _dataSource[i] = PlayerNames[i - 1];
                }

                accountDropDown.DisplayMember = "Text";
                accountDropDown.ValueMember = "Value";
                accountDropDown.DataSource = _dataSource;
                accountDropDown.SelectedIndex = 0;
            }
        }

        private void AccountDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(accountDropDown.SelectedIndex == 0)
            {
                // We don't want to trigger the buttons to enable off of the default "Please select" option
                return;
            }                        

            if(SelectedPlayerId != 9999999)
            {
                // We already have a player selected. We shouldn't let them change players at this point unless they start a new game.
                // Prompt them?
                EndGame("Game over! Changing profiles.");
                SelectedPlayerId = (int)accountDropDown.SelectedValue;
                EnableButtons();
                return;
            }

            SelectedPlayerId = (int)accountDropDown.SelectedValue;
            NewGame();
            EnableButtons();
            rollButton.Enabled = true;            
        }

        private void EnableButtons()
        {
            //Edit button
            editButton.Enabled = true;
            //Delete button
            deleteButton.Enabled = true;
        }


        public void NewGame()
        {
            isFirstRoll = true;
            isFirstRound = true;
            isGameOver = false;
            rollButton.Text = "Roll";
            label1.Text = "";
            label2.Text = "";
            pointSet = 0;            

            Game newGame = new Game
            {
                PlayerId = SelectedPlayerId,
            };
            using(var _context = new CrapsContext())
            {
                _context.Game.Add(newGame);
                _context.SaveChanges();
                GameId = newGame.GameId;
            }
        }

        public void EndGame(string EndGameMessage)
        {
            isGameOver = true;
            rollButton.Text = "New Game";
            label2.Text = " ";
            label1.Text = EndGameMessage;
        }

        public void CreateRound(RoundOutcome outcome)
        {
            var round = new Round()
            {
                GameId = GameId,
                Outcome = outcome
            };

            using(var _context = new CrapsContext())
            {
                _context.Round.Add(round);
                _context.SaveChanges();
                CurrentRoundId = round.RoundId;
            }
        }

        public void UpdateRound(RoundOutcome outcome)
        {
            using(var _context = new CrapsContext())
            {

            }
        }

        public void SaveRound(RoundOutcome outcome)
        {
            var round = new Round()
            {
                RoundId = RoundId++,
                GameId = GameId,
                Outcome = outcome
            };

            using (var ctx = new CrapsContext())
            {
                ctx.Round.Add(round);
                ctx.SaveChanges();
            }
        }

        private void RollButton_Click(object sender, EventArgs e)
        {
            if (isGameOver)
            {
                NewGame();
            } else
            {
                RollDice();
            }
        }

        private RoundOutcome RollDice()
        {
            if(isFirstRound)
            {
                //CreateRound();
            }


            // Replace this with call to API.
            Random rand = new Random();
            int dice1 = rand.Next(1, 7);
            int dice2 = rand.Next(1, 7);
            int sum = dice1 + dice2;
            RoundOutcome outcome;
            if (isFirstRoll)
            {
                outcome = CheckFirstRoll(dice1,dice2);
                isFirstRoll = false;
            }
            else
            {
                outcome = CheckRoll(dice1,dice2);
            }

            if(outcome == RoundOutcome.WIN)
            {
                EndGame("You Win!");
            } else if (outcome == RoundOutcome.LOSE)
            {
                EndGame("You Lose!");
            } else if (outcome == RoundOutcome.POINT)
            {
                pointSet = sum;
                label1.Text = "Point set at: " + sum;
            }            

            label2.Text = "You rolled: " + sum;

            return outcome;
        }

        private RoundOutcome CheckFirstRoll(int dice1, int dice2)
        {
            int[] win_nums = { 7, 11 };
            int[] lose_nums = { 2, 3, 12 };
            int sum = dice1 + dice2;
            if (win_nums.Contains(sum)){
                return RoundOutcome.WIN;
            } else if (lose_nums.Contains(sum)) {
                return RoundOutcome.LOSE;
            } else
            {
                isFirstRound = false;
                CreateRound(RoundOutcome.POINT);
                return RoundOutcome.POINT;
            }
        }

        private RoundOutcome CheckRoll(int dice1, int dice2)
        {
            int sum = dice1 + dice2;
            if (sum == 7)
            {
                UpdateRound(RoundOutcome.LOSE);
                return RoundOutcome.LOSE;
            } else if (sum == pointSet)
            {
                UpdateRound(RoundOutcome.WIN);
                return RoundOutcome.WIN;
            } else
            {
                return RoundOutcome.ROLL_AGAIN;
            }
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    _context.Dispose();
        //}
    }
}
