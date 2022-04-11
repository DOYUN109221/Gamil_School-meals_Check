using Discord;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gamil_SCH_Bot
{
    class Program
    {
        public static WebClient client = new WebClient();

        private readonly DiscordSocketClient _client;
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;
            _client.Ready += Ready;
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, "OTYzMDIxMDYxNzc4MjEwODM4.YlQBQQ.1GGGB3EZQxiGEsTwS0tr1A5kT30");
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task Ready()
        {
            Console.WriteLine($"{_client.CurrentUser} 연결됨!");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
            {
                return;
            }

            if (message.Content == "급식")
            {
                message.Channel.SendMessageAsync(ToDayKS(1));
                return;
            }
            else if (message.Content == "오늘급식")
            {
                message.Channel.SendMessageAsync(ToDayKS(2));
                return;
            }
        }
        public static string ToDayKS(int i)
        {
            client.Encoding = Encoding.UTF8;
            string[] timeDate = DateTime.Now.ToString("yyyy-MM-dd").Split('-');
            var a = JObject.Parse(client.DownloadString($"https://open.neis.go.kr/hub/mealServiceDietInfo?Type=json&KEY=ecabe857ea114a09a0db1163ae5fa947&ATPT_OFCDC_SC_CODE=J10&SD_SCHUL_CODE=7692183&MLSV_FROM_YMD=20220404&MLSV_TO_YMD={timeDate[0]}{timeDate[1]}{Convert.ToInt32(timeDate[2]) + 1}"));
            int list_total_count = Convert.ToInt32(a["mealServiceDietInfo"][0]["head"][0]["list_total_count"]);

            string MMEAL_SC_NM = (a["mealServiceDietInfo"][1]["row"][list_total_count - i]["MMEAL_SC_NM"]).ToString();
            //오늘 급식 종류

            string MLSV_YMD =(a["mealServiceDietInfo"][1]["row"][list_total_count - i]["MLSV_YMD"]).ToString();
            //오늘 날자

            string DDISH_NM = ((a["mealServiceDietInfo"][1]["row"][list_total_count - i]["DDISH_NM"]).ToString().Replace("<br/>", "\n"));
            //오늘 급식 메뉴

            string CAL_INFO = (a["mealServiceDietInfo"][1]["row"][list_total_count - i]["CAL_INFO"]).ToString();
            //칼로리
            double temp = Double.Parse(((CAL_INFO).Replace(" ", "")).Replace("Kcal", ""));
            string CAL = CAL_INFO + $"({(Math.Round(temp) / 4).ToString()}분 걷기)";

            //중국산 음식
            string[] CHlist = a["mealServiceDietInfo"][1]["row"][list_total_count - 1]["ORPLC_INFO"].ToString().Replace("<br/>", "\n").Split('\n');
            string todaych = "";
            foreach (string CH in CHlist)
            {
                if (CH.Contains("수입산"))
                {
                    todaych += $"{CH}\n";
                }
            }
            int aaa = (i == 1) ? (Convert.ToInt32(timeDate[2]) + 1) : Convert.ToInt32(timeDate[2]);
            return $"{timeDate[1]}월 {aaa}일\n\n칼로리:{CAL}\n\n메뉴\n{DDISH_NM}\n\n중국산 재료 리스트\n{todaych}";
        }
    }
}
