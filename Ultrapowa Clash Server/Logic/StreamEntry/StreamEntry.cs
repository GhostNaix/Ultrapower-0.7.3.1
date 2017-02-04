using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UCS.Helpers;
using UCS.Logic.DataSlots;
using UCS.Files.Logic;
using UCS.Core;

namespace UCS.Logic.StreamEntry
{
    internal class StreamEntry
    {
        public StreamEntry()
        {
            m_vMessageTime = DateTime.UtcNow;
        }

        public List<DonationSlot> m_vUnitDonation;
        public List<BookmarkSlot> m_vDonatorList;
        private long m_vHomeId;
        private long m_vSenderId;
        private int m_vId;
        private int m_vSenderLeagueId;
        private int m_vSenderLevel;
        private int m_vSenderRole;
        private int m_vType        = -1;
        private string m_vSenderName;
        private DateTime m_vMessageTime;
        public string m_vMessage;
        public int m_vMaxTroop;
        public int m_vDonatedTroop = 0;
        public int m_vDonatedSpell = 0;
        public int m_vState        = 1; // 3 Refused - 2 Accepted - 1 Waiting
        public string m_vJudge;

        public virtual byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.AddInt32(GetStreamEntryType());
            data.AddInt32(0);
            data.AddInt32(m_vId);
            data.Add(3);
            data.AddInt64(m_vSenderId);
            data.AddInt64(m_vHomeId);
            data.AddString(m_vSenderName);
            data.AddInt32(m_vSenderLevel);
            data.AddInt32(m_vSenderLeagueId);
            data.AddInt32(m_vSenderRole);
            data.AddInt32(GetAgeSeconds());
            return data.ToArray();
        }

        public int GetAgeSeconds() => (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds -
        (int)m_vMessageTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public long GetHomeId() => m_vHomeId;

        public int GetId() => m_vId;

        public long GetSenderId() => m_vSenderId;

        public int GetSenderLeagueId() => m_vSenderLeagueId;

        public int GetSenderLevel() => m_vSenderLevel;

        public string GetSenderName() => m_vSenderName;

        public int GetSenderRole() => m_vSenderRole;

        public string GetMessage() =>  m_vMessage;

        public int GetMaxTroop() => m_vMaxTroop;

        public virtual int GetStreamEntryType() => m_vType;

        public virtual void Load(JObject jsonObject)
        {
            m_vType           = jsonObject["type"].ToObject<int>();
            m_vId             = jsonObject["id"].ToObject<int>();
            m_vSenderId       = jsonObject["sender_id"].ToObject<long>();
            m_vHomeId         = jsonObject["home_id"].ToObject<long>();
            m_vSenderLevel    = jsonObject["sender_level"].ToObject<int>();
            m_vSenderName     = jsonObject["sender_name"].ToObject<string>();
            m_vSenderLeagueId = jsonObject["sender_leagueId"].ToObject<int>();
            m_vSenderRole     = jsonObject["sender_role"].ToObject<int>();
            m_vMessageTime    = jsonObject["message_time"].ToObject<DateTime>();
        }

        public virtual JObject Save(JObject jsonObject)
        {
            jsonObject.Add("type", GetStreamEntryType());
            jsonObject.Add("id", m_vId);
            jsonObject.Add("sender_id", m_vSenderId);
            jsonObject.Add("home_id", m_vHomeId);
            jsonObject.Add("sender_level", m_vSenderLevel);
            jsonObject.Add("sender_name", m_vSenderName);
            jsonObject.Add("sender_leagueId", m_vSenderLeagueId);
            jsonObject.Add("sender_role", m_vSenderRole);
            jsonObject.Add("message_time", m_vMessageTime);

            return jsonObject;
        }

        public void SetSender(ClientAvatar avatar)
        {
            m_vSenderId       = avatar.GetId();
            m_vHomeId         = avatar.GetId();
            m_vSenderName     = avatar.GetAvatarName();
            m_vSenderLeagueId = avatar.GetLeagueId();
            m_vSenderLevel    = avatar.GetAvatarLevel();
            m_vSenderRole     = avatar.GetAllianceRole();
        }

        public void SetHomeId(long id) => m_vHomeId = id;

        public void SetId(int id) => m_vId = id;

        public void SetSenderId(long id) => m_vSenderId = id;

        public void SetSenderLeagueId(int leagueId) => m_vSenderLeagueId = leagueId;

        public void SetSenderLevel(int level) => m_vSenderLevel = level;

        public void SetSenderName(string name) => m_vSenderName = name;

        public void SetSenderRole(int role) => m_vSenderRole = role;

        public void SetMessage(string message) => m_vMessage = message;

        public void SetState(int status) => m_vState = status;

        public void SetJudgeName(string name) => m_vJudge = name;

        public void SetType(int type) => m_vType = type;

        public void SetMaxTroop(int size) => m_vMaxTroop = size;

        public void AddDonatedTroop(long did, int id, int value, int level)
        {
            m_vDonatedTroop = m_vDonatedTroop + ((CombatItemData)CSVManager.DataTables.GetDataById(id)).GetHousingSpace();
            DonationSlot e  = m_vUnitDonation.Find(t => t.ID == id && t.UnitLevel == level);
            if (e != null)
            {
                int i              = m_vUnitDonation.IndexOf(e);
                e.Count            = e.Count + value;
                m_vUnitDonation[i] = e;
            }
            else
            {
                DonationSlot ds = new DonationSlot(did, id, value, level);
                m_vUnitDonation.Add(ds);
            }
        }
    }
}
