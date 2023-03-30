using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniGamesAPI.Enum;

namespace MiniGamesAPI
{
    public interface IRoom
    {
        
        int GetPlayerCount();
        void Initialize();
        void Dispose();
        void Conclude();
        void Start();
        void Stop();
        void Restore();
        void ShowRoomMemberInfo();
        void ShowVictory();
        void Broadcast(string msg,Color color);
    }
}
