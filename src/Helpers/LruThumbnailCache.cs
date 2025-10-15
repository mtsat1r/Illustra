using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Illustra.Models;
using Illustra.Helpers.Interfaces;

namespace Illustra.Helpers
{
    /// <summary>
    /// LRU（Least Recently Used）方式のサムネイルキャッシュ実装
    /// </summary>
    public class LruThumbnailCache : IImageCache
    {
        private readonly int _maxCacheSize;
        private readonly Dictionary<string, CacheEntry> _cache;
        private readonly LinkedList<string> _accessOrder;

        /// <summary>
        /// キャッシュエントリ
        /// </summary>
        private class CacheEntry
        {
            public BitmapSource Image { get; set; }
            public LinkedListNode<string> AccessNode { get; set; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="maxCacheSize">最大キャッシュサイズ</param>
        public LruThumbnailCache(int maxCacheSize = 50)
        {
            _maxCacheSize = maxCacheSize;
            _cache = new Dictionary<string, CacheEntry>();
            _accessOrder = new LinkedList<string>();
        }

        /// <inheritdoc/>
        public BitmapSource? GetImage(string path)
        {
            if (_cache.TryGetValue(path, out var entry))
            {
                // アクセス順序を更新（LRUの核心部分）
                _accessOrder.Remove(entry.AccessNode);
                entry.AccessNode = _accessOrder.AddLast(path);
                
                return entry.Image;
            }

            // キャッシュミスした場合は読み込んでキャッシュに追加
            try
            {
                var newImage = LoadImageFromFile(path);
                AddToCache(path, newImage);
                return newImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"画像の読み込みエラー: {ex.Message}");
                throw; // 呼び出し元に例外を伝播
            }
        }

        /// <inheritdoc/>
        public bool HasImage(string path)
        {
            return _cache.ContainsKey(path);
        }

        /// <inheritdoc/>
        public void UpdateCache(List<FileNodeModel> files, int currentIndex)
        {
            // 現在のファイルが画像ファイルかチェック
            var currentFile = files[currentIndex];
            if (!FileHelper.IsImageFile(currentFile.FullPath))
            {
                return; // 画像ファイルでない場合は何もしない
            }

            // 現在のファイルをキャッシュに追加（まだない場合）
            if (!_cache.ContainsKey(currentFile.FullPath))
            {
                try
                {
                    var image = LoadImageFromFile(currentFile.FullPath);
                    AddToCache(currentFile.FullPath, image);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"画像のプリロードエラー: {ex.Message}");
                }
            }
        }

        /// <inheritdoc/>
        public void PreloadCache(List<FileNodeModel> files, int currentIndex)
        {
            // 画像ファイルのみをフィルタリング
            var imageFiles = files.Where(f => FileHelper.IsImageFile(f.FullPath)).ToList();
            
            if (imageFiles.Count == 0) return;

            // 現在位置の前後数枚をプリロード
            var preloadCount = Math.Min(5, _maxCacheSize / 2); // 最大キャッシュサイズの半分まで
            var startIndex = Math.Max(0, currentIndex - preloadCount);
            var endIndex = Math.Min(imageFiles.Count - 1, currentIndex + preloadCount);

            for (int i = startIndex; i <= endIndex; i++)
            {
                var filePath = imageFiles[i].FullPath;
                if (!_cache.ContainsKey(filePath))
                {
                    try
                    {
                        var image = LoadImageFromFile(filePath);
                        AddToCache(filePath, image);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"画像のプリロードエラー: {ex.Message}");
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _cache.Clear();
            _accessOrder.Clear();
        }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, BitmapSource> CachedItems => 
            _cache.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Image);

        /// <summary>
        /// キャッシュに画像を追加し、必要に応じて古いエントリを削除
        /// </summary>
        private void AddToCache(string path, BitmapSource image)
        {
            // キャッシュサイズ制限チェック
            if (_cache.Count >= _maxCacheSize)
            {
                // 最も古いエントリ（最初の要素）を削除
                var oldestPath = _accessOrder.First.Value;
                var oldestEntry = _cache[oldestPath];
                
                _accessOrder.Remove(oldestEntry.AccessNode);
                _cache.Remove(oldestPath);
                
                System.Diagnostics.Debug.WriteLine($"LRUキャッシュから削除: {oldestPath}");
            }

            // 新しいエントリを追加
            var accessNode = _accessOrder.AddLast(path);
            _cache[path] = new CacheEntry
            {
                Image = image,
                AccessNode = accessNode
            };
        }

        /// <summary>
        /// ファイルから画像を読み込む
        /// </summary>
        private static BitmapSource LoadImageFromFile(string path)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.UriSource = new Uri(path);
            image.EndInit();
            image.Freeze();
            return image;
        }
    }
}
