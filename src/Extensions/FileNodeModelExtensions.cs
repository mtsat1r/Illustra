using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using Illustra.Models;
using Illustra.Helpers;

namespace Illustra.Extensions
{
    /// <summary>
    /// FileNodeModelの拡張メソッド
    /// </summary>
    public static class FileNodeModelExtensions
    {
        // LRUキャッシュのインスタンス（静的）
        private static LruThumbnailCache? _thumbnailCache;

        // サムネイル状態を保持する静的ディクショナリ
        private static readonly ConditionalWeakTable<FileNodeModel, ThumbnailState> _thumbnailStates =
            new ConditionalWeakTable<FileNodeModel, ThumbnailState>();

        // サムネイル状態を保持するクラス
        private class ThumbnailState
        {
            public bool HasThumbnail { get; set; }
            public bool IsLoadingThumbnail { get; set; }
        }

        /// <summary>
        /// LRUキャッシュを初期化します
        /// </summary>
        /// <param name="maxCacheSize">最大キャッシュサイズ</param>
        public static void InitializeThumbnailCache(int maxCacheSize = 50)
        {
            _thumbnailCache = new LruThumbnailCache(maxCacheSize);
        }

        /// <summary>
        /// サムネイルが存在するかどうかを取得します
        /// </summary>
        public static bool HasThumbnail(this FileNodeModel model)
        {
            if (_thumbnailCache == null) return false;
            return _thumbnailCache.HasImage(model.FullPath);
        }

        /// <summary>
        /// サムネイルが読み込み中かどうかを取得します
        /// </summary>
        public static bool IsLoadingThumbnail(this FileNodeModel model)
        {
            var state = _thumbnailStates.GetOrCreateValue(model);
            return state.IsLoadingThumbnail;
        }

        /// <summary>
        /// サムネイルが存在するかどうかを設定します
        /// </summary>
        public static void SetHasThumbnail(this FileNodeModel model, bool value)
        {
            var state = _thumbnailStates.GetOrCreateValue(model);
            state.HasThumbnail = value;
        }

        /// <summary>
        /// サムネイルが読み込み中かどうかを設定します
        /// </summary>
        public static void SetIsLoadingThumbnail(this FileNodeModel model, bool value)
        {
            var state = _thumbnailStates.GetOrCreateValue(model);
            state.IsLoadingThumbnail = value;
        }

        /// <summary>
        /// サムネイル画像を取得します（LRUキャッシュから）
        /// </summary>
        public static BitmapSource? GetThumbnail(this FileNodeModel model)
        {
            if (_thumbnailCache == null) return null;
            return _thumbnailCache.GetImage(model.FullPath);
        }

        /// <summary>
        /// サムネイル画像を設定します（LRUキャッシュに追加）
        /// </summary>
        public static void SetThumbnail(this FileNodeModel model, BitmapSource? value)
        {
            var state = _thumbnailStates.GetOrCreateValue(model);
            state.HasThumbnail = value != null;
            
            if (value != null && _thumbnailCache != null)
            {
                // LRUキャッシュに追加（既存の場合は更新される）
                try
                {
                    _thumbnailCache.GetImage(model.FullPath); // キャッシュに追加
                }
                catch
                {
                    // エラーが発生した場合は無視
                }
            }
        }

        /// <summary>
        /// キャッシュをクリアします
        /// </summary>
        public static void ClearThumbnailCache()
        {
            _thumbnailCache?.Clear();
        }

        /// <summary>
        /// キャッシュサイズを更新します
        /// </summary>
        /// <param name="maxCacheSize">新しい最大キャッシュサイズ</param>
        public static void UpdateCacheSize(int maxCacheSize)
        {
            _thumbnailCache = new LruThumbnailCache(maxCacheSize);
        }
    }
}