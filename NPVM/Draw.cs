using System.Text.Json;
using System.Text.RegularExpressions;
using NPVM.Entity;
using NPVM.Enum;
using SkiaSharp;

namespace NPVM;

public abstract partial class Draw
{
    public static void DoRun(string path)
    {
        const int imgHeight = 1024;
        const int imgWidth = 768;
        const string PingFang_Regular = "Assets/Fonts/PingFang-Regular.ttf"; 
        const string PingFang_Blod = "Assets/Fonts/PingFang-Bold.ttf";
        const string PingFang_Heavy = "Assets/Fonts/PingFang-Heavy.ttf";
        const string dataPath = "songs.json";
        var data = JsonSerializer.Deserialize<SongList>(File.ReadAllText(dataPath))!;
        
        var coverFiles = Directory.GetFiles(path);
        var covers = new Dictionary<int, SKBitmap>();
        var coverCount = 0;
        foreach (var coverFile in coverFiles)
        {
            covers[coverCount] = SKBitmap.Decode(coverFile);
            coverCount++;
        }
        var layouts = new Dictionary<Layouts, SKBitmap>
        {
            [Layouts.Buttons] = SKBitmap.Decode("Assets/button.png"),
            [Layouts.ProgBar] = SKBitmap.Decode("Assets/prog_bar.png"),
            [Layouts.ProgPtr] = SKBitmap.Decode("Assets/prog_ptr.png"),
            [Layouts.Disc] = SKBitmap.Decode("Assets/disc.png"),
            [Layouts.Needle] = SKBitmap.Decode("Assets/needle.png")
        };

        var titleBrusher = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 34f,
            Typeface = SKTypeface.FromFile(PingFang_Heavy),
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };
        var artistBrusher = new SKPaint
        {
            Color = new SKColor((uint)Color.Gray),
            TextSize = 15f,
            Typeface = SKTypeface.FromFile(PingFang_Regular),
            IsAntialias = true,
            TextAlign = SKTextAlign.Center
        };
        var timeBrusher = new SKPaint
        {
            Color = SKColors.White,
            TextSize = 16f,
            Typeface = SKTypeface.FromFile(PingFang_Blod),
            IsAntialias = true,
            TextAlign = SKTextAlign.Left,
        };

        for (var i = 0; i < coverCount; i++)
        {
            var image = new SKBitmap(imgWidth, imgHeight);
            var c = new SKCanvas(image);
            var cover = covers[i];
            var fileName = RegexFilename().Match(coverFiles[i]).Value;
            var song = data.Songs!.FirstOrDefault(x => x.Id == uint.Parse(fileName));
            if (song == null)
            {
                Console.WriteLine($"没有在数据文件中找到 {fileName} 的信息。");
                continue;
            }
            
            // 黑色边框
            c.Clear(SKColors.Black);
            
            // 高斯模糊背景
            using var bgPaint = new SKPaint();
            var blurFilter = SKImageFilter.CreateBlur(50, 50);
            bgPaint.ImageFilter = blurFilter;
            // 创建颜色滤镜矩阵来降低高斯模糊背景的亮度，以提高白色文字的可读性
            var colorMatrix = new[]
            {
                0.75f, 0, 0, 0, 0,  // R
                0, 0.75f, 0, 0, 0,  // G
                0, 0, 0.75f, 0, 0,  // B
                0, 0, 0, 1, 0      // A
            };
            var colorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
            bgPaint.ColorFilter = colorFilter;
            c.DrawBitmap(cover, -116, 12, bgPaint);
            
            // 中心曲绘
            const float scaleFactor = 0.38f;
            c.DrawBitmap(cover, SKRect.Create(194, 248, cover.Width * scaleFactor, cover.Height * scaleFactor));
            
            // 页面布局
            c.DrawBitmap(layouts[Layouts.Buttons], 207, 890);
            c.DrawBitmap(layouts[Layouts.ProgBar], 102, 845);
            c.DrawBitmap(layouts[Layouts.ProgPtr], 248, 838);
            c.DrawBitmap(layouts[Layouts.Disc], 104, 165);
            c.DrawBitmap(layouts[Layouts.Needle], 365, 13);
            
            // 文字信息
            c.DrawText(song.Title, new SKPoint(384.00f, 757.74f), titleBrusher);
            c.DrawText(song.Artist, new SKPoint(384.00f, 790.07f), artistBrusher);
            c.DrawText(song.Length, new SKPoint(674.88f, 853.63f), timeBrusher);
            c.DrawText("01:17", new SKPoint(49.7f, 853.63f), timeBrusher);
            
            // 导出
            var byteImage = image.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
            if (!Directory.Exists("Output")) Directory.CreateDirectory("Output");
            File.WriteAllBytes($"Output/{fileName}.jpg", byteImage);
            Console.WriteLine($"{fileName}.jpg 制作成功。");
            
            if (i == coverCount) image.Dispose();
        }
        foreach (var (_, temp) in covers) temp.Dispose();
        timeBrusher.Dispose();
        titleBrusher.Dispose();
        artistBrusher.Dispose();
        Console.WriteLine("按任意键退出。");
        Console.ReadKey();
    }
    
    [GeneratedRegex(@"(?<=\\)[^.]+")]
    private static partial Regex RegexFilename();
}
