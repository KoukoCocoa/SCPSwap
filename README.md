# An improved version of [KoukoCocoa/SCPSwap](https://github.com/KoukoCocoa/SCPSwap) with these added options:
- option to allow duplicates of the same SCP
- an option to disallow specific SCP combinations
- an option to allow SCPs to change to class-d/scientist/guard, if someone with that role already exists, they'll receive a role swap request they can accept or reject.

# Installation

Install the latest of version of [EXILED](https://github.com/galaxy119/EXILED) if you don't have it, then download [ScpSwap.dll](https://github.com/Aevann1/SCPSwap/releases) and place it in in %appdata%\EXILED\Plugins

# Default configs:
```yaml
  is_enabled: true
  display_start_message: true
  swap_allow_new_scps: false
  swap_allow_duplicates_of_the_same_scp: false
  swap_timeout: 60
  swap_request_timeout: 20
  start_message_time: 15
  swap_blacklist:
  - 10
  disallowed_scp_compinations:
    0: 9
    9: 0
  allow_scps_to_change_to_other_roles: true
  display_message_text: <b>You can change your role through the console (~ key) by typing <color=purple>.scpswap insert_role</color> If someone with that role already exists, they'll receive a request to swap they can accept or reject. You can also use <color=purple>.scpswap list</color> to list all available roles.
```
