using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

namespace ScpSwap
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool DisplayStartMessage { get; set; } = true;
        public bool SwapAllowNewScps { get; set; } = false;
        public bool SwapAllowDuplicatesOfTheSameScp { get; set; } = false;
        public float SwapTimeout { get; set; } = 60f;
        public float SwapRequestTimeout { get; set; } = 20f;
        public ushort StartMessageTime { get; set; } = 15;
        public string DisplayMessageText { get; set; } = "<color=yellow><b>Did you know you can swap classes with other SCP's?</b></color> Simply type <color=orange>.scpswap (role number)</color> in your in-game console (not RA) to swap!";
        public List<int> SwapBlacklist { get; private set; } = new List<int>() { 10 };
    }
}
