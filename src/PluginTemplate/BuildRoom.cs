using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Xna.Framework;
using MiniGamesAPI;
using MiniGamesAPI.Core;
using Newtonsoft.Json;
using Terraria;
using Terraria.Utilities;
using TShockAPI;

namespace BuildMaster
{
	// Token: 0x02000004 RID: 4
	public class BuildRoom : MiniRoom, IRoom
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600001A RID: 26 RVA: 0x000023A7 File Offset: 0x000005A7
		// (set) Token: 0x0600001B RID: 27 RVA: 0x000023AF File Offset: 0x000005AF
		[JsonIgnore]
		public MiniRegion GamingArea { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000023B8 File Offset: 0x000005B8
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000023C0 File Offset: 0x000005C0
		[JsonIgnore]
		public PrebuildBoard GameBoard { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000023C9 File Offset: 0x000005C9
		// (set) Token: 0x0600001F RID: 31 RVA: 0x000023D1 File Offset: 0x000005D1
		[JsonIgnore]
		public List<MiniRegion> PlayerAreas { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000020 RID: 32 RVA: 0x000023DA File Offset: 0x000005DA
		// (set) Token: 0x06000021 RID: 33 RVA: 0x000023E2 File Offset: 0x000005E2
		[JsonIgnore]
		public List<BuildPlayer> Players { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000023EB File Offset: 0x000005EB
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000023F3 File Offset: 0x000005F3
		public Dictionary<string, int> Topics { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000024 RID: 36 RVA: 0x000023FC File Offset: 0x000005FC
		// (set) Token: 0x06000025 RID: 37 RVA: 0x00002404 File Offset: 0x00000604
		public Point WaitingPoint { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000026 RID: 38 RVA: 0x0000240D File Offset: 0x0000060D
		// (set) Token: 0x06000027 RID: 39 RVA: 0x00002415 File Offset: 0x00000615
		public Point TL { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000028 RID: 40 RVA: 0x0000241E File Offset: 0x0000061E
		// (set) Token: 0x06000029 RID: 41 RVA: 0x00002426 File Offset: 0x00000626
		public Point BR { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600002A RID: 42 RVA: 0x0000242F File Offset: 0x0000062F
		// (set) Token: 0x0600002B RID: 43 RVA: 0x00002437 File Offset: 0x00000637
		[JsonIgnore]
		public bool Loaded { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002C RID: 44 RVA: 0x00002440 File Offset: 0x00000640
		// (set) Token: 0x0600002D RID: 45 RVA: 0x00002448 File Offset: 0x00000648
		public int PerWidth { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002E RID: 46 RVA: 0x00002451 File Offset: 0x00000651
		// (set) Token: 0x0600002F RID: 47 RVA: 0x00002459 File Offset: 0x00000659
		public int PerHeight { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000030 RID: 48 RVA: 0x00002462 File Offset: 0x00000662
		// (set) Token: 0x06000031 RID: 49 RVA: 0x0000246A File Offset: 0x0000066A
		public int Gap { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00002473 File Offset: 0x00000673
		// (set) Token: 0x06000033 RID: 51 RVA: 0x0000247B File Offset: 0x0000067B
		[JsonIgnore]
		public string Topic { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002484 File Offset: 0x00000684
		// (set) Token: 0x06000035 RID: 53 RVA: 0x0000248C File Offset: 0x0000068C
		[JsonIgnore]
		public int PlayerIndex { get; set; }

		// Token: 0x06000036 RID: 54 RVA: 0x00002498 File Offset: 0x00000698
		public BuildRoom(string name, int id)
		{
			base.Name = name;
			base.ID = id;
			this.Loaded = false;
			this.Initialize();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002504 File Offset: 0x00000704
		public BuildRoom()
		{
			this.Waiting_Timer.Elapsed += this.OnWaiting;
			this.Gaming_Timer.Elapsed += this.OnGaming;
			this.Scoring_Timer.Elapsed += this.OnScoring;
			this.PlayerAreas = new List<MiniRegion>();
			this.Players = new List<BuildPlayer>();
			this.Start();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000025B4 File Offset: 0x000007B4
		private void OnGaming(object sender, ElapsedEventArgs e)
		{
			if (this.Players.Count == 0)
			{
				this.Conclude();
			}
			if (base.Status != 2)
			{
				return;
			}
			this.ShowRoomMemberInfo();
			if (base.GamingTime == 0)
			{
				this.PlayerIndex = 0;
				this.Gaming_Timer.Stop();
				for (int i = this.Players.Count - 1; i >= 0; i--)
				{
					BuildPlayer buildPlayer = this.Players[i];
					ConfigUtils.evaluatePack.RestoreCharacter(buildPlayer);
					buildPlayer.Locked = true;
					buildPlayer.UnCreative();
					buildPlayer.Teleport(this.Players[this.PlayerIndex].CurrentRegion.Center);
					buildPlayer.SendInfoMessage("已来到 " + this.Players[this.PlayerIndex].Name + " 的作品");
				}
				this.Scoring_Timer.Start();
				base.Status = 1;
				return;
			}
			if (base.GamingTime <= 5)
			{
				this.Broadcast(string.Format("还有 {0} 秒进入评分环节...", base.GamingTime), Color.MediumAquamarine);
			}
			base.GamingTime--;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000026D4 File Offset: 0x000008D4
		private void OnWaiting(object sender, ElapsedEventArgs e)
		{
			this.ShowRoomMemberInfo();
			if (this.Players.Count != 0)
			{
				if (this.Players.Where((BuildPlayer p) => p.IsReady).Count<BuildPlayer>() >= base.MinPlayer)
				{
					if (base.WaitingTime == 0)
					{
						this.Waiting_Timer.Stop();
						this.SelectTopic();
						this.HandleRegions();
						for (int i = this.Players.Count - 1; i >= 0; i--)
						{
							BuildPlayer buildPlayer = this.Players[i];
							buildPlayer.Status = 2;
							buildPlayer.IsReady = true;
							buildPlayer.Godmode(true);
							buildPlayer.Creative();
							ConfigUtils.defaultPack.RestoreCharacter(buildPlayer);
						}
						base.Status = 2;
						this.Gaming_Timer.Start();
						return;
					}
					this.Broadcast(string.Format("游戏还有 {0} 秒开始....", base.WaitingTime), Color.DarkTurquoise);
					base.WaitingTime--;
					return;
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x000027D8 File Offset: 0x000009D8
		public int GetPlayerCount()
		{
			return this.Players.Count;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000027E8 File Offset: 0x000009E8
		public void Initialize()
		{
			this.Waiting_Timer.Elapsed += this.OnWaiting;
			this.Gaming_Timer.Elapsed += this.OnGaming;
			this.Scoring_Timer.Elapsed += this.OnScoring;
			this.PlayerAreas = new List<MiniRegion>();
			this.Players = new List<BuildPlayer>();
			this.Topics = new Dictionary<string, int>();
			this.Start();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002864 File Offset: 0x00000A64
		private void OnScoring(object sender, ElapsedEventArgs e)
		{
			if (this.Players.Count == 0)
			{
				this.Conclude();
			}
			if (base.Status != 1)
			{
				return;
			}
			this.ShowRoomMemberInfo();
			if (base.SeletingTime == 0)
			{
				this.Scoring_Timer.Stop();
				this.RoundClude();
				this.Scoring_Timer.Start();
				return;
			}
			if (base.SeletingTime <= 10)
			{
				if (this.PlayerIndex + 1 < this.Players.Count)
				{
					this.Broadcast(string.Format("还有 {0} 秒进入下一个玩家的建筑区域评分,下一个玩家为{1}", base.SeletingTime, this.Players[this.PlayerIndex + 1].Name), Color.MediumAquamarine);
				}
				else
				{
					this.Broadcast(string.Format("还有 {0} 秒结束游戏", base.SeletingTime), Color.MediumAquamarine);
				}
			}
			base.SeletingTime--;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002940 File Offset: 0x00000B40
		public void Dispose()
		{
			this.Stop();
			for (int i = this.Players.Count - 1; i >= 0; i--)
			{
				BuildPlayer buildPlayer = this.Players[i];
				buildPlayer.Teleport(Main.spawnTileX, Main.spawnTileY);
				buildPlayer.SendInfoMessage("游戏被强制停止！");
				buildPlayer.BackUp.RestoreCharacter(buildPlayer);
				buildPlayer.BackUp = null;
				buildPlayer.CurrentRoomID = 0;
				buildPlayer.CurrentRegion = null;
				buildPlayer.AquiredMarks = 0;
				buildPlayer.GiveMarks = 0;
				buildPlayer.SelectedTopic = "";
				buildPlayer.IsReady = false;
				buildPlayer.Locked = false;
				buildPlayer.Marked = false;
				buildPlayer.Godmode(false);
			}
			this.Restore();
			this.Players.Clear();
			base.Status = 5;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002A08 File Offset: 0x00000C08
		public void Conclude()
		{
			base.Status = 3;
			this.ShowVictory();
			for (int i = this.Players.Count - 1; i >= 0; i--)
			{
				BuildPlayer buildPlayer = this.Players[i];
				buildPlayer.Teleport(this.WaitingPoint);
				buildPlayer.SendInfoMessage("游戏结束！");
				buildPlayer.BackUp.RestoreCharacter(buildPlayer);
				buildPlayer.CurrentRegion = null;
				buildPlayer.AquiredMarks = 0;
				buildPlayer.IsReady = false;
				buildPlayer.Locked = false;
				buildPlayer.Marked = false;
				buildPlayer.SelectedTopic = "";
				buildPlayer.GiveMarks = 0;
				buildPlayer.Godmode(false);
			}
			this.Restore();
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002AAC File Offset: 0x00000CAC
		public void RoundClude()
		{
			int num = 0;
			BuildRoom roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(base.ID);
			base.SeletingTime = roomByIDFromLocal.SeletingTime;
			this.PlayerIndex++;
			if (this.PlayerIndex < this.Players.Count)
			{
				for (int i = this.Players.Count - 1; i >= 0; i--)
				{
					BuildPlayer buildPlayer = this.Players[i];
					buildPlayer.Marked = true;
					num += buildPlayer.GiveMarks;
					buildPlayer.GiveMarks = 0;
					buildPlayer.Teleport(this.Players[this.PlayerIndex].CurrentRegion.Center);
					buildPlayer.SendInfoMessage("已来到 " + this.Players[this.PlayerIndex].Name + " 的建筑区域");
					buildPlayer.Marked = false;
				}
			}
			this.Players[this.PlayerIndex - 1].AquiredMarks = num;
			if (this.PlayerIndex >= this.Players.Count)
			{
				this.Conclude();
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002BB8 File Offset: 0x00000DB8
		public void Start()
		{
			this.Waiting_Timer.Start();
			base.Status = 0;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002BCC File Offset: 0x00000DCC
		public void Stop()
		{
			this.Waiting_Timer.Stop();
			this.Gaming_Timer.Stop();
			this.Scoring_Timer.Stop();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002BF0 File Offset: 0x00000DF0
		public void Restore()
		{
			base.Status = 4;
			BuildRoom roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(base.ID);
			base.WaitingTime = roomByIDFromLocal.WaitingTime;
			base.GamingTime = roomByIDFromLocal.GamingTime;
			base.SeletingTime = roomByIDFromLocal.SeletingTime;
			this.Topics = roomByIDFromLocal.Topics;
			base.MaxPlayer = roomByIDFromLocal.MaxPlayer;
			base.MinPlayer = roomByIDFromLocal.MinPlayer;
			this.PerHeight = roomByIDFromLocal.PerHeight;
			this.PerWidth = roomByIDFromLocal.PerWidth;
			this.Gap = roomByIDFromLocal.Gap;
			this.PlayerAreas.Clear();
			this.GameBoard.ReBuild(true);
			base.Status = 0;
			this.Start();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002CA0 File Offset: 0x00000EA0
		public void ShowRoomMemberInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Utils.EndLine_10);
			stringBuilder.AppendLine("————房内信息————");
			switch (base.Status)
			{
			case 0:
			{
				int num = base.WaitingTime / 60;
				int num2 = base.WaitingTime % 60;
				stringBuilder.AppendLine(string.Format("倒计时[{0}:{1}]", num, num2));
				stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(this.Topic) ? "无" : this.Topic));
				for (int i = this.Players.Count - 1; i >= 0; i--)
				{
					BuildPlayer buildPlayer = this.Players[i];
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"[",
						buildPlayer.Name,
						"][",
						buildPlayer.IsReady ? "已准备" : "未准备",
						"]"
					}));
				}
				stringBuilder.AppendLine(string.Format("当前人数:{0}/{1}", this.GetPlayerCount(), base.MaxPlayer));
				stringBuilder.AppendLine("输入/bm ready 准备/未准备");
				stringBuilder.AppendLine("输入/bm leave 离开房间");
				stringBuilder.AppendLine("输入/bm tl 查看主题列表");
				stringBuilder.AppendLine("输入/bm vote [主题] 进行主题投票");
				break;
			}
			case 1:
			{
				int num = base.SeletingTime / 60;
				int num2 = base.SeletingTime % 60;
				stringBuilder.AppendLine(string.Format("倒计时[{0}:{1}]", num, num2));
				stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(this.Topic) ? "无" : this.Topic));
				stringBuilder.AppendLine(string.Format("当前人数:{0}/{1}", this.GetPlayerCount(), base.MaxPlayer));
				break;
			}
			case 2:
			{
				int num = base.GamingTime / 60;
				int num2 = base.GamingTime % 60;
				stringBuilder.AppendLine(string.Format("倒计时[{0}:{1}]", num, num2));
				stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(this.Topic) ? "无" : this.Topic));
				stringBuilder.AppendLine(string.Format("当前人数:{0}/{1}", this.GetPlayerCount(), base.MaxPlayer));
				break;
			}
			}
			stringBuilder.AppendLine(Utils.EndLine_15);
			for (int j = this.Players.Count - 1; j >= 0; j--)
			{
				this.Players[j].SendBoardMsg(stringBuilder.ToString());
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002F64 File Offset: 0x00001164
		public void ShowVictory()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.Players.Sort();
			int num = 1;
			foreach (BuildPlayer buildPlayer in this.Players)
			{
				stringBuilder.AppendLine(string.Format("[{0}] {1} 得分:{2}", num, buildPlayer.Name, buildPlayer.AquiredMarks));
				num++;
			}
			this.Broadcast(stringBuilder.ToString(), Color.MediumAquamarine);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00003000 File Offset: 0x00001200
		public void Broadcast(string msg, Color color)
		{
			for (int i = this.Players.Count - 1; i >= 0; i--)
			{
				this.Players[i].SendMessage(msg, color);
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003038 File Offset: 0x00001238
		public bool HandleRegions()
		{
			this.PlayerAreas = this.GamingArea.Divide(this.PerWidth, this.PerHeight, this.GetPlayerCount(), this.Gap);
			if (this.PlayerAreas == null)
			{
				this.Dispose();
				return false;
			}
			foreach (MiniRegion miniRegion in this.PlayerAreas)
			{
				miniRegion.BuildFramework(118, false);
			}
			TSPlayer.All.SendTileRect((short)this.GamingArea.Area.X, (short)this.GamingArea.Area.Y, (byte)(this.GamingArea.Area.Width + 3), (byte)(this.GamingArea.Area.Height + 3), 0);
			for (int i = 0; i < this.PlayerAreas.Count; i++)
			{
				MiniRegion miniRegion2 = this.PlayerAreas[i];
				BuildPlayer buildPlayer = this.Players[i];
				miniRegion2.Owners.Add(this.Players[i].Name);
				buildPlayer.CurrentRegion = miniRegion2;
				buildPlayer.Teleport(miniRegion2.Center);
				buildPlayer.SendInfoMessage("已将你传送到建筑区域");
			}
			return true;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003184 File Offset: 0x00001384
		public void LoadRegion()
		{
			this.GamingArea = new MiniRegion(base.Name, base.ID, this.TL.X, this.TL.Y, this.BR.X, this.BR.Y);
			this.GameBoard = new PrebuildBoard(this.GamingArea);
			this.Loaded = true;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000031EC File Offset: 0x000013EC
		public void SelectTopic()
		{
			int max = 0;
			foreach (string text in this.Topics.Keys)
			{
				if (this.Topics[text] >= max)
				{
					max = this.Topics[text];
				}
			}
			List<string> list = (from p in this.Topics
				where p.Value == max
				select p.Key).ToList<string>();
			if (list.Count<string>() == 1)
			{
				this.Topic = list[0];
			}
			else
			{
				UnifiedRandom unifiedRandom = new UnifiedRandom();
				this.Topic = list[unifiedRandom.Next(0, list.Count - 1)];
			}
			this.Broadcast("本次建筑的主题是 " + this.Topic, Color.MediumAquamarine);
		}

		// Token: 0x0400000A RID: 10
		[JsonIgnore]
		public System.Timers.Timer MyTimer = new System.Timers.Timer(1000.0);

		// Token: 0x0400000B RID: 11
		[JsonIgnore]
		public Timer Gaming_Timer = new Timer(1000.0);

		// Token: 0x0400000C RID: 12
		[JsonIgnore]
		public Timer Scoring_Timer = new Timer(1000.0);
	}
}
