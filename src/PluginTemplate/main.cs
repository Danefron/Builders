using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using MiniGamesAPI.Core;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace BuildMaster
{
	// Token: 0x02000006 RID: 6
	[ApiVersion(2, 1)]
	public class MainPlugin : TerrariaPlugin
	{
		// Token: 0x06000053 RID: 83 RVA: 0x000036DB File Offset: 0x000018DB
		public MainPlugin(Main game)
			: base(game)
		{
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000054 RID: 84 RVA: 0x000036E4 File Offset: 0x000018E4
		public override string Name
		{
			get
			{
				return "BuildMaster";
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000036EB File Offset: 0x000018EB
		public override Version Version
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000056 RID: 86 RVA: 0x000036FC File Offset: 0x000018FC
		public override string Author
		{
			get
			{
				return "豆沙";
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00003703 File Offset: 0x00001903
		public override string Description
		{
			get
			{
				return "A minigame that is named BuildMaster";
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000370C File Offset: 0x0000190C
		public override void Initialize()
		{
			ServerApi.Hooks.GamePostInitialize.Register(this, new HookHandler<EventArgs>(this.OnPostInitialize));
			ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.OnJoin));
			ServerApi.Hooks.ServerLeave.Register(this, new HookHandler<LeaveEventArgs>(this.OnLeave));
			ServerApi.Hooks.ServerChat.Register(this, new HookHandler<ServerChatEventArgs>(this.OnChat));
			GetDataHandlers.TogglePvp += new EventHandler<GetDataHandlers.TogglePvpEventArgs>(this.OnTogglePVP);
			GetDataHandlers.TileEdit += new EventHandler<GetDataHandlers.TileEditEventArgs>(this.OnTileEdit);
			GetDataHandlers.PlayerUpdate += new EventHandler<GetDataHandlers.PlayerUpdateEventArgs>(this.OnPlayerUpdate);
			GetDataHandlers.PlayerTeam += new EventHandler<GetDataHandlers.PlayerTeamEventArgs>(this.OnTeam);
			GetDataHandlers.LiquidSet += new EventHandler<GetDataHandlers.LiquidSetEventArgs>(this.OnSetLiquid);
			GetDataHandlers.PlayerSlot += new EventHandler<GetDataHandlers.PlayerSlotEventArgs>(this.OnPlayerSlot);
			ConfigUtils.LoadConfig();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003830 File Offset: 0x00001A30
		private void OnChat(ServerChatEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(TShock.Players[args.Who].Name);
			MiniGamesAPI.Core.MiniRoom roomByID = ConfigUtils.GetRoomByID(base.CurrentRoomID);
			if (args.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier) || args.Text.StartsWith(TShock.Config.Settings.CommandSpecifier))
			{
				return;
			}
			if (playerByName != null && roomByID != null)
			{
				roomByID.Broadcast("[房内聊天]" + playerByName.Name + ":" + args.Text, Color.DodgerBlue);
				args.Handled = true;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000038CC File Offset: 0x00001ACC
		private void OnTeam(object sender, GetDataHandlers.PlayerTeamEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			if (playerByName != null)
			{
				BuildRoom roomByID = (BuildMaster.BuildRoom)ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && roomByID.Status != null)
				{
					playerByName.SetTeam(0);
					playerByName.SendInfoMessage("当前状态不能切换队伍");
					args.Handled = true;
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003920 File Offset: 0x00001B20
		private void OnPlayerSlot(object sender, GetDataHandlers.PlayerSlotEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			TSPlayer player = args.Player;
			if (playerByName != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && roomByID.Status == 1)
				{
					ConfigUtils.evaluatePack.RestoreCharacter(playerByName);
				}
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003970 File Offset: 0x00001B70
		private void OnTogglePVP(object sender, GetDataHandlers.TogglePvpEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			if (playerByName != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && roomByID.Status != null)
				{
					playerByName.SetPVP(false);
					playerByName.SendInfoMessage("当前状态不能开PVP");
					args.Handled = true;
				}
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000039C4 File Offset: 0x00001BC4
		private void OnSetLiquid(object sender, GetDataHandlers.LiquidSetEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			if (playerByName != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && playerByName.CurrentRegion != null && roomByID.Status == 2 && !playerByName.CurrentRegion.Area.Contains(args.TileX, args.TileY))
				{
					NetMessage.sendWater(args.TileX, args.TileY);
					args.Handled = true;
					playerByName.SendInfoMessage("你不能在别人的区域内恶意倒液体");
				}
				if (roomByID != null && roomByID.Status == 1)
				{
					NetMessage.sendWater(args.TileX, args.TileY);
					args.Handled = true;
					playerByName.SendInfoMessage("评选阶段不允许倒液体");
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00003A80 File Offset: 0x00001C80
		private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			if (playerByName != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && playerByName.CurrentRegion != null && roomByID.Status == 2)
				{
					if (args.X == playerByName.CurrentRegion.TopLeft.X || args.Y == playerByName.CurrentRegion.TopLeft.Y || args.X == playerByName.CurrentRegion.BottomRight.X || args.Y == playerByName.CurrentRegion.BottomRight.Y)
					{
						TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 1, 1, 0);
						playerByName.SendInfoMessage("不可以破坏边框");
						args.Handled = true;
					}
					if (!playerByName.CurrentRegion.Area.Contains(args.X, args.Y))
					{
						TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 1, 1, 0);
						playerByName.SendInfoMessage("这不是你区域内的物块哦");
						args.Handled = true;
					}
				}
				if (roomByID != null && roomByID.Status == 1)
				{
					args.Handled = true;
					TSPlayer.All.SendTileRect((short)args.X, (short)args.Y, 3, 3, 0);
					playerByName.SendInfoMessage("评选阶段不允许建造");
				}
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003BE8 File Offset: 0x00001DE8
		private void OnPlayerUpdate(object sender, GetDataHandlers.PlayerUpdateEventArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			TSPlayer player = args.Player;
			if (playerByName != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (roomByID != null && roomByID.Status == 2 && playerByName.CurrentRegion != null)
				{
					Point point;
					point..ctor(args.Player.TileX, args.Player.TileY);
					Point point2;
					point2..ctor(args.Player.TileX, args.Player.TileY + 1);
					Point point3;
					point3..ctor(args.Player.TileX, args.Player.TileY + 2);
					if (!playerByName.CurrentRegion.Area.Contains(point) && !playerByName.CurrentRegion.Area.Contains(point2) && !playerByName.CurrentRegion.Area.Contains(point3) && !playerByName.Locked)
					{
						playerByName.Teleport(playerByName.CurrentRegion.Center);
						playerByName.SendInfoMessage("不可擅自离开建筑区域");
					}
				}
				if (roomByID != null && roomByID.Status == 1)
				{
					if (args.Control.IsUsingItem && args.Player.TPlayer.HeldItem.netID == 75)
					{
						if (playerByName.Marked)
						{
							playerByName.SendInfoMessage("你已经评分过了");
							return;
						}
						int num = player.TPlayer.selectedItem + 1;
						BuildPlayer buildPlayer = roomByID.Players[roomByID.PlayerIndex];
						if (buildPlayer.Name == playerByName.Name)
						{
							playerByName.SendInfoMessage("你不能给自己评分");
							return;
						}
						if (playerByName.GiveMarks == 0)
						{
							playerByName.SendInfoMessage(string.Format("已给 {0} 的建筑评分:{1}分", buildPlayer.Name, num));
						}
						else
						{
							playerByName.SendInfoMessage(string.Format("已给 {0} 的建筑更改评分:{1}分", buildPlayer.Name, num));
						}
						playerByName.GiveMarks = num;
						return;
					}
					else if (ConfigUtils.config.BanItem.Contains(args.Player.TPlayer.HeldItem.netID))
					{
						playerByName.SetBuff(156, 600, false);
					}
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003E28 File Offset: 0x00002028
		private void OnPostInitialize(EventArgs args)
		{
			Commands.ChatCommands.Add(new Command("bm.user", new CommandDelegate(this.BM), new string[] { "bm", "建筑大师" }));
			Commands.ChatCommands.Add(new Command("bm.admin", new CommandDelegate(this.BMA), new string[] { "bma", "建筑大师管理" }));
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003EA4 File Offset: 0x000020A4
		private void BMA(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendInfoMessage("请输入 /bma help 查看帮助");
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = args.Parameters[0];
			uint num = PrivateImplementationDetails.ComputeStringHash(text);
			if (num <= 1226617017U)
			{
				if (num <= 987190784U)
				{
					if (num <= 649812317U)
					{
						if (num != 217798785U)
						{
							if (num == 649812317U)
							{
								if (text == "create")
								{
									if (args.Parameters.Count != 2)
									{
										args.Player.SendInfoMessage("正确指令 /bma create [房间名]");
										return;
									}
									BuildRoom buildRoom = new BuildRoom(args.Parameters[1], ConfigUtils.rooms.Count + 1);
									ConfigUtils.rooms.Add(buildRoom);
									ConfigUtils.AddRoom(buildRoom);
									args.Player.SendInfoMessage(string.Format("成功创建房间(id:{0})", buildRoom.ID));
									return;
								}
							}
						}
						else if (text == "list")
						{
							foreach (BuildRoom buildRoom2 in ConfigUtils.rooms)
							{
								stringBuilder.AppendLine(string.Format("[{0}][{1}][{2}/{3}][{4}]", new object[]
								{
									buildRoom2.ID,
									buildRoom2.Name,
									buildRoom2.GetPlayerCount(),
									buildRoom2.MaxPlayer,
									buildRoom2.Status
								}));
							}
							args.Player.SendMessage(stringBuilder.ToString(), Color.DarkTurquoise);
							return;
						}
					}
					else if (num != 946971642U)
					{
						if (num != 978454399U)
						{
							if (num == 987190784U)
							{
								if (text == "addt")
								{
									if (args.Parameters.Count != 3)
									{
										args.Player.SendInfoMessage("正确指令 /bma addt [房间ID] [主题名]");
										return;
									}
									int num2;
									if (!int.TryParse(args.Parameters[1], out num2))
									{
										args.Player.SendInfoMessage("请输入正确的数字");
										return;
									}
									BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
									string text2 = args.Parameters[2];
									if (buildRoom != null)
									{
										buildRoom.Topics.Add(text2, 0);
										args.Player.SendInfoMessage(string.Format("成功添加房间(id:{0}) 的一个主题 {1}", buildRoom.ID, text2));
										ConfigUtils.UpdateRooms(num2);
										return;
									}
									args.Player.SendInfoMessage("房间不存在");
									return;
								}
							}
						}
						else if (text == "sg")
						{
							if (args.Parameters.Count != 3)
							{
								args.Player.SendInfoMessage("正确指令 /bma sg [房间ID] [间隔]");
								return;
							}
							int num2;
							int num3;
							if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
							{
								args.Player.SendInfoMessage("请输入正确的数字");
								return;
							}
							BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
							if (buildRoom != null)
							{
								buildRoom.Gap = num3;
								args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的区域间隔为{1}", buildRoom.ID, num3));
								ConfigUtils.UpdateRooms(num2);
								return;
							}
							args.Player.SendInfoMessage("房间不存在");
							return;
						}
					}
					else if (text == "help")
					{
						stringBuilder.AppendLine("/bma list 列出所有房间");
						stringBuilder.AppendLine("/bma create [房间名] 创建房间");
						stringBuilder.AppendLine("/bma remove [房间ID] 移除指定房间");
						stringBuilder.AppendLine("/bma start [房间ID] 开启指定房间");
						stringBuilder.AppendLine("/bma stop [房间ID] 关闭指定房间");
						stringBuilder.AppendLine("/bma smp [房间ID] [人数] 设置房间最大玩家数");
						stringBuilder.AppendLine("/bma sdp [房间ID] [人数] 设置房间最小玩家数");
						stringBuilder.AppendLine("/bma swt [房间ID] [时间] 设置等待时间(单位：秒)");
						stringBuilder.AppendLine("/bma sgt [房间ID] [时间] 设置游戏时间(单位：秒)");
						stringBuilder.AppendLine("/bma sst [房间ID] [时间] 设置评分时间(单位：秒)");
						stringBuilder.AppendLine("/bma sp [1/2] 选取点1/2");
						stringBuilder.AppendLine("/bma sr [房间ID] 设置房间的游戏区域");
						stringBuilder.AppendLine("/bma addt [房间ID] [主题名] 添加主题");
						stringBuilder.AppendLine("/bma sh [房间ID] [高] 设置小区域高");
						stringBuilder.AppendLine("/bma sw [房间ID] [宽] 设置小区域宽");
						stringBuilder.AppendLine("/bma sg [房间ID] [间隔] 设置小区域间隔");
						stringBuilder.AppendLine("/bma dp [玩家名字] 设置基础建造背包");
						stringBuilder.AppendLine("/bma ep 设置评分套装");
						stringBuilder.AppendLine("/bma reload 重载配置文件非房间文件");
						args.Player.SendMessage(stringBuilder.ToString(), Color.DarkTurquoise);
						return;
					}
				}
				else if (num <= 1125804208U)
				{
					if (num != 1062342494U)
					{
						if (num == 1125804208U)
						{
							if (text == "ep")
							{
								ConfigUtils.evaluatePack = ConfigUtils.defaultPack.GetCopyNoItems("评分套", 3);
								for (int i = 0; i < 10; i++)
								{
									ConfigUtils.evaluatePack.Items.Add(new MiniItem(i, 0, 75, i + 1));
								}
								ConfigUtils.UpdatePack();
								args.Player.SendInfoMessage("已设置评分套装");
								return;
							}
						}
					}
					else if (text == "sh")
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendInfoMessage("正确指令 /bma sh [房间ID] [高]");
							return;
						}
						int num2;
						int num3;
						if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
						{
							args.Player.SendInfoMessage("请输入正确的数字");
							return;
						}
						BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
						if (buildRoom != null)
						{
							buildRoom.PerHeight = num3;
							args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的区域高度为{1}", buildRoom.ID, num3));
							ConfigUtils.UpdateRooms(num2);
							return;
						}
						args.Player.SendInfoMessage("房间不存在");
						return;
					}
				}
				else if (num != 1163008208U)
				{
					if (num != 1196563446U)
					{
						if (num == 1226617017U)
						{
							if (text == "dp")
							{
								if (args.Parameters.Count != 2)
								{
									args.Player.SendInfoMessage("正确指令 /bma dp [玩家名]");
									return;
								}
								List<TSPlayer> list = TSPlayer.FindByNameOrID(args.Parameters[1]);
								TSPlayer tsplayer = null;
								if (list.Count != 0)
								{
									tsplayer = list[0];
								}
								ConfigUtils.defaultPack.CopyFromPlayer(tsplayer);
								ConfigUtils.UpdatePack();
								args.Player.SendInfoMessage("已设置基础套装");
								return;
							}
						}
					}
					else if (text == "sp")
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendInfoMessage("正确指令 /bma sp [1/2]");
							return;
						}
						if (args.Parameters[1] == "1")
						{
							args.Player.AwaitingTempPoint = 1;
							args.Player.SendInfoMessage("请选择点1");
						}
						if (args.Parameters[1] == "2")
						{
							args.Player.AwaitingTempPoint = 2;
							args.Player.SendInfoMessage("请选择点2");
							return;
						}
						return;
					}
				}
				else if (text == "sr")
				{
					if (args.Parameters.Count != 2)
					{
						args.Player.SendInfoMessage("正确指令 /bma sr [房间ID]");
						return;
					}
					if (args.Player.TempPoints[0] == Point.Zero || args.Player.TempPoints[1] == Point.Zero)
					{
						args.Player.SendInfoMessage("点未选取完毕");
						return;
					}
					int num2;
					if (!int.TryParse(args.Parameters[1], out num2))
					{
						args.Player.SendInfoMessage("请输入正确的数字");
						return;
					}
					BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
					if (buildRoom != null)
					{
						buildRoom.TL = args.Player.TempPoints[0];
						buildRoom.BR = args.Player.TempPoints[1];
						args.Player.TempPoints[0] = Point.Zero;
						args.Player.TempPoints[1] = Point.Zero;
						args.Player.SendInfoMessage(string.Format("成功添加房间(id:{0}) 的游戏区域", buildRoom.ID));
						ConfigUtils.UpdateRooms(num2);
						return;
					}
					args.Player.SendInfoMessage("房间不存在");
					return;
				}
			}
			else if (num <= 3411225317U)
			{
				if (num <= 1697318111U)
				{
					if (num != 1246896303U)
					{
						if (num == 1697318111U)
						{
							if (text == "start")
							{
								if (args.Parameters.Count != 2)
								{
									args.Player.SendInfoMessage("正确指令 /bma start [房间ID]");
									return;
								}
								int num2;
								if (!int.TryParse(args.Parameters[1], out num2))
								{
									args.Player.SendInfoMessage("请输入正确的数字");
									return;
								}
								BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
								if (buildRoom != null)
								{
									buildRoom.Restore();
									buildRoom.Start();
									args.Player.SendInfoMessage(string.Format("成功开启房间(id:{0})", buildRoom.ID));
									ConfigUtils.UpdateRooms(num2);
									return;
								}
								args.Player.SendInfoMessage("房间不存在");
								return;
							}
						}
					}
					else if (text == "sw")
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendInfoMessage("正确指令 /bma sw [房间ID] [宽]");
							return;
						}
						int num2;
						int num3;
						if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
						{
							args.Player.SendInfoMessage("请输入正确的数字");
							return;
						}
						BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
						if (buildRoom != null)
						{
							buildRoom.PerWidth = num3;
							args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的区域宽度为{1}", buildRoom.ID, num3));
							ConfigUtils.UpdateRooms(num2);
							return;
						}
						args.Player.SendInfoMessage("房间不存在");
						return;
					}
				}
				else if (num != 3290744134U)
				{
					if (num != 3393131061U)
					{
						if (num == 3411225317U)
						{
							if (text == "stop")
							{
								if (args.Parameters.Count != 2)
								{
									args.Player.SendInfoMessage("正确指令 /bma stop [房间ID]");
									return;
								}
								int num2;
								if (!int.TryParse(args.Parameters[1], out num2))
								{
									args.Player.SendInfoMessage("请输入正确的数字");
									return;
								}
								BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
								if (buildRoom != null)
								{
									buildRoom.Stop();
									buildRoom.Dispose();
									args.Player.SendInfoMessage(string.Format("成功强制关闭房间(id:{0})", buildRoom.ID));
									ConfigUtils.UpdateRooms(num2);
									return;
								}
								args.Player.SendInfoMessage("房间不存在");
								return;
							}
						}
					}
					else if (text == "sst")
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
							return;
						}
						int num2;
						int num3;
						if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
						{
							args.Player.SendInfoMessage("请输入正确的数字");
							return;
						}
						BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
						if (buildRoom != null)
						{
							buildRoom.SeletingTime = num3;
							args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的评分时间为{1}秒", buildRoom.ID, num3));
							ConfigUtils.UpdateRooms(num2);
							return;
						}
						args.Player.SendInfoMessage("房间不存在");
						return;
					}
				}
				else if (text == "sdp")
				{
					if (args.Parameters.Count != 3)
					{
						args.Player.SendInfoMessage("正确指令 /bma sdp [房间ID] [人数]");
						return;
					}
					int num2;
					int num3;
					if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
					{
						args.Player.SendInfoMessage("请输入正确的数字");
						return;
					}
					BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
					if (buildRoom != null)
					{
						buildRoom.MinPlayer = num3;
						args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的最小玩家数为{1}", buildRoom.ID, num3));
						ConfigUtils.UpdateRooms(num2);
						return;
					}
					args.Player.SendInfoMessage("房间不存在");
					return;
				}
			}
			else if (num <= 3662264513U)
			{
				if (num != 3659601489U)
				{
					if (num != 3661278775U)
					{
						if (num == 3662264513U)
						{
							if (text == "swt")
							{
								if (args.Parameters.Count != 3)
								{
									args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
									return;
								}
								int num2;
								int num3;
								if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
								{
									args.Player.SendInfoMessage("请输入正确的数字");
									return;
								}
								BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
								if (buildRoom != null)
								{
									buildRoom.WaitingTime = num3;
									args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的等待时间为{1}秒", buildRoom.ID, num3));
									ConfigUtils.UpdateRooms(num2);
									return;
								}
								args.Player.SendInfoMessage("房间不存在");
								return;
							}
						}
					}
					else if (text == "smp")
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
							return;
						}
						int num2;
						int num3;
						if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
						{
							args.Player.SendInfoMessage("请输入正确的数字");
							return;
						}
						BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
						if (buildRoom != null)
						{
							buildRoom.MaxPlayer = num3;
							args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的最大玩家数为{1}", buildRoom.ID, num3));
							ConfigUtils.UpdateRooms(num2);
							return;
						}
						args.Player.SendInfoMessage("房间不存在");
						return;
					}
				}
				else if (text == "sgt")
				{
					if (args.Parameters.Count != 3)
					{
						args.Player.SendInfoMessage("正确指令 /bma smp [房间ID] [人数]");
						return;
					}
					int num2;
					int num3;
					if (!int.TryParse(args.Parameters[1], out num2) || !int.TryParse(args.Parameters[2], out num3))
					{
						args.Player.SendInfoMessage("请输入正确的数字");
						return;
					}
					BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
					if (buildRoom != null)
					{
						buildRoom.GamingTime = num3;
						args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的游戏时间为{1}秒", buildRoom.ID, num3));
						ConfigUtils.UpdateRooms(num2);
						return;
					}
					args.Player.SendInfoMessage("房间不存在");
					return;
				}
			}
			else if (num != 3683784189U)
			{
				if (num != 3729374989U)
				{
					if (num == 3984383372U)
					{
						if (text == "reload")
						{
							ConfigUtils.ReloadConfig();
							args.Player.SendInfoMessage("插件重载成功");
							return;
						}
					}
				}
				else if (text == "swp")
				{
					if (args.Parameters.Count != 2)
					{
						args.Player.SendInfoMessage("正确指令 /bma swp [房间ID]");
						return;
					}
					int num2;
					if (!int.TryParse(args.Parameters[1], out num2))
					{
						args.Player.SendInfoMessage("请输入正确的数字");
						return;
					}
					BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
					if (buildRoom != null)
					{
						buildRoom.WaitingPoint = new Point(args.Player.TileX, args.Player.TileY - 3);
						args.Player.SendInfoMessage(string.Format("成功设置房间(id:{0}) 的等待点", buildRoom.ID));
						ConfigUtils.UpdateRooms(num2);
						return;
					}
					args.Player.SendInfoMessage("房间不存在");
					return;
				}
			}
			else if (text == "remove")
			{
				if (args.Parameters.Count != 2)
				{
					args.Player.SendInfoMessage("正确指令 /bma remove [房间ID]");
					return;
				}
				int num2;
				if (!int.TryParse(args.Parameters[1], out num2))
				{
					args.Player.SendInfoMessage("请输入正确的数字");
					return;
				}
				BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
				if (buildRoom != null)
				{
					buildRoom.Dispose();
					ConfigUtils.rooms.Remove(buildRoom);
					ConfigUtils.RemoveRoom(buildRoom.ID);
					args.Player.SendInfoMessage("成功移除房间");
					return;
				}
				args.Player.SendInfoMessage("房间不存在");
				return;
			}
			args.Player.SendInfoMessage("输入/bma help 查看帮助");
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00004F34 File Offset: 0x00003134
		private void BM(CommandArgs args)
		{
			BuildPlayer playerByName = ConfigUtils.GetPlayerByName(args.Player.Name);
			if (playerByName == null)
			{
				args.Player.SendInfoMessage("[BuildMaster] 玩家数据出错,请尝试重新进入服务器");
				return;
			}
			if (args.Parameters.Count < 1)
			{
				playerByName.SendInfoMessage("请输入/bm help 查看指令帮助");
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = args.Parameters[0];
			uint num = PrivateImplementationDetails.ComputeStringHash(text);
			if (num <= 1162169565U)
			{
				if (num != 217798785U)
				{
					if (num != 946971642U)
					{
						if (num == 1162169565U)
						{
							if (text == "tl")
							{
								BuildRoom buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
								if (buildRoom != null)
								{
									stringBuilder.AppendLine("此房支持的主题");
									foreach (string text2 in buildRoom.Topics.Keys)
									{
										stringBuilder.AppendLine("[" + text2 + "]");
									}
									playerByName.SendSuccessMessage(stringBuilder.ToString());
									return;
								}
								playerByName.SendInfoMessage("不在房间中");
								return;
							}
						}
					}
					else if (text == "help")
					{
						stringBuilder.AppendLine("/bm list 查看房间列表");
						stringBuilder.AppendLine("/bm join [房间ID] 加入房间");
						stringBuilder.AppendLine("/bm leave 离开房间");
						stringBuilder.AppendLine("/bm ready 准备/未准备");
						stringBuilder.AppendLine("/bm vote [主题] 投票主题");
						playerByName.SendSuccessMessage(stringBuilder.ToString());
						return;
					}
				}
				else if (text == "list")
				{
					foreach (BuildRoom buildRoom2 in ConfigUtils.rooms)
					{
						stringBuilder.AppendLine(string.Format("[{0}][{1}][{2}/{3}][{4}]", new object[]
						{
							buildRoom2.ID,
							buildRoom2.Name,
							buildRoom2.GetPlayerCount(),
							buildRoom2.MaxPlayer,
							buildRoom2.Status
						}));
					}
					playerByName.SendSuccessMessage(stringBuilder.ToString());
					return;
				}
			}
			else if (num <= 1712242932U)
			{
				if (num != 1416872128U)
				{
					if (num == 1712242932U)
					{
						if (text == "ready")
						{
							BuildRoom buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
							if (buildRoom != null)
							{
								playerByName.Ready();
								buildRoom.Broadcast("玩家 " + playerByName.Name + " " + (playerByName.IsReady ? "已准备" : "未准备"), Color.OrangeRed);
								return;
							}
							playerByName.SendInfoMessage("你不在房间中");
							return;
						}
					}
				}
				else if (text == "leave")
				{
					playerByName.Leave();
					return;
				}
			}
			else if (num != 1803669013U)
			{
				if (num == 3374496889U)
				{
					if (text == "join")
					{
						if (args.Parameters.Count != 2)
						{
							playerByName.SendInfoMessage("正确指令 /bm join [房间号]");
							return;
						}
						int num2;
						if (!int.TryParse(args.Parameters[1], out num2))
						{
							playerByName.SendInfoMessage("请输入正确的数字");
							return;
						}
						BuildRoom buildRoom = ConfigUtils.GetRoomByID(num2);
						if (buildRoom != null)
						{
							playerByName.Join(buildRoom);
							return;
						}
						playerByName.SendInfoMessage("房间不存在");
						return;
					}
				}
			}
			else if (text == "vote")
			{
				if (args.Parameters.Count != 2)
				{
					playerByName.SendInfoMessage("正确指令/bm vote 主题名");
					return;
				}
				string text3 = args.Parameters[1];
				BuildRoom buildRoom = ConfigUtils.GetRoomByID(playerByName.CurrentRoomID);
				if (buildRoom == null)
				{
					playerByName.SendInfoMessage("不在房间中");
					return;
				}
				if (buildRoom.Status != null)
				{
					playerByName.SendInfoMessage("当前房间状态不允许投票选主题");
					return;
				}
				if (buildRoom.Topics.ContainsKey(text3))
				{
					if (string.IsNullOrEmpty(playerByName.SelectedTopic))
					{
						Dictionary<string, int> dictionary = buildRoom.Topics;
						string text4 = text3;
						dictionary[text4]++;
						buildRoom.Broadcast("玩家 " + playerByName.Name + " 投票主题 " + text3, Color.DarkTurquoise);
					}
					else
					{
						Dictionary<string, int> dictionary = buildRoom.Topics;
						string text4 = playerByName.SelectedTopic;
						dictionary[text4]--;
						buildRoom.Broadcast("玩家 " + playerByName.Name + " 更改投票主题为 " + text3, Color.DarkTurquoise);
						dictionary = buildRoom.Topics;
						text4 = text3;
						dictionary[text4]++;
					}
					playerByName.SelectedTopic = text3;
					return;
				}
				playerByName.SendInfoMessage("该房间不存在此主题");
				return;
			}
			args.Player.SendInfoMessage("输入/bm help 查看帮助");
		}

		// Token: 0x06000063 RID: 99 RVA: 0x0000541C File Offset: 0x0000361C
		private void OnLeave(LeaveEventArgs args)
		{
			TSPlayer tsplr = TShock.Players[args.Who];
			BuildPlayer buildPlayer = ConfigUtils.players.Find((BuildPlayer p) => p.Name == tsplr.Name);
			if (buildPlayer != null)
			{
				BuildRoom roomByID = ConfigUtils.GetRoomByID(buildPlayer.CurrentRoomID);
				if (roomByID != null)
				{
					roomByID.Players.Remove(buildPlayer);
					roomByID.Broadcast("玩家 " + buildPlayer.Name + " 强制退出了房间", Color.Crimson);
				}
				buildPlayer.CurrentRoomID = 0;
				buildPlayer.IsReady = false;
				buildPlayer.Marked = false;
				buildPlayer.GiveMarks = 0;
				buildPlayer.AquiredMarks = 0;
				buildPlayer.SelectedTopic = "";
				if (buildPlayer.BackUp != null)
				{
					buildPlayer.BackUp.RestoreCharacter(buildPlayer);
				}
				buildPlayer.CurrentRegion = null;
				buildPlayer.Player = null;
				buildPlayer.BackUp = null;
				buildPlayer.Status = 0;
				buildPlayer.Locked = false;
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005500 File Offset: 0x00003700
		private void OnJoin(GreetPlayerEventArgs args)
		{
			TSPlayer tsplr = TShock.Players[args.Who];
			BuildPlayer buildPlayer = ConfigUtils.players.Find((BuildPlayer p) => p.Name == tsplr.Name);
			if (buildPlayer == null)
			{
				buildPlayer = new BuildPlayer(ConfigUtils.players.Count + 1, tsplr.Name, tsplr);
				ConfigUtils.players.Add(buildPlayer);
				return;
			}
			buildPlayer.Player = tsplr;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000557C File Offset: 0x0000377C
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.GamePostInitialize.Register(this, new HookHandler<EventArgs>(this.OnPostInitialize));
				ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.OnJoin));
				ServerApi.Hooks.ServerLeave.Register(this, new HookHandler<LeaveEventArgs>(this.OnLeave));
				GetDataHandlers.TogglePvp -= new EventHandler<GetDataHandlers.TogglePvpEventArgs>(this.OnTogglePVP);
				GetDataHandlers.TileEdit -= new EventHandler<GetDataHandlers.TileEditEventArgs>(this.OnTileEdit);
				GetDataHandlers.PlayerUpdate -= new EventHandler<GetDataHandlers.PlayerUpdateEventArgs>(this.OnPlayerUpdate);
				GetDataHandlers.PlayerTeam -= new EventHandler<GetDataHandlers.PlayerTeamEventArgs>(this.OnTeam);
				ConfigUtils.players.Clear();
			}
			base.Dispose(disposing);
		}
	}
}
