# Improved version of [KoukoCocoa/SCPSwap](https://github.com/KoukoCocoa/SCPSwap) with with these added options:
- option to allow duplicates of the same SCP
- an option to disallow specific SCP combinations
- an option to allow SCPs to change to class-d/scientist/guard, if someone with that role already exists, they'll receive a role swap request they can accept or reject.

# Installation

Download [ScpSwap.dll](https://github.com/Aevann1/SCPSwap/releases) and place it in in %appdata%\EXILED\Plugins

# Default configs:
```yaml
  is_enabled: true
  display_start_message: true
  swap_allow_new_scps: false
  swap_allow_duplicates_of_the_same_scp: false
  swap_timeout: 60
  swap_request_timeout: 20
  start_message_time: 15
  display_message_text: <color=yellow><b>Did you know you can swap classes with other SCP's?</b></color> Simply type <color=orange>.scpswap (role number)</color> in your in-game console (not RA) to swap!
  swap_blacklist:
  - 10
  disallowed_scp_compinations:
    0: 9
    9: 0
```

