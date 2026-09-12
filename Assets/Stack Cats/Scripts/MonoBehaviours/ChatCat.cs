using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tofuwu.StackCats
{
    public delegate void Talking(ChatCatExpression expression);

    public enum ChatCatEmote
    {
        Normal,
        Surprised,
        Thinking,
        Heart
    }

    [Serializable]
    public class ChatCatExpression
    {
        public string Message;
        public float LetterInterval;
        public ChatCatEmote Emote;
    }

    public class ChatCat : MonoBehaviour
    {
        public event Talking onTalking;

        public void Say(string message, ChatCatEmote emote = ChatCatEmote.Normal, float letterInterval = 0.01f)
        {
            ChatCatExpression expression = new ChatCatExpression { Message = message, Emote = emote, LetterInterval = letterInterval };

            if (onTalking != null) onTalking(expression);
        }

        public void Say(List<string> messages)
        {
            foreach (string message in messages)
            {
                Say(message);
            }
        }
    }
}