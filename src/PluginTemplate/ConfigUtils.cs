using System;
using System.Collections.Generic;
using System.IO;
using MiniGamesAPI.Core;
using Newtonsoft.Json;
using TShockAPI;

namespace BuildMaster
{
	// Token: 0x02000005 RID: 5
	public static class ConfigUtils
	{
		// Token: 0x06000049 RID: 73 RVA: 0x00003308 File Offset: 0x00001508
		public static void LoadConfig()
		{
			if (!Directory.Exists(ConfigUtils.configDir))
			{
				Directory.CreateDirectory(ConfigUtils.configDir);
				File.WriteAllText(ConfigUtils.roomsPath, JsonConvert.SerializeObject(ConfigUtils.rooms, Newtonsoft.Json.Formatting.Indented));
				File.WriteAllText(ConfigUtils.evaluatePath, JsonConvert.SerializeObject(ConfigUtils.evaluatePack, Newtonsoft.Json.Formatting.Indented));
				File.WriteAllText(ConfigUtils.defaultPath, JsonConvert.SerializeObject(ConfigUtils.defaultPack, Newtonsoft.Json.Formatting.Indented));
				File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, Newtonsoft.Json.Formatting.Indented));
				return;
			}
			if (File.Exists(ConfigUtils.roomsPath))
			{
				List<BuildRoom> rooms = JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(ConfigUtils.roomsPath));
			}
			else
			{
				File.WriteAllText(ConfigUtils.roomsPath, JsonConvert.SerializeObject(ConfigUtils.rooms, Newtonsoft.Json.Formatting.Indented));
			}
			if (File.Exists(ConfigUtils.evaluatePath))
			{
				ConfigUtils.evaluatePack = JsonConvert.DeserializeObject<MiniPack>(File.ReadAllText(ConfigUtils.evaluatePath));
			}
			else
			{
				File.WriteAllText(ConfigUtils.evaluatePath, JsonConvert.SerializeObject(ConfigUtils.evaluatePack, Newtonsoft.Json.Formatting.Indented));
			}
			if (File.Exists(ConfigUtils.defaultPath))
			{
				ConfigUtils.defaultPack = JsonConvert.DeserializeObject<MiniPack>(File.ReadAllText(ConfigUtils.defaultPath));
			}
			else
			{
				File.WriteAllText(ConfigUtils.defaultPath, JsonConvert.SerializeObject(ConfigUtils.defaultPack, 1));
			}
			if (File.Exists(ConfigUtils.configPath))
			{
				ConfigUtils.config = JsonConvert.DeserializeObject<BuildConfig>(File.ReadAllText(ConfigUtils.configPath));
				return;
			}
			File.WriteAllText(ConfigUtils.configPath, JsonConvert.SerializeObject(ConfigUtils.config, 1));
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00003460 File Offset: 0x00001660
		public static void UpdateRooms(int id)
		{
			List<BuildRoom> list = JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(ConfigUtils.roomsPath));
			int num = list.FindIndex((BuildRoom r) => r.ID == id);
			list[num] = ConfigUtils.rooms.Find((BuildRoom r) => r.ID == id);
			File.WriteAllText(ConfigUtils.roomsPath, JsonConvert.SerializeObject(ConfigUtils.rooms, 1));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000034CD File Offset: 0x000016CD
		public static void AddRoom(int id)
		{
			JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(ConfigUtils.roomsPath)).Add(room);
			File.WriteAllText(ConfigUtils.roomsPath, JsonConvert.SerializeObject(ConfigUtils.rooms, 1));
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000034FC File Offset: 0x000016FC
		public static void RemoveRoom(int id)
		{
			JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(ConfigUtils.roomsPath)).RemoveAll((BuildRoom r) => r.ID == id);
			File.WriteAllText(ConfigUtils.roomsPath, JsonConvert.SerializeObject(ConfigUtils.rooms, 1));
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000354C File Offset: 0x0000174C
		public static MiniRoom GetRoomByID(int id)
		{
			return ConfigUtils.rooms.Find((BuildRoom r) => r.ID == id);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x0000357C File Offset: 0x0000177C
		public static MiniRoom GetRoomByIDFromLocal(int id)
		{
			return JsonConvert.DeserializeObject<List<BuildRoom>>(File.ReadAllText(ConfigUtils.roomsPath)).Find((BuildRoom r) => r.ID == id);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000035B8 File Offset: 0x000017B8
		public static BuildPlayer GetPlayerByName(string name)
		{
			return ConfigUtils.players.Find((BuildPlayer p) => p.Name == name);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000035E8 File Offset: 0x000017E8
		public static void UpdatePack()
		{
			File.WriteAllText(ConfigUtils.defaultPath, JsonConvert.SerializeObject(ConfigUtils.defaultPack, 1));
			File.WriteAllText(ConfigUtils.evaluatePath, JsonConvert.SerializeObject(ConfigUtils.evaluatePack, 1));
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003614 File Offset: 0x00001814
		public static void ReloadConfig()
		{
			ConfigUtils.config = JsonConvert.DeserializeObject<BuildConfig>(File.ReadAllText(ConfigUtils.configPath));
		}

		// Token: 0x0400001B RID: 27
		public static readonly string configDir = TShock.SavePath + "/BuildMaster";

		// Token: 0x0400001C RID: 28
		public static readonly string roomsPath = ConfigUtils.configDir + "/rooms.json";

		// Token: 0x0400001D RID: 29
		public static readonly string evaluatePath = ConfigUtils.configDir + "/eva.json";

		// Token: 0x0400001E RID: 30
		public static readonly string defaultPath = ConfigUtils.configDir + "/default.json";

		// Token: 0x0400001F RID: 31
		public static readonly string configPath = ConfigUtils.configDir + "/config.json";

		// Token: 0x04000020 RID: 32
		public static List<MiniRoom> rooms = new List<MiniRoom>();

		// Token: 0x04000021 RID: 33
		public static List<MiniPlayer> players = new List<MiniPlayer>();

		// Token: 0x04000022 RID: 34
		public static MiniPack defaultPack = new MiniPack("基础套", 2);

		// Token: 0x04000023 RID: 35
		public static MiniPack evaluatePack = new MiniPack("评分套", 3);

		// Token: 0x04000024 RID: 36
		public static BuildConfig config = new BuildConfig();
	}
}
