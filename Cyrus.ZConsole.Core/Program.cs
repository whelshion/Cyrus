﻿using Cyrus.ZConsole.Core.Contexts;
using Cyrus.ZConsole.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cyrus.ZConsole.Core
{
    class Program
    {
        private static IDictionary<string, AntennasPattern> _antMap = new Dictionary<string, AntennasPattern>();
        static async Task Main(string[] args)
        {
            var directory = "F:\\下载\\antennas_pattern_190328";

            var fileNames = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);

            _antMap.Clear();

            using (var ctx = new SosasContext())
            {
                for (int i = 0; i < fileNames.Length; i++)
                {
                    AntennasPattern entity = null;
                    var name = Path.GetFileNameWithoutExtension(fileNames[i]).Trim().Replace(" ", "_");
                    Console.WriteLine($"正在处理：{fileNames[i]}");
                    if (_antMap.ContainsKey(name)) entity = _antMap[name];
                    var lines = await File.ReadAllLinesAsync(fileNames[i]);
                    entity = await MapToEntityAsync(lines, entity);
                    entity.DefName = name;
                    entity.CommentDate = entity.LogDate.ToString("yyyy-MM-dd");
                    if (!_antMap.ContainsKey(name)) _antMap.Add(name, entity);
                    //await ctx.AntennasPatterns.AddOrUpdateAsync(antennasPattern);
                }
                Console.WriteLine($"正在保存：{_antMap.Count()}");
                await ctx.AntennasPatterns.AddRangeAsync(_antMap.Values);
                var rows = ctx.SaveChanges();
                //var rows = await ctx.SaveChangesAsync();
                Console.WriteLine($"影响行数：{rows}");
            }
        }

        private static async Task<AntennasPattern> MapToEntityAsync(string[] lines, AntennasPattern entity)
        {
            lines = lines.Select(o => o.Trim()).ToArray();
            entity = entity ?? new AntennasPattern();
            var propInfos = lines.Where(o => !string.IsNullOrWhiteSpace(o) && o.First() >= 65);
            foreach (var line in propInfos)
            {
                var kv = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (kv.Length > 1)
                {
                    double v;
                    switch (kv[0])
                    {
                        case "NAME":
                            entity.Name = kv[1];
                            break;
                        case "MAKE":
                            entity.Make = kv[1];
                            break;
                        case "FREQUENCY":
                            if (double.TryParse(kv[1], out v))
                                entity.Frequency = v;
                            break;
                        case "H_WIDTH":
                            if (double.TryParse(kv[1], out v))
                                entity.HWidth = v;
                            break;
                        case "V_WIDTH":
                            if (double.TryParse(kv[1], out v))
                                entity.VWidth = v;
                            break;
                        case "FRONT_TO_BACK":
                            if (double.TryParse(kv[1], out v))
                                entity.FrontToBack = v;
                            break;
                        case "GAIN":
                            if (double.TryParse(kv[1], out v))
                                entity.Gain = v;
                            break;
                        case "TILT":
                            entity.Tilt = kv[1];
                            break;
                        default:
                            break;

                    }
                }
            }
            var dataHs = lines.Where(o => !string.IsNullOrWhiteSpace(o))
                .SkipWhile(o => !o.StartsWith("HORIZONTAL"))
                .Skip(1).TakeWhile(o => o.First() >= 48 && o.First() < 58)
                .Select(o =>
                {
                    var splits = o.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    return $"{double.Parse(splits[0])},{double.Parse(splits[1])}";
                });
            var dataVs = lines.Where(o => !string.IsNullOrWhiteSpace(o))
                .SkipWhile(o => !o.StartsWith("VERTICAL"))
                .Skip(1).TakeWhile(o => o.First() >= 48 && o.First() < 58)
                .Select(o =>
                {
                    var splits = o.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    return $"{double.Parse(splits[0])},{double.Parse(splits[1])}";
                });
            entity.Horizontal = string.Join(";", dataHs);
            entity.Vertical = string.Join(";", dataVs);
            return await Task.FromResult(entity);
        }
    }
}
