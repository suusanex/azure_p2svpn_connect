# azure_p2svpn_connect
Azure Point to Site VPNへの接続を自動で行うスクリプト

Azure Point to Site VPNへの接続は、専用の画面を表示して接続ボタンを押す必要があるため、単純なスクリプト実行ができない。

その問題をUWSCを併用することで解決するスクリプト。

## 使い方

1. UWSCを入手し、本スクリプトと同じフォルダへ配置する。（UWSCのライセンスに従ってください）http://www.uwsc.info/
1. 「AzureVMConnectPush_[Your VPN Connect Name].UWS」のファイル名の[Your VPN Connect Name]をVPN接続先の名前に書き換える
1. 「AzureP2SVPNConnect.csx」の中に書かれている[Your VPN Connect Name]をVPN接続先の名前に書き換える
1. csharp_script_launcherを入手してビルドし、本スクリプトと同じフォルダへ配置する。https://github.com/suusanex/csharp_script_launcher
1. csharp_script_launcherを使ってスクリプトを呼び出す。バッチファイル等を作ってスタートアップに登録すれば、スタートアップでのVPN接続が可能。

