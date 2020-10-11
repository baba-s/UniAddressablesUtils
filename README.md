# UniAddressablesUtils

Addressable に関する汎用的な関数を管理するクラス

## 使用例

```cs
using Kogane;
using UnityEngine;

public class Example : MonoBehaviour
{
    private void Start()
    {
        // 現在の Play Mode Script の名前を取得
        // 取得できる名前は以下のようなもの
        // 
        // ・Use Asset Database (fastest)
        // ・Default Build Script
        // ・Use Existing Build (requires built groups)
        // ・Simulate Groups (advanced)
        Debug.Log( AddressablesUtils.GetPlayModeScriptName() );

        // Streaming Assets フォルダから catalog.json を読み込み
        StartCoroutine( AddressablesUtils.LoadCatalogJson( json => Debug.Log( json ) ) );

        // Streaming Assets フォルダから link.xml を読み込み
        StartCoroutine( AddressablesUtils.LoadLinkXml( xml => Debug.Log( xml ) ) );

        // Streaming Assets フォルダから settings.json を読み込み
        StartCoroutine( AddressablesUtils.LoadSettingsJson( json => Debug.Log( json ) ) );

        // Addressable の初期化処理が失敗した場合に初期化前の状態に戻す為の関数
        AddressablesUtils.ResetInitializationFlag();

        // Addressable 1.8.5 以下のバージョンなら引数に false を渡さないとエラーになります
        //AddressablesUtils.ResetInitializationFlag( false );
    }
}
```
