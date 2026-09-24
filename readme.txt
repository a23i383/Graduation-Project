.venvと.venv_mlagetnsの2つの仮想環境を作る。（.venvはgymnasium用、.venv_mlagentsはML-Agents用)

.venvはPython 3.12.8で
pip install -r requirements.txt
をするといい。ただしノートPCで試したところ足りないモジュラーがあったらしいのでエラーに合わせて追加のインストールを行う。

.venv_mlagentsはPython 3.11.9で
pip install -r requirements_mlagents.txt

ModuleNotFoundError: No module named 'pkg_resources'
のエラーが出るはずなので
pip install "setuptools<82"
をする。ついでに
pip install "protobuf==3.20.3"
もする。（多分どこかで最新のprotobufを入れてしまっているため）

\.venv_mlagents\Lib\site-packages\mlagents\trainers\settings.py
のすべての
cattr.register_structure_hook(
    Dict[RewardSignalType, RewardSignalSettings], RewardSignalSettings.structure
)
を
cattr.register_structure_hook_func(
    lambda t: t == Dict[RewardSignalType, RewardSignalSettings],
    RewardSignalSettings.structure,
)
に書き換える。さらに
cattr.register_structure_hook(
    Dict[str, EnvironmentParameterSettings], EnvironmentParameterSettings.structure
)
を
cattr.register_structure_hook_func(
    lambda t: t == Dict[str, EnvironmentParameterSettings],
    EnvironmentParameterSettings.structure,
)
に書き換えること
