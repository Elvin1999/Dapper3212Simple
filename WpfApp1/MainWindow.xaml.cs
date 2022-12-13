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


        public void Update(Player player)
        {
            var conn = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            using (var connection=new SqlConnection(conn))
            {
                connection.Execute(@"
UPDATE Players
SET Name=@PName,Score=@PScore,IsStar=@PIsStar
WHERE Id=@Pid",
new { PName = player.Name, PScore = player.Score, PIsStar = player.IsStar, PId = player.Id });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var player = GetById(1);
            player.Name = "Rafiq";
            player.Score = 10;

            Update(player);

            var players = GetAll();
            myDataGrid.ItemsSource = players;
            ////var player = GetById(1);
            ////myDataGrid.ItemsSource = new List<Player> {player };

        }
    }
}
