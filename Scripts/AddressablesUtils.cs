using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;

#endif

namespace Kogane
{
	/// <summary>
	/// Addressable に関する汎用的な処理を管理するクラス
	/// </summary>
	public static class AddressablesUtils
	{
		//================================================================================
		// 関数(static)
		//================================================================================
#if UNITY_EDITOR

		/// <summary>
		/// 現在の Play Mode Script の情報を取得します
		/// </summary>
		private static BuildScriptBase GetPlayModeScript()
		{
			var settings = AddressableAssetSettingsDefaultObject.Settings;
			var index    = settings.ActivePlayModeDataBuilderIndex;
			var builder  = ( BuildScriptBase ) settings.DataBuilders[ index ];

			return builder;
		}
#endif

		/// <summary>
		/// <para>以下のような Play Mode Script の名前を取得します</para>
		/// <para>・Use Asset Database</para>
		/// <para>・Simulate Groups</para>
		/// <para>・Use Existing Build</para>
		/// </summary>
		public static string GetPlayModeScriptName()
		{
#if UNITY_EDITOR

			var builder = GetPlayModeScript();

			return builder.Name;
#else
			return string.Empty;
#endif
		}

		/// <summary>
		/// Streaming Assets フォルダから catalog.json を読み込みます
		/// </summary>
		public static IEnumerator LoadCatalogJson( Action<string> onComplete )
		{
			return LoadImpl( "catalog.json", json => onComplete( json ) );
		}

		/// <summary>
		/// Streaming Assets フォルダから link.xml を読み込みます
		/// </summary>
		public static IEnumerator LoadLinkXml( Action<string> onComplete )
		{
			return LoadImpl( "link.xml", xml => onComplete( xml ) );
		}

		/// <summary>
		/// Streaming Assets フォルダから settings.json を読み込みます
		/// </summary>
		public static IEnumerator LoadSettingsJson( Action<string> onComplete )
		{
			return LoadImpl( "settings.json", json => onComplete( json ) );
		}

		/// <summary>
		/// <para>Streaming Assets フォルダから settings.json を読み込みます</para>
		/// <para>Unity エディタの場合は Library フォルダから読み込みます</para>
		/// </summary>
		private static IEnumerator LoadImpl( string filename, Action<string> onComplete )
		{
			var path = $"{Addressables.RuntimePath}/{filename}";

			if ( Application.isEditor )
			{
				var text = File.ReadAllText( path );

				onComplete( text );
			}
			else
			{
				var www = UnityWebRequest.Get( path );

				yield return www.SendWebRequest();

				var text = www.downloadHandler.text;

				onComplete( text );
			}
		}

		/// <summary>
		/// Addressable の初期化処理が失敗した場合に初期化前の状態に戻す為の関数
		/// </summary>
		public static void ResetInitializationFlag()
		{
			ResetInitializationFlag( true );
		}

		/// <summary>
		/// Addressable の初期化処理が失敗した場合に初期化前の状態に戻す為の関数
		/// </summary>
		public static void ResetInitializationFlag( bool version_1_9_2_OrNewer )
		{
			// Addressables.InitializeAsync に失敗した場合も、内部では初期化済みフラグが立ってしまうため、
			// リフレクションを使用して内部の初期化フラグを落とします
			var addressablesType     = typeof( Addressables );
			var assembly             = addressablesType.Assembly;
			var addressablesImplType = assembly.GetType( "UnityEngine.AddressableAssets.AddressablesImpl" );

			var addressablesImpl = version_1_9_2_OrNewer
				? addressablesType.GetProperty( "m_Addressables", BindingFlags.Static | BindingFlags.NonPublic ).GetValue( null )
				: addressablesType.GetField( "m_Addressables", BindingFlags.Static | BindingFlags.NonPublic ).GetValue( null );

			var hasStartedInitializationField = addressablesImplType.GetField( "hasStartedInitialization", BindingFlags.Instance | BindingFlags.NonPublic );

			hasStartedInitializationField.SetValue( addressablesImpl, false );

			// Addressables.InitializeAsync に失敗した場合も、
			// ローカルカタログから情報が読み込まれてしまうため、
			// 読み込まれているローカルカタログの情報を破棄します
			Addressables.ClearResourceLocators();
		}
	}
}