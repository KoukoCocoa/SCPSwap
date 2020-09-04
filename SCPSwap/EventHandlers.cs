using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using Respawning;
using UnityEngine;
using Player = Exiled.API.Features.Player;


namespace ScpSwap
{
	public sealed class EventHandlers
	{
		private Dictionary<Player, Player> ongoingReqs = new Dictionary<Player, Player>();

		private List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
		private Dictionary<string, CoroutineHandle> reqCoroutines = new Dictionary<string, CoroutineHandle>();

		private bool allowSwaps = false;
		private bool isRoundStarted = false;

		private StringBuilder listBuilder = new StringBuilder();

		private Dictionary<string, RoleType> valid = new Dictionary<string, RoleType>(){};
		
		if 	plugin.Config.AllowScpsToChangeToOtherRoles 
		valid =  {"SCP-173", RoleType.Scp173},
			{"173", RoleType.Scp173},
			{"peanut", RoleType.Scp173},
			{"SCP-939", RoleType.Scp93953},
			{"939", RoleType.Scp93953},
			{"dog", RoleType.Scp93953},
			{"SCP-079", RoleType.Scp079},
			{"079", RoleType.Scp079},
			{"computer", RoleType.Scp079},
			{"SCP-106", RoleType.Scp106},
			{"106", RoleType.Scp106},
			{"larry", RoleType.Scp106},
			{"SCP-096", RoleType.Scp096},
			{"096", RoleType.Scp096},
			{"shyguy", RoleType.Scp096},
			{"SCP-049", RoleType.Scp049},
			{"049", RoleType.Scp049},
			{"doctor", RoleType.Scp049},
			{"SCP-049-2", RoleType.Scp0492},
			{"0492", RoleType.Scp0492},
			{"zombie", RoleType.Scp0492},
			{"Class-D Personnel", RoleType.ClassD},
			{"classd", RoleType.ClassD},
			{"Scientist", RoleType.ClassD},
			{"scientist", RoleType.Scientist},
			{"Facility Guard", RoleType.ClassD},
			{"guard", RoleType.FacilityGuard}
		
		else valid =  {"SCP-173", RoleType.Scp173},
			{"173", RoleType.Scp173},
			{"peanut", RoleType.Scp173},
			{"SCP-939", RoleType.Scp93953},
			{"939", RoleType.Scp93953},
			{"dog", RoleType.Scp93953},
			{"SCP-079", RoleType.Scp079},
			{"079", RoleType.Scp079},
			{"computer", RoleType.Scp079},
			{"SCP-106", RoleType.Scp106},
			{"106", RoleType.Scp106},
			{"larry", RoleType.Scp106},
			{"SCP-096", RoleType.Scp096},
			{"096", RoleType.Scp096},
			{"shyguy", RoleType.Scp096},
			{"SCP-049", RoleType.Scp049},
			{"049", RoleType.Scp049},
			{"doctor", RoleType.Scp049},
			{"SCP-049-2", RoleType.Scp0492},
			{"0492", RoleType.Scp0492},
			{"zombie", RoleType.Scp0492},

		public ScpSwap plugin;

		public EventHandlers(ScpSwap plugin) => this.plugin = plugin;

		private IEnumerator<float> SendRequest(Player source, Player dest)
		{
			ongoingReqs.Add(dest, source);
			dest.Broadcast(5, "<b>You have received a role swap request!\nCheck your console by pressing <color=#8A2BE2>~</color></b>");
			dest.ReferenceHub.characterClassManager.TargetConsolePrint(dest.ReferenceHub.scp079PlayerScript.connectionToClient, $"You have received a role swap request from {source.ReferenceHub.nicknameSync.Network_myNickSync} who is {valid.FirstOrDefault(x => x.Value == source.Role).Key}. Would you like to swap with them? Type \".scpswap yes\" to accept or \".scpswap no\" to decline.", "yellow");
			yield return Timing.WaitForSeconds(plugin.Config.SwapRequestTimeout);
			TimeoutRequest(source, dest);
		}

		public void OnChangingRole(ChangingRoleEventArgs ev)
		{
			Timing.CallDelayed(1.5f, () =>
			{
				if (plugin.Config.DisallowedScpCompinations.ContainsKey((int)ev.NewRole) && !Player.Get((RoleType)plugin.Config.DisallowedScpCompinations[(int)ev.NewRole]).IsEmpty())
				{
					var scplist = new List<RoleType> {RoleType.Scp049, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989};
					var random = new System.Random();
					int scplistindex = random.Next(scplist.Count);
					while (ev.NewRole == scplist[scplistindex]) scplistindex = random.Next(scplist.Count);
					ev.Player.Role = scplist[scplistindex];
				}
			});
		}
		
		private void TimeoutRequest(Player source, Player dest)
		{
			if (ongoingReqs.ContainsKey(dest))
			{
				dest.ReferenceHub.characterClassManager.TargetConsolePrint(dest.ReferenceHub.scp079PlayerScript.connectionToClient, "The swap request has timed out.", "red");
				source.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "No players responded to your request.", "red");
				ongoingReqs.Remove(dest);
			}
		}

		private void PerformSwap(Player source, Player dest)
		{
			source.ReferenceHub.characterClassManager.TargetConsolePrint(source.ReferenceHub.scp079PlayerScript.connectionToClient, "Swap successful!", "green");

			RoleType sRole = source.Role;
			RoleType dRole = dest.Role;

			Vector3 sPos = source.Position;
			Vector3 dPos = dest.Position;

			float sHealth = source.Health;
			float dHealth = dest.Health;

			source.Role = dRole;
			source.Position = dPos;
			source.Health = dHealth;

			dest.Role = sRole;
			dest.Position = sPos;
			dest.Health = sHealth;

			ongoingReqs.Remove(dest);
		}

		public void OnRoundStart()
		{
			allowSwaps = true;
			isRoundStarted = true;
			Timing.CallDelayed(plugin.Config.SwapTimeout, () => allowSwaps = false);
			Timing.CallDelayed(1f, () => BroadcastMessage());
		}

		public void OnRoundRestart()
		{
			isRoundStarted = false;
			Timing.KillCoroutines(coroutines);
			Timing.KillCoroutines(reqCoroutines.Values);
			coroutines.Clear();
			reqCoroutines.Clear();
		}

		public void OnRoundEnd(RoundEndedEventArgs ev)
		{
			isRoundStarted = false;
			Timing.KillCoroutines(coroutines);
			Timing.KillCoroutines(reqCoroutines.Values);
			coroutines.Clear();
			reqCoroutines.Clear();
		}

		public void OnWaitingForPlayers()
		{
			allowSwaps = false;
		}

		public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
		{
			if (ev.Name.ToLower().Contains("scpswap"))
			{
				ev.Allow = false;
				if (!isRoundStarted)
				{
					ev.ReturnMessage = "The round hasn't started yet!";
					ev.Color = "red";
					return;
				}

				if (!allowSwaps)
				{
					ev.ReturnMessage = "SCP swap period has expired.";
					ev.Color = "red";
					return;
				}

				switch (ev.Arguments.Count)
				{
					case 1:
						switch (ev.Arguments[0].ToLower())
						{
							case "yes":
								if (ongoingReqs.ContainsKey(ev.Player))
								{
									var swap = ongoingReqs[ev.Player];
									PerformSwap(swap, ev.Player);
									ev.ReturnMessage = "Swap successful!";
									foreach(KeyValuePair<string, CoroutineHandle> entry in reqCoroutines)
									{
										if (entry.Key.Contains($"{swap}")) Timing.KillCoroutines(entry.Value);
										reqCoroutines.Remove(entry.Key);
									}
									ev.Color = "green";
									return;
								}
								ev.ReturnMessage = "You do not have a swap request.";
								break;
							case "no":
								if (ongoingReqs.ContainsKey(ev.Player))
								{
									var swap = ongoingReqs[ev.Player];
									ev.ReturnMessage = "1 swap request denied.";
									swap.ReferenceHub.characterClassManager.TargetConsolePrint(swap.ReferenceHub.scp079PlayerScript.connectionToClient, "Your swap request has been denied.", "red");
									Timing.KillCoroutines(reqCoroutines[$"{swap}{ev.Player}"]);
									reqCoroutines.Remove($"{swap}{ev.Player}");
									ongoingReqs.Remove(ev.Player);
									return;
								}
								ev.ReturnMessage = "You do not have a swap reqest.";
								break;
							case "cancel":
								foreach(KeyValuePair<Player, Player> entry in ongoingReqs)
								{
									if (entry.Value == ev.Player)
									{
										entry.Key.ReferenceHub.characterClassManager.TargetConsolePrint(entry.Key.ReferenceHub.scp079PlayerScript.connectionToClient, "The swap request has been cancelled.", "red");
										ongoingReqs.Remove(entry.Key);
									}
								}
								foreach(KeyValuePair<string, CoroutineHandle> entry in reqCoroutines)
								{
									if (entry.Key.Contains($"{ev.Player}")) Timing.KillCoroutines(entry.Value);
									reqCoroutines.Remove(entry.Key);
								}
								ev.ReturnMessage = "You have cancelled your swap request.";
								return;
							case "list":
								listBuilder.AppendLine("Here are the available roles to swap to (Some may be blacklisted):");
								foreach (KeyValuePair<string, RoleType> kvp in valid)
								{
									listBuilder.Append(kvp.Key);
									listBuilder.Append(" (");
									listBuilder.Append((int)kvp.Value);
									listBuilder.AppendLine(")");
								}
								string message = listBuilder.ToString();
								listBuilder.Clear();
								ev.ReturnMessage = message;
								break;
							default:
								if (!(ev.Player.Team == Team.SCP))
								{
									ev.ReturnMessage = "You're not an SCP. You can't swap to another role.";
									ev.Color = "red";
									return;
								}
								
								if (!valid.ContainsKey(ev.Arguments[0]))
								{
									ev.ReturnMessage = "Invalid role.";
									ev.Color = "red";
									return;
								}
								
								var requests = 0;
								foreach(KeyValuePair<Player, Player> entry in ongoingReqs)
								{
									if (entry.Value == ev.Player)  requests += 1;
								}
							
								if (requests > 0)
								{
									ev.ReturnMessage = $"You already have {requests} request pending!";
									ev.Color = "red";
									return;
								}

								RoleType role = valid[ev.Arguments[0]];
								
								if (plugin.Config.SwapBlacklist.Contains((int)role))
								{
									ev.ReturnMessage = "That role is blacklisted.";
									ev.Color = "red";
									return;
								}
								
								if (ev.Player.Role == role)
								{
									ev.ReturnMessage = "You cannot swap with your own role.";
									ev.Color = "red";
									return;
								}
								
								if ((plugin.Config.DisallowedScpCompinations.ContainsKey((int)role) && !Player.Get((RoleType)plugin.Config.DisallowedScpCompinations[(int)role]).IsEmpty())
								&& (ev.Player.Role != (RoleType)plugin.Config.DisallowedScpCompinations[(int)role] || (ev.Player.Role == (RoleType)plugin.Config.DisallowedScpCompinations[(int)role] && Player.Get(ev.Player.Role).ToList().Count > 1)))
								{
									ev.ReturnMessage = $"{role} and {(RoleType)plugin.Config.DisallowedScpCompinations[(int)role]} cannot be in the same round.";
									ev.Color = "red";
									return;
								}
								
								List<Player> pList = Player.List.Where(x => x.Role == role && x.UserId != null && x.UserId != string.Empty).ToList();
								if (!plugin.Config.SwapAllowDuplicatesOfTheSameScp || role == RoleType.ClassD || role == RoleType.Scientist || role == RoleType.FacilityGuard)
								{
									foreach (Player ply in pList)
									{
										reqCoroutines.Add($"{ev.Player}{ply}", Timing.RunCoroutine(SendRequest(ev.Player, ply)));
									}
									ev.ReturnMessage = "Swap requests sent!";
									ev.Color = "green";
					
									return;
								}
								if (plugin.Config.SwapAllowNewScps)
								{
									ev.ReturnMessage = "You have been made the specified role.";
									ev.Color = "green";
									ev.Player.Role = role;
									return;
								}
								ev.ReturnMessage = "No players found to swap with.";
								ev.Color = "red";
								break;
						}
						break;
					default:
						ev.ReturnMessage = "USAGE: SCPSWAP INSERTROLE";
						ev.Color = "red";
						break;
				}
			}
		}

		public void BroadcastMessage()
		{
			if (plugin.Config.DisplayStartMessage)
			{
				foreach (Player ply in Player.List)
					if (ply.Role.IsSCP())
						ply.Broadcast(plugin.Config.StartMessageTime, plugin.Config.DisplayMessageText);
			}
		}
	}
}
