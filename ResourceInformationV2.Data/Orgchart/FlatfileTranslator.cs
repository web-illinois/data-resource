namespace ResourceInformationV2.Data.Orgchart {
    public static class FlatfileTranslator {
        public static Search.Models.Orgchart? TranslateFromFlatfile(string flatfile) {
            var lines = flatfile.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) {
                return null;
            }
            var index = lines[0].StartsWith("title", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            var returnValue = ParseLine(lines[index]);
            index++;
            returnValue.Children = ParseLevel(lines, ref index, 1);
            return returnValue;
        }

        public static string TranslateToFlatfile(Search.Models.Orgchart orgchart) {
            var lines = new List<string> { "title\tsubtitle\tlink\tlarge\tweight" };
            AppendOrgchartLines(orgchart, lines, 0);
            return string.Join(Environment.NewLine, lines);
        }

        private static void AppendOrgchartLines(Search.Models.Orgchart orgchart, List<string> lines, int depth) {
            var indent = new string('\t', depth);
            lines.Add($"{indent}{orgchart.Title}\t{orgchart.Subtitle}\t{orgchart.Link}\t{orgchart.Large}\t{orgchart.Weight}");
            if (orgchart.Children != null) {
                foreach (var child in orgchart.Children) {
                    AppendOrgchartLines(child, lines, depth + 1);
                }
            }
        }

        private static List<Search.Models.Orgchart> ParseLevel(string[] lines, ref int index, int depth) {
            var result = new List<Search.Models.Orgchart>();

            while (index < lines.Length) {
                var currentLineDepth = GetDepth(lines[index]);
                if (currentLineDepth != depth) {
                    break;
                }
                var node = ParseLine(lines[index]);
                index++;

                if (index < lines.Length) {
                    var nextLineDepth = GetDepth(lines[index]);

                    if (nextLineDepth > depth) {
                        var children = ParseLevel(lines, ref index, nextLineDepth);
                        if (children.Count > 0) {
                            node.Children = children;
                        }
                    }
                }
                result.Add(node);
            }
            return result;
        }

        private static Search.Models.Orgchart ParseLine(string line) {
            var fields = TrimIndent(line).Split('\t', StringSplitOptions.None);
            return new Search.Models.Orgchart {
                Title = fields.Length > 0 ? fields[0].Trim('"', ' ') : string.Empty,
                Subtitle = fields.Length > 1 ? fields[1].Trim('"', ' ') : null,
                Link = fields.Length > 2 ? fields[2].Trim('"', ' ') : null,
                Large = fields.Length > 3 && bool.TryParse(fields[3].Trim('"', ' '), out var large) ? large : false,
                Weight = fields.Length > 4 && int.TryParse(fields[4].Trim('"', ' '), out var weight) ? weight : 0
            };
        }

        private static int GetDepth(string line) {
            var segments = line.Split('\t', StringSplitOptions.None);
            var depth = 0;
            foreach (var segment in segments) {
                if (string.IsNullOrWhiteSpace(segment)) {
                    depth++;
                    continue;
                }
                break;
            }
            return depth;
        }

        private static string TrimIndent(string line) {
            var index = 0;

            while (index < line.Length && (line[index] == '\t' || line[index] == ' ')) {
                index++;
            }

            return line.Substring(index);
        }
    }
}
