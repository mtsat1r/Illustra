using System.Windows.Media;

namespace Illustra.Helpers
{
    /// <summary>
    /// レーティングの表示に関するヘルパーメソッドを提供するクラス
    /// </summary>
    public static class RatingHelper
    {
        public enum RatingTheme
        {
            GoldOrange,
            Blue,
            Green,
            Red,
            Colorful
        }
        /// <summary>
        /// レーティング値に対応した色を取得します
        /// </summary>
        /// <param name="rating">レーティング値 (0=なし, 1=あり)</param>
        /// <returns>レーティング値に対応した色のブラシ</returns>
        public static Brush GetRatingColor(int rating, RatingTheme theme = RatingTheme.Colorful)
        {
            return theme switch
            {
                RatingTheme.GoldOrange => rating switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(249, 168, 37)),  // Gold
                    _ => Brushes.Gray
                },

                RatingTheme.Blue => rating switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(66, 165, 245)),  // Soft Blue
                    _ => Brushes.Gray
                },

                RatingTheme.Green => rating switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(102, 187, 106)), // Lime Green
                    _ => Brushes.Gray
                },

                RatingTheme.Red => rating switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(229, 115, 115)), // Moderate Red
                    _ => Brushes.Gray
                },

                RatingTheme.Colorful => rating switch
                {
                    1 => new SolidColorBrush(Color.FromRgb(230, 81, 0)),    // Dark Orange
                    _ => Brushes.Gray
                },

                _ => Brushes.Gray
            };
        }

        /// <summary>
        /// レーティング値とテーマに応じたテキスト色を取得します
        /// </summary>
        /// <param name="rating">レーティング値 (0=なし, 1=あり)</param>
        /// <param name="theme">テーマ</param>
        /// <returns>テキスト色のブラシ</returns>
        public static SolidColorBrush GetTextColor(int rating = 0, RatingTheme theme = RatingTheme.Colorful)
        {
            // レーティングが0の場合はデフォルト色を返す
            if (rating <= 0)
            {
                return new SolidColorBrush(Color.FromRgb(96, 96, 96)); // デフォルトのダークグレー
            }

            // レーティング値に応じて暗さを調整
            double brightness = 1.0;

            // テーマ別の基本色を取得
            Color baseColor = theme switch
            {
                RatingTheme.GoldOrange => Color.FromRgb(255, 248, 225), // 薄い金色
                RatingTheme.Blue => Color.FromRgb(187, 222, 251),       // 薄い青
                RatingTheme.Green => Color.FromRgb(200, 230, 201),      // 薄い緑
                RatingTheme.Red => Color.FromRgb(255, 205, 210),        // 薄いピンク
                _ => Color.FromRgb(245, 245, 245)                       // 薄いグレー
            };

            // 明るさを調整した新しい色を作成
            byte r = (byte)(baseColor.R * brightness);
            byte g = (byte)(baseColor.G * brightness);
            byte b = (byte)(baseColor.B * brightness);

            return new SolidColorBrush(Color.FromRgb(r, g, b));
        }

        // 後方互換性のためのプロパティ
        public static SolidColorBrush TextColor => new SolidColorBrush(Color.FromRgb(96, 96, 96)); // DarkGrayに相当

        /// <summary>
        /// レーティング値に対応した星マークの文字列を取得します
        /// </summary>
        /// <param name="rating">レーティング値 (0=なし, 1=あり)</param>
        /// <returns>レーティング値に対応した星マークの文字列</returns>
        public static string GetRatingStars(int rating)
        {
            return rating switch
            {
                1 => "★",
                _ => "☆"
            };
        }

        /// <summary>
        /// サムネイル表示用の数字入りレーティングスターを取得します
        /// </summary>
        /// <param name="rating">レーティング値 (0=なし, 1=あり)</param>
        /// <returns>数字入りの星マーク</returns>
        public static string GetRatingStarWithNumber(int rating)
        {
            if (rating <= 0 || rating > 1)
                return "";

            // Unicode文字で★に数字を近似的に表現
            return $"★{rating}";
        }

        /// <summary>
        /// 単一のレーティング位置に対応する星マークを取得します（選択用）
        /// </summary>
        /// <param name="position">星の位置 (1)</param>
        /// <param name="rating">現在のレーティング値 (0=なし, 1=あり)</param>
        /// <returns>対応する星マーク（塗りつぶしまたは輪郭）</returns>
        public static string GetStarAtPosition(int position, int rating)
        {
            return position == 1 && rating == 1 ? "★" : "☆";
        }
    }
}
