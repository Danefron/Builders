using System;
using Microsoft.Xna.Framework;
using MiniGamesAPI.Core;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using TShockAPI;

namespace BuildMaster
{
	// Token: 0x02000003 RID: 3
	public class BuildPlayer : MiniPlayer, IComparable<BuildPlayer>
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000008 RID: 8 RVA: 0x000020A8 File Offset: 0x000002A8
		// (set) Token: 0x06000009 RID: 9 RVA: 0x000020B0 File Offset: 0x000002B0
		public int AquiredMarks { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000A RID: 10 RVA: 0x000020B9 File Offset: 0x000002B9
		// (set) Token: 0x0600000B RID: 11 RVA: 0x000020C1 File Offset: 0x000002C1
		public MiniRegion CurrentRegion { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000020CA File Offset: 0x000002CA
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000020D2 File Offset: 0x000002D2
		public int GiveMarks { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000020DB File Offset: 0x000002DB
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000020E3 File Offset: 0x000002E3
		public string SelectedTopic { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000020EC File Offset: 0x000002EC
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000020F4 File Offset: 0x000002F4
		public bool Locked { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000020FD File Offset: 0x000002FD
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002105 File Offset: 0x00000305
		public bool Marked { get; set; }

		// Token: 0x06000014 RID: 20 RVA: 0x0000210E File Offset: 0x0000030E
		public BuildPlayer(int id, string name, TSPlayer player)
		{
			base.ID = id;
			base.Name = name;
			base.Player = player;
			base.CurrentRoomID = 0;
			this.CurrentRegion = null;
			this.Locked = false;
			this.Marked = false;
			this.AquiredMarks = 0;
               
                }
		// Token: 0x06000015 RID: 21 RVA: 0x00002150 File Offset: 0x00000350
		public void Join()
                {
                       if (MiniRoom room.GetPlayerCount() >= room.MaxPlayer)
                       {
                              this.SendInfoMessage("此房间满人了");
                              return;
                }
                if (room.Status != null)
                {
                        this.SendInfoMessage("该房间状态无法加入游戏");
                        return;
                 }
                if (ConfigUtils.GetRoomByID(base.CurrentRoomID) != null)
                {
                        this.Leave();
                }
                base.Join(room.ID);
                MiniRoom room.Players.Add(this);
                MiniRoom room.Broadcast("玩家 " + base.Name + " 加入了房间", Color.MediumAquamarine);
                this.Teleport(MiniRoom room.WaitingPoint);
                if (!MiniRoom room.Loaded)
                {
                        MiniRoom room.LoadRegion()
                }
             }

		// Token: 0x06000016 RID: 22 RVA: 0x000021F0 File Offset: 0x000003F0
		public void Leave()
		{
			MiniGamesAPI.Core.MiniRoom roomByID = ConfigUtils.GetRoomByID(base.CurrentRoomID);
			if (roomByID == null)
			{
				this.SendInfoMessage("房间不存在");
				return;
			}
			if (room.Status.Equals(null))
                        {
                            this.SendInfoMessage("该房间状态无法加入游戏");
                            return;
                         }
			roomByID.Players.Remove(this);
			roomByID.Broadcast("玩家 " + base.Name + " 离开了房间", Color.Crimson);
			this.Teleport(Main.spawnTileX, Main.spawnTileY);
			base.Leave();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002270 File Offset: 0x00000470
		public int CompareTo(BuildPlayer other)
		{
			return other.AquiredMarks.CompareTo(this.AquiredMarks);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002294 File Offset: 0x00000494
		public void Creative()
		{
			if (ConfigUtils.config.UnlockAll)
			{
				for (int i = 1; i <= 5042; i++)
				{
					NetPacket netPacket = NetCreativeUnlocksModule.SerializeItemSacrifice(i, 999);
					NetManager.Instance.SendToClient(netPacket, base.Player.Index);
				}
				return;
			}
			foreach (int num in ConfigUtils.config.Range.Keys)
			{
				for (int j = num; j < ConfigUtils.config.Range[num]; j++)
				{
					NetPacket netPacket2 = NetCreativeUnlocksModule.SerializeItemSacrifice(j, 999);
					NetManager.Instance.SendToClient(netPacket2, base.Player.Index);
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000236C File Offset: 0x0000056C
		public void UnCreative()
		{
			for (int i = 1; i <= 5042; i++)
			{
				NetPacket netPacket = NetCreativeUnlocksModule.SerializeItemSacrifice(i, 0);
				NetManager.Instance.SendToClient(netPacket, base.Player.Index);
			}
		}
	}
}
