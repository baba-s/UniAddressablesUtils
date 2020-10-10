using System;
using System.Collections;
using System.IO;
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
	}
}