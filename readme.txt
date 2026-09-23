.venvと.venv_mlagetnsの2つの仮想環境を作る。（.venvはgymnasium用、.venv_mlagentsはML-Agents用)

.venvはPython 3.12.8で
pip install -r requirements.txt
をするといい。ただしノートPCで試したところ足りないモジュラーがあったらしいのでエラーに合わせて追加のインストールを行う。

.venv_mlagentsはPython 3.11.9で
pip install -r requirements_mlagents.txt
ほかのPCでは試してないのでうまくいくかわからない。さらに
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
