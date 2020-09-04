using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

namespace ScpSwap
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool DisplayStartMessage { get; set; } = true;
        public float SwapTimeout { get; set; } = 60f;
        public float SwapRequestTimeout { get; set; } = 20f;
        public ushort StartMessageTime { get; set; } = 15;
        public List<int> SwapBlacklist { get; private set; } = new List<int>() { 10 };
        public bool SwapAllowNewScps { get; set; } = false;
        public bool SwapAllowDuplicatesOfTheSameScp { get; set; } = false;
        public Dictionary<int, int> DisallowedScpCompinations { get; set; } = new Dictionary<int, int>
		{
			{
				0, 9
			},
			{
				9, 0
			},
		};
        public bool AllowScpsToChangeToOtherRoles { get; set; } = true;
        public string DisplayMessageText { get; set; } = "<b>You can change your role through the console (~ key) by typing <color=purple>.scpswap insert_role</color> If someone with that role already exists, they'll receive a request to swap they can accept or reject. You can also use <color=purple>.scpswap list</color> to list all available roles.";
	}
}
