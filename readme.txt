\GraduationProject\.venv_mlagents\Lib\site-packages\mlagents\trainers\settings.py
のすべての
cattr.register_structure_hook(
    Dict[RewardSignalType, RewardSignalSettings], RewardSignalSettings.structure
)
を
cattr.register_structure_hook_func(
    lambda t: t == Dict[RewardSignalType, RewardSignalSettings],
    RewardSignalSettings.structure,
)
に書き換えること