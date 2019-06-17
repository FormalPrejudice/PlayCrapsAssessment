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
        // ID's set to auto-increment
        static int pointSet = 0;
        static int RoundId = 0;
        static int GameId = 0;
        static int PlayerID = 0;
        //
        static int? CurrentRoundId = null;
        static List<Rolls> Rolls = new List<Rolls>();
        static int SelectedPlayerId = 9999999;

        public Form1()
        {
            InitializeComponent();
            PopulatePlayers();
            rollButton.Enabled = false;
            rollSum.Text = "";
            winCount.Text = "";
            lossCount.Text = "";
        }

        #region Player Population
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

            if (accountDropDown.SelectedIndex == 0)
            {
                return;
            }

            if (SelectedPlayerId != 9999999)
            {
                // We already have a player selected. We shouldn't let them change players at this point unless they start a new game.

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
        #endregion

        #region New Game/ End Game Handler
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
            using (var _context = new CrapsContext())
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
        #endregion

        #region Round Management
        public void CreateRound()
        {
            var round = new Round()
            {
                GameId = GameId
            };

            using (var _context = new CrapsContext())
            {
                _context.Round.Add(round);
                _context.SaveChanges();
                CurrentRoundId = round.RoundId;
            }
        }

        public void UpdateRound(RoundOutcome outcome)
        {
            using (var _context = new CrapsContext())
            {
                var currentRound = _context.Round.Where(d => d.RoundId == (int)CurrentRoundId).FirstOrDefault();
                currentRound.Outcome = outcome;
                _context.SaveChanges();                    
            }

            FinalizeRoundRolls();
            CurrentRoundId = null;
        }

        public void FinalizeRoundRolls()
        {
            using (var _context = new CrapsContext())
            {
                _context.Rolls.AddRange(Rolls);
                _context.SaveChanges();
                Rolls = new List<Rolls>();
            }            
        }

        private RoundOutcome CheckFirstRoll(int sum)
        {
            int[] win_nums = { 7, 11 };
            int[] lose_nums = { 2, 3, 12 };

            if (win_nums.Contains(sum))
            {
                return RoundOutcome.WIN;
            }
            else if (lose_nums.Contains(sum))
            {
                return RoundOutcome.LOSE;
            }
            else
            {
                isFirstRound = false;
                UpdateRound(RoundOutcome.POINT);
                CreateRound();
                return RoundOutcome.POINT;
            }
        }

        private RoundOutcome CheckRoll(int sum)
        {
            if (sum == 7)
            {
                UpdateRound(RoundOutcome.LOSE);
                return RoundOutcome.LOSE;
            }
            else if (sum == pointSet)
            {
                UpdateRound(RoundOutcome.WIN);
                return RoundOutcome.WIN;
            }
            else
            {
                return RoundOutcome.ROLL_AGAIN;
            }
        }
        #endregion

        #region Roll Handling
        private void RollButton_Click(object sender, EventArgs e)
        {
            if (isGameOver)
            {
                NewGame();
            }
            else
            {
                RollDice();
            }
        }

        private RoundOutcome RollDice()
        {
            if(isFirstRoll)
            {
                CreateRound();
            }

            Random rand = new Random();
            int dice1 = rand.Next(1, 7);
            int dice2 = rand.Next(1, 7);
            int sum = dice1 + dice2;           

            Rolls CurrentRoll = new Rolls()
            {
                RoundId = (int)CurrentRoundId,
                RollValue = sum                
            };

            Rolls.Add(CurrentRoll);

            RoundOutcome outcome;
            if (isFirstRoll)
            {
                outcome = CheckFirstRoll(sum);
                isFirstRoll = false;
            }
            else
            {
                outcome = CheckRoll(sum);
            }

            if (outcome == RoundOutcome.WIN)
            {
                EndGame("You Win!");
            }
            else if (outcome == RoundOutcome.LOSE)
            {
                EndGame("You Lose!");
            }
            else if (outcome == RoundOutcome.POINT)
            {
                pointSet = sum;
                label1.Text = "Point set at: " + sum;
            }

            label2.Text = "You rolled: " + sum;

            return outcome;
        }
        #endregion

        #region Button Click Events

        private void EnableButtons()
        {
            editButton.Enabled = true;
            deleteButton.Enabled = true;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            Player playerToAdd = new Player
            {
                Name = accountDropDown.Text,
                Password = "Password"
            };

            using (var _context = new CrapsContext())
            {
                _context.Player.Add(playerToAdd);
                _context.SaveChanges();
                PlayerID = playerToAdd.PlayerId;
                MessageBox.Show("Player Added");
                PopulatePlayers();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            using (var _context = new CrapsContext())
            {
                var playerToDelete = _context.Player.Where(x => x.PlayerId == SelectedPlayerId).FirstOrDefault();

                _context.Player.Remove(playerToDelete);
                _context.SaveChanges();
                MessageBox.Show("Player Removed");
                PopulatePlayers();
            }

        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            using (var _context = new CrapsContext())
            {
                var playerToEdit = _context.Player.Where(x => x.PlayerId == SelectedPlayerId).FirstOrDefault();

                playerToEdit.Name = accountDropDown.Text;
                _context.SaveChanges();
                MessageBox.Show("Player Edited");
                PopulatePlayers();
            }
        }

        #endregion


    }
}