using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Entities;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Player> GetAll()
        {
            List<Player> players = new List<Player>();
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                players = connection.Query<Player>("SELECT Id,Name,Score,IsStar FROM Players").ToList();
            }
            return players;
        }

        public Player GetById(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                var player = connection.QueryFirstOrDefault<Player>("SELECT * FROM Players WHERE Id=@PId",
                    new { PId = id });
                return player;
            }
        }

        public void Insert(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                connection.Execute(@"
INSERT INTO Players(Name,Score,IsStar)
VALUES(@PName,@PScore,@PIsStar)
", new
                {
                    PName = player.Name,
                    PScore = player.Score,
                    PIsStar = player.IsStar
                });
            }
            MessageBox.Show("New player added successfully");
        }

        public void Update(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                connection.Execute(@"
UPDATE Players
SET Name=@PName,Score=@PScore,IsStar=@PIsStar
WHERE Id=@Pid",
new { PName = player.Name, PScore = player.Score, PIsStar = player.IsStar, PId = player.Id });
            }
        }

        public void Delete(int id)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                connection.Execute("DELETE FROM Players WHERE Id=@PId", new { PId = id });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            //var player = GetById(1);
            //player.Name = "Rafiq";
            //player.Score = 10;

            //Update(player);

            //Insert(new Player { IsStar = true, Name = "AAA", Score = 88 });


            CallSP(0);
            //var players = GetAll();
            //myDataGrid.ItemsSource = players;
            ////var player = GetById(1);
            ////myDataGrid.ItemsSource = new List<Player> {player };

        }

        public void CallSP(float score)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection = new SqlConnection(conn))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PScore", score, DbType.Double);
                var data = connection.Query<Player>("ShowGreaterThan", parameters, commandType: CommandType.StoredProcedure);
                myDataGrid.ItemsSource = data;
            }
        }

        public bool HasAlreadyDeleted { get; set; }
        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var player = myDataGrid.SelectedItem as Player;
            if (!HasAlreadyDeleted)
            {
                var result = MessageBox.Show($"Are you sure to delete {player?.Name}", "Info", MessageBoxButton.YesNo);
                HasAlreadyDeleted = true;
                if (result == MessageBoxResult.Yes)
                {
                    Delete(player.Id);
                    var players = GetAll();
                    myDataGrid.ItemsSource = players;
                    HasAlreadyDeleted = false;
                }
                else
                {
                    Name.Text = player.Name;
                    Score.Value = player.Score;
                    ISStar.IsChecked = player.IsStar;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var player = myDataGrid.SelectedItem as Player;
            var newPlayer = new Player
            {
                Id = player.Id,
                Name = Name.Text,
                IsStar = ISStar.IsChecked.Value,
                Score = Score.Value,

            };

            Update(newPlayer);
            HasAlreadyDeleted = false;

            var players = GetAll();
            myDataGrid.ItemsSource = players;
        }
    }
}
