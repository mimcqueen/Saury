using System.Collections.Generic;
using Saury.Dad;
using UnityEngine;

namespace Saury.Lobby
{
    /// <summary>
    /// Local / same-PC lobby stub. Listen-server / Steam lobbies can replace this later.
    /// Lobby owner still plays as a dad — Host is CPU only.
    /// </summary>
    [System.Serializable]
    public class LocalLobbyStub
    {
        public string roomCode { get; private set; } = "SAUR";
        public int ownerSlot { get; private set; }
        public List<DadSeed> seats { get; private set; } = new List<DadSeed>();

        public void OpenLocal(int playerCount = 4)
        {
            playerCount = Mathf.Clamp(playerCount, 2, 6);
            ownerSlot = 0;
            roomCode = MakeCode();
            seats.Clear();

            string[] names = { "Mitch", "Doug", "Kyle", "Randy", "Troy", "Chad" };
            for (int i = 0; i < playerCount; i++)
            {
                seats.Add(new DadSeed
                {
                    slotIndex = i,
                    displayName = names[i % names.Length],
                    creature = (Core.CreatureId)(i % 6),
                    bodyColor = Color.HSVToRGB((i * 0.17f) % 1f, 0.5f, 0.8f),
                    ready = false
                });
            }
        }

        public bool AllReady()
        {
            if (seats.Count == 0) return false;
            foreach (var s in seats)
                if (!s.ready) return false;
            return true;
        }

        public void MarkReady(int slot)
        {
            var dad = seats.Find(d => d.slotIndex == slot);
            if (dad != null) dad.ready = true;
        }

        static string MakeCode()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            char[] c = new char[4];
            for (int i = 0; i < 4; i++)
                c[i] = alphabet[Random.Range(0, alphabet.Length)];
            return new string(c);
        }
    }
}
